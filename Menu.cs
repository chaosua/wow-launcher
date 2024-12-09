using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace wow_launcher_cs
{
    public partial class Menu : Form
    {
        string locale;

        public Menu()
        {
            InitializeComponent();
        }

        private bool mouseDown;
        private Point lastLocation;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        static public void UpdateWow()
        {
            if (Updater.data.disabled)
                return;

            if (File.Exists("Wow.exe"))
            {
                if (Updater.CalculateMD5("WoW.exe").CompareTo(Updater.data.Wow.md5) == 0)
                {
                    PlayWow();
                    return;
                }
                if (File.Exists("WoW.exe.old"))
                    File.Delete("WoW.exe.old");
                File.Move("WoW.exe", "WoW.exe.old");
            }
            //DownloadInfoLabel = MainMenu.Equals();
            using (WebClient wc = new WebClient())
            {
              //  DownloadInfoLabel.Text = "Оновлення WoW.exe";
                wc.DownloadFileCompleted += ((sender, args) =>
                {
                 //   DownloadInfoLabel.Text = "WoW.exe оновлено. СТАРТУЮ";
                    PlayWow();
                });
                wc.DownloadFileAsync(new System.Uri(Updater.data.Wow.link), "WoW.exe"); //Качає WoW.exe коли натиснуто кнопку Play
            }
        }

        static private void PlayWow()
        {
            Process.Start("WoW.exe");
            Environment.Exit(0); // закриваєм Launcher
        }

        private void titleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void titleBar_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            UpdatePlayButton(playButton);
            DownloadInfoLabel.Text = "Клієнт оновлено.";
            Text = "Launcher";
            if (File.Exists("Launcher.exe.old"))
                File.Delete("Launcher.exe.old");
            if (Directory.Exists("Data/ruRU"))
                locale = "ruRU";
          /*  else if (Directory.Exists("Data/enGB"))
                locale = "enGB";*/
            else
            {
                var result = MessageBox.Show("Папка з Data\ruRU не знайдена! Перемістіть Launcher в корінь папки World of Warcraft.", "Error", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            // Очистка Кешу
            if (Directory.Exists("Cache"))
                Directory.Delete("Cache", true);
        }

        public void UpdatePatches()
        {
            Thread thread = new Thread(() =>
            {
                if (Updater.data.disabled)
                    return;
                foreach (Updater.PatchData patch in Updater.data.Patches)
                {
                    bool dlCpt = false;

                    if (File.Exists("Data/ruRU/" + patch.name) && Updater.CalculateMD5("Data/ruRU/" + patch.name).CompareTo(patch.md5) == 0)
                        continue;
                    if (File.Exists("Data/ruRU/" + patch.name))
                        File.Delete("Data/ruRU/" + patch.name);
                    using (WebClient wc = new WebClient())
                    {
                        DownloadInfoLabel.Invoke(new MethodInvoker(delegate { DownloadInfoLabel.Text = "Завантаження: " + patch.name; }));

                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdateProgress);
                        wc.DownloadFileCompleted += ((sender, args) =>
                        {
                            dlCpt = true;
                            DownloadInfoLabel.Invoke(new MethodInvoker(delegate { DownloadInfoLabel.Text = "Клієнт оновлено."; }));
                        });
                        wc.DownloadFileAsync(new System.Uri(patch.link), "Data/ruRU/" + patch.name);
                    }
                    while (!dlCpt)
                    {
                        Application.DoEvents();
                    }
                }

                playButton.Invoke(new MethodInvoker(delegate { playButton.Enabled = true; })); //Фікс компіляції
                UpdatePlayButton(playButton);
            });
            thread.Start();
        }

        public void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            SetProgressBarPct(Convert.ToInt32(percentage));
        }

        public void SetProgressBarPct(int pct)
        {
            Bitmap bmp = new Bitmap(Properties.Resources.dl_bar_green);
            for (int Xcount = (pct * bmp.Width) / 100; Xcount < bmp.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < bmp.Height; Ycount++)
                {
                    bmp.SetPixel(Xcount, Ycount, Color.Transparent);
                }
            }
            progressBar.BackgroundImage = bmp;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void playButton_MouseDown(object sender, MouseEventArgs e)
        {
            playButton.BackgroundImage = Properties.Resources.PlayButtonDown;
        }

        private void playButton_MouseUp(object sender, MouseEventArgs e)
        {
            playButton.BackgroundImage = Properties.Resources.PlayButtonBase;
        }

        private void playButton_MouseEnter(object sender, EventArgs e)
        {
            playButton.BackgroundImage = Properties.Resources.PlayButtonHover;
        }

        private void playButton_MouseLeave(object sender, EventArgs e)
        {
            playButton.BackgroundImage = Properties.Resources.PlayButtonBase;
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizeButton_MouseEnter(object sender, EventArgs e)
        {
            minimizeButton.BackgroundImage = Properties.Resources.MinimizeButtonHover;
        }

        private void minimizeButton_MouseDown(object sender, MouseEventArgs e)
        {
            minimizeButton.BackgroundImage = Properties.Resources.MinimizeButtonDown;
        }

        private void minimizeButton_MouseUp(object sender, MouseEventArgs e)
        {
            minimizeButton.BackgroundImage = Properties.Resources.MinimizeButtonBase;
        }

        private void minimizeButton_MouseLeave(object sender, EventArgs e)
        {
            minimizeButton.BackgroundImage = Properties.Resources.MinimizeButtonBase;
        }

        private void closeButton_MouseDown(object sender, MouseEventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.CloseButtonDown;
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.CloseButtonHover;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.CloseButtonBase;
        }

        private void closeButton_MouseUp(object sender, MouseEventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.CloseButtonBase;
        }

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void playButton_EnabledChanged(object sender, EventArgs e)
        {
            UpdatePlayButton(sender);
        }

        private void UpdatePlayButton(object b)
        {
            if (playButton.Enabled)
                playButton.BackgroundImage = Properties.Resources.PlayButtonBase;
            else
                playButton.BackgroundImage = Properties.Resources.PlayButtonDisabled;
        }

        private void CheckkRealmlistAndUpdate()
        {

            string wtfpath = $@"Data/{locale}/realmlist.wtf";

            if (File.Exists(wtfpath))
            {
                FileAttributes attributes = File.GetAttributes(wtfpath);

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(wtfpath, FileAttributes.Normal);
                if (File.ReadAllText(wtfpath).CompareTo("set realmlist login1.freedom-wow.in.ua") != 0)
                {
                    if (CheckboxRealmlist.Checked)
                    {
                        ChangeRealmlist(wtfpath);

                    }

                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(wtfpath, true, Encoding.UTF8))
                {
                    sw.Write("set realmlist login1.freedom-wow.in.ua");
                    playButton.Enabled = true;

                }
            }
        }

        private void Menu_Shown(object sender, EventArgs e)
        {
            UpdatePatches();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            UpdateWow();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeRealmName();
        }

        private void CheckboxRealmlist_CheckedChanged(object sender, EventArgs e)
        {
            CheckkRealmlistAndUpdate();
        }

        private void ChangeRealmlist(string wtfpath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(wtfpath, false, Encoding.UTF8))
                {
                    sw.Write("set realmlist login1.freedom-wow.in.ua");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Щоось пішло не так {ex.Message}");
            }
        }

        private void ChangeRealmName()
        {
            string path = "WTF/Config.wtf"; // Шлях до файлу
            string searchText = "SET realmName"; // Рядок, який потрібно знайти
            string replaceText = "SET realmName \"Freedom x5\""; // Новий текст, яким замінити
            bool found = false;

            try
            {
                // Читаємо всі рядки з файлу
                string[] lines = File.ReadAllLines(path);

                // Перебираємо всі рядки та замінюємо необхідний
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(searchText))
                    {
                        lines[i] = replaceText; // Заміна рядка
                        found = true; // Позначаємо, що рядок знайдено
                        break; // Виходимо з циклу, якщо рядок знайдено
                    }
                }

                // Якщо рядок не знайдено, додаємо його в кінець масиву
                if (!found)
                {
                    var linesList = new List<string>(lines);
                    linesList.Add(replaceText);
                    lines = linesList.ToArray(); // Оновлюємо масив рядків
                }

                // Перезаписуємо файл з модифікованими рядками
                File.WriteAllLines(path, lines);
            }
            catch(Exception e)
            {
                if(CheckBoxRealmName.Checked)
                {
                MessageBox.Show("Файлу Config.wtf не існує!");
                CheckBoxRealmName.Checked = false;

                }

            }
        }

        
    }
}
