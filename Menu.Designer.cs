namespace wow_launcher_cs
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.frameBottom = new System.Windows.Forms.Panel();
            this.titleBar = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.launcherLabel = new System.Windows.Forms.Label();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.frameLeft = new System.Windows.Forms.Panel();
            this.frameRight = new System.Windows.Forms.Panel();
            this.bottomBackground = new System.Windows.Forms.Panel();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.DownloadInfoLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.PictureBox();
            this.playButton = new System.Windows.Forms.Button();
            this.topBackground = new System.Windows.Forms.Panel();
            this.discordLinkLabel = new System.Windows.Forms.LinkLabel();
            this.titleBar.SuspendLayout();
            this.bottomBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).BeginInit();
            this.topBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // frameBottom
            // 
            this.frameBottom.BackgroundImage = global::wow_launcher_cs.Properties.Resources.FrameBottom;
            this.frameBottom.Location = new System.Drawing.Point(3, 627);
            this.frameBottom.Name = "frameBottom";
            this.frameBottom.Size = new System.Drawing.Size(802, 4);
            this.frameBottom.TabIndex = 0;
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.Lime;
            this.titleBar.BackgroundImage = global::wow_launcher_cs.Properties.Resources.BorderMain;
            this.titleBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.titleBar.Controls.Add(this.panel1);
            this.titleBar.Controls.Add(this.launcherLabel);
            this.titleBar.Controls.Add(this.minimizeButton);
            this.titleBar.Controls.Add(this.closeButton);
            this.titleBar.Location = new System.Drawing.Point(3, 0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(806, 29);
            this.titleBar.TabIndex = 1;
            this.titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseDown);
            this.titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseMove);
            this.titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseUp);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::wow_launcher_cs.Properties.Resources._128;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Location = new System.Drawing.Point(22, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(20, 20);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseUp);
            // 
            // launcherLabel
            // 
            this.launcherLabel.AutoSize = true;
            this.launcherLabel.BackColor = System.Drawing.Color.Transparent;
            this.launcherLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.launcherLabel.ForeColor = System.Drawing.Color.White;
            this.launcherLabel.Location = new System.Drawing.Point(48, 10);
            this.launcherLabel.Name = "launcherLabel";
            this.launcherLabel.Size = new System.Drawing.Size(187, 13);
            this.launcherLabel.TabIndex = 3;
            this.launcherLabel.Text = "World of Warcraft v3.3.5.12340";
            this.launcherLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseDown);
            this.launcherLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseMove);
            this.launcherLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseUp);
            // 
            // minimizeButton
            // 
            this.minimizeButton.BackgroundImage = global::wow_launcher_cs.Properties.Resources.MinimizeButtonBase;
            this.minimizeButton.FlatAppearance.BorderSize = 0;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.Location = new System.Drawing.Point(757, 5);
            this.minimizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(20, 20);
            this.minimizeButton.TabIndex = 4;
            this.minimizeButton.TabStop = false;
            this.minimizeButton.UseVisualStyleBackColor = true;
            this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
            this.minimizeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minimizeButton_MouseDown);
            this.minimizeButton.MouseEnter += new System.EventHandler(this.minimizeButton_MouseEnter);
            this.minimizeButton.MouseLeave += new System.EventHandler(this.minimizeButton_MouseLeave);
            this.minimizeButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.minimizeButton_MouseUp);
            // 
            // closeButton
            // 
            this.closeButton.BackgroundImage = global::wow_launcher_cs.Properties.Resources.CloseButtonBase;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(779, 5);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(20, 20);
            this.closeButton.TabIndex = 3;
            this.closeButton.TabStop = false;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseDown);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            this.closeButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseUp);
            // 
            // frameLeft
            // 
            this.frameLeft.BackgroundImage = global::wow_launcher_cs.Properties.Resources.FrameLeft;
            this.frameLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.frameLeft.Location = new System.Drawing.Point(3, 0);
            this.frameLeft.Name = "frameLeft";
            this.frameLeft.Size = new System.Drawing.Size(3, 633);
            this.frameLeft.TabIndex = 0;
            // 
            // frameRight
            // 
            this.frameRight.BackgroundImage = global::wow_launcher_cs.Properties.Resources.FrameRight;
            this.frameRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.frameRight.Location = new System.Drawing.Point(804, 0);
            this.frameRight.Name = "frameRight";
            this.frameRight.Size = new System.Drawing.Size(3, 633);
            this.frameRight.TabIndex = 1;
            // 
            // bottomBackground
            // 
            this.bottomBackground.BackColor = System.Drawing.Color.White;
            this.bottomBackground.BackgroundImage = global::wow_launcher_cs.Properties.Resources.BottomUpdateLauncher;
            this.bottomBackground.Controls.Add(this.SettingsButton);
            this.bottomBackground.Controls.Add(this.DownloadInfoLabel);
            this.bottomBackground.Controls.Add(this.progressBar);
            this.bottomBackground.Controls.Add(this.playButton);
            this.bottomBackground.Location = new System.Drawing.Point(3, 509);
            this.bottomBackground.Name = "bottomBackground";
            this.bottomBackground.Size = new System.Drawing.Size(806, 124);
            this.bottomBackground.TabIndex = 0;
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackColor = System.Drawing.Color.Transparent;
            this.SettingsButton.BackgroundImage = global::wow_launcher_cs.Properties.Resources.config_button_base;
            this.SettingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SettingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsButton.Location = new System.Drawing.Point(22, 60);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(105, 38);
            this.SettingsButton.TabIndex = 2;
            this.SettingsButton.TabStop = false;
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click_1);
            this.SettingsButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SettingsButton_MouseDown);
            this.SettingsButton.MouseEnter += new System.EventHandler(this.SettingsButton_MouseEnter);
            this.SettingsButton.MouseLeave += new System.EventHandler(this.SettingsButton_MouseLeave);
            this.SettingsButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SettingsButton_MouseUp);
            // 
            // DownloadInfoLabel
            // 
            this.DownloadInfoLabel.AutoSize = true;
            this.DownloadInfoLabel.BackColor = System.Drawing.Color.Transparent;
            this.DownloadInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.DownloadInfoLabel.ForeColor = System.Drawing.Color.White;
            this.DownloadInfoLabel.Location = new System.Drawing.Point(23, 29);
            this.DownloadInfoLabel.Name = "DownloadInfoLabel";
            this.DownloadInfoLabel.Size = new System.Drawing.Size(109, 20);
            this.DownloadInfoLabel.TabIndex = 4;
            this.DownloadInfoLabel.Text = "DynamicTXT";
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Transparent;
            this.progressBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.progressBar.Location = new System.Drawing.Point(27, 6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(610, 20);
            this.progressBar.TabIndex = 0;
            this.progressBar.TabStop = false;
            // 
            // playButton
            // 
            this.playButton.BackgroundImage = global::wow_launcher_cs.Properties.Resources.PlayButtonBase;
            this.playButton.Enabled = false;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Location = new System.Drawing.Point(678, 23);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(102, 75);
            this.playButton.TabIndex = 0;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.EnabledChanged += new System.EventHandler(this.playButton_EnabledChanged);
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            this.playButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.playButton_MouseDown);
            this.playButton.MouseEnter += new System.EventHandler(this.playButton_MouseEnter);
            this.playButton.MouseLeave += new System.EventHandler(this.playButton_MouseLeave);
            this.playButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.playButton_MouseUp);
            // 
            // topBackground
            // 
            this.topBackground.BackgroundImage = global::wow_launcher_cs.Properties.Resources.MainWindowImagePatch;
            this.topBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.topBackground.Controls.Add(this.discordLinkLabel);
            this.topBackground.Location = new System.Drawing.Point(3, 29);
            this.topBackground.Name = "topBackground";
            this.topBackground.Size = new System.Drawing.Size(806, 480);
            this.topBackground.TabIndex = 2;
            // 
            // discordLinkLabel
            // 
            this.discordLinkLabel.ActiveLinkColor = System.Drawing.Color.White;
            this.discordLinkLabel.AutoSize = true;
            this.discordLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.discordLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.discordLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.discordLinkLabel.LinkColor = System.Drawing.Color.White;
            this.discordLinkLabel.Location = new System.Drawing.Point(56, 274);
            this.discordLinkLabel.MinimumSize = new System.Drawing.Size(200, 150);
            this.discordLinkLabel.Name = "discordLinkLabel";
            this.discordLinkLabel.Size = new System.Drawing.Size(200, 150);
            this.discordLinkLabel.TabIndex = 0;
            this.discordLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            this.discordLinkLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnLinkAreaClicked);
            this.discordLinkLabel.MouseEnter += new System.EventHandler(this.discordLinkLabel_MouseEnter);
            this.discordLinkLabel.MouseLeave += new System.EventHandler(this.discordLinkLabel_MouseLeave);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(812, 639);
            this.ControlBox = false;
            this.Controls.Add(this.frameBottom);
            this.Controls.Add(this.titleBar);
            this.Controls.Add(this.frameLeft);
            this.Controls.Add(this.frameRight);
            this.Controls.Add(this.bottomBackground);
            this.Controls.Add(this.topBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Menu_FormClosed);
            this.Load += new System.EventHandler(this.Menu_Load);
            this.Shown += new System.EventHandler(this.Menu_Shown);
            this.titleBar.ResumeLayout(false);
            this.titleBar.PerformLayout();
            this.bottomBackground.ResumeLayout(false);
            this.bottomBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).EndInit();
            this.topBackground.ResumeLayout(false);
            this.topBackground.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel titleBar;
        private System.Windows.Forms.Panel topBackground;
        private System.Windows.Forms.Panel frameLeft;
        private System.Windows.Forms.Panel frameRight;
        private System.Windows.Forms.Panel frameBottom;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.Label launcherLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.PictureBox progressBar;
        private System.Windows.Forms.Panel bottomBackground;
        private System.Windows.Forms.Label DownloadInfoLabel;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.LinkLabel discordLinkLabel;
    }
}