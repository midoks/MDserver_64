using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Security.Principal;
using IWshRuntimeLibrary;

//self class
using MDserver;
using System.Xml;
using System.Text.RegularExpressions;

namespace MDserver
{
    public partial class MDserv : Form
    {
        //private static string version = "2.0.0";
        //PID 进程显示时钟
        private System.Timers.Timer timer = new System.Timers.Timer(3000);

        //打开
        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
        //创建服务
        [DllImport("advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
         int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
         string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        //[DllImport("advapi32.dll")]
        //public static extern void ChangeServiceConfig2(IntPtr SCHANDLE, int lpDescType, string lpDesc);
        //关闭服务
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);
        //启动服务
        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        //打开资源
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
        //删除服务
        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        private static string BaseDir = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");

        //service name
        private static string MD_ApacheName = "MDserver-Apache";
        private static string MD_MySQL = "MDserver-MySQL";
        private static string MD_Memcached = "memcached";
        private static string MD_Redis = "MDserver-Redis";
        private static string MD_MongoDB = "MDserver-MongoDB";

        //默认php版本
        private static string PHP_VERSION = "php55";

        private static string HOSTS_NOTES = "#MDserver Hosts Don`t Remove and Change";

        private SystemINI ini;
        private SystemXml iniXml;

        public MDserv()
        {

            InitializeComponent();
            this.ini = new SystemINI(BaseDir + "md.ini");
            this.iniXml = new SystemXml(BaseDir + "host.xml");
            //拦截标题栏的关闭事件
            this.Closing += new CancelEventHandler(MDserv_Closing);

            //延迟执行
            System.Timers.Timer timers = new System.Timers.Timer(200);
            timers.Elapsed += new System.Timers.ElapsedEventHandler(_MDserv_start);
            timers.Enabled = true;
            timers.AutoReset = false;
            Control.CheckForIllegalCrossThreadCalls = false;


            addPHPMenulist();
        }



        //拦截标题栏的关闭事件
        private void MDserv_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消关闭窗体
            e.Cancel = true;

            // 将窗体变为最小化
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false; //不显示在系统任务栏 
            notifyIcon_main.Visible = true;
        }

        public void log(string log)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + "---------------------" + log);
        }

        public bool IsAdmin()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        //获取超级权限
        public void GetSuperAcl()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = true;
            info.WorkingDirectory = Environment.CurrentDirectory;
            info.FileName = Application.ExecutablePath;
            info.Verb = "runas";
            Process.Start(info);
        }

        //获取当前运行环境
        //64位  true
        //32位  false
        public bool isSystemType64()
        {
            if (IntPtr.Size == 8)
            {
                //64 bit
                //MessageBox.Show("64"); 
                return true;
            }
            else if (IntPtr.Size == 4)
            {
                //32 bit
                //MessageBox.Show("32");
                return false;
            }
            else
            {
                //...NotSupport
            }
            return false;
        }

        static IntPtr GetHiveHandle(RegistryHive hive)
        {
            IntPtr preexistingHandle = IntPtr.Zero;

            IntPtr HKEY_CLASSES_ROOT = new IntPtr(-2147483648);
            IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
            IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
            IntPtr HKEY_USERS = new IntPtr(-2147483645);
            IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
            IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
            IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);
            switch (hive)
            {
                case RegistryHive.ClassesRoot: preexistingHandle = HKEY_CLASSES_ROOT; break;
                case RegistryHive.CurrentUser: preexistingHandle = HKEY_CURRENT_USER; break;
                case RegistryHive.LocalMachine: preexistingHandle = HKEY_LOCAL_MACHINE; break;
                case RegistryHive.Users: preexistingHandle = HKEY_USERS; break;
                case RegistryHive.PerformanceData: preexistingHandle = HKEY_PERFORMANCE_DATA; break;
                case RegistryHive.CurrentConfig: preexistingHandle = HKEY_CURRENT_CONFIG; break;
                case RegistryHive.DynData: preexistingHandle = HKEY_DYN_DATA; break;
            }
            return preexistingHandle;
        }

        //初始化服务检查
        private void _MDserv_start(object sender, System.Timers.ElapsedEventArgs e)
        {
            //检查原有的服务
            string[] service = { MD_ApacheName, MD_MySQL, MD_Redis };
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            foreach (string s in service)
            {
                if (WServiceIsExisted(s) && !WQueryServiceIsStart(s))
                {

                    if (!WQueryServiceIsStart(s))
                    {

                        if (s == MD_ApacheName)
                        {
                            button_start.Enabled = false;

                            radioButton_Apache.Checked = true;
                            WStart(MD_ApacheName);

                            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Apache_lazy);
                            timer.Enabled = true;
                            timer.AutoReset = false;
                        }
                        else if (s == MD_MySQL)
                        {
                            WStart(MD_MySQL);

                            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_mysql_lazy);
                            timer.Enabled = true;
                            timer.AutoReset = false;
                        }
                    }

                }

                //线程检查
                timer = new System.Timers.Timer(100);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_check_status);
                timer.Enabled = true;
                timer.AutoReset = false;
            }

        }

        //检查状态
        private void _check_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Memcached
            if (WQueryServiceIsStart(MD_Memcached))
            {
                checkBox_memcached.Checked = true;
            }
            //MongoDB
            if (WQueryServiceIsStart(MD_MongoDB))
            {
                checkBox_MongoDB.Checked = true;
            }
            //MD_Redis
            if (WQueryServiceIsStart(MD_Redis.ToLower()))
            {
                checkBox_Redis.Checked = true;
            }

            //MD_MySQL
            if (WQueryServiceIsStart(MD_MySQL))
            {
                checkBox_MySQL.Checked = true;
            }

            //main server
            _check_ws_status();
        }

        //检测webserver + sql 状态
        private void _check_ws_status()
        {

            if (WQueryServiceIsStart(MD_ApacheName))
            {
                radioButton_Apache.Checked = true;
            }

            //跟随系统检测
            if (runWhenStartExist("MDserver"))
            {
                checkBox_go_system.Checked = true;
            }

            int md_run = this.ini.ReadInteger("MDSERVER", "MD_RUN", 0);
            string md_dir = this.ini.ReadString("MDSERVER", "RUN_DIR", "");
            if (1 == md_run || md_dir != "")
            {
                this.button_start.Enabled = false;
            }
        }

        //添加PHP列表
        private void addPHPMenulist()
        {
            //查看PHP版本
            string php_list = BaseDir.Replace("/", "\\") + @"bin\PHP";
            //当前默认php版本
            string php_cver = this.ini.ReadString("MDSERVER", "PHP_DIR", PHP_VERSION);

            var fhost = Directory.GetDirectories(php_list);
            foreach (string f in fhost)
            {
                string _pvn = System.IO.Path.GetFileName(f);

                MenuItem _pversion = new System.Windows.Forms.MenuItem();
                _pversion.Text = _pvn;
                _pversion.Index = 1;
                _pversion.Click += new System.EventHandler(this.php_list_click);

                if (_pvn.Equals(php_cver))
                {
                    _pversion.Checked = true;
                }

                this.menuItem_php_list.MenuItems.AddRange(
                    new System.Windows.Forms.MenuItem[] { _pversion }
                );

            }
        }

        //重新启动
        public void restart()
        {

            bool ret = WStop_S(MD_ApacheName);
            if (ret)
            {
                _stop_apache();

                after_stop_SERVICE();

                Thread.Sleep(3000);

                _clear_record();
                pre_start_SERVICE();
                _start_Apache();
            }
        }

        private void php_list_click(object sender, EventArgs e)
        {
            foreach (MenuItem i in this.menuItem_php_list.MenuItems)
            {
                i.Checked = false;
            }

            MenuItem pitem = (MenuItem)sender;
            pitem.Checked = true;

            if (WQueryServiceIsStart(MD_ApacheName))
            {
                DialogResult qa = MessageBox.Show("你要切换到" + pitem.Text + ",当前正在运行中,是否重新启动!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (qa == DialogResult.Yes)
                {
                    //_stop_apache();
                    bool ret = WStop_S(MD_ApacheName);
                    if (ret)
                    {
                        string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

                        string apache = BaseDir + @"bin\" + apache_dir + @"\bin\httpd.exe";
                        string arg = "-k uninstall -n " + MD_ApacheName;
                        Wcmd(apache + " " + arg);
                    }
                    after_stop_SERVICE();
                    this.ini.WriteString("MDSERVER", "PHP_DIR", pitem.Text);


                    Thread.Sleep(3000);
                    //this.PHPCurlFixAndPath(pitem.Text);
                    _clear_record();
                    pre_start_SERVICE();
                    _start_Apache();
                }
                else
                {
                    this.after_stop_SERVICE();
                }
            }
            else
            {
                this.ini.WriteString("MDSERVER", "PHP_DIR", pitem.Text);
            }
        }

        private void PHPCurlFixAndPath(string s)
        {


            string php_list = BaseDir.Replace("/", "\\") + @"bin\PHP";
            string php_cver = this.ini.ReadString("MDSERVER", "PHP_DIR", PHP_VERSION);
            string php_pos_s = php_list + "\\" + s;
            string php_pos_m = php_list + "\\" + php_cver;
            string sys_path = System.Environment.GetEnvironmentVariable("PATH");
            string[] list = sys_path.Split(';');
            //string[] t;
            ArrayList new_list = new ArrayList();
            foreach (string i in list)
            {
                if (i == php_pos_s)
                {
                    continue;
                }
                else if (i == php_pos_m)
                {
                    continue;
                }
                new_list.Add(i);
                log(i);
            }

            string t = "";
            foreach (string i2 in new_list)
            {
                t += i2 + ";";
            }

            //设置Path
            System.Environment.SetEnvironmentVariable("PATH", t + php_pos_m + ";");


            //string[] php_ts_dll_list = Directory.GetFiles(php_pos, "*ts.dll");
            //string fixcurl_bat = php_list + "\\fixcurl.bat";
            //string fixcurl_bat_tmp = php_list + "\\fixcurl_tmp.bat";
            //if (System.IO.File.Exists(fixcurl_bat))
            //{
            //    string r = _ReadContent(fixcurl_bat);
            //    r = r.Replace("MD:/", BaseDir);
            //    r = r.Replace("{PHP_VER}", php_cver);
            //    r = r.Replace("{SYS_DIR}", Environment.GetEnvironmentVariable("SystemRoot"));
            //    r = r.Replace("{PHP_TS}", System.IO.Path.GetFileName(php_ts_dll_list[0]));
            //    _WriteContent(fixcurl_bat_tmp, r);
            //    Wcmd(fixcurl_bat_tmp);
            //    System.IO.File.Delete(fixcurl_bat_tmp);
            //}
        }

        //状态提示
        private void Wstatus(string name)
        {
            toolStripStatusLabel_show.Text = name;
        }

        //状态提示内容增加
        private void Wstatus_add(string content)
        {
            toolStripStatusLabel_show.Text = toolStripStatusLabel_show.Text + content;
        }

        //监听框初始化
        private void listBox_listen_init()
        {
            listBox_listen.Items.Clear();
            listBox_listen.Items.Add("镜像名称                       PID 会话名                 会话#     内存使用");
            listBox_listen.Items.Add("=========================== ====== ================= =========== ===========");
        }


        //初始化
        private void MDserv_Load(object sender, EventArgs e)
        {
            Wstatus("欢迎使用MDsever(PHP开发环境)...");
            listBox_listen_init();

            if (checkedIsHasSelfRun())
            {
                MessageBox.Show("已经在运行了!!!");
                Application.Exit();
            }

            //init timer
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
        }

        //检查是自己已经运行了
        private bool checkedIsHasSelfRun()
        {
            string cmdtext = "tasklist | findstr MDserver.exe";
            Process Tcmd = new Process();
            Tcmd.StartInfo.FileName = "cmd.exe";//设定程序名
            Tcmd.StartInfo.UseShellExecute = false;//关闭Shell的使用 
            Tcmd.StartInfo.RedirectStandardInput = true;//重定向标准输入 
            Tcmd.StartInfo.RedirectStandardOutput = true;//重定向标准输出
            Tcmd.StartInfo.RedirectStandardError = true;//重定向错误输出
            Tcmd.StartInfo.CreateNoWindow = true;
            Tcmd.StartInfo.Arguments = "/C " + cmdtext;//设置不显示窗口
            Tcmd.Start();//执行VER命令 

            string s = Tcmd.StandardOutput.ReadToEnd();
            Tcmd.Close();

            string[] ss;
            ArrayList str = new ArrayList();
            if (s != null)
            {
                ss = s.Split('\n');
                foreach (string d in ss)
                {
                    if (d != "")
                    {
                        str.Add(d);
                    }
                }

                if (str.Count > 1)
                {
                    return true;
                }
            }
            return false;
        }



        /**
         *  所有菜单选项 
         */

        //打开配置文件
        private void menuItem_a_conf_Click(object sender, EventArgs e)
        {
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_conf = BaseDir + @"bin\" + apache_dir + @"\conf\httpd.conf";
            Wcmd_Exe(vim, httpd_conf);
        }

        //查看日志
        private void menuItem_a_log_Click(object sender, EventArgs e)
        {
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_log = BaseDir + @"bin\" + apache_dir + @"\logs\access.log";
            Wcmd_Exe(vim, httpd_log);
        }

        //查看错误日志
        private void menuItem_a_error_Click(object sender, EventArgs e)
        {
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_log = BaseDir + @"bin\" + apache_dir + @"\logs\error.log";
            Wcmd_Exe(vim, httpd_log);
        }

        //打开虚拟目录
        private void menuItem_apache_vhost_Click(object sender, EventArgs e)
        {
            string dir_E = "explorer.exe";
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");
            string dir = BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost";
            Wcmd_Exe(dir_E, dir);
        }

        //MySQL数据库配置文件
        private void menuItem_msql_conf_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string mysql_err = BaseDir + @"bin\MySQL\my.ini";
            Wcmd_Exe(vim, mysql_err);
        }


        //PHP配置文件
        private void menuItem_php_conf_Click(object sender, EventArgs e)
        {
            string php_dir = this.ini.ReadString(@"MDSERVER", @"PHP_DIR", PHP_VERSION);

            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string php = BaseDir + @"bin\PHP\" + php_dir + @"\php.ini";
            Wcmd_Exe(vim, php);
        }

        //扩展目录
        private void menuItem_PHPext_Click(object sender, EventArgs e)
        {
            string dir_E = "explorer.exe";
            string php_dir = this.ini.ReadString(@"MDSERVER", @"PHP_DIR", PHP_VERSION);
            string dir = BaseDir.Replace("/", "\\") + @"bin\PHP\" + php_dir + @"\ext";
            Wcmd_Exe(dir_E, dir);
        }

        //www/htdocs
        private void menuItem_op_www_Click(object sender, EventArgs e)
        {
            string dir_E = "explorer.exe";
            string dir = BaseDir.Replace("/", "\\") + @"www\htdocs";
            Wcmd_Exe(dir_E, dir);
        }

        private void menuItem_scan_web_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://127.0.0.1");
        }

        private void menuItem_scan_cgi_bin_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://127.0.0.1/cgi-bin/index.pl");
        }

        //帮助信息
        private void menuItem_help_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://midoks.cachecha.com/2014/09/30/mdserver.html");
        }


        //cgi_bin
        private void menuItem_cgi_bin_Click(object sender, EventArgs e)
        {
            string dir_E = "explorer.exe";
            string dir = BaseDir.Replace("/", "\\") + @"www\cgi-bin";
            Wcmd_Exe(dir_E, dir);
        }

        //关于(一些简单的介绍)
        private void menuItem_instr_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string readme = BaseDir + @"readme.txt";
            Wcmd_Exe(vim, readme);
        }

        //捐赠
        private void menuItem_alipay_Click(object sender, EventArgs e)
        {
            Form b1 = new AboutForm();
            b1.ShowDialog();
        }

        /**
         *  常用工具 
         */
        //修改host
        private void button_mod_host_Click(object sender, EventArgs e)
        {

            WebSite ws = new WebSite();
            ws.mainUI = this;
            ws.ShowDialog();
        }

        private void button_host_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string path = @"C:\Windows\System32\drivers\etc\hosts";
            System.Diagnostics.Process.Start(vim, path);
        }

        //定时显示 监听状态
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //判断是否关闭
            if (timer.Enabled)
            {
                ArrayList str = new ArrayList();
                string[] nn = {
                    "MDserver", "memcached","mongod",
                    "redis","mysql","php", "httpd"
                };
                foreach (string i in nn)
                {
                    ArrayList b = listen_process_name(i);
                    if (b != null)
                    {
                        foreach (string z in b)
                        {
                            str.Add(z);
                        }
                    }
                }
                listBox_listen_init();
                foreach (string c in str)
                {
                    listBox_listen.Items.Add(c);
                }
            }
            //模拟的做一些耗时的操作
            //System.Threading.Thread.Sleep(2000);
        }


        //监控进程名
        private ArrayList listen_process_name(string name)
        {
            ArrayList str = new ArrayList();
            string cmdtext = "tasklist | findstr " + name;
            Process Tcmd = new Process();
            Tcmd.StartInfo.FileName = "cmd.exe";//设定程序名
            Tcmd.StartInfo.UseShellExecute = false;//关闭Shell的使用 
            Tcmd.StartInfo.RedirectStandardInput = true;//重定向标准输入 
            Tcmd.StartInfo.RedirectStandardOutput = true;//重定向标准输出
            Tcmd.StartInfo.RedirectStandardError = true;//重定向错误输出
            Tcmd.StartInfo.CreateNoWindow = true;
            Tcmd.StartInfo.Arguments = "/C " + cmdtext;//设置不显示窗口
            Tcmd.Start();//执行VER命令 

            string s = Tcmd.StandardOutput.ReadToEnd();
            Tcmd.Close();
            string[] ss;
            if (s != null)
            {
                ss = s.Split('\n');
                foreach (string d in ss)
                {
                    //MessageBox.Show(d);
                    if (d != "")
                    {
                        str.Add(d);
                    }
                }
            }
            return str;
        }

        private void checkBox_MySQL_CheckedChanged(object sender, EventArgs e)
        {

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_mysql_status);
            timer.Enabled = true;
            timer.AutoReset = false;

        }

        private void _mysql_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart(MD_MySQL) && checkBox_MySQL.Checked)
            {
                return;
            }
            try
            {
                if (checkBox_MySQL.Checked)
                {
                    pre_start_mysql();
                    _start_mysql();
                }
                else
                {
                    Wstatus("MySQL: 停止中...");
                    _stop_mysql();
                    after_stop_msyql();
                }
            }
            catch (Exception ex)
            {
                Wstatus("MySQL: " + ex.Message);
            }

        }

        //memcached 启动
        private void checkBox_memcached_CheckedChanged(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_memcached_status);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _memcached_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart(MD_Memcached) && checkBox_memcached.Checked)
            {
                return;
            }
            try
            {
                string mdir = BaseDir + @"bin\Memcached\";
                if (checkBox_memcached.Checked)
                {
                    //Wcmd(BaseDir + @"bin/Memcached/memcached.exe -d stop");
                    //Wcmd(BaseDir + @"bin/Memcached/memcached.exe -d uninstall");
                    Wcmd(mdir + @"memcached.exe -d install");
                    Thread.Sleep(1000);
                    WStart_S(MD_Memcached);
                    //Wcmd(mdir + @"memcached.exe -d start");
                    //Wcmd(str + @"bin/Memcached/memcached.exe -d runservice -m 64 -c 2048 -p 11211");
                }
                else
                {
                    bool ret = WStop_S(MD_Memcached);
                    if (ret)
                    {
                        Wcmd(mdir + @"memcached.exe -d uninstall");
                    }
                }
            }
            catch (Exception ex)
            {
                Wstatus("memcached " + ex.Message);
            }
        }

        //MongoDB 
        private void checkBox_MongoDB_CheckedChanged(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_MongoDB_status);
            timer.Enabled = true;
            timer.AutoReset = false;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        //MongDB 安装
        private void _MongoDB_status(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (WQueryServiceIsStart(MD_MongoDB) && checkBox_MongoDB.Checked)
            {
                return;
            }
            try
            {
                string mdir = BaseDir + @"bin\Mongo\";
                string mdir_Exe = BaseDir + @"bin\Mongo\bin\mongod.exe";
                if (checkBox_MongoDB.Checked)
                {
                    Wcmd_Exe(mdir_Exe, @" --logpath " + mdir + @"log\log.txt --dbpath " + mdir + @"data --serviceName " + MD_MongoDB + " --install ");
                    Thread.Sleep(1000);
                    WStart_S(MD_MongoDB);
                }
                else
                {
                    bool ret = WStop_S(MD_MongoDB);
                    if (ret)
                    {
                        Wcmd_Exe(mdir_Exe, @" --logpath " + mdir + @"log\log.txt --dbpath " + mdir + @"data --serviceName " + MD_MongoDB + " --remove");
                    }
                }
            }
            catch (Exception ex)
            {
                Wstatus(MD_MongoDB + ex.Message);
            }
        }

        //Redis
        private void checkBox_Redis_CheckedChanged(object sender, EventArgs e)
        {

            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_Redis_status);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _Redis_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart(MD_Redis.ToLower()) && checkBox_Redis.Checked)
            {
                return;
            }
            try
            {
                string mdir = BaseDir + @"bin\Redis\";
                string mdir_Exe = mdir + @"redis-server.exe";
                if (checkBox_Redis.Checked)
                {
                    Wcmd_Exe(mdir_Exe, " --service-install " + mdir + @"redis.conf --service-name " + MD_Redis);
                    Thread.Sleep(1000);
                    WStart_S(MD_Redis);
                }
                else
                {
                    bool ret = WStop_S(MD_Redis);
                    if (ret)
                    {
                        Wcmd_Exe(mdir_Exe, " --service-uninstall " + mdir + @"redis.conf --service-name " + MD_Redis);
                    }
                }
            }
            catch (Exception ex)
            {
                Wstatus("Redis " + ex.Message);
            }
        }


        //查看注册表
        private void button_regedit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("regedit.exe");
        }

        //开始,暂停监听进程
        private void button_listen_Click(object sender, EventArgs e)
        {
            string name = button_listen.Text;
            Application.DoEvents();
            if ("开始监听" == name)
            {
                timer.Start();//timer.Enabled = true;
                Wstatus("开始监听...");
                button_listen.Text = "取消监听";
            }
            else if ("取消监听" == name)
            {
                timer.Stop();//timer.Enabled = false;
                listBox_listen_init();
                Wstatus("取消监听...");
                button_listen.Text = "开始监听";
            }
        }

        //启动服务之前的初始化配置
        private void pre_start_SERVICE()
        {


            string php_dir = this.ini.ReadString(@"MDSERVER", @"PHP_DIR", PHP_VERSION);
            string php_dir_pos = BaseDir + "bin/PHP/" + php_dir;
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

            //make apache vhost conf
            XmlNode hostXml = this.iniXml.rootNode();
            string tplFile = (BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost\host.tpl");
            //修改hosts
            string hosts = @"C:\Windows\System32\drivers\etc\HOSTS";

            if (System.IO.File.Exists(tplFile))
            {
                string hostsContent = _ReadContent(hosts);
                hostsContent = hostsContent.Trim();

                string vhostDir = (BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost\");
                if (hostXml.HasChildNodes)
                {
                    ArrayList portList = new ArrayList();

                    foreach (XmlNode host in hostXml)
                    {
                        string hostname = host.Attributes["name"].Value;

                        string port = host.Attributes["port"].Value;
                        string root_dir = host.Attributes["root_dir"].Value;

                        string c = _ReadContent(tplFile);

                        c = c.Replace("{HOSTNAME}", hostname.Trim());
                        c = c.Replace("{ROOTDIR}", root_dir.Trim());
                        c = c.Replace("{PORT}", port.Trim());

                        if (System.IO.File.Exists(vhostDir + "own_" + hostname.Trim().Replace(".", "_") + ".conf"))
                        {
                            continue;
                        }

                        //写入vhost
                        _WriteContent(vhostDir + "tmp_" + hostname.Trim().Replace(".", "_") + ".conf", c);

                        //host组装
                        hostsContent += "\r\n127.0.0.1\t\t" + hostname + "\t" + HOSTS_NOTES;

                        //log(port);
                        //监听端口组装
                        if (port.Equals("80") || port.Equals("8080"))
                        {

                        }
                        else
                        {
                            if (portList.IndexOf(port) == -1)
                            {
                                portList.Add(port);
                            }
                        }
                    }

                    string portContent = "";
                    foreach (string portv in portList)
                    {
                        portContent += "Listen " + portv + "\r\n";
                    }

                    _WriteContent(vhostDir + "tmp_listen.conf", portContent);
                }
                else
                {
                    _WriteContent(vhostDir + "tmp_listen.conf", "");
                }

                _WriteContent(hosts, hostsContent);
                //log(hostsContent);
            }

            var php_config = php_dir;
            if (!System.IO.File.Exists(BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\php\" + php_dir + ".conf"))
            {
                php_config = "php_default";
            }

            //httpd.conf,php.ini替换
            string[] conf = {
                @"bin/"+ apache_dir + @"/conf/httpd.conf",
                @"bin/PHP/"+ php_dir + @"/php.ini"};

            //apache2_4.dll 找到
            string[] php_apache_dll_list = Directory.GetFiles(php_dir_pos, "*apache2_4.dll");
            string php_apache_dll = System.IO.Path.GetFileName(php_apache_dll_list[0]);
            string php_apache_module = "php5_module";
            if (php_apache_dll == "php7apache2_4.dll")
            {
                php_apache_module = "php7_module";
            }

            string[] php_ts_dll_list = Directory.GetFiles(php_dir_pos, "*ts.dll");

            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace("MD:/", BaseDir);
                r = r.Replace("{PHP_VER}", php_dir);
                r = r.Replace("{PHP_CONF}", php_config);
                r = r.Replace("{PHP_APACHE_MODULE}", php_apache_module);
                r = r.Replace("{PHP_APACHE_DLL}", php_apache_dll);
                r = r.Replace("{PHP_TS}", System.IO.Path.GetFileName(php_ts_dll_list[0]));
                _WriteContent(BaseDir + i, r);
            }

            //conf替换
            string[] conf_v_list = {
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost",
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\alias",
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\php"
            };


            foreach (var conf_i in conf_v_list)
            {
                if (Directory.Exists(conf_i))
                {
                    var fhost = Directory.GetFiles(conf_i, "*.conf");
                    foreach (var f in fhost)
                    {
                        string r = _ReadContent(f);
                        r = r.Replace("MD:/", BaseDir);
                        r = r.Replace("{PHP_VER}", php_dir);
                        r = r.Replace("{PHP_CONF}", php_config);
                        r = r.Replace("{PHP_APACHE_DLL}", php_apache_dll);
                        r = r.Replace("{PHP_TS}", System.IO.Path.GetFileName(php_ts_dll_list[0]));
                        _WriteContent(f, r);
                    }
                }
            }
        }

        private void pre_start_mysql()
        {
            string[] conf = { @"bin/MySQL/my.ini" };
            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace("MD:/", BaseDir);
                _WriteContent(BaseDir + i, r, System.Text.Encoding.ASCII);
            }
        }

        private void after_stop_msyql()
        {
            string[] conf = { @"bin/MySQL/my.ini" };

            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace(BaseDir, "MD:/");
                _WriteContent(BaseDir + i, r, System.Text.Encoding.ASCII);
            }
        }

        //停止后配置恢复原状
        private void after_stop_SERVICE()
        {


            string php_dir = this.ini.ReadString(@"MDSERVER", @"PHP_DIR", PHP_VERSION);
            string php_dir_pos = BaseDir + "bin/PHP/" + php_dir;
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

            var php_config = php_dir;
            if (!System.IO.File.Exists(BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\php\" + php_dir + ".conf"))
            {
                php_config = "php_default";
            }

            //删除自动生成的配置（apache vhost conf ）
            string vhostDir = (BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost\");
            var tmpVhost = Directory.GetFiles(vhostDir, "tmp_*.conf");
            foreach (var tV in tmpVhost)
            {
                System.IO.File.Delete(tV);
            }

            //删除APACHE的日志
            string apacheLogPath = (BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\logs\");
            var apacheLogPathFiles = Directory.GetFiles(apacheLogPath, "tmp_*.log");
            foreach (var logFile in apacheLogPathFiles)
            {
                System.IO.File.Delete(logFile);
            }

            //删除HOSTS
            string hosts = @"C:\Windows\System32\drivers\etc\HOSTS";
            string hostsContent = _ReadContent(hosts);

            Regex reg = new Regex("(.*)" + HOSTS_NOTES);
            hostsContent = reg.Replace(hostsContent, "");
            hostsContent = hostsContent.Trim();
            _WriteContent(hosts, hostsContent);
            //log(hostsContent);


            //apache2_4.dll 找到
            string[] php_apache_dll_list = Directory.GetFiles(php_dir_pos, "*apache2_4.dll");
            string php_apache_dll = System.IO.Path.GetFileName(php_apache_dll_list[0]);

            //httpd.conf,php.ini替换
            string[] conf = {
                @"bin/" +  apache_dir + "/conf/httpd.conf",
                @"bin/PHP/"+ php_dir +@"/php.ini"};

            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace(BaseDir, "MD:/");
                r = r.Replace(php_dir, "{PHP_VER}");
                r = r.Replace(php_config, "{PHP_CONF}");
                r = r.Replace("php7_module", "{PHP_APACHE_MODULE}");
                r = r.Replace("php5_module", "{PHP_APACHE_MODULE}");
                r = r.Replace("php5ts.dll", "{PHP_TS}");
                r = r.Replace("php7ts.dll", "{PHP_TS}");
                r = r.Replace(php_apache_dll, "{PHP_APACHE_DLL}");
                _WriteContent(BaseDir + i, r);
            }

            //conf替换
            string[] conf_v_list = {
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\vhost",
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\alias",
                BaseDir.Replace("/", "\\") + @"bin\" + apache_dir + @"\conf\php"
            };

            foreach (var conf_i in conf_v_list)
            {
                if (Directory.Exists(conf_i))
                {
                    var fhost = Directory.GetFiles(conf_i, "*.conf");
                    foreach (var f in fhost)
                    {
                        string r = _ReadContent(f);
                        r = r.Replace(BaseDir, "MD:/");
                        r = r.Replace(php_dir, "{PHP_VER}");
                        r = r.Replace(php_config, "{PHP_CONF}");
                        r = r.Replace(php_apache_dll, "{PHP_APACHE_DLL}");
                        r = r.Replace("php5ts.dll", "{PHP_TS}");
                        r = r.Replace("php7ts.dll", "{PHP_TS}");
                        _WriteContent(f, r);
                    }
                }
            }
        }

        //清除记录文件
        private void _clear_record()
        {
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

            string[] file = {
                BaseDir + @"bin\"+ apache_dir + @"\logs\access.log",
                BaseDir + @"bin\" + apache_dir + @"\logs\error.log",
            };

            foreach (string f in file)
            {
                _clear_file_log(f);
            }

        }

        //清楚文件
        private void _clear_file_log(string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Truncate, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                log("清空日志文件失败：" + ex.Message);
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                }
            }
        }

        //启动
        private void button_start_Click(object sender, EventArgs e)
        {
            _clear_record();
            //log(Environment.CurrentDirectory);
            //log(Application.ExecutablePath);
            Wstatus("正在启动中...");
            pre_start_SERVICE();
            _start_Apache();

            this.button_start.Enabled = false;
            this.ini.WriteInteger("MDSERVER", "MD_RUN", 1);
            this.ini.WriteString("MDSERVER", "RUN_DIR", BaseDir.Replace("/", "\\"));
        }

        //启动apache
        private void _start_Apache()
        {
            string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

            string apache = BaseDir + @"bin\" + apache_dir + @"\bin\httpd.exe";
            string arg = "-k install -n " + MD_ApacheName;
            log(apache + " " + arg);
            Wcmd(apache + " " + arg);

            Thread.Sleep(3000);

            //延迟执行
            System.Timers.Timer timer = new System.Timers.Timer(1500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Apache_lazy);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _start_Apache_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            WStart_S(MD_ApacheName);
            //不可以点击启动按钮
            button_start.Enabled = false;
        }

        //MySQL
        private void _start_mysql()
        {
            string dir = BaseDir + @"bin\MySQL\";
            string pathName = dir + @"bin\mysqld " + "--defaults-file=\"" + dir + "my.ini\" \"" + MD_MySQL + "\"";
            string installName = dir + @"bin\mysqld " + " install " + MD_MySQL;
            if (!WServiceIsExisted(MD_MySQL))
            {
                Wcmd(installName);
            }
            Thread.Sleep(1000);

            //延迟执行
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_mysql_lazy);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _start_mysql_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            WStart_S(MD_MySQL);
        }

        //停止服务
        private void button_stop_Click(object sender, EventArgs e)
        {

            _stop_apache();
            after_stop_SERVICE();

            this.ini.WriteInteger("MDSERVER", "MD_RUN", 0);
            this.ini.WriteString("MDSERVER", "RUN_DIR", "");
        }


        //停止卸载Apache
        private void _stop_apache()
        {
            if (WServiceIsExisted(MD_ApacheName))
            {
                //延迟执行
                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_stop_apache_lazy);
                timer.Enabled = true;
                timer.AutoReset = false;
            }
            else
            {
                Wstatus(MD_ApacheName + "已经停止!");
                //可以点击启动按钮
                button_start.Enabled = true;
            }
        }

        private void _stop_apache_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool ret = WStop_S(MD_ApacheName);
            if (ret)
            {
                string apache_dir = this.ini.ReadString(@"MDSERVER", @"APACHE_DIR", @"Apache24");

                string apache = BaseDir + @"bin\" + apache_dir + @"\bin\httpd.exe";
                string arg = "-k uninstall -n " + MD_ApacheName;
                Wcmd(apache + " " + arg);
            }
            //可以点击启动按钮
            button_start.Enabled = true;
        }

        //停止mysql
        private void _stop_mysql()
        {
            string dir = BaseDir + @"bin\MySQL\";
            if (WServiceIsExisted(MD_MySQL))
            {
                //string installName = dir + @"bin\mysqld " + "remove " + MD_MySQL;
                //Wcmd(installName);

                //延迟执行
                System.Timers.Timer timer = new System.Timers.Timer(500);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_stop_mysql_lazy);
                timer.Enabled = true;
                timer.AutoReset = false;
            }
        }

        private void _stop_mysql_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool ret = WStop_S(MD_MySQL);
            if (ret)
            {
                WUninstall(MD_MySQL);
            }
            else
            {
                Wstatus("MySQL: 停止失败!");
            }
        }


        /**
         *  小工具配置
         */

        //打开服务管理器
        private void button_openserver_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("services.msc");
        }

        private void button_author_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://midoks.cachecha.com");
        }

        //打开数据管理地址
        private void button_MySQL_Click(object sender, EventArgs e)
        {
            if (checkHttp())
            {
                System.Diagnostics.Process.Start("http://127.0.0.1/phpMyAdmin");
            }
        }

        public Boolean checkHttp()
        {
            bool isRun = !button_start.Enabled;
            if (!isRun)
            {
                MessageBox.Show("HTTP服务没有启动!!!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 打开memcache后台地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_memcached_admin_Click(object sender, EventArgs e)
        {
            if (checkHttp())
            {
                System.Diagnostics.Process.Start("http://127.0.0.1/memadmin");
            }

        }

        /// <summary>
        /// 打开redis后台管理地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_redis_admin_Click(object sender, EventArgs e)
        {
            if (checkHttp())
            {
                System.Diagnostics.Process.Start("http://127.0.0.1/phpRedisAdmin");
            }

        }

        /// <summary>
        /// 打开mongo后台管理地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_mongo_admin_Click(object sender, EventArgs e)
        {
            if (checkHttp())
            {
                System.Diagnostics.Process.Start("http://127.0.0.1/phpMongodb");
            }
        }


        //退出整个应用
        private void button_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //执行shell
        private void button_shell_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"tool\shell\shell.bat");
        }

        //tool putty
        private void button_putty_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/Putty.exe");
        }

        //tool WinSCP
        private void button_WinSCP_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/WinSCP/WinSCP.exe");
        }

        //tool calc
        private void button_calc_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("calc.exe");
        }

        //生成快捷键
        private void button_make_link_Click(object sender, EventArgs e)
        {
            string deskTop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            if (System.IO.File.Exists(deskTop + "\\MDserver.lnk"))  //
            {
                System.IO.File.Delete(deskTop + "\\MDserver.lnk");//删除原来的桌面快捷键方式
                //return;
            }

            WshShell shell = new WshShell();

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "MDserver.lnk");
            shortcut.TargetPath = @Application.StartupPath + "\\MDserver.exe"; //目标文件
            shortcut.WorkingDirectory = System.Environment.CurrentDirectory;//该属性指定应用程序的工作目录，当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。
            shortcut.WindowStyle = 1; //目标应用程序的窗口状态分为普通、最大化、最小化【1,3,7】
            shortcut.Description = "MDserver"; //描述
            //shortcut.IconLocation = Application.StartupPath + "\\app.ico";  //快捷方式图标
            shortcut.Arguments = "";
            shortcut.Hotkey = "CTRL+ALT+F11"; // 快捷键
            shortcut.Save(); //必须调用保存快捷才成创建成功

        }

        //Rythem
        private void button_Rythem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/Rythem/Rythem.exe");
        }

        //ftp
        private void button_ftp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/Flashfxp/flashfxp.exe");
        }

        //VIM
        private void button_VIM_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/Vim/vim73/gvim.exe");
        }

        //查看xdebug数据
        private void button_WinCacheGrind_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(BaseDir.Replace("/", "\\") + @"tool\WinCacheGrind\WinCacheGrind.exe");
            System.Diagnostics.Process.Start(BaseDir.Replace("/", "\\") + @"tool\kcachegrind\kcachegrind.exe");
            //string dir_E = "explorer.exe";
            //string dir = BaseDir + @"tool/kcachegrind/kcachegrind";
            //Wcmd_Exe(dir_E, dir.Replace("/", "\\"));
        }

        //打开web
        private void button_OP_lweb_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://127.0.0.1");
        }


        //** 两个特殊的功能  **//
        private void checkBox_go_system_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_go_system.Checked && !runWhenStartExist("MDserver"))
            {
                runWhenStart(true, "MDserver", BaseDir.Replace("/", "\\") + "MDserver.exe");
            }
            else if (runWhenStartExist("MDserver") && !checkBox_go_system.Checked)
            {
                runWhenStart(false, "MDserver", BaseDir.Replace("/", "\\") + "MDserver.exe");
            }
        }

        //检测是否设置开机启动项
        private bool runWhenStartExist(string exeName)
        {
            bool ok = false;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

            if (key == null) { }
            else if (exeName != "")
            {
                string r = (string)key.GetValue(exeName);
                if (r != null)
                {
                    ok = true;
                }
                key.Close();
            }
            return ok;
        }

        //是否加入开机启动
        private bool runWhenStart(bool started, string exeName, string path)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                try
                {
                    key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                catch (Exception ex)
                {
                    Wstatus(ex.Message);
                }
            }
            if (started == true)
            {
                try
                {
                    key.SetValue(exeName, path);//设置为开机启动
                    key.Close();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    key.DeleteValue(exeName);//取消开机启动
                    key.Close();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        //执行cmd命令
        private void Wcmd(string cmdtext)
        {
            if (isSystemType64())
            {
                Wcmd64(cmdtext);
            }
            else
            {
                Wcmd32(cmdtext);
            }
        }

        private void Wcmd32(string cmdtext)
        {
            Process Tcmd = new Process();
            Tcmd.StartInfo.FileName = "cmd.exe";//设定程序名 
            Tcmd.StartInfo.UseShellExecute = false;//关闭Shell的使用 
            Tcmd.StartInfo.RedirectStandardInput = true; //重定向标准输入 
            Tcmd.StartInfo.RedirectStandardOutput = true;//重定向标准输出 
            Tcmd.StartInfo.RedirectStandardError = true;//重定向错误输出 
            Tcmd.StartInfo.CreateNoWindow = true;//设置不显示窗口 
            Tcmd.StartInfo.Arguments = "/c " + cmdtext;
            Tcmd.StartInfo.Verb = "runas";
            //Tcmd.StandardInput.WriteLine("exit");
            //Tcmd.WaitForExit();
            Tcmd.Start();//执行VER命令 
            //string str = Tcmd.StandardOutput.ReadToEnd();
            //MessageBox.Show(str);
            Tcmd.Close();
        }

        //获取超级权限
        private void Wcmd64(string cmdtext)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            //info.RedirectStandardError = true;
            //info.RedirectStandardInput = true;
            //info.RedirectStandardOutput = true;
            //info.WorkingDirectory = Environment.CurrentDirectory;
            info.FileName = "cmd.exe";
            info.Arguments = "/c " + cmdtext;
            info.Verb = "runas";
            Process.Start(info);
        }

        //执行cmd命令
        private void Wcmd_Exe(string cmdexe, string cmdtext)
        {
            Process Tcmd = new Process();
            Tcmd.StartInfo.FileName = cmdexe;//设定程序名 
            Tcmd.StartInfo.UseShellExecute = false;//关闭Shell的使用 
            Tcmd.StartInfo.RedirectStandardInput = true; //重定向标准输入 
            Tcmd.StartInfo.RedirectStandardOutput = true;//重定向标准输出 
            Tcmd.StartInfo.RedirectStandardError = true;//重定向错误输出 
            Tcmd.StartInfo.CreateNoWindow = true;//设置不显示窗口 
            Tcmd.StartInfo.Arguments = cmdtext;
            Tcmd.Start();//执行VER命令
            //string str = Tcmd.StandardOutput.ReadToEnd();
            Tcmd.Close();
            //return str;
        }


        //test
        public string Wcmd_test(string cmdtext)
        {
            Process Tcmd = new Process();
            Tcmd.StartInfo.FileName = "cmd.exe";//设定程序名 
            Tcmd.StartInfo.UseShellExecute = false;//关闭Shell的使用 
            Tcmd.StartInfo.RedirectStandardInput = true; //重定向标准输入 
            Tcmd.StartInfo.RedirectStandardOutput = true;//重定向标准输出 
            Tcmd.StartInfo.RedirectStandardError = true;//重定向错误输出 
            Tcmd.StartInfo.CreateNoWindow = true;//设置不显示窗口 
            Tcmd.StartInfo.Arguments = "/c " + cmdtext;
            Tcmd.Start();//执行VER命令 
            string str = Tcmd.StandardOutput.ReadToEnd();
            Tcmd.Close();
            return str;
        }

        //获取进程PID
        private int WgetPid(string ProcName)
        {
            int pid = -1;
            Process[] pp = Process.GetProcessesByName(ProcName);
            for (int i = 0; i < pp.Length; i++)
            {
                if (pp[i].ProcessName == ProcName)
                {
                    pid = pp[i].Id;//这个就是进程的ID     
                }
            }
            return pid;
        }

        //安装service
        private void WRegiste(string serviceName, string servicePath)
        {
            IDictionary saveStatus = new System.Collections.Hashtable();
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                service.Refresh();
                //System.Collections.Hashtable ht = new System.Collections.Hashtable();
                AssemblyInstaller r = new AssemblyInstaller();
                r.UseNewContext = true;
                saveStatus.Clear();
                r.Path = servicePath;
                r.Install(saveStatus);
                r.Commit(saveStatus);
                r.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装服务失败! " + ex.Message);
            }
        }


        //安装服务
        private bool WInstall(string svcPath, string svcName, string svcDispName)
        {

            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
             SERVICE_QUERY_CONFIG |
             SERVICE_CHANGE_CONFIG |
             SERVICE_QUERY_STATUS |
             SERVICE_ENUMERATE_DEPENDENTS |
             SERVICE_START |
             SERVICE_STOP |
             SERVICE_PAUSE_CONTINUE |
             SERVICE_INTERROGATE |
             SERVICE_USER_DEFINED_CONTROL);
            int SERVICE_AUTO_START = 0x00000002;

            bool ok = false;
            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName,
                        SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START,
                        SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() != 0)
                    {
                        ok = true;
                    }
                    CloseServiceHandle(sv_handle);
                }
                CloseServiceHandle(sc_handle);
            }
            catch (Exception e)
            {
                Wstatus(e.Message);
            }

            return ok;
        }


        //卸载服务(测试未OK)
        private bool WUninstall2(string serviceName)
        {
            int GENERIC_WRITE = 0x40000000;
            int DELETE = 0x0001;
            int SERVICE_QUERY_STATUS = 0x0004;
            bool ok = false;
            WQueryServiceStatus(serviceName);
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                IntPtr svc_hndl = OpenService(sc_hndl, serviceName, SERVICE_QUERY_STATUS | DELETE);
                if (svc_hndl.ToInt32() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        ok = true;
                    }
                }
                CloseServiceHandle(svc_hndl);
            }
            CloseServiceHandle(sc_hndl);
            return ok;
        }

        //卸载服务(OK)
        public bool WUninstall(string svcName)
        {
            log(svcName);
            int GENERIC_WRITE = 0x40000000;
            int DELETE = 0x10000;

            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);

            bool ok = false;

            //MessageBox.Show(sc_hndl.ToString());

            if (sc_hndl.ToInt32() != 0)
            {

                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt32() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        ok = true;
                    }
                }
                CloseServiceHandle(svc_hndl);
            }
            CloseServiceHandle(sc_hndl);
            return ok;
        }

        private bool WStart(string serviceName)
        {
            Wcmd("net start " + serviceName);
            Thread.Sleep(3000);
            return true;
        }

        private bool WStop(string serviceName)
        {

            Wcmd("net stop " + serviceName);
            Thread.Sleep(3000);
            return true;
        }

        //启动服务
        private bool WStart_S(string serviceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Wstatus(serviceName + "已经启动成功!!!");
                    return true;
                }
                else
                {
                    service.Start();
                    for (int i = 0; i < 20; i++)
                    {
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            Wstatus(serviceName + "启动成功!!!");
                            return true;
                        }
                        if (i == 19)
                        {
                            Wstatus(serviceName + "启动失败!!!");
                        }
                        service.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Wstatus(ex.Message);
            }
            return false;
        }


        //暂停服务
        private bool WStop_S(string serviceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    Wstatus(serviceName + "已经停止成功!!!");
                    return true;
                }
                else
                {
                    service.Stop();
                    for (int i = 0; i < 20; i++)
                    {
                        System.Threading.Thread.Sleep(500);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                        {
                            Wstatus(serviceName + "停止成功!!!");
                            return true;
                        }
                        if (i == 19)
                        {
                            Wstatus(serviceName + "停止失败!!!");
                        }
                        service.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Wstatus(ex.Message);
            }
            return false;
        }

        //查看服务状态
        public bool WQueryServiceIsStart(string serviceName)
        {
            bool ok = false;
            if (WServiceIsExisted(serviceName))
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                ServiceControllerStatus s = service.Status;
                if ("Running" == s.ToString())
                {
                    ok = true;
                }
            }
            return ok;
        }

        //查看服务状态
        public void WQueryServiceStatus(string serviceName)
        {
            System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
            ServiceControllerStatus s = service.Status;
            MessageBox.Show(s.ToString());
        }


        //判断windows服务是否存在
        //string serviceName 服务名称
        private bool WServiceIsExisted(string serviceName)
        {
            bool ok = false;
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (serviceName == s.ServiceName)
                {
                    ok = true;
                }
            }
            return ok;
        }

        //设置windows服务描述信息
        private void WsetDesc(string serviceName, string DescInfo)
        {
            System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
        }

        //写入内容
        public bool _WriteContent(string path, string content)
        {
            bool ok = false;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8); //System.Text.Encoding.UTF8
            try
            {
                sw.Write(content);
                ok = true;
                sw.Close();
            }
            catch { }
            return ok;
        }

        public bool _WriteContent(string path, string content, Encoding code)
        {
            bool ok = false;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, code);
            try
            {
                sw.Write(content);
                ok = true;
                sw.Close();
            }
            catch { }
            return ok;
        }



        //读取内容
        private string _ReadContent(string path)
        {
            string str = System.IO.File.ReadAllText(path);
            return str;
        }

        //删除文件
        private void __Delete(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        //notify
        //退出程序
        private void ToolStripMenuItem_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //缩小窗口
        private void ToolStripMenuItem_close_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //打开窗口
        private void ToolStripMenuItem_open_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        //双击小图标
        private void notifyIcon_main_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        //SSH连接管理工具
        private void button_SecureCRT_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/SecureCRT/SecureCRT.exe");
        }

        //数据库管理工具
        private void button_HeidiSQL_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/HeidiSQL/heidisql.exe");
        }

        //截图取色工具
        private void button_FSCapture_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(BaseDir + "tool/FSCapture/FSCapture.exe");
        }

        private void menuItem_apache_Click(object sender, EventArgs e)
        { }

        private void groupBox_tool_Enter(object sender, EventArgs e)
        {

        }


    }
}