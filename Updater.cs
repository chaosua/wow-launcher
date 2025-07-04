﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using wow_launcher_cs.Migration;

namespace wow_launcher_cs
{
    static class Updater
    {
#if WITH_MIGRATION
        private static LauncherMigrationUpdater _newUpdater;
#endif
        
        private static int _status = 0;

        static public int GetStatus()
        {
            return _status;
        }

        public struct PatchData
        {
            public string name;
            public string type;
            public string locale;
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
        static string remoteHost = "https://updater.freedom-wow.in.ua/client/"; //основна адреса сервера оновлення

        public static void Init()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            data.disabled = false;

            if (File.Exists("update.xml")) //перевіряє чи присутній локальний файл налаштувань поруч з лаунчером
                InitValidation("update.xml");
            else
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteHost);
                request.Timeout = 6000; //Таймаут відповіді сервера
                request.Method = "HEAD";
                request.AllowAutoRedirect = true; // якщо сервер перенаправляє
                request.UserAgent = "FreedomLauncher";
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                            InitValidation(remoteHost + "update-new.xml");
                    }
                }
                catch (WebException e)
                {
                    MessageBox.Show("Помилка Сервер оновлень недоступний!\nError: " + e.Message, "Помилка", MessageBoxButtons.OK);
                    data.disabled = true;
                }  
            }
        }

        public static async Task UpdateLauncher()
        {
            if (data.disabled)
                return;
            
#if WITH_MIGRATION
            if (OSValidator.SystemIsValidForUpdate())
            {
                _newUpdater = new LauncherMigrationUpdater();
                try
                {
                    var path = await _newUpdater.DownloadUpdateAsync();
                    _newUpdater.PrepareUpdaterScript(path);
                    Environment.Exit(0);
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Не вдалося з'єднатися з сервером оновлень:\n" + ex.Message, "Помилка", MessageBoxButtons.OK);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Невідома помилка:\nError: " + ex.Message, "Помилка", MessageBoxButtons.OK);
                }
                return;
            }
#endif
            Version productVersion = Version.Parse(Application.ProductVersion);
            Version latestVersion = Version.Parse(data.Launcher.version);
            string filename = Assembly.GetEntryAssembly().Location;
            string tempFilename = filename + ".tmp";

            if (productVersion.CompareTo(latestVersion) < 0)
            {
                _status = 1;

                try
                {
                    using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = await client.GetAsync(data.Launcher.link, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                       fileStream = new FileStream(tempFilename, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }

                    // Перевіряємо MD5 перед заміною файлу
                    if (CalculateMD5(tempFilename).CompareTo(data.Launcher.md5) == 0)
                    {
                        // Використовуємо CMD для оновлення лаунчера після закриття
                        string cmd = $@"/C timeout 1 & del /F /Q ""{filename}"" & move ""{tempFilename}"" ""{filename}"" & start """" ""{filename}""";

                        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", cmd)
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        };

                        Process.Start(psi);
                        Application.Exit();
                    }
                    else
                    {
                        throw new Exception("Хеш MD5 не співпадає!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButtons.OK);
                    data.disabled = true;

                    if (File.Exists(tempFilename))
                        File.Delete(tempFilename);
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
                MessageBox.Show("Помилка перевірки update.xml!", "Помилка", MessageBoxButtons.OK);
            }
        }

        static bool ValidateAndLoad()
        {
            return ValidateRoot()
                   && ValidateComponentLimit()
                   && ValidateWowComponent()
                   && ValidateLauncherComponent()
                   && ValidatePatchesComponents();
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
                if (compPatch.Attributes["file"] != null && compPatch.Attributes["type"] != null &&
                    compPatch.Attributes["locale"] != null && compPatch.Attributes["md5"] != null &&
                    compPatch.Attributes["link"] != null)
                {
                    tmp.name = compPatch.Attributes["file"].Value;
                    tmp.type = compPatch.Attributes["type"].Value;
                    tmp.locale = compPatch.Attributes["locale"].Value;
                    tmp.md5 = compPatch.Attributes["md5"].Value;
                    tmp.link = compPatch.Attributes["link"].Value;
                    tmp.link = tmp.link.Replace("$BASE", data.baseURL);

                    foreach (PatchData existingPatch in data.Patches)
                    {
                        if ((existingPatch.name == tmp.name && existingPatch.type == tmp.type) || existingPatch.link == tmp.link/* || existingPatch.md5 == tmp.md5*/)
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
