namespace wow_launcher_cs
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.closeButton = new System.Windows.Forms.Button();
            this.CheckBoxRealmName = new System.Windows.Forms.CheckBox();
            this.CheckboxRealmlist = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.BackgroundImage = global::wow_launcher_cs.Properties.Resources.OK_button_base;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(196, 198);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(88, 33);
            this.closeButton.TabIndex = 4;
            this.closeButton.TabStop = false;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseDown);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            this.closeButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseUp);
            // 
            // CheckBoxRealmName
            // 
            this.CheckBoxRealmName.BackColor = System.Drawing.Color.Transparent;
            this.CheckBoxRealmName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckBoxRealmName.ForeColor = System.Drawing.SystemColors.Menu;
            this.CheckBoxRealmName.Location = new System.Drawing.Point(36, 43);
            this.CheckBoxRealmName.Name = "CheckBoxRealmName";
            this.CheckBoxRealmName.Size = new System.Drawing.Size(420, 52);
            this.CheckBoxRealmName.TabIndex = 5;
            this.CheckBoxRealmName.Text = "Виправити проблему з входом на реалм";
            this.CheckBoxRealmName.UseVisualStyleBackColor = false;
            this.CheckBoxRealmName.CheckedChanged += new System.EventHandler(this.CheckBoxRealmName_CheckedChanged);
            // 
            // CheckboxRealmlist
            // 
            this.CheckboxRealmlist.BackColor = System.Drawing.Color.Transparent;
            this.CheckboxRealmlist.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckboxRealmlist.ForeColor = System.Drawing.SystemColors.Menu;
            this.CheckboxRealmlist.Location = new System.Drawing.Point(36, 101);
            this.CheckboxRealmlist.Name = "CheckboxRealmlist";
            this.CheckboxRealmlist.Size = new System.Drawing.Size(420, 37);
            this.CheckboxRealmlist.TabIndex = 6;
            this.CheckboxRealmlist.Text = "Відновити realmlist.wtf";
            this.CheckboxRealmlist.UseVisualStyleBackColor = false;
            this.CheckboxRealmlist.CheckedChanged += new System.EventHandler(this.CheckboxRealmlist_CheckedChanged);
            // 
            // Settings
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(487, 253);
            this.Controls.Add(this.CheckboxRealmlist);
            this.Controls.Add(this.CheckBoxRealmName);
            this.Controls.Add(this.closeButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Settings_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox CheckBoxRealmName;
        public System.Windows.Forms.CheckBox CheckboxRealmlist;
    }
}