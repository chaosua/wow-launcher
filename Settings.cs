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

        private void Налаштування_Load(object sender, EventArgs e)
        {
            CheckBoxRealmName.Font = myFont;
            CheckboxRealmlist.Font = myFont;
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
            mainMenu.CheckkRealmlistAndUpdate();
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
                    MessageBox.Show("Файлу Config.wtf не існує!");
                    CheckBoxRealmName.Checked = false;

                }

            }
        }

        
    }
    }
