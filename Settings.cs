using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            myFont = new Font(fonts.Families[0], 14.0F);
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            CheckBoxRealmName.Font = myFont;
            CheckboxRealmlist.Font = myFont;
            LanguageTxT.Font = myFont;
            LanguageBoxList.Font = myFont;

            GetAvailableLocales();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
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
            ChangeRealmName();
        }

        private void CheckboxRealmlist_CheckedChanged(object sender, EventArgs e)
        {
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
            catch (Exception e)
            {
                if (CheckBoxRealmName.Checked)
                {
                    MessageBox.Show("Файлу Config.wtf не існує!\nError: " + e.Message, "Error", MessageBoxButtons.OK);
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

            // Перевірка, чи існує директорія Data
            if (!Directory.Exists(dataDirectory))
            {
                throw new DirectoryNotFoundException($"Директорія {dataDirectory} не знайдена.");
            }

            // Список пунктів меню
            var items = Directory.GetDirectories(dataDirectory)
                .Where(subDir => File.Exists(Path.Combine(subDir, "realmlist.wtf")))
                .Select(subDir => new ComboItem
                {
                    ID = Guid.NewGuid().GetHashCode(), // Унікальний ID для кожного пункту
                    Text = Path.GetFileName(subDir) // Назва субдиректорії
                })
                .ToArray();

            // Присвоєння пунктів до меню
            LanguageBoxList.DataSource = items;

            // Перевірка наявності файлу Config.wtf
            if (File.Exists(configFilePath))
            {
                // Читання файлу Config.wtf
                string[] configLines = File.ReadAllLines(configFilePath);
                string localeLine = configLines.FirstOrDefault(line => line.StartsWith("SET locale "));

                if (!string.IsNullOrEmpty(localeLine))
                {
                    // Отримання значення локалі з лінійки SET locale ""
                    string locale = localeLine.Split('"')[1];

                    // Встановлення активного пункту меню
                    var selectedItem = items.FirstOrDefault(item => item.Text.Equals(locale, StringComparison.OrdinalIgnoreCase));
                    if (selectedItem != null)
                    {
                        LanguageBoxList.SelectedItem = selectedItem;
                    }
                }
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
                throw new FileNotFoundException($"Файл {configFilePath} не знайдений.");
            }

            // Отримання вибраного пункту меню
            if (LanguageBoxList.SelectedItem is ComboItem selectedItem)
            {
                string selectedLocale = selectedItem.Text;

                // Читання та оновлення файлу Config.wtf
                string[] configLines = File.ReadAllLines(configFilePath);
                for (int i = 0; i < configLines.Length; i++)
                {
                    if (configLines[i].StartsWith("SET locale "))
                    {
                        configLines[i] = $"SET locale \"{selectedLocale}\"";
                        break;
                    }
                }

                // Запис оновлених даних назад у файл
                File.WriteAllLines(configFilePath, configLines);
            }
        }
    }
}
