namespace MDserver
{
    partial class MDserv
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDserv));
            this.statusStrip_main = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_notice = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_show = new System.Windows.Forms.ToolStripStatusLabel();
            this.button_openserver = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.button_listen = new System.Windows.Forms.Button();
            this.groupBox_manage = new System.Windows.Forms.GroupBox();
            this.button_regedit = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.radioButton_MySQL = new System.Windows.Forms.RadioButton();
            this.groupBox_sql = new System.Windows.Forms.GroupBox();
            this.checkBox_MongoDB = new System.Windows.Forms.CheckBox();
            this.checkBox_Redis = new System.Windows.Forms.CheckBox();
            this.checkBox_memcached = new System.Windows.Forms.CheckBox();
            this.button_ftp = new System.Windows.Forms.Button();
            this.button_putty = new System.Windows.Forms.Button();
            this.button_WinSCP = new System.Windows.Forms.Button();
            this.button_calc = new System.Windows.Forms.Button();
            this.button_Rythem = new System.Windows.Forms.Button();
            this.button_shell = new System.Windows.Forms.Button();
            this.groupBox_tool = new System.Windows.Forms.GroupBox();
            this.button_HeidiSQL = new System.Windows.Forms.Button();
            this.button_SecureCRT = new System.Windows.Forms.Button();
            this.button_WinCacheGrind = new System.Windows.Forms.Button();
            this.button_VIM = new System.Windows.Forms.Button();
            this.button_mod_host = new System.Windows.Forms.Button();
            this.groupBox_usually = new System.Windows.Forms.GroupBox();
            this.button_OP_lweb = new System.Windows.Forms.Button();
            this.button_author = new System.Windows.Forms.Button();
            this.button_MySQL = new System.Windows.Forms.Button();
            this.listBox_listen = new System.Windows.Forms.ListBox();
            this.groupBox_status = new System.Windows.Forms.GroupBox();
            this.button_start = new System.Windows.Forms.Button();
            this.radioButton_Apache = new System.Windows.Forms.RadioButton();
            this.radioButton_Nginx = new System.Windows.Forms.RadioButton();
            this.groupBox_webserver = new System.Windows.Forms.GroupBox();
            this.serviceController_main = new System.ServiceProcess.ServiceController();
            this.groupBox_MDservice = new System.Windows.Forms.GroupBox();
            this.checkBox_go_system = new System.Windows.Forms.CheckBox();
            this.checkBox_Min = new System.Windows.Forms.CheckBox();
            this.mainMenu_main = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem_webserver = new System.Windows.Forms.MenuItem();
            this.menuItem_apache = new System.Windows.Forms.MenuItem();
            this.menuItem_a_conf = new System.Windows.Forms.MenuItem();
            this.menuItem_a_log = new System.Windows.Forms.MenuItem();
            this.menuItem_a_error = new System.Windows.Forms.MenuItem();
            this.menuItem_nginx = new System.Windows.Forms.MenuItem();
            this.menuItem_n_conf = new System.Windows.Forms.MenuItem();
            this.menuItem_n_record = new System.Windows.Forms.MenuItem();
            this.menuItem_n_error = new System.Windows.Forms.MenuItem();
            this.menuItem_SQL = new System.Windows.Forms.MenuItem();
            this.menuItem_SQL_m = new System.Windows.Forms.MenuItem();
            this.menuItem_msql_conf = new System.Windows.Forms.MenuItem();
            this.menuItem_php = new System.Windows.Forms.MenuItem();
            this.menuItem_php_conf = new System.Windows.Forms.MenuItem();
            this.menuItem_webOP = new System.Windows.Forms.MenuItem();
            this.menuItem_op_www = new System.Windows.Forms.MenuItem();
            this.menuItem_cgi_bin = new System.Windows.Forms.MenuItem();
            this.menuItem_scan_web = new System.Windows.Forms.MenuItem();
            this.menuItem_scan_cgi_bin = new System.Windows.Forms.MenuItem();
            this.menuItem_help = new System.Windows.Forms.MenuItem();
            this.menuItem_about = new System.Windows.Forms.MenuItem();
            this.menuItem_instr = new System.Windows.Forms.MenuItem();
            this.menuItem_alipay = new System.Windows.Forms.MenuItem();
            this.notifyIcon_main = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip_main = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_open = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_close = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip_main.SuspendLayout();
            this.groupBox_manage.SuspendLayout();
            this.groupBox_sql.SuspendLayout();
            this.groupBox_tool.SuspendLayout();
            this.groupBox_usually.SuspendLayout();
            this.groupBox_status.SuspendLayout();
            this.groupBox_webserver.SuspendLayout();
            this.groupBox_MDservice.SuspendLayout();
            this.contextMenuStrip_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip_main
            // 
            this.statusStrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_notice,
            this.toolStripStatusLabel_show});
            this.statusStrip_main.Location = new System.Drawing.Point(0, 278);
            this.statusStrip_main.Name = "statusStrip_main";
            this.statusStrip_main.Size = new System.Drawing.Size(636, 22);
            this.statusStrip_main.TabIndex = 14;
            this.statusStrip_main.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_notice
            // 
            this.toolStripStatusLabel_notice.Name = "toolStripStatusLabel_notice";
            this.toolStripStatusLabel_notice.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel_notice.Text = "状态 : ";
            // 
            // toolStripStatusLabel_show
            // 
            this.toolStripStatusLabel_show.Name = "toolStripStatusLabel_show";
            this.toolStripStatusLabel_show.Size = new System.Drawing.Size(25, 17);
            this.toolStripStatusLabel_show.Text = "init";
            // 
            // button_openserver
            // 
            this.button_openserver.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_openserver.Location = new System.Drawing.Point(8, 73);
            this.button_openserver.Name = "button_openserver";
            this.button_openserver.Size = new System.Drawing.Size(75, 23);
            this.button_openserver.TabIndex = 0;
            this.button_openserver.Text = "查看服务";
            this.button_openserver.UseVisualStyleBackColor = true;
            this.button_openserver.Click += new System.EventHandler(this.button_openserver_Click);
            // 
            // button_exit
            // 
            this.button_exit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_exit.Location = new System.Drawing.Point(8, 100);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(75, 23);
            this.button_exit.TabIndex = 1;
            this.button_exit.Text = "退出程序";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button_listen
            // 
            this.button_listen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_listen.Location = new System.Drawing.Point(8, 46);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(75, 23);
            this.button_listen.TabIndex = 2;
            this.button_listen.Text = "开始监听";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // groupBox_manage
            // 
            this.groupBox_manage.Controls.Add(this.button_regedit);
            this.groupBox_manage.Controls.Add(this.button_listen);
            this.groupBox_manage.Controls.Add(this.button_exit);
            this.groupBox_manage.Controls.Add(this.button_openserver);
            this.groupBox_manage.Location = new System.Drawing.Point(313, 13);
            this.groupBox_manage.Name = "groupBox_manage";
            this.groupBox_manage.Size = new System.Drawing.Size(94, 134);
            this.groupBox_manage.TabIndex = 17;
            this.groupBox_manage.TabStop = false;
            this.groupBox_manage.Text = "常规管理";
            // 
            // button_regedit
            // 
            this.button_regedit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_regedit.Location = new System.Drawing.Point(8, 18);
            this.button_regedit.Name = "button_regedit";
            this.button_regedit.Size = new System.Drawing.Size(75, 23);
            this.button_regedit.TabIndex = 3;
            this.button_regedit.Text = "注册表";
            this.button_regedit.UseVisualStyleBackColor = true;
            this.button_regedit.Click += new System.EventHandler(this.button_regedit_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(540, 221);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(91, 40);
            this.button_stop.TabIndex = 2;
            this.button_stop.Text = "停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // radioButton_MySQL
            // 
            this.radioButton_MySQL.AutoSize = true;
            this.radioButton_MySQL.Checked = true;
            this.radioButton_MySQL.Location = new System.Drawing.Point(16, 22);
            this.radioButton_MySQL.Name = "radioButton_MySQL";
            this.radioButton_MySQL.Size = new System.Drawing.Size(53, 16);
            this.radioButton_MySQL.TabIndex = 0;
            this.radioButton_MySQL.TabStop = true;
            this.radioButton_MySQL.Text = "MySQL";
            this.radioButton_MySQL.UseVisualStyleBackColor = true;
            // 
            // groupBox_sql
            // 
            this.groupBox_sql.Controls.Add(this.checkBox_MongoDB);
            this.groupBox_sql.Controls.Add(this.radioButton_MySQL);
            this.groupBox_sql.Controls.Add(this.checkBox_Redis);
            this.groupBox_sql.Controls.Add(this.checkBox_memcached);
            this.groupBox_sql.Location = new System.Drawing.Point(99, 12);
            this.groupBox_sql.Name = "groupBox_sql";
            this.groupBox_sql.Size = new System.Drawing.Size(100, 135);
            this.groupBox_sql.TabIndex = 8;
            this.groupBox_sql.TabStop = false;
            this.groupBox_sql.Text = "SQL and NoSQL";
            // 
            // checkBox_MongoDB
            // 
            this.checkBox_MongoDB.AutoSize = true;
            this.checkBox_MongoDB.Location = new System.Drawing.Point(16, 106);
            this.checkBox_MongoDB.Name = "checkBox_MongoDB";
            this.checkBox_MongoDB.Size = new System.Drawing.Size(66, 16);
            this.checkBox_MongoDB.TabIndex = 2;
            this.checkBox_MongoDB.Text = "MongoDB";
            this.checkBox_MongoDB.UseVisualStyleBackColor = true;
            this.checkBox_MongoDB.CheckedChanged += new System.EventHandler(this.checkBox_MongoDB_CheckedChanged);
            // 
            // checkBox_Redis
            // 
            this.checkBox_Redis.AutoSize = true;
            this.checkBox_Redis.Location = new System.Drawing.Point(16, 80);
            this.checkBox_Redis.Name = "checkBox_Redis";
            this.checkBox_Redis.Size = new System.Drawing.Size(54, 16);
            this.checkBox_Redis.TabIndex = 1;
            this.checkBox_Redis.Text = "Redis";
            this.checkBox_Redis.UseVisualStyleBackColor = true;
            this.checkBox_Redis.CheckedChanged += new System.EventHandler(this.checkBox_Redis_CheckedChanged);
            // 
            // checkBox_memcached
            // 
            this.checkBox_memcached.AutoSize = true;
            this.checkBox_memcached.Location = new System.Drawing.Point(16, 53);
            this.checkBox_memcached.Name = "checkBox_memcached";
            this.checkBox_memcached.Size = new System.Drawing.Size(78, 16);
            this.checkBox_memcached.TabIndex = 0;
            this.checkBox_memcached.Text = "Memcached";
            this.checkBox_memcached.UseVisualStyleBackColor = true;
            this.checkBox_memcached.CheckedChanged += new System.EventHandler(this.checkBox_memcached_CheckedChanged);
            // 
            // button_ftp
            // 
            this.button_ftp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_ftp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_ftp.ForeColor = System.Drawing.Color.Black;
            this.button_ftp.Location = new System.Drawing.Point(5, 75);
            this.button_ftp.Name = "button_ftp";
            this.button_ftp.Size = new System.Drawing.Size(77, 23);
            this.button_ftp.TabIndex = 7;
            this.button_ftp.Text = "Flashfxp";
            this.button_ftp.UseVisualStyleBackColor = true;
            this.button_ftp.Click += new System.EventHandler(this.button_ftp_Click);
            // 
            // button_putty
            // 
            this.button_putty.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_putty.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_putty.Location = new System.Drawing.Point(84, 47);
            this.button_putty.Name = "button_putty";
            this.button_putty.Size = new System.Drawing.Size(59, 23);
            this.button_putty.TabIndex = 8;
            this.button_putty.Text = "Putty";
            this.button_putty.UseVisualStyleBackColor = true;
            this.button_putty.Click += new System.EventHandler(this.button_putty_Click);
            // 
            // button_WinSCP
            // 
            this.button_WinSCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_WinSCP.Location = new System.Drawing.Point(84, 73);
            this.button_WinSCP.Name = "button_WinSCP";
            this.button_WinSCP.Size = new System.Drawing.Size(59, 23);
            this.button_WinSCP.TabIndex = 9;
            this.button_WinSCP.Text = "WinSCP";
            this.button_WinSCP.UseVisualStyleBackColor = true;
            this.button_WinSCP.Click += new System.EventHandler(this.button_WinSCP_Click);
            // 
            // button_calc
            // 
            this.button_calc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_calc.Location = new System.Drawing.Point(84, 101);
            this.button_calc.Name = "button_calc";
            this.button_calc.Size = new System.Drawing.Size(59, 23);
            this.button_calc.TabIndex = 10;
            this.button_calc.Text = "计算器";
            this.button_calc.UseVisualStyleBackColor = true;
            this.button_calc.Click += new System.EventHandler(this.button_calc_Click);
            // 
            // button_Rythem
            // 
            this.button_Rythem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Rythem.Location = new System.Drawing.Point(84, 19);
            this.button_Rythem.Name = "button_Rythem";
            this.button_Rythem.Size = new System.Drawing.Size(59, 23);
            this.button_Rythem.TabIndex = 11;
            this.button_Rythem.Text = "Rythem";
            this.button_Rythem.UseVisualStyleBackColor = true;
            this.button_Rythem.Click += new System.EventHandler(this.button_Rythem_Click);
            // 
            // button_shell
            // 
            this.button_shell.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_shell.Location = new System.Drawing.Point(5, 19);
            this.button_shell.Name = "button_shell";
            this.button_shell.Size = new System.Drawing.Size(77, 23);
            this.button_shell.TabIndex = 12;
            this.button_shell.Text = "执行Shell";
            this.button_shell.UseVisualStyleBackColor = true;
            this.button_shell.Click += new System.EventHandler(this.button_shell_Click);
            // 
            // groupBox_tool
            // 
            this.groupBox_tool.Controls.Add(this.button_HeidiSQL);
            this.groupBox_tool.Controls.Add(this.button_SecureCRT);
            this.groupBox_tool.Controls.Add(this.button_WinCacheGrind);
            this.groupBox_tool.Controls.Add(this.button_VIM);
            this.groupBox_tool.Controls.Add(this.button_calc);
            this.groupBox_tool.Controls.Add(this.button_shell);
            this.groupBox_tool.Controls.Add(this.button_Rythem);
            this.groupBox_tool.Controls.Add(this.button_WinSCP);
            this.groupBox_tool.Controls.Add(this.button_putty);
            this.groupBox_tool.Controls.Add(this.button_ftp);
            this.groupBox_tool.Location = new System.Drawing.Point(410, 12);
            this.groupBox_tool.Name = "groupBox_tool";
            this.groupBox_tool.Size = new System.Drawing.Size(221, 135);
            this.groupBox_tool.TabIndex = 10;
            this.groupBox_tool.TabStop = false;
            this.groupBox_tool.Text = "小工具";
            // 
            // button_HeidiSQL
            // 
            this.button_HeidiSQL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_HeidiSQL.Location = new System.Drawing.Point(145, 48);
            this.button_HeidiSQL.Name = "button_HeidiSQL";
            this.button_HeidiSQL.Size = new System.Drawing.Size(69, 23);
            this.button_HeidiSQL.TabIndex = 16;
            this.button_HeidiSQL.Text = "HeidiSQL";
            this.button_HeidiSQL.UseVisualStyleBackColor = true;
            this.button_HeidiSQL.Click += new System.EventHandler(this.button_HeidiSQL_Click);
            // 
            // button_SecureCRT
            // 
            this.button_SecureCRT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_SecureCRT.Location = new System.Drawing.Point(6, 48);
            this.button_SecureCRT.Name = "button_SecureCRT";
            this.button_SecureCRT.Size = new System.Drawing.Size(76, 23);
            this.button_SecureCRT.TabIndex = 15;
            this.button_SecureCRT.Text = "SecureCRT";
            this.button_SecureCRT.UseVisualStyleBackColor = true;
            this.button_SecureCRT.Click += new System.EventHandler(this.button_SecureCRT_Click);
            // 
            // button_WinCacheGrind
            // 
            this.button_WinCacheGrind.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_WinCacheGrind.Location = new System.Drawing.Point(5, 101);
            this.button_WinCacheGrind.Name = "button_WinCacheGrind";
            this.button_WinCacheGrind.Size = new System.Drawing.Size(77, 23);
            this.button_WinCacheGrind.TabIndex = 14;
            this.button_WinCacheGrind.Text = "CacheGrind";
            this.button_WinCacheGrind.UseVisualStyleBackColor = true;
            this.button_WinCacheGrind.Click += new System.EventHandler(this.button_WinCacheGrind_Click);
            // 
            // button_VIM
            // 
            this.button_VIM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_VIM.Location = new System.Drawing.Point(145, 19);
            this.button_VIM.Name = "button_VIM";
            this.button_VIM.Size = new System.Drawing.Size(70, 23);
            this.button_VIM.TabIndex = 13;
            this.button_VIM.Text = "VIM";
            this.button_VIM.UseVisualStyleBackColor = true;
            this.button_VIM.Click += new System.EventHandler(this.button_VIM_Click);
            // 
            // button_mod_host
            // 
            this.button_mod_host.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_mod_host.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button_mod_host.FlatAppearance.BorderSize = 0;
            this.button_mod_host.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button_mod_host.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button_mod_host.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_mod_host.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button_mod_host.Location = new System.Drawing.Point(8, 18);
            this.button_mod_host.Name = "button_mod_host";
            this.button_mod_host.Size = new System.Drawing.Size(99, 23);
            this.button_mod_host.TabIndex = 5;
            this.button_mod_host.Text = "修改HOST文件";
            this.button_mod_host.UseVisualStyleBackColor = true;
            this.button_mod_host.Click += new System.EventHandler(this.button_mod_host_Click);
            // 
            // groupBox_usually
            // 
            this.groupBox_usually.Controls.Add(this.button_OP_lweb);
            this.groupBox_usually.Controls.Add(this.button_author);
            this.groupBox_usually.Controls.Add(this.button_MySQL);
            this.groupBox_usually.Controls.Add(this.button_mod_host);
            this.groupBox_usually.Location = new System.Drawing.Point(199, 13);
            this.groupBox_usually.Name = "groupBox_usually";
            this.groupBox_usually.Size = new System.Drawing.Size(114, 134);
            this.groupBox_usually.TabIndex = 12;
            this.groupBox_usually.TabStop = false;
            this.groupBox_usually.Text = "常用";
            // 
            // button_OP_lweb
            // 
            this.button_OP_lweb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_OP_lweb.Location = new System.Drawing.Point(8, 100);
            this.button_OP_lweb.Name = "button_OP_lweb";
            this.button_OP_lweb.Size = new System.Drawing.Size(99, 23);
            this.button_OP_lweb.TabIndex = 8;
            this.button_OP_lweb.Text = "打开本地地址";
            this.button_OP_lweb.UseVisualStyleBackColor = true;
            this.button_OP_lweb.Click += new System.EventHandler(this.button_OP_lweb_Click);
            // 
            // button_author
            // 
            this.button_author.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_author.Location = new System.Drawing.Point(8, 74);
            this.button_author.Name = "button_author";
            this.button_author.Size = new System.Drawing.Size(99, 23);
            this.button_author.TabIndex = 7;
            this.button_author.Text = "作者博客";
            this.button_author.UseVisualStyleBackColor = true;
            this.button_author.Click += new System.EventHandler(this.button_author_Click);
            // 
            // button_MySQL
            // 
            this.button_MySQL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_MySQL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_MySQL.Location = new System.Drawing.Point(8, 46);
            this.button_MySQL.Name = "button_MySQL";
            this.button_MySQL.Size = new System.Drawing.Size(99, 23);
            this.button_MySQL.TabIndex = 6;
            this.button_MySQL.Text = "管理MySQL数据库";
            this.button_MySQL.UseVisualStyleBackColor = true;
            this.button_MySQL.Click += new System.EventHandler(this.button_MySQL_Click);
            // 
            // listBox_listen
            // 
            this.listBox_listen.FormattingEnabled = true;
            this.listBox_listen.ItemHeight = 12;
            this.listBox_listen.Items.AddRange(new object[] {
            "init"});
            this.listBox_listen.Location = new System.Drawing.Point(6, 15);
            this.listBox_listen.Name = "listBox_listen";
            this.listBox_listen.Size = new System.Drawing.Size(515, 100);
            this.listBox_listen.TabIndex = 1;
            // 
            // groupBox_status
            // 
            this.groupBox_status.Controls.Add(this.listBox_listen);
            this.groupBox_status.Location = new System.Drawing.Point(7, 148);
            this.groupBox_status.Name = "groupBox_status";
            this.groupBox_status.Size = new System.Drawing.Size(527, 129);
            this.groupBox_status.TabIndex = 15;
            this.groupBox_status.TabStop = false;
            this.groupBox_status.Text = "状态栏";
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(540, 171);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(91, 40);
            this.button_start.TabIndex = 1;
            this.button_start.Text = "启动";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // radioButton_Apache
            // 
            this.radioButton_Apache.AutoSize = true;
            this.radioButton_Apache.Checked = true;
            this.radioButton_Apache.Location = new System.Drawing.Point(8, 19);
            this.radioButton_Apache.Name = "radioButton_Apache";
            this.radioButton_Apache.Size = new System.Drawing.Size(59, 16);
            this.radioButton_Apache.TabIndex = 0;
            this.radioButton_Apache.TabStop = true;
            this.radioButton_Apache.Text = "Apache";
            this.radioButton_Apache.UseVisualStyleBackColor = true;
            // 
            // radioButton_Nginx
            // 
            this.radioButton_Nginx.AutoSize = true;
            this.radioButton_Nginx.Location = new System.Drawing.Point(8, 40);
            this.radioButton_Nginx.Name = "radioButton_Nginx";
            this.radioButton_Nginx.Size = new System.Drawing.Size(53, 16);
            this.radioButton_Nginx.TabIndex = 1;
            this.radioButton_Nginx.Text = "Nginx";
            this.radioButton_Nginx.UseVisualStyleBackColor = true;
            // 
            // groupBox_webserver
            // 
            this.groupBox_webserver.Controls.Add(this.radioButton_Nginx);
            this.groupBox_webserver.Controls.Add(this.radioButton_Apache);
            this.groupBox_webserver.Location = new System.Drawing.Point(7, 12);
            this.groupBox_webserver.Name = "groupBox_webserver";
            this.groupBox_webserver.Size = new System.Drawing.Size(91, 61);
            this.groupBox_webserver.TabIndex = 0;
            this.groupBox_webserver.TabStop = false;
            this.groupBox_webserver.Text = "WebServer";
            // 
            // groupBox_MDservice
            // 
            this.groupBox_MDservice.Controls.Add(this.checkBox_go_system);
            this.groupBox_MDservice.Controls.Add(this.checkBox_Min);
            this.groupBox_MDservice.Location = new System.Drawing.Point(7, 79);
            this.groupBox_MDservice.Name = "groupBox_MDservice";
            this.groupBox_MDservice.Size = new System.Drawing.Size(91, 68);
            this.groupBox_MDservice.TabIndex = 18;
            this.groupBox_MDservice.TabStop = false;
            this.groupBox_MDservice.Text = "MDservice";
            // 
            // checkBox_go_system
            // 
            this.checkBox_go_system.AutoSize = true;
            this.checkBox_go_system.Location = new System.Drawing.Point(7, 45);
            this.checkBox_go_system.Name = "checkBox_go_system";
            this.checkBox_go_system.Size = new System.Drawing.Size(72, 16);
            this.checkBox_go_system.TabIndex = 1;
            this.checkBox_go_system.Text = "跟随系统";
            this.checkBox_go_system.UseVisualStyleBackColor = true;
            this.checkBox_go_system.CheckedChanged += new System.EventHandler(this.checkBox_go_system_CheckedChanged);
            // 
            // checkBox_Min
            // 
            this.checkBox_Min.AutoSize = true;
            this.checkBox_Min.Location = new System.Drawing.Point(7, 22);
            this.checkBox_Min.Name = "checkBox_Min";
            this.checkBox_Min.Size = new System.Drawing.Size(84, 16);
            this.checkBox_Min.TabIndex = 0;
            this.checkBox_Min.Text = "运行最小化";
            this.checkBox_Min.UseVisualStyleBackColor = true;
            // 
            // mainMenu_main
            // 
            this.mainMenu_main.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_webserver,
            this.menuItem_SQL,
            this.menuItem_php,
            this.menuItem_webOP,
            this.menuItem_help,
            this.menuItem_about});
            // 
            // menuItem_webserver
            // 
            this.menuItem_webserver.Index = 0;
            this.menuItem_webserver.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_apache,
            this.menuItem_nginx});
            this.menuItem_webserver.Text = "服务器";
            // 
            // menuItem_apache
            // 
            this.menuItem_apache.Index = 0;
            this.menuItem_apache.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_a_conf,
            this.menuItem_a_log,
            this.menuItem_a_error});
            this.menuItem_apache.Text = "Apache";
            // 
            // menuItem_a_conf
            // 
            this.menuItem_a_conf.Index = 0;
            this.menuItem_a_conf.Text = "配置文件";
            this.menuItem_a_conf.Click += new System.EventHandler(this.menuItem_a_conf_Click);
            // 
            // menuItem_a_log
            // 
            this.menuItem_a_log.Index = 1;
            this.menuItem_a_log.Text = "日志记录";
            this.menuItem_a_log.Click += new System.EventHandler(this.menuItem_a_log_Click);
            // 
            // menuItem_a_error
            // 
            this.menuItem_a_error.Index = 2;
            this.menuItem_a_error.Text = "错误日志";
            this.menuItem_a_error.Click += new System.EventHandler(this.menuItem_a_error_Click);
            // 
            // menuItem_nginx
            // 
            this.menuItem_nginx.Index = 1;
            this.menuItem_nginx.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_n_conf,
            this.menuItem_n_record,
            this.menuItem_n_error});
            this.menuItem_nginx.Text = "Nginx";
            // 
            // menuItem_n_conf
            // 
            this.menuItem_n_conf.Index = 0;
            this.menuItem_n_conf.Text = "配置文件";
            this.menuItem_n_conf.Click += new System.EventHandler(this.menuItem_n_conf_Click);
            // 
            // menuItem_n_record
            // 
            this.menuItem_n_record.Index = 1;
            this.menuItem_n_record.Text = "日志记录";
            this.menuItem_n_record.Click += new System.EventHandler(this.menuItem_n_record_Click);
            // 
            // menuItem_n_error
            // 
            this.menuItem_n_error.Index = 2;
            this.menuItem_n_error.Text = "错误日志";
            this.menuItem_n_error.Click += new System.EventHandler(this.menuItem_n_error_Click);
            // 
            // menuItem_SQL
            // 
            this.menuItem_SQL.Index = 1;
            this.menuItem_SQL.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_SQL_m});
            this.menuItem_SQL.Text = "数据库";
            // 
            // menuItem_SQL_m
            // 
            this.menuItem_SQL_m.Index = 0;
            this.menuItem_SQL_m.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_msql_conf});
            this.menuItem_SQL_m.Text = "MySQL";
            // 
            // menuItem_msql_conf
            // 
            this.menuItem_msql_conf.Index = 0;
            this.menuItem_msql_conf.Text = "配置文件";
            this.menuItem_msql_conf.Click += new System.EventHandler(this.menuItem_msql_conf_Click);
            // 
            // menuItem_php
            // 
            this.menuItem_php.Index = 2;
            this.menuItem_php.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_php_conf});
            this.menuItem_php.Text = "PHP设置";
            // 
            // menuItem_php_conf
            // 
            this.menuItem_php_conf.Index = 0;
            this.menuItem_php_conf.Text = "配置文件";
            this.menuItem_php_conf.Click += new System.EventHandler(this.menuItem_php_conf_Click);
            // 
            // menuItem_webOP
            // 
            this.menuItem_webOP.Index = 3;
            this.menuItem_webOP.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_op_www,
            this.menuItem_cgi_bin,
            this.menuItem_scan_web,
            this.menuItem_scan_cgi_bin});
            this.menuItem_webOP.Text = "网站目录";
            // 
            // menuItem_op_www
            // 
            this.menuItem_op_www.Index = 0;
            this.menuItem_op_www.Text = "默认WEB目录[HTML,PHP]";
            this.menuItem_op_www.Click += new System.EventHandler(this.menuItem_op_www_Click);
            // 
            // menuItem_cgi_bin
            // 
            this.menuItem_cgi_bin.Index = 1;
            this.menuItem_cgi_bin.Text = "默认CGI_BIN(Apache有效)";
            this.menuItem_cgi_bin.Click += new System.EventHandler(this.menuItem_cgi_bin_Click);
            // 
            // menuItem_scan_web
            // 
            this.menuItem_scan_web.Index = 2;
            this.menuItem_scan_web.Text = "浏览默认WEB地址";
            this.menuItem_scan_web.Click += new System.EventHandler(this.menuItem_scan_web_Click);
            // 
            // menuItem_scan_cgi_bin
            // 
            this.menuItem_scan_cgi_bin.Index = 3;
            this.menuItem_scan_cgi_bin.Text = "浏览CGI_BIN地址(Apache有效)";
            this.menuItem_scan_cgi_bin.Click += new System.EventHandler(this.menuItem_scan_cgi_bin_Click);
            // 
            // menuItem_help
            // 
            this.menuItem_help.Index = 4;
            this.menuItem_help.Text = "在线帮助";
            this.menuItem_help.Click += new System.EventHandler(this.menuItem_help_Click);
            // 
            // menuItem_about
            // 
            this.menuItem_about.Index = 5;
            this.menuItem_about.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_instr,
            this.menuItem_alipay});
            this.menuItem_about.Text = "关于";
            // 
            // menuItem_instr
            // 
            this.menuItem_instr.Index = 0;
            this.menuItem_instr.Text = "查看简介";
            this.menuItem_instr.Click += new System.EventHandler(this.menuItem_instr_Click);
            // 
            // menuItem_alipay
            // 
            this.menuItem_alipay.Index = 1;
            this.menuItem_alipay.Text = "捐赠";
            this.menuItem_alipay.Click += new System.EventHandler(this.menuItem_alipay_Click);
            // 
            // notifyIcon_main
            // 
            this.notifyIcon_main.ContextMenuStrip = this.contextMenuStrip_main;
            this.notifyIcon_main.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_main.Icon")));
            this.notifyIcon_main.Text = "MDserver";
            this.notifyIcon_main.Visible = true;
            this.notifyIcon_main.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_main_MouseDoubleClick);
            // 
            // contextMenuStrip_main
            // 
            this.contextMenuStrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_open,
            this.ToolStripMenuItem_close,
            this.ToolStripMenuItem_exit});
            this.contextMenuStrip_main.Name = "contextMenuStrip_main";
            this.contextMenuStrip_main.Size = new System.Drawing.Size(182, 70);
            // 
            // ToolStripMenuItem_open
            // 
            this.ToolStripMenuItem_open.Name = "ToolStripMenuItem_open";
            this.ToolStripMenuItem_open.Size = new System.Drawing.Size(181, 22);
            this.ToolStripMenuItem_open.Text = "打开MDserver窗口";
            this.ToolStripMenuItem_open.Click += new System.EventHandler(this.ToolStripMenuItem_open_Click);
            // 
            // ToolStripMenuItem_close
            // 
            this.ToolStripMenuItem_close.Name = "ToolStripMenuItem_close";
            this.ToolStripMenuItem_close.Size = new System.Drawing.Size(181, 22);
            this.ToolStripMenuItem_close.Text = "关闭MDserver窗口";
            this.ToolStripMenuItem_close.Click += new System.EventHandler(this.ToolStripMenuItem_close_Click);
            // 
            // ToolStripMenuItem_exit
            // 
            this.ToolStripMenuItem_exit.Name = "ToolStripMenuItem_exit";
            this.ToolStripMenuItem_exit.Size = new System.Drawing.Size(181, 22);
            this.ToolStripMenuItem_exit.Text = "退出MDserver程序";
            this.ToolStripMenuItem_exit.Click += new System.EventHandler(this.ToolStripMenuItem_exit_Click);
            // 
            // MDserv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(636, 300);
            this.Controls.Add(this.groupBox_status);
            this.Controls.Add(this.groupBox_MDservice);
            this.Controls.Add(this.groupBox_manage);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.statusStrip_main);
            this.Controls.Add(this.groupBox_usually);
            this.Controls.Add(this.groupBox_tool);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.groupBox_webserver);
            this.Controls.Add(this.groupBox_sql);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.mainMenu_main;
            this.MinimizeBox = false;
            this.Name = "MDserv";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = " MDserver";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MDserv_Load);
            this.statusStrip_main.ResumeLayout(false);
            this.statusStrip_main.PerformLayout();
            this.groupBox_manage.ResumeLayout(false);
            this.groupBox_sql.ResumeLayout(false);
            this.groupBox_sql.PerformLayout();
            this.groupBox_tool.ResumeLayout(false);
            this.groupBox_usually.ResumeLayout(false);
            this.groupBox_status.ResumeLayout(false);
            this.groupBox_webserver.ResumeLayout(false);
            this.groupBox_webserver.PerformLayout();
            this.groupBox_MDservice.ResumeLayout(false);
            this.groupBox_MDservice.PerformLayout();
            this.contextMenuStrip_main.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip_main;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_notice;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_show;
        private System.Windows.Forms.Button button_openserver;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.GroupBox groupBox_manage;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.RadioButton radioButton_MySQL;
        private System.Windows.Forms.GroupBox groupBox_sql;
        private System.Windows.Forms.CheckBox checkBox_memcached;
        private System.Windows.Forms.CheckBox checkBox_Redis;
        private System.Windows.Forms.CheckBox checkBox_MongoDB;
        private System.Windows.Forms.Button button_ftp;
        private System.Windows.Forms.Button button_putty;
        private System.Windows.Forms.Button button_WinSCP;
        private System.Windows.Forms.Button button_calc;
        private System.Windows.Forms.Button button_Rythem;
        private System.Windows.Forms.Button button_shell;
        private System.Windows.Forms.GroupBox groupBox_tool;
        private System.Windows.Forms.Button button_mod_host;
        private System.Windows.Forms.GroupBox groupBox_usually;
        private System.Windows.Forms.ListBox listBox_listen;
        private System.Windows.Forms.GroupBox groupBox_status;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.RadioButton radioButton_Apache;
        private System.Windows.Forms.RadioButton radioButton_Nginx;
        private System.Windows.Forms.GroupBox groupBox_webserver;
        private System.Windows.Forms.Button button_regedit;
        private System.ServiceProcess.ServiceController serviceController_main;
        private System.Windows.Forms.Button button_MySQL;
        private System.Windows.Forms.Button button_author;
        private System.Windows.Forms.Button button_VIM;
        private System.Windows.Forms.GroupBox groupBox_MDservice;
        private System.Windows.Forms.CheckBox checkBox_Min;
        private System.Windows.Forms.CheckBox checkBox_go_system;
        private System.Windows.Forms.MainMenu mainMenu_main;
        private System.Windows.Forms.MenuItem menuItem_webserver;
        private System.Windows.Forms.MenuItem menuItem_apache;
        private System.Windows.Forms.MenuItem menuItem_a_conf;
        private System.Windows.Forms.MenuItem menuItem_nginx;
        private System.Windows.Forms.MenuItem menuItem_n_conf;
        private System.Windows.Forms.MenuItem menuItem_SQL;
        private System.Windows.Forms.MenuItem menuItem_SQL_m;
        private System.Windows.Forms.MenuItem menuItem_php;
        private System.Windows.Forms.MenuItem menuItem_webOP;
        private System.Windows.Forms.MenuItem menuItem_op_www;
        private System.Windows.Forms.MenuItem menuItem_help;
        private System.Windows.Forms.MenuItem menuItem_about;
        private System.Windows.Forms.Button button_WinCacheGrind;
        private System.Windows.Forms.Button button_OP_lweb;
        private System.Windows.Forms.MenuItem menuItem_a_log;
        private System.Windows.Forms.MenuItem menuItem_a_error;
        private System.Windows.Forms.MenuItem menuItem_n_record;
        private System.Windows.Forms.MenuItem menuItem_n_error;
        private System.Windows.Forms.MenuItem menuItem_msql_conf;
        private System.Windows.Forms.MenuItem menuItem_php_conf;
        private System.Windows.Forms.MenuItem menuItem_cgi_bin;
        private System.Windows.Forms.MenuItem menuItem_instr;
        private System.Windows.Forms.MenuItem menuItem_alipay;
        private System.Windows.Forms.MenuItem menuItem_scan_web;
        private System.Windows.Forms.MenuItem menuItem_scan_cgi_bin;
        private System.Windows.Forms.NotifyIcon notifyIcon_main;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_main;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_open;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_close;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_exit;
        private System.Windows.Forms.Button button_SecureCRT;
        private System.Windows.Forms.Button button_HeidiSQL;
    }
}

