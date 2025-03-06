using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wow_launcher_cs
{
    public partial class Settings : Form
    {
        private Menu mainMenu;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private bool mouseDown;
        private Point lastLocation;

        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font myFont;

        class ComboItem
        {
            public int ID { get; set; }
            public string Text { get; set; }
        }

        public Settings(Menu form)
        {
            mainMenu = form;
            InitializeComponent();

            byte[] fontData = Properties.Resources.NimrodMT;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.NimrodMT.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.NimrodMT.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            myFont = new Font(fonts.Families[0], 12.0F);
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            CheckBoxRealmName.Font = myFont;
            CheckboxRealmlist.Font = myFont;
            LanguageTxT.Font = myFont;
            LanguageBoxList.Font = myFont;
            DownloadUALocale.Font = myFont;
            patchClientWoW.Font = myFont;
            LoadConfig();
            GetAvailableLocales();
        }

        private async void closeButton_Click(object sender, EventArgs e)
        {
            Close();
            // Перевірка, чи запущений процес Wow.exe
            if (Process.GetProcessesByName("Wow").Any())
            {
                mainMenu.UpdateDownloadInfoLabel("Гра запущена. Перевірка оновлення скасована.");
                MessageBox.Show("Гра запущена! Закрийте WoW перед оновленням патчів.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                await mainMenu.UpdateWowExecutable();
                await mainMenu.UpdatePatches();
            }
        }

        private void closeButton_MouseDown(object sender, MouseEventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.OK_button_down;
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.OK_button_hover;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.OK_button_base;
        }

        private void closeButton_MouseUp(object sender, MouseEventArgs e)
        {
            closeButton.BackgroundImage = Properties.Resources.OK_button_base;
        }

        private void Settings_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Settings_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Settings_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void CheckBoxRealmName_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxRealmName.CheckState == CheckState.Checked)
                RestoreRealmName();
        }

        private void CheckboxRealmlist_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckboxRealmlist.CheckState == CheckState.Checked)
                mainMenu.CheckRealmlistAndUpdate();
        }

        public void ChangeRealmlist(string wtfpath)
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

        private void RestoreRealmName()
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

                var fileAttributes = File.GetAttributes(path);
                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(path, fileAttributes & ~FileAttributes.ReadOnly);
                }

                // Перезаписуємо файл з модифікованими рядками
                File.WriteAllLines(path, lines);
            }
            catch (Exception e)
            {
                if (CheckBoxRealmName.Checked)
                {
                    MessageBox.Show("Файлу Config.wtf не існує!\nError: " + e.Message, "Помилка", MessageBoxButtons.OK);
                    CheckBoxRealmName.Checked = false;
                }
            }
        }
        private void GetAvailableLocales()
        {
            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataDirectory = Path.Combine(baseDirectory, "Data");
            string wtfDirectory = Path.Combine(baseDirectory, "WTF");
            string configFilePath = Path.Combine(wtfDirectory, "Config.wtf");

            if (!Directory.Exists(dataDirectory))
            {
                throw new DirectoryNotFoundException($"Директорія {dataDirectory} не знайдена.");
            }

            var items = Directory.GetDirectories(dataDirectory)
                .Select(subDir =>
                {
                    string locale = Path.GetFileName(subDir);
                    string localeFile = $"lichking-locale-{locale}.mpq";
                    string fullPath = Path.Combine(subDir, localeFile);

                    if (File.Exists(fullPath))
                    {
                        return new ComboItem
                        {
                            ID = Guid.NewGuid().GetHashCode(), // Унікальний ID для кожного пункту
                            Text = locale                      // Назва субдиректорії (локаль)
                        };
                    }
                    return null;
                })
                .Where(item => item != null)
                .ToArray();

            bool localeSet = false;

            if (File.Exists(configFilePath))
            {
                string[] configLines = File.ReadAllLines(configFilePath);

                string localeLine = configLines.FirstOrDefault(line => line.StartsWith("SET locale "));

                if (!string.IsNullOrEmpty(localeLine))
                {
                    //Console.WriteLine($"Локаль знайдено: {localeLine}");
                    string localeConfig = localeLine.Split('"')[1];

                    var selectedItem = items.FirstOrDefault(item => item.Text.Equals(localeConfig, StringComparison.OrdinalIgnoreCase));
                    if (selectedItem != null)
                    {
                        // Тимчасово відключаємо обробку подій
                        LanguageBoxList.SelectedIndexChanged -= ChangeClientLocale;
                        LanguageBoxList.DataSource = items;
                        LanguageBoxList.SelectedItem = selectedItem;
                        LanguageBoxList.SelectedIndexChanged += ChangeClientLocale;

                        localeSet = true;
                    }
                }
            }

            // Присвоєння першого доступного варіанта, якщо локаль не була встановлена
            if (!localeSet && items.Any())
            {
                LanguageBoxList.SelectedIndexChanged -= ChangeClientLocale;
                LanguageBoxList.DataSource = items;
                LanguageBoxList.SelectedItem = items.First();
                LanguageBoxList.SelectedIndexChanged += ChangeClientLocale;
            }
        }

        private void ChangeClientLocale(object sender, EventArgs e)
        {
            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string wtfDirectory = Path.Combine(baseDirectory, "WTF");
            string configFilePath = Path.Combine(wtfDirectory, "Config.wtf");

            // Перевірка, чи існує файл Config.wtf
            if (!File.Exists(configFilePath))
            {
                // Створення файлу, якщо його немає
                Directory.CreateDirectory(wtfDirectory);
                File.WriteAllText(configFilePath, "");
            }

            // Отримання вибраного пункту меню
            if (LanguageBoxList.SelectedItem is ComboItem selectedItem)
            {
                string selectedLocale = selectedItem.Text;

                // Читання та оновлення файлу Config.wtf
                string[] configLines = File.ReadAllLines(configFilePath);
                bool localeFound = false;

                for (int i = 0; i < configLines.Length; i++)
                {
                    if (configLines[i].StartsWith("SET locale "))
                    {
                        configLines[i] = $"SET locale \"{selectedLocale}\"";
                        localeFound = true;
                        break;
                    }
                }

                // Якщо параметр SET locale відсутній, додаємо його
                if (!localeFound)
                {
                    var newConfigLines = configLines.ToList();
                    newConfigLines.Add($"SET locale \"{selectedLocale}\"");
                    configLines = newConfigLines.ToArray();
                }

                var fileAttributes = File.GetAttributes(configFilePath);
                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(configFilePath, fileAttributes & ~FileAttributes.ReadOnly);
                }

                // Запис оновлених даних назад у файл
                File.WriteAllLines(configFilePath, configLines);
            }
        }

        private void LoadConfig()
        {
            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string LauncherConfigFilePath = Path.Combine(baseDirectory, "Launcher.ini");

            // Перевірка, чи існує файл launcher.ini
            if (!File.Exists(LauncherConfigFilePath))
            {
                // Створюємо новий файл
                SetDefaultLauncherConfig();
            }

            // Читання файлу Launcher.ini
            string[] configLines = File.ReadAllLines(LauncherConfigFilePath);

            // Пошук параметра DownloadUALocale
            string ConfigLine = configLines.FirstOrDefault(line => line.StartsWith("DownloadUALocale "));

            if (!string.IsNullOrEmpty(ConfigLine))
            {
                // Отримання значення параметра DownloadUALocale
                string downloadUALocale = ConfigLine.Split('"')[1];
                // Встановлення значення чекбокса
                DownloadUALocale.Checked = downloadUALocale == "1";
            }

            ConfigLine = null;
            // Пошук параметра PatchClient
            ConfigLine = configLines.FirstOrDefault(line => line.StartsWith("PatchClient "));

            if (!string.IsNullOrEmpty(ConfigLine))
            {
                // Отримання значення параметра PatchClient
                string PatchClient = ConfigLine.Split('"')[1];
                // Встановлення значення чекбокса
                patchClientWoW.Checked = PatchClient == "1";
            }
        }

        private void DownloadUALocaleStateChanged(object sender, EventArgs e)
        {
            // Отримання вибраного пункту меню
            if (DownloadUALocale.CheckState == CheckState.Checked)
            {
                UpdateLauncherConfig("DownloadUALocale", true);
                if (patchClientWoW.CheckState != CheckState.Checked)
                    patchClientWoW.CheckState = CheckState.Checked;
            }
            else
            {
                UpdateLauncherConfig("DownloadUALocale", false);
            }
        }

        private void ConfigPatchClientWoWStateChanged(object sender, EventArgs e)
        {
            // Отримання вибраного пункту меню
            if (patchClientWoW.CheckState == CheckState.Checked)
            {
                UpdateLauncherConfig("PatchClient", true);
            }
            else
            {   //Якщо завантаження UAперекладу увімкнено не можна вимикати оновлення WoW.exe
                if (DownloadUALocale.CheckState != CheckState.Checked)
                    UpdateLauncherConfig("PatchClient", false);
                else
                    patchClientWoW.CheckState = CheckState.Checked;
            }
        }

        public void SetDefaultLauncherConfig()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string launcherConfigFilePath = Path.Combine(baseDirectory, "Launcher.ini");

            // Перевірка, чи існує файл, якщо ні — створюємо
            if (!File.Exists(launcherConfigFilePath))
            {
                try
                {
                    File.WriteAllText(launcherConfigFilePath, ""); // Створюємо пустий файл
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося створити файл Launcher.ini\nПомилка: {ex.Message}",
                                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Вихід у разі помилки
                }
            }

            try
            {
                // Формат запису параметрів у файл: ключ "значення"
                string[] configLines =
                {
                    "DownloadUALocale \"1\"",
                    "PatchClient \"1\"",
                    "Patch-D-Cleanup \"1\""
                 };

                File.WriteAllLines(launcherConfigFilePath, configLines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при записі конфігурації у Launcher.ini\n{ex.Message}",
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateLauncherConfig(string config, bool enabled)
        {
            string state = enabled ? "1" : "0";

            // Основна папка
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string LauncherConfigFilePath = Path.Combine(baseDirectory, "Launcher.ini");

            // Перевірка, чи існує файл launcher.ini
            if (!File.Exists(LauncherConfigFilePath))
            {
                // Створення файлу, якщо його немає
                SetDefaultLauncherConfig();
            }

            // Читання та оновлення файлу Launcher.ini
            string[] configLines = File.ReadAllLines(LauncherConfigFilePath);
            bool LauncherConfigFound = false;

            for (int i = 0; i < configLines.Length; i++)
            {
                // Перевіряємо, чи рядок починається з заданого параметра
                if (configLines[i].StartsWith($"{config} "))
                {
                    // Перезаписуємо рядок
                    configLines[i] = $"{config} \"{state}\"";
                    LauncherConfigFound = true;
                    break;
                }
            }

            // Якщо параметр відсутній, додаємо його
            if (!LauncherConfigFound)
            {
                var newConfigLines = configLines.ToList();
                newConfigLines.Add($"{config} \"{state}\"");
                configLines = newConfigLines.ToArray();
            }

            var fileAttributes = File.GetAttributes(LauncherConfigFilePath);
            if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(LauncherConfigFilePath, fileAttributes & ~FileAttributes.ReadOnly);
            }

            // Запис оновлених даних назад у файл
            File.WriteAllLines(LauncherConfigFilePath, configLines);
        }
    }
}
