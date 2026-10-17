namespace Lab4
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabFiles = new System.Windows.Forms.TabPage();
            this.txtFiles = new System.Windows.Forms.TextBox();
            this.flpFiles = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDrive = new System.Windows.Forms.Label();
            this.cmbDrive = new System.Windows.Forms.ComboBox();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.txtMemory = new System.Windows.Forms.TextBox();
            this.flpMemory = new System.Windows.Forms.FlowLayoutPanel();
            this.tabClipboard = new System.Windows.Forms.TabPage();
            this.lstFormats = new System.Windows.Forms.ListBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.flpClipboard = new System.Windows.Forms.FlowLayoutPanel();
            this.chkTimer = new System.Windows.Forms.CheckBox();
            this.txtClipText = new System.Windows.Forms.TextBox();
            this.timerClipboard = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabFiles.SuspendLayout();
            this.flpFiles.SuspendLayout();
            this.tabMemory.SuspendLayout();
            this.tabClipboard.SuspendLayout();
            this.flpClipboard.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip (пункты меню команд добавляются в MainForm.cs)
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFile, this.mnuFiles, this.mnuMemory, this.mnuClipboard, this.mnuHelp });
            this.menuStrip.Name = "menuStrip";
            this.mnuFile.DropDownItems.Add(this.mnuExit);
            this.mnuFile.Text = "&Файл";
            this.mnuExit.Text = "В&ыход";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            this.mnuFiles.Text = "Файловые &операции";
            this.mnuMemory.Text = "&Память";
            this.mnuClipboard.Text = "&Буфер обмена";
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuHelpContents, this.mnuAbout });
            this.mnuHelp.Text = "&Справка";
            this.mnuHelpContents.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuHelpContents.Text = "&Содержание";
            this.mnuHelpContents.Click += new System.EventHandler(this.mnuHelpContents_Click);
            this.mnuAbout.Text = "&О программе";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            //
            // tabControl
            //
            this.tabControl.Controls.Add(this.tabFiles);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Controls.Add(this.tabClipboard);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Name = "tabControl";
            //
            // tabFiles: слева кнопки п.1–12, справа вывод
            //
            this.tabFiles.Controls.Add(this.txtFiles);
            this.tabFiles.Controls.Add(this.flpFiles);
            this.tabFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiles.Text = "Файловые операции";
            this.tabFiles.UseVisualStyleBackColor = true;
            this.txtFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFiles.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtFiles.Multiline = true;
            this.txtFiles.ReadOnly = true;
            this.txtFiles.BackColor = System.Drawing.Color.White;
            this.txtFiles.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFiles.WordWrap = false;
            this.flpFiles.AutoScroll = true;
            this.flpFiles.Controls.Add(this.lblDrive);
            this.flpFiles.Controls.Add(this.cmbDrive);
            this.flpFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.flpFiles.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpFiles.Size = new System.Drawing.Size(330, 600);
            this.flpFiles.WrapContents = false;
            this.lblDrive.AutoSize = true;
            this.lblDrive.Text = "Диск для п.10–12:";
            this.cmbDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrive.Size = new System.Drawing.Size(300, 23);
            //
            // tabMemory: сверху кнопки, ниже вывод
            //
            this.tabMemory.Controls.Add(this.txtMemory);
            this.tabMemory.Controls.Add(this.flpMemory);
            this.tabMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemory.Text = "Память";
            this.tabMemory.UseVisualStyleBackColor = true;
            this.txtMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMemory.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtMemory.Multiline = true;
            this.txtMemory.ReadOnly = true;
            this.txtMemory.BackColor = System.Drawing.Color.White;
            this.txtMemory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMemory.WordWrap = false;
            this.flpMemory.AutoSize = true;
            this.flpMemory.Dock = System.Windows.Forms.DockStyle.Top;
            //
            // tabClipboard: сверху кнопки, число форматов, ниже список форматов
            //
            this.tabClipboard.Controls.Add(this.lstFormats);
            this.tabClipboard.Controls.Add(this.lblCount);
            this.tabClipboard.Controls.Add(this.flpClipboard);
            this.tabClipboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabClipboard.Text = "Буфер обмена";
            this.tabClipboard.UseVisualStyleBackColor = true;
            this.lstFormats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFormats.Font = new System.Drawing.Font("Consolas", 10F);
            this.lstFormats.IntegralHeight = false;
            this.lblCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCount.Size = new System.Drawing.Size(800, 28);
            this.lblCount.Text = "Всего форматов доступно: —";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.flpClipboard.AutoSize = true;
            this.flpClipboard.Controls.Add(this.chkTimer);
            this.flpClipboard.Controls.Add(this.txtClipText);
            this.flpClipboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkTimer.AutoSize = true;
            this.chkTimer.Margin = new System.Windows.Forms.Padding(3, 7, 12, 3);
            this.chkTimer.Text = "Таймер (1 с)";
            this.chkTimer.CheckedChanged += new System.EventHandler(this.chkTimer_CheckedChanged);
            this.txtClipText.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtClipText.Size = new System.Drawing.Size(220, 23);
            this.txtClipText.Text = "Привет из C#!";
            //
            // timerClipboard
            //
            this.timerClipboard.Interval = 1000;
            this.timerClipboard.Tick += new System.EventHandler(this.timerClipboard_Tick);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Лабораторная работа 4";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabFiles.ResumeLayout(false);
            this.tabFiles.PerformLayout();
            this.flpFiles.ResumeLayout(false);
            this.flpFiles.PerformLayout();
            this.tabMemory.ResumeLayout(false);
            this.tabMemory.PerformLayout();
            this.tabClipboard.ResumeLayout(false);
            this.tabClipboard.PerformLayout();
            this.flpClipboard.ResumeLayout(false);
            this.flpClipboard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuMemory;
        private System.Windows.Forms.ToolStripMenuItem mnuClipboard;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpContents;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabFiles;
        private System.Windows.Forms.TextBox txtFiles;
        private System.Windows.Forms.FlowLayoutPanel flpFiles;
        private System.Windows.Forms.Label lblDrive;
        private System.Windows.Forms.ComboBox cmbDrive;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.TextBox txtMemory;
        private System.Windows.Forms.FlowLayoutPanel flpMemory;
        private System.Windows.Forms.TabPage tabClipboard;
        private System.Windows.Forms.ListBox lstFormats;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.FlowLayoutPanel flpClipboard;
        private System.Windows.Forms.CheckBox chkTimer;
        private System.Windows.Forms.TextBox txtClipText;
        private System.Windows.Forms.Timer timerClipboard;
    }
}
