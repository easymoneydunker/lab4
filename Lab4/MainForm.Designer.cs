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
            this.mnuRunAsAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSysInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVirtualQuery = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemMap = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMemClear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuClipWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipWriteCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipRead = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClipEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCurDir = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabFiles = new System.Windows.Forms.TabPage();
            this.splitFiles = new System.Windows.Forms.SplitContainer();
            this.tvStructure = new System.Windows.Forms.TreeView();
            this.lblTree = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.flpSteps = new System.Windows.Forms.FlowLayoutPanel();
            this.grpParams = new System.Windows.Forms.GroupBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpCreation = new System.Windows.Forms.DateTimePicker();
            this.lblDrive = new System.Windows.Forms.Label();
            this.cmbDrive = new System.Windows.Forms.ComboBox();
            this.lblLabel = new System.Windows.Forms.Label();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.splitMem = new System.Windows.Forms.SplitContainer();
            this.txtMemo = new System.Windows.Forms.TextBox();
            this.lvRegions = new System.Windows.Forms.ListView();
            this.colBase = new System.Windows.Forms.ColumnHeader();
            this.colAllocBase = new System.Windows.Forms.ColumnHeader();
            this.colAllocProtect = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colState = new System.Windows.Forms.ColumnHeader();
            this.colProtect = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colModule = new System.Windows.Forms.ColumnHeader();
            this.flpMemTop = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSysInfo = new System.Windows.Forms.Button();
            this.btnMemStatus = new System.Windows.Forms.Button();
            this.lblAddrPreset = new System.Windows.Forms.Label();
            this.cmbAddrPreset = new System.Windows.Forms.ComboBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnVirtualQuery = new System.Windows.Forms.Button();
            this.btnMemMap = new System.Windows.Forms.Button();
            this.btnMemClear = new System.Windows.Forms.Button();
            this.tabClipboard = new System.Windows.Forms.TabPage();
            this.splitClip = new System.Windows.Forms.SplitContainer();
            this.lstFormats = new System.Windows.Forms.ListBox();
            this.txtClipOut = new System.Windows.Forms.TextBox();
            this.lblOwner = new System.Windows.Forms.Label();
            this.lblFormatCount = new System.Windows.Forms.Label();
            this.flpClipTop = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClipRefresh = new System.Windows.Forms.Button();
            this.chkClipTimer = new System.Windows.Forms.CheckBox();
            this.lblClipText = new System.Windows.Forms.Label();
            this.txtClipText = new System.Windows.Forms.TextBox();
            this.btnClipWrite = new System.Windows.Forms.Button();
            this.btnClipWriteCustom = new System.Windows.Forms.Button();
            this.btnClipRead = new System.Windows.Forms.Button();
            this.btnClipEmpty = new System.Windows.Forms.Button();
            this.timerClipboard = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitFiles)).BeginInit();
            this.splitFiles.Panel1.SuspendLayout();
            this.splitFiles.Panel2.SuspendLayout();
            this.splitFiles.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.grpParams.SuspendLayout();
            this.tabMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMem)).BeginInit();
            this.splitMem.Panel1.SuspendLayout();
            this.splitMem.Panel2.SuspendLayout();
            this.splitMem.SuspendLayout();
            this.flpMemTop.SuspendLayout();
            this.tabClipboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitClip)).BeginInit();
            this.splitClip.Panel1.SuspendLayout();
            this.splitClip.Panel2.SuspendLayout();
            this.splitClip.SuspendLayout();
            this.flpClipTop.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFile, this.mnuFiles, this.mnuMemory, this.mnuClipboard, this.mnuHelp });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip.TabIndex = 0;
            //
            // mnuFile
            //
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuRunAsAdmin, this.mnuFileSep, this.mnuExit });
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Text = "&Файл";
            this.mnuRunAsAdmin.Name = "mnuRunAsAdmin";
            this.mnuRunAsAdmin.Text = "Перезапустить от имени &администратора";
            this.mnuRunAsAdmin.Click += new System.EventHandler(this.mnuRunAsAdmin_Click);
            this.mnuFileSep.Name = "mnuFileSep";
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;
            this.mnuExit.Text = "В&ыход";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            //
            // mnuFiles (пункты добавляются в конструкторе формы)
            //
            this.mnuFiles.Name = "mnuFiles";
            this.mnuFiles.Text = "Файловые &операции";
            //
            // mnuMemory
            //
            this.mnuMemory.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuSysInfo, this.mnuMemStatus, this.mnuVirtualQuery, this.mnuMemMap,
                this.mnuMemSep, this.mnuMemClear });
            this.mnuMemory.Name = "mnuMemory";
            this.mnuMemory.Text = "&Память";
            this.mnuSysInfo.Name = "mnuSysInfo";
            this.mnuSysInfo.Text = "1) Информация о системе (GetSystemInfo)";
            this.mnuSysInfo.Click += new System.EventHandler(this.btnSysInfo_Click);
            this.mnuMemStatus.Name = "mnuMemStatus";
            this.mnuMemStatus.Text = "2) Состояние памяти (GlobalMemoryStatus)";
            this.mnuMemStatus.Click += new System.EventHandler(this.btnMemStatus_Click);
            this.mnuVirtualQuery.Name = "mnuVirtualQuery";
            this.mnuVirtualQuery.Text = "3) Запрос по адресу (VirtualQuery)";
            this.mnuVirtualQuery.Click += new System.EventHandler(this.btnVirtualQuery_Click);
            this.mnuMemMap.Name = "mnuMemMap";
            this.mnuMemMap.Text = "3) Карта виртуального адресного пространства (VirtualQuery)";
            this.mnuMemMap.Click += new System.EventHandler(this.btnMemMap_Click);
            this.mnuMemSep.Name = "mnuMemSep";
            this.mnuMemClear.Name = "mnuMemClear";
            this.mnuMemClear.Text = "Очистить";
            this.mnuMemClear.Click += new System.EventHandler(this.btnMemClear_Click);
            //
            // mnuClipboard
            //
            this.mnuClipboard.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuClipRefresh, this.mnuClipTimer, this.mnuClipSep, this.mnuClipWrite,
                this.mnuClipWriteCustom, this.mnuClipRead, this.mnuClipEmpty });
            this.mnuClipboard.Name = "mnuClipboard";
            this.mnuClipboard.Text = "&Буфер обмена";
            this.mnuClipRefresh.Name = "mnuClipRefresh";
            this.mnuClipRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuClipRefresh.Text = "Определить форматы буфера обмена";
            this.mnuClipRefresh.Click += new System.EventHandler(this.btnClipRefresh_Click);
            this.mnuClipTimer.CheckOnClick = true;
            this.mnuClipTimer.Name = "mnuClipTimer";
            this.mnuClipTimer.Text = "Автообновление по таймеру";
            this.mnuClipTimer.CheckedChanged += new System.EventHandler(this.mnuClipTimer_CheckedChanged);
            this.mnuClipSep.Name = "mnuClipSep";
            this.mnuClipWrite.Name = "mnuClipWrite";
            this.mnuClipWrite.Text = "Поместить текст (SetClipboardData)";
            this.mnuClipWrite.Click += new System.EventHandler(this.btnClipWrite_Click);
            this.mnuClipWriteCustom.Name = "mnuClipWriteCustom";
            this.mnuClipWriteCustom.Text = "Поместить текст + собственный формат (RegisterClipboardFormat)";
            this.mnuClipWriteCustom.Click += new System.EventHandler(this.btnClipWriteCustom_Click);
            this.mnuClipRead.Name = "mnuClipRead";
            this.mnuClipRead.Text = "Прочитать текст (GetClipboardData)";
            this.mnuClipRead.Click += new System.EventHandler(this.btnClipRead_Click);
            this.mnuClipEmpty.Name = "mnuClipEmpty";
            this.mnuClipEmpty.Text = "Очистить буфер обмена (EmptyClipboard)";
            this.mnuClipEmpty.Click += new System.EventHandler(this.btnClipEmpty_Click);
            //
            // mnuHelp
            //
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuHelpContents, this.mnuHelpSep, this.mnuAbout });
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Text = "&Справка";
            this.mnuHelpContents.Name = "mnuHelpContents";
            this.mnuHelpContents.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuHelpContents.Text = "&Содержание";
            this.mnuHelpContents.Click += new System.EventHandler(this.mnuHelpContents_Click);
            this.mnuHelpSep.Name = "mnuHelpSep";
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Text = "&О программе";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus, this.lblCurDir });
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip.TabIndex = 2;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Готово";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurDir.Name = "lblCurDir";
            this.lblCurDir.Text = "Текущий каталог:";
            //
            // tabControl
            //
            this.tabControl.Controls.Add(this.tabFiles);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Controls.Add(this.tabClipboard);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1184, 715);
            this.tabControl.TabIndex = 1;
            //
            // tabFiles
            //
            this.tabFiles.Controls.Add(this.splitFiles);
            this.tabFiles.Controls.Add(this.pnlLeft);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiles.Text = "4.2.1. Файловые операции";
            this.tabFiles.UseVisualStyleBackColor = true;
            //
            // splitFiles
            //
            this.splitFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitFiles.Name = "splitFiles";
            this.splitFiles.Panel1.Controls.Add(this.tvStructure);
            this.splitFiles.Panel1.Controls.Add(this.lblTree);
            this.splitFiles.Panel2.Controls.Add(this.txtLog);
            this.splitFiles.Size = new System.Drawing.Size(840, 683);
            this.splitFiles.SplitterDistance = 280;
            this.splitFiles.TabIndex = 1;
            //
            // tvStructure
            //
            this.tvStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvStructure.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tvStructure.Name = "tvStructure";
            this.tvStructure.TabIndex = 1;
            //
            // lblTree
            //
            this.lblTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTree.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTree.Name = "lblTree";
            this.lblTree.Size = new System.Drawing.Size(280, 22);
            this.lblTree.Text = "Каталожная структура";
            this.lblTree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtLog
            //
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtLog.MaxLength = 0;
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            //
            // pnlLeft
            //
            this.pnlLeft.Controls.Add(this.flpSteps);
            this.pnlLeft.Controls.Add(this.grpParams);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.pnlLeft.Size = new System.Drawing.Size(330, 683);
            this.pnlLeft.TabIndex = 0;
            //
            // flpSteps (кнопки п.1–12 создаются в конструкторе формы)
            //
            this.flpSteps.AutoScroll = true;
            this.flpSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSteps.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpSteps.Name = "flpSteps";
            this.flpSteps.TabIndex = 1;
            this.flpSteps.WrapContents = false;
            //
            // grpParams
            //
            this.grpParams.Controls.Add(this.lblDate);
            this.grpParams.Controls.Add(this.dtpCreation);
            this.grpParams.Controls.Add(this.lblDrive);
            this.grpParams.Controls.Add(this.cmbDrive);
            this.grpParams.Controls.Add(this.lblLabel);
            this.grpParams.Controls.Add(this.txtLabel);
            this.grpParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpParams.Name = "grpParams";
            this.grpParams.Size = new System.Drawing.Size(324, 116);
            this.grpParams.TabIndex = 0;
            this.grpParams.TabStop = false;
            this.grpParams.Text = "Параметры";
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(8, 24);
            this.lblDate.Name = "lblDate";
            this.lblDate.Text = "Дата создания (п.7):";
            //
            // dtpCreation
            //
            this.dtpCreation.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpCreation.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCreation.Location = new System.Drawing.Point(140, 20);
            this.dtpCreation.Name = "dtpCreation";
            this.dtpCreation.Size = new System.Drawing.Size(176, 23);
            this.dtpCreation.TabIndex = 0;
            this.dtpCreation.Value = new System.DateTime(2001, 1, 1, 10, 0, 0, 0);
            //
            // lblDrive
            //
            this.lblDrive.AutoSize = true;
            this.lblDrive.Location = new System.Drawing.Point(8, 55);
            this.lblDrive.Name = "lblDrive";
            this.lblDrive.Text = "Диск (п.10–12):";
            //
            // cmbDrive
            //
            this.cmbDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrive.Location = new System.Drawing.Point(140, 51);
            this.cmbDrive.Name = "cmbDrive";
            this.cmbDrive.Size = new System.Drawing.Size(176, 23);
            this.cmbDrive.TabIndex = 1;
            //
            // lblLabel
            //
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(8, 86);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Text = "Метка тома (п.10):";
            //
            // txtLabel
            //
            this.txtLabel.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLabel.Location = new System.Drawing.Point(140, 82);
            this.txtLabel.MaxLength = 32;
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(176, 23);
            this.txtLabel.TabIndex = 2;
            this.txtLabel.Text = "LABEL";
            //
            // tabMemory
            //
            this.tabMemory.Controls.Add(this.splitMem);
            this.tabMemory.Controls.Add(this.flpMemTop);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemory.Text = "4.2.2. Память";
            this.tabMemory.UseVisualStyleBackColor = true;
            //
            // splitMem
            //
            this.splitMem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMem.Name = "splitMem";
            this.splitMem.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitMem.Panel1.Controls.Add(this.txtMemo);
            this.splitMem.Panel2.Controls.Add(this.lvRegions);
            this.splitMem.Size = new System.Drawing.Size(1170, 640);
            this.splitMem.SplitterDistance = 300;
            this.splitMem.TabIndex = 1;
            //
            // txtMemo
            //
            this.txtMemo.BackColor = System.Drawing.Color.White;
            this.txtMemo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMemo.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtMemo.MaxLength = 0;
            this.txtMemo.Multiline = true;
            this.txtMemo.Name = "txtMemo";
            this.txtMemo.ReadOnly = true;
            this.txtMemo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMemo.TabIndex = 0;
            this.txtMemo.WordWrap = false;
            //
            // lvRegions
            //
            this.lvRegions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colBase, this.colAllocBase, this.colAllocProtect, this.colSize,
                this.colState, this.colProtect, this.colType, this.colModule });
            this.lvRegions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRegions.Font = new System.Drawing.Font("Consolas", 9F);
            this.lvRegions.FullRowSelect = true;
            this.lvRegions.GridLines = true;
            this.lvRegions.HideSelection = false;
            this.lvRegions.Name = "lvRegions";
            this.lvRegions.TabIndex = 0;
            this.lvRegions.UseCompatibleStateImageBehavior = false;
            this.lvRegions.View = System.Windows.Forms.View.Details;
            this.colBase.Text = "BaseAddress";
            this.colBase.Width = 140;
            this.colAllocBase.Text = "AllocationBase";
            this.colAllocBase.Width = 140;
            this.colAllocProtect.Text = "AllocationProtect";
            this.colAllocProtect.Width = 150;
            this.colSize.Text = "RegionSize";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 150;
            this.colState.Text = "State";
            this.colState.Width = 100;
            this.colProtect.Text = "Protect";
            this.colProtect.Width = 170;
            this.colType.Text = "Type";
            this.colType.Width = 100;
            this.colModule.Text = "Образ / файл";
            this.colModule.Width = 220;
            //
            // flpMemTop
            //
            this.flpMemTop.AutoSize = true;
            this.flpMemTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpMemTop.Controls.Add(this.btnSysInfo);
            this.flpMemTop.Controls.Add(this.btnMemStatus);
            this.flpMemTop.Controls.Add(this.lblAddrPreset);
            this.flpMemTop.Controls.Add(this.cmbAddrPreset);
            this.flpMemTop.Controls.Add(this.lblAddress);
            this.flpMemTop.Controls.Add(this.txtAddress);
            this.flpMemTop.Controls.Add(this.btnVirtualQuery);
            this.flpMemTop.Controls.Add(this.btnMemMap);
            this.flpMemTop.Controls.Add(this.btnMemClear);
            this.flpMemTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpMemTop.Name = "flpMemTop";
            this.flpMemTop.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.flpMemTop.TabIndex = 0;
            //
            // btnSysInfo
            //
            this.btnSysInfo.AutoSize = true;
            this.btnSysInfo.Name = "btnSysInfo";
            this.btnSysInfo.TabIndex = 0;
            this.btnSysInfo.Text = "1) GetSystemInfo";
            this.btnSysInfo.UseVisualStyleBackColor = true;
            this.btnSysInfo.Click += new System.EventHandler(this.btnSysInfo_Click);
            //
            // btnMemStatus
            //
            this.btnMemStatus.AutoSize = true;
            this.btnMemStatus.Name = "btnMemStatus";
            this.btnMemStatus.TabIndex = 1;
            this.btnMemStatus.Text = "2) GlobalMemoryStatus";
            this.btnMemStatus.UseVisualStyleBackColor = true;
            this.btnMemStatus.Click += new System.EventHandler(this.btnMemStatus_Click);
            //
            // lblAddrPreset
            //
            this.lblAddrPreset.AutoSize = true;
            this.lblAddrPreset.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.lblAddrPreset.Name = "lblAddrPreset";
            this.lblAddrPreset.Text = "lpAddress:";
            //
            // cmbAddrPreset
            //
            this.cmbAddrPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddrPreset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.cmbAddrPreset.Name = "cmbAddrPreset";
            this.cmbAddrPreset.Size = new System.Drawing.Size(300, 23);
            this.cmbAddrPreset.TabIndex = 2;
            this.cmbAddrPreset.SelectedIndexChanged += new System.EventHandler(this.cmbAddrPreset_SelectedIndexChanged);
            //
            // lblAddress
            //
            this.lblAddress.AutoSize = true;
            this.lblAddress.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Text = "$";
            //
            // txtAddress
            //
            this.txtAddress.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(0, 4, 3, 3);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(150, 23);
            this.txtAddress.TabIndex = 3;
            //
            // btnVirtualQuery
            //
            this.btnVirtualQuery.AutoSize = true;
            this.btnVirtualQuery.Name = "btnVirtualQuery";
            this.btnVirtualQuery.TabIndex = 4;
            this.btnVirtualQuery.Text = "3) VirtualQuery";
            this.btnVirtualQuery.UseVisualStyleBackColor = true;
            this.btnVirtualQuery.Click += new System.EventHandler(this.btnVirtualQuery_Click);
            //
            // btnMemMap
            //
            this.btnMemMap.AutoSize = true;
            this.btnMemMap.Name = "btnMemMap";
            this.btnMemMap.TabIndex = 5;
            this.btnMemMap.Text = "3) Карта адресного пространства";
            this.btnMemMap.UseVisualStyleBackColor = true;
            this.btnMemMap.Click += new System.EventHandler(this.btnMemMap_Click);
            //
            // btnMemClear
            //
            this.btnMemClear.AutoSize = true;
            this.btnMemClear.Name = "btnMemClear";
            this.btnMemClear.TabIndex = 6;
            this.btnMemClear.Text = "Очистить";
            this.btnMemClear.UseVisualStyleBackColor = true;
            this.btnMemClear.Click += new System.EventHandler(this.btnMemClear_Click);
            //
            // tabClipboard
            //
            this.tabClipboard.Controls.Add(this.splitClip);
            this.tabClipboard.Controls.Add(this.lblOwner);
            this.tabClipboard.Controls.Add(this.lblFormatCount);
            this.tabClipboard.Controls.Add(this.flpClipTop);
            this.tabClipboard.Name = "tabClipboard";
            this.tabClipboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabClipboard.Text = "4.2.3. Буфер обмена";
            this.tabClipboard.UseVisualStyleBackColor = true;
            //
            // splitClip
            //
            this.splitClip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitClip.Name = "splitClip";
            this.splitClip.Panel1.Controls.Add(this.lstFormats);
            this.splitClip.Panel2.Controls.Add(this.txtClipOut);
            this.splitClip.Size = new System.Drawing.Size(1170, 600);
            this.splitClip.SplitterDistance = 560;
            this.splitClip.TabIndex = 3;
            //
            // lstFormats
            //
            this.lstFormats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFormats.Font = new System.Drawing.Font("Consolas", 10F);
            this.lstFormats.IntegralHeight = false;
            this.lstFormats.Name = "lstFormats";
            this.lstFormats.TabIndex = 0;
            //
            // txtClipOut
            //
            this.txtClipOut.BackColor = System.Drawing.Color.White;
            this.txtClipOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClipOut.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtClipOut.MaxLength = 0;
            this.txtClipOut.Multiline = true;
            this.txtClipOut.Name = "txtClipOut";
            this.txtClipOut.ReadOnly = true;
            this.txtClipOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtClipOut.TabIndex = 0;
            this.txtClipOut.WordWrap = false;
            //
            // lblOwner
            //
            this.lblOwner.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(1170, 22);
            this.lblOwner.Text = "Владелец буфера обмена: —";
            this.lblOwner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFormatCount
            //
            this.lblFormatCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFormatCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFormatCount.Name = "lblFormatCount";
            this.lblFormatCount.Size = new System.Drawing.Size(1170, 26);
            this.lblFormatCount.Text = "Всего форматов доступно: —";
            this.lblFormatCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // flpClipTop
            //
            this.flpClipTop.AutoSize = true;
            this.flpClipTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpClipTop.Controls.Add(this.btnClipRefresh);
            this.flpClipTop.Controls.Add(this.chkClipTimer);
            this.flpClipTop.Controls.Add(this.lblClipText);
            this.flpClipTop.Controls.Add(this.txtClipText);
            this.flpClipTop.Controls.Add(this.btnClipWrite);
            this.flpClipTop.Controls.Add(this.btnClipWriteCustom);
            this.flpClipTop.Controls.Add(this.btnClipRead);
            this.flpClipTop.Controls.Add(this.btnClipEmpty);
            this.flpClipTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpClipTop.Name = "flpClipTop";
            this.flpClipTop.TabIndex = 0;
            //
            // btnClipRefresh
            //
            this.btnClipRefresh.AutoSize = true;
            this.btnClipRefresh.Name = "btnClipRefresh";
            this.btnClipRefresh.TabIndex = 0;
            this.btnClipRefresh.Text = "Определить форматы (F5)";
            this.btnClipRefresh.UseVisualStyleBackColor = true;
            this.btnClipRefresh.Click += new System.EventHandler(this.btnClipRefresh_Click);
            //
            // chkClipTimer
            //
            this.chkClipTimer.AutoSize = true;
            this.chkClipTimer.Margin = new System.Windows.Forms.Padding(8, 7, 3, 3);
            this.chkClipTimer.Name = "chkClipTimer";
            this.chkClipTimer.TabIndex = 1;
            this.chkClipTimer.Text = "Таймер (1 с)";
            this.chkClipTimer.UseVisualStyleBackColor = true;
            this.chkClipTimer.CheckedChanged += new System.EventHandler(this.chkClipTimer_CheckedChanged);
            //
            // lblClipText
            //
            this.lblClipText.AutoSize = true;
            this.lblClipText.Margin = new System.Windows.Forms.Padding(16, 8, 0, 0);
            this.lblClipText.Name = "lblClipText";
            this.lblClipText.Text = "Текст:";
            //
            // txtClipText
            //
            this.txtClipText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.txtClipText.Name = "txtClipText";
            this.txtClipText.Size = new System.Drawing.Size(220, 23);
            this.txtClipText.TabIndex = 2;
            this.txtClipText.Text = "Привет из Win32 API!";
            //
            // btnClipWrite
            //
            this.btnClipWrite.AutoSize = true;
            this.btnClipWrite.Name = "btnClipWrite";
            this.btnClipWrite.TabIndex = 3;
            this.btnClipWrite.Text = "SetClipboardData";
            this.btnClipWrite.UseVisualStyleBackColor = true;
            this.btnClipWrite.Click += new System.EventHandler(this.btnClipWrite_Click);
            //
            // btnClipWriteCustom
            //
            this.btnClipWriteCustom.AutoSize = true;
            this.btnClipWriteCustom.Name = "btnClipWriteCustom";
            this.btnClipWriteCustom.TabIndex = 4;
            this.btnClipWriteCustom.Text = "+ собственный формат";
            this.btnClipWriteCustom.UseVisualStyleBackColor = true;
            this.btnClipWriteCustom.Click += new System.EventHandler(this.btnClipWriteCustom_Click);
            //
            // btnClipRead
            //
            this.btnClipRead.AutoSize = true;
            this.btnClipRead.Name = "btnClipRead";
            this.btnClipRead.TabIndex = 5;
            this.btnClipRead.Text = "GetClipboardData";
            this.btnClipRead.UseVisualStyleBackColor = true;
            this.btnClipRead.Click += new System.EventHandler(this.btnClipRead_Click);
            //
            // btnClipEmpty
            //
            this.btnClipEmpty.AutoSize = true;
            this.btnClipEmpty.Name = "btnClipEmpty";
            this.btnClipEmpty.TabIndex = 6;
            this.btnClipEmpty.Text = "EmptyClipboard";
            this.btnClipEmpty.UseVisualStyleBackColor = true;
            this.btnClipEmpty.Click += new System.EventHandler(this.btnClipEmpty_Click);
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
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Лабораторная работа 4 — средства Win32 API";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabFiles.ResumeLayout(false);
            this.splitFiles.Panel1.ResumeLayout(false);
            this.splitFiles.Panel2.ResumeLayout(false);
            this.splitFiles.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitFiles)).EndInit();
            this.splitFiles.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.grpParams.ResumeLayout(false);
            this.grpParams.PerformLayout();
            this.tabMemory.ResumeLayout(false);
            this.tabMemory.PerformLayout();
            this.splitMem.Panel1.ResumeLayout(false);
            this.splitMem.Panel1.PerformLayout();
            this.splitMem.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMem)).EndInit();
            this.splitMem.ResumeLayout(false);
            this.flpMemTop.ResumeLayout(false);
            this.flpMemTop.PerformLayout();
            this.tabClipboard.ResumeLayout(false);
            this.tabClipboard.PerformLayout();
            this.splitClip.Panel1.ResumeLayout(false);
            this.splitClip.Panel2.ResumeLayout(false);
            this.splitClip.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitClip)).EndInit();
            this.splitClip.ResumeLayout(false);
            this.flpClipTop.ResumeLayout(false);
            this.flpClipTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuRunAsAdmin;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuMemory;
        private System.Windows.Forms.ToolStripMenuItem mnuSysInfo;
        private System.Windows.Forms.ToolStripMenuItem mnuMemStatus;
        private System.Windows.Forms.ToolStripMenuItem mnuVirtualQuery;
        private System.Windows.Forms.ToolStripMenuItem mnuMemMap;
        private System.Windows.Forms.ToolStripSeparator mnuMemSep;
        private System.Windows.Forms.ToolStripMenuItem mnuMemClear;
        private System.Windows.Forms.ToolStripMenuItem mnuClipboard;
        private System.Windows.Forms.ToolStripMenuItem mnuClipRefresh;
        private System.Windows.Forms.ToolStripMenuItem mnuClipTimer;
        private System.Windows.Forms.ToolStripSeparator mnuClipSep;
        private System.Windows.Forms.ToolStripMenuItem mnuClipWrite;
        private System.Windows.Forms.ToolStripMenuItem mnuClipWriteCustom;
        private System.Windows.Forms.ToolStripMenuItem mnuClipRead;
        private System.Windows.Forms.ToolStripMenuItem mnuClipEmpty;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpContents;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSep;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCurDir;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabFiles;
        private System.Windows.Forms.SplitContainer splitFiles;
        private System.Windows.Forms.TreeView tvStructure;
        private System.Windows.Forms.Label lblTree;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.FlowLayoutPanel flpSteps;
        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpCreation;
        private System.Windows.Forms.Label lblDrive;
        private System.Windows.Forms.ComboBox cmbDrive;
        private System.Windows.Forms.Label lblLabel;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.SplitContainer splitMem;
        private System.Windows.Forms.TextBox txtMemo;
        private System.Windows.Forms.ListView lvRegions;
        private System.Windows.Forms.ColumnHeader colBase;
        private System.Windows.Forms.ColumnHeader colAllocBase;
        private System.Windows.Forms.ColumnHeader colAllocProtect;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colState;
        private System.Windows.Forms.ColumnHeader colProtect;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colModule;
        private System.Windows.Forms.FlowLayoutPanel flpMemTop;
        private System.Windows.Forms.Button btnSysInfo;
        private System.Windows.Forms.Button btnMemStatus;
        private System.Windows.Forms.Label lblAddrPreset;
        private System.Windows.Forms.ComboBox cmbAddrPreset;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnVirtualQuery;
        private System.Windows.Forms.Button btnMemMap;
        private System.Windows.Forms.Button btnMemClear;
        private System.Windows.Forms.TabPage tabClipboard;
        private System.Windows.Forms.SplitContainer splitClip;
        private System.Windows.Forms.ListBox lstFormats;
        private System.Windows.Forms.TextBox txtClipOut;
        private System.Windows.Forms.Label lblOwner;
        private System.Windows.Forms.Label lblFormatCount;
        private System.Windows.Forms.FlowLayoutPanel flpClipTop;
        private System.Windows.Forms.Button btnClipRefresh;
        private System.Windows.Forms.CheckBox chkClipTimer;
        private System.Windows.Forms.Label lblClipText;
        private System.Windows.Forms.TextBox txtClipText;
        private System.Windows.Forms.Button btnClipWrite;
        private System.Windows.Forms.Button btnClipWriteCustom;
        private System.Windows.Forms.Button btnClipRead;
        private System.Windows.Forms.Button btnClipEmpty;
        private System.Windows.Forms.Timer timerClipboard;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
