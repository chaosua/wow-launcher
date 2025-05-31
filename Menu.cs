using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
#if WITH_MIGRATION
using wow_launcher_cs.Migration;
#endif

namespace wow_launcher_cs
{
    public partial class Menu : Form
    {
        private Settings settings;

        public Menu()
        {
            InitializeComponent();
        }

#if WITH_MIGRATION
        ~Menu()
        {
            _gameUpdater.UpdateProgress -= GameUpdaterOnUpdateProgress;
        }
#endif

        private bool mouseDown;
        private Point lastLocation;
        private bool DLUALocalePatch;
        private bool DLConfigWoW;
        private bool ClenupPatchD;
        private string locale;
        
#if WITH_MIGRATION
        private readonly NewGameUpdater _gameUpdater = new();
#endif

        private async void Menu_Shown(object sender, EventArgs e)
        {
            //Обираєм кращий доступний протокол для безпечного з'єднання
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            // Перевірка, чи запущений процес Wow.exe
            if (Process.GetProcessesByName("Wow").Any())
            {
                UpdateDownloadInfoLabel("Перевірку оновлення скасовано. Запущена гра!");
                MessageBox.Show("Гра запущена! Закрийте WoW перед оновленням патчів.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
#if !WITH_MIGRATION
            await UpdateWowExecutable();
            await UpdatePatches();
#else
            _gameUpdater.UpdateProgress += GameUpdaterOnUpdateProgress;
            await _gameUpdater.RunAsync();
#endif
        }

#if WITH_MIGRATION
        public void GameUpdaterOnUpdateProgress(object sender, ProgressEventArgs e)
        {
            SetProgressBarPct((int)e.Progress);
        }
#endif

        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
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
            locale = GetClientLocaleConfig();

            if (locale == "NULL")
            {
                if (Directory.Exists("Data/ruRU"))
                    locale = "ruRU";
                else if (Directory.Exists("Data/enGB"))
                    locale = "enGB";
                else if (Directory.Exists("Data/enUS"))
                    locale = "enUS";
            }

            if (!Directory.Exists("Data") || locale=="NULL")
            {
                var result = MessageBox.Show("Не знайдено папки Data\\ \rПеремістіть Launcher в корінь папки World of Warcraft 3.3.5", "Помилка", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            //UpdatePlayButton(playButton);
            UpdateDownloadInfoLabel("");
            Text = "Launcher";

            // Очистка Кешу ЗРОБИТИ ОПЦІОНАЛЬНИМ
            /*
             if (Directory.Exists("Cache"))
                Directory.Delete("Cache", true);
            */
        }
        public async Task UpdatePatches()
        {
            UpdateDownloadInfoLabel("Перевірка оновлень.");
            DLUALocalePatch = GetLauncherConfig("DownloadUALocale");
            ClenupPatchD = GetLauncherConfig("Patch-D-Cleanup");
            locale = GetClientLocaleConfig();

            if (File.Exists("Data/ruRU/patch-ruRU-D.MPQ") && ClenupPatchD)
            {
                File.Delete("Data/ruRU/patch-ruRU-D.MPQ");
            }

            SetPlayButtonState(false);

            if (Updater.data.disabled)
            {
                UpdateDownloadInfoLabel("Оновлення скасовано. Немає з'єднання?");
                SetPlayButtonState(true);
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                foreach (Updater.PatchData patch in Updater.data.Patches)
                {
                    // ігноруємо інші локалі
                    if (!string.Equals(patch.locale, locale, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // перевірка типу
                    if ((DLUALocalePatch && patch.type != "1") || (!DLUALocalePatch && patch.type != "0"))
                        continue;

                    string patchPath = $"Data/{locale}/{patch.name}";
                    string tempPath = $"{patchPath}.tmp";

                    if (File.Exists(patchPath) && Updater.CalculateMD5(patchPath).CompareTo(patch.md5) == 0)
                    {
                        UpdateDownloadInfoLabel($"Оновлення {patch.name} не потрібне.");
                        continue;
                    }

                    if (File.Exists(patchPath))
                        File.Delete(patchPath);

                    try
                    {
                        UpdateDownloadInfoLabel($"Завантаження: {patch.name}");

                        using (HttpResponseMessage response = await client.GetAsync(patch.link, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            long totalBytes = response.Content.Headers.ContentLength ?? -1;
                            long receivedBytes = 0;
                            byte[] buffer = new byte[65536]; // **Буфер 64КБ для пришвидшення**
                            int lastProgress = -1;

                            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                           fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                            {
                                int bytesRead;
                                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    receivedBytes += bytesRead;
                                    
                                    if (totalBytes > 0)
                                    {
                                        int progress = (int)((receivedBytes * 100) / totalBytes);
                                        if (progress != lastProgress)
                                        {
                                            SetProgressBarPct(progress);
                                            lastProgress = progress;
                                        }
                                    }
                                }
                            }
                        }

                        SetProgressBarPct(100);

                        if (Updater.CalculateMD5(tempPath).CompareTo(patch.md5) == 0)
                        {
                            File.Move(tempPath, patchPath);
                            UpdateDownloadInfoLabel($"{patch.name} завантажено успішно.");
                        }
                        else
                        {
                            throw new Exception($"Контрольна сума {patch.name} не збігається!");
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateDownloadInfoLabel($"Помилка завантаження {patch.name}: {ex.Message}");

                        if (File.Exists(tempPath))
                            File.Delete(tempPath);
                    }
                }
            }

            UpdateDownloadInfoLabel("Завантажено останнє оновлення.");
            SetPlayButtonState(true);
        }

        public async Task UpdateWowExecutable()
        {
            DLConfigWoW = GetLauncherConfig("PatchClient");

            if (Updater.data.disabled || !DLConfigWoW)
                return;

            UpdateDownloadInfoLabel("Перевірка Wow.exe.");

            SetPlayButtonState(false);

            string wowExe = "Wow.exe";
            string wowBackup = "Wow.exe.old";
            string wowTemp = "Wow.exe.tmp";

            if (File.Exists(wowExe))
            {
                if (Updater.CalculateMD5(wowExe).CompareTo(Updater.data.Wow.md5) == 0)
                {
                    UpdateDownloadInfoLabel("Оновлення Wow.exe не потрібне.");
                    SetPlayButtonState(true);
                    return;
                }

                // Якщо є стара резервна копія, не видаляємо її, щоб мати можливість відновлення
                if (!File.Exists(wowBackup))
                {
                    File.Move(wowExe, wowBackup); // Зберігаємо поточний Wow.exe як резервну копію
                }
            }

            // Завантаження нового файлу у тимчасовий файл
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(Updater.data.Wow.link, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                   fileStream = new FileStream(wowTemp, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }

                // Перевіряємо MD5 нового файлу перед заміною
                if (Updater.CalculateMD5(wowTemp).CompareTo(Updater.data.Wow.md5) == 0)
                {
                    if (File.Exists(wowExe)) File.Delete(wowExe);
                    File.Move(wowTemp, wowExe);
                    UpdateDownloadInfoLabel("Wow.exe оновлено.");
                }
                else
                {
                    throw new Exception("Контрольна сума файлу не збігається!");
                }
            }
            catch (Exception ex)
            {
                UpdateDownloadInfoLabel($"Помилка оновлення Wow.exe: {ex.Message}");

                // Відновлення резервної копії у разі невдачі
                if (File.Exists(wowBackup))
                {
                    File.Copy(wowBackup, wowExe, true);
                    UpdateDownloadInfoLabel("Оновлення не вдалося, відновлено резервну копію.");
                }
            }
            finally
            {
                // Видаляємо тимчасовий файл, якщо він залишився після помилки
                if (File.Exists(wowTemp)) File.Delete(wowTemp);
                SetPlayButtonState(true);
            }
        }

        static private void PlayWow()
        {
            string wowExePath = "Wow.exe";
            if (File.Exists(wowExePath))
            {
                Process.Start("Wow.exe");
                Environment.Exit(0); // закриваєм Launcher
            }
            else
            {
                MessageBox.Show("Wow.exe не знайдено!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        public void CheckRealmlistAndUpdate()
        {
            locale = GetClientLocaleConfig();

            string wtfpath = $@"Data/{locale}/realmlist.wtf";

            if (File.Exists(wtfpath))
            {
                FileAttributes attributes = File.GetAttributes(wtfpath);

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(wtfpath, FileAttributes.Normal);

                if (File.ReadAllText(wtfpath).CompareTo("set realmlist login1.freedom-wow.in.ua") != 0)
                {
                    if (settings.CheckboxRealmlist.Checked)
                    {
                        settings.ChangeRealmlist(wtfpath);

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
        private void playButton_Click(object sender, EventArgs e)
        {
            PlayWow();
        }

        private void SettingsButton_Click_1(object sender, EventArgs e)
        {
            if (settings == null || settings.IsDisposed)
            {
                settings = new Settings(this);
                settings.Show();
            }
            else
            {
                settings.BringToFront();
            }
        }

        private void SettingsButton_MouseDown(object sender, MouseEventArgs e)
        {
            SettingsButton.BackgroundImage = Properties.Resources.config_button_down;
        }
        private void SettingsButton_MouseEnter(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = Properties.Resources.config_button_hover;
        }

        private void SettingsButton_MouseLeave(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = Properties.Resources.config_button_base;
        }

        private void SettingsButton_MouseUp(object sender, MouseEventArgs e)
        {
            SettingsButton.BackgroundImage = Properties.Resources.config_button_base;
        }

        public bool GetLauncherConfig(string config)
        {
            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string LauncherConfigFilePath = Path.Combine(baseDirectory, "Launcher.ini");

            bool state = false;

            // Перевірка, чи існує файл Launcher.ini
            if (!File.Exists(LauncherConfigFilePath))
            {
                // Створюємо файл із дефолтними налаштуваннями
                var settings = new wow_launcher_cs.Settings(this);
                settings.SetDefaultLauncherConfig();
            }

            try
            {
                // Читання файлу Launcher.ini
                string[] configLines = File.ReadAllLines(LauncherConfigFilePath);
                // Пошук параметра
                string configLine = configLines.FirstOrDefault(line => line.StartsWith($"{config} "));

                if (!string.IsNullOrEmpty(configLine))
                {
                    // Отримання значення параметра
                    string status = configLine.Split('"')[1];

                    // Встановлення значення чекбокса
                    if (status == "1")
                        state = true;
                    else
                        state = false;
                }
                else
                {
                    // Якщо параметра не існує, створюємо його і ставимо значення "1"
                    var settings = new wow_launcher_cs.Settings(this);
                    settings.UpdateLauncherConfig(config, true);
                    state = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка читання Launcher.ini\nПомилка: {ex.Message}",
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return state;
        }

        public string GetClientLocaleConfig()
        {
            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataDirectory = Path.Combine(baseDirectory, "Data");
            string wtfDirectory = Path.Combine(baseDirectory, "WTF");
            string configFilePath = Path.Combine(wtfDirectory, "Config.wtf");
            string clientLocaleConfig = "NULL";

            if (File.Exists(configFilePath))
            {
                string[] configLines = File.ReadAllLines(configFilePath);

                string localeLine = configLines.FirstOrDefault(line => line.StartsWith("SET locale "));

                if (!string.IsNullOrEmpty(localeLine))
                {
                    //Console.WriteLine($"Локаль знайдено: {localeLine}");
                    clientLocaleConfig = localeLine.Split('"')[1];

                    if (clientLocaleConfig.Length == 4)
                        clientLocaleConfig = clientLocaleConfig.Substring(0, 2).ToLower() + clientLocaleConfig.Substring(2, 2).ToUpper();
                    else clientLocaleConfig = "NULL";
                }
            }
            
            if (clientLocaleConfig == "NULL")
            {
                if (Directory.Exists(Path.Combine(dataDirectory, "ruRU")) || Directory.Exists(Path.Combine(dataDirectory, "ruru")))
                    clientLocaleConfig = "ruRU";
                else if (Directory.Exists(Path.Combine(dataDirectory, "enGB")) || Directory.Exists(Path.Combine(dataDirectory, "engb")))
                    clientLocaleConfig = "enGB";
                else if (Directory.Exists(Path.Combine(dataDirectory, "enUS")) || Directory.Exists(Path.Combine(dataDirectory, "enus")))
                    clientLocaleConfig = "enUS";
                else
                    MessageBox.Show("Жодної папки з локалізацією не знайдено!\nПомістіть Лаунчер в корінь папки з грою World of Warcraft 3.3.5", "Помилка", MessageBoxButtons.OK);
            }

            return clientLocaleConfig;
        }

        public void UpdateDownloadInfoLabel(string text)
        {
            if (DownloadInfoLabel.InvokeRequired)
            {
                DownloadInfoLabel.Invoke(new MethodInvoker(() => DownloadInfoLabel.Text = text));
            }
            else
            {
                DownloadInfoLabel.Text = text;
            }
        }
        private void SetPlayButtonState(bool enabled)
        {
            if (playButton.InvokeRequired)
            {
                playButton.Invoke(new MethodInvoker(() => playButton.Enabled = enabled));
            }
            else
            {
                playButton.Enabled = enabled;
            }
            UpdatePlayButton(playButton);
        }

        private void OnLinkAreaClicked(object sender, MouseEventArgs e)
        {
            // Відкриття сайту у браузері за замовчуванням
            string url = "https://discord.gg/pGmU9YTwY8";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Це потрібно для відкриття URL у браузері
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити сайт: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void discordLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            discordLinkLabel.Cursor = Cursors.Hand;
        }

        private void discordLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            discordLinkLabel.Cursor = Cursors.Default;
        }
    }
}
