using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;

namespace wow_launcher_cs
{
    static class Updater
    {
        private static int _status = 0;

        static public int GetStatus()
        {
            return _status;
        }

        public struct PatchData
        {
            public string name;
            public string md5;
            public string link;
        }

        public struct LauncherData
        {
            public string version;
            public string md5;
            public string link;
        }

        public struct WoWData
        {
            public string md5;
            public string link;
        }

        public struct UpdateData
        {
            public bool disabled;
            public LauncherData Launcher;
            public List<PatchData> Patches;
            public WoWData Wow;
            public string baseURL;
        }

        static XmlDocument xml;
        static XmlElement root;
        public static UpdateData data;
        static string remoteHost = "http://updater.freedom-wow.in.ua/client/"; //основна адреса сервера оновлення

        static public void Init()
        {
            data.disabled = false;

            if (File.Exists("update.xml")) //перевіряє чи присутній локальний файл налаштувань поруч з лаунчером
                InitValidation("update.xml");
            else
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteHost);
                request.Timeout = 5000;
                request.Method = "HEAD";
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                            InitValidation(remoteHost + "update.xml");
                    }
                }
                catch (WebException e)
                {
                    _ = MessageBox.Show("Помилка Сервер оновлень недоступний!\nError: " + e.Message, "Помилка", MessageBoxButtons.OK);
                    data.disabled = true;
                }
            }
        }

        static public void UpdateLauncher()
        {
            if (data.disabled)
                return;

            Version productVersion = Version.Parse(Application.ProductVersion);
            Version latestVersion = Version.Parse(data.Launcher.version);
            string filename = Assembly.GetEntryAssembly().Location;
            if (productVersion.CompareTo(latestVersion) < 0)
            {
                _status = 1;
                File.Move(filename, filename + ".old");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += ((sender, args) =>
                    {
                        if (CalculateMD5(filename).CompareTo(data.Launcher.md5) == 0)
                        {
                            _ = Process.Start(filename);
                            Application.Exit();
                        }
                        else
                        {
                            _ = MessageBox.Show("Хеш MD5 не співпадає!", "Помилка", MessageBoxButtons.OK);
                            data.disabled = true;
                            File.Delete(filename);
                            File.Move(filename + "old", filename);
                            return;
                        }
                    }
                    );
                    wc.DownloadFileAsync(new System.Uri(data.Launcher.link), filename);
                }
            }
        }

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static void InitValidation(string URL)
        {
            xml = new XmlDocument();
            xml.Load(URL);
            root = xml.DocumentElement;
            if (!ValidateAndLoad())
            {
                data.disabled = true;
                _ = MessageBox.Show("Помилка перевірки update.xml!", "Помилка", MessageBoxButtons.OK);
            }
        }

        static bool ValidateAndLoad()
        {
            if (ValidateRoot() && ValidateComponentLimit() && ValidateWowComponent() && ValidateLauncherComponent() && ValidatePatchesComponents())
                return true;
            return false;
        }

        static bool ValidateRoot()
        {
            if (root.HasAttribute("base"))
            {
                data.baseURL = root.Attributes["base"].Value;
                return true;
            }
            return false;
        }

        static bool ValidateComponentLimit()
        {
            XmlNodeList componentLauncher = xml.SelectNodes("//component[@name='launcher']");
            XmlNodeList componentWow = xml.SelectNodes("//component[@name='wow']");
            if (componentLauncher.Count > 1 || componentWow.Count > 1)
                return false;
            return true;
        }

        static bool ValidateWowComponent()
        {
            XmlNode component = xml.SelectSingleNode("//component[@name='wow']");
            if (component == null)
            {
                data.Wow.md5 = null;
                data.Wow.link = null;
                return true;
            }

            if (component.Attributes["md5"] != null && component.Attributes["link"] != null)
            {
                data.Wow.md5 = component.Attributes["md5"].Value;
                data.Wow.link = component.Attributes["link"].Value;
                data.Wow.link = data.Wow.link.Replace("$BASE", data.baseURL);
                return true;
            }
            return false;
        }

        static bool ValidateLauncherComponent()
        {
            XmlNode component = xml.SelectSingleNode("//component[@name='launcher']");
            if (component == null)
            {
                data.Launcher.version = null;
                data.Launcher.md5 = null;
                data.Launcher.link = null;
                return true;
            }

            if (component.Attributes["version"] != null && component.Attributes["md5"] != null && component.Attributes["link"] != null)
            {
                data.Launcher.version = component.Attributes["version"].Value;
                data.Launcher.md5 = component.Attributes["md5"].Value;
                data.Launcher.link = component.Attributes["link"].Value;
                data.Launcher.link = data.Launcher.link.Replace("$BASE", data.baseURL);
                return true;
            }
            return false;
        }

        static bool ValidatePatchesComponents()
        {
            data.Patches = new List<PatchData>();
            XmlNodeList components = xml.SelectNodes("//component[@name='patch']");
            foreach (XmlNode compPatch in components)
            {
                PatchData tmp;
                if (compPatch.Attributes["file"] != null && compPatch.Attributes["md5"] != null && compPatch.Attributes["link"] != null)
                {
                    tmp.name = compPatch.Attributes["file"].Value;
                    tmp.md5 = compPatch.Attributes["md5"].Value;
                    tmp.link = compPatch.Attributes["link"].Value;
                    tmp.link = tmp.link.Replace("$BASE", data.baseURL);

                    foreach (PatchData existingPatch in data.Patches)
                    {
                        if (existingPatch.name == tmp.name || existingPatch.link == tmp.link || existingPatch.md5 == tmp.md5)
                            return false;
                    }
                    data.Patches.Add(tmp);
                }
                else
                    return false;
            }
            return true;
        }
    }
}
