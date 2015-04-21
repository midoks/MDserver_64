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

//self class
using MDserver;



namespace MDserver
{
    public partial class MDserv : Form
    {
        //private static string version = "1.1.5";
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
        private static string MD_NginxName = "MDserver-Nginx";
        private static string MD_MySQL = "MDserver-MySQL";
        private static string MD_Redis = "MDserver-Redis";

        private SystemINI ini;

        public MDserv()
        {

            InitializeComponent();
            this.ini = new SystemINI(BaseDir + "md.ini");
            //拦截标题栏的关闭事件
            this.Closing += new CancelEventHandler(MDserv_Closing);

            //延迟执行
            System.Timers.Timer timers = new System.Timers.Timer(500);
            timers.Elapsed += new System.Timers.ElapsedEventHandler(_MDserv_start);
            timers.Enabled = true;
            timers.AutoReset = false;
            Control.CheckForIllegalCrossThreadCalls = false;
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
            string[] service = { MD_ApacheName, MD_NginxName, MD_MySQL, MD_Redis };
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            foreach (string s in service)
            {
                if (WServiceIsExisted(s) && !WQueryServiceIsStart(s))
                {
                    button_start.Enabled = false;
                    if (!WQueryServiceIsStart(s))
                    {
                        if (s == MD_ApacheName)
                        {
                            radioButton_Apache.Checked = true;
                            WStart(MD_ApacheName);

                            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Apache_lazy);
                            timer.Enabled = true;
                            timer.AutoReset = false;
                        }
                        else if (s == MD_NginxName)
                        {
                            radioButton_Nginx.Checked = true;
                            WStart(MD_NginxName);

                            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Nginx_lazy);
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

            //检查是否要启动PHP-CGI
            if (WServiceIsExisted(MD_NginxName))
            {
                string[] ss = { "php-cgi", "PHP-CGI" };
                string q = "";
                foreach (string cgi in ss)
                {
                    ArrayList l = listen_process_name(cgi);
                    foreach (string i in l)
                    {
                        q += i;
                    }
                }
                if (q == "")
                {
                    //MessageBox.Show("你需要启动cgi");
                    _start_PHP_CGI();
                }
            }
        }

        //检查状态
        private void _check_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Memcached
            if (WQueryServiceIsStart("memcached"))
            {
                checkBox_memcached.Checked = true;
            }
            //MongoDB
            if (WQueryServiceIsStart("MongoDB"))
            {
                checkBox_MongoDB.Checked = true;
            }
            //MD_Redis
            if (WQueryServiceIsStart(MD_Redis.ToLower()))
            {
                checkBox_Redis.Checked = true;
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
            else if (WQueryServiceIsStart(MD_NginxName))
            {
                radioButton_Nginx.Checked = true;
            }


            radioButton_MySQL.Checked = true;


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

            //init timer
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
        }



        /**
         *  所有菜单选项 
         */

        //打开配置文件
        private void menuItem_a_conf_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_conf = BaseDir + @"bin\Apache\conf\httpd.conf";
            Wcmd_Exe(vim, httpd_conf);
        }

        //查看日志
        private void menuItem_a_log_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_log = BaseDir + @"bin\Apache\logs\access.log";
            Wcmd_Exe(vim, httpd_log);
        }

        //查看错误日志
        private void menuItem_a_error_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string httpd_log = BaseDir + @"bin\Apache\logs\error.log";
            Wcmd_Exe(vim, httpd_log);
        }

        //nginx配置文件
        private void menuItem_n_conf_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string nginx_conf = BaseDir + @"bin\Nginx\conf\nginx.conf";
            Wcmd_Exe(vim, nginx_conf);
        }

        //日志
        private void menuItem_n_record_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string nginx_log = BaseDir + @"bin\Nginx\logs\access.log";
            Wcmd_Exe(vim, nginx_log);
        }

        //错误日志
        private void menuItem_n_error_Click(object sender, EventArgs e)
        {
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string nginx_err = BaseDir + @"bin\Nginx\logs\error.log";
            Wcmd_Exe(vim, nginx_err);
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
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string php = BaseDir + @"bin\PHP\php.ini";
            Wcmd_Exe(vim, php);
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
            System.Diagnostics.Process.Start("http://midoks.duapp.com/p/mdserver.html");
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
            string vim = BaseDir + @"tool\Vim\vim73\gvim.exe";
            string path = @"C:\Windows\System32\drivers\etc\hosts";
            System.Diagnostics.Process.Start(vim, path);
        }

        //定时显示 监听状态
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //判断是否关闭
            if (timer.Enabled)
            {
                ArrayList str = new ArrayList();
                string[] nn = { 
                    "nginx","MDserver", "php-cgi", "PHP-CGI", "memcached"
                    ,"mongod","redis","mysql","php", "httpd" 
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

        //memcached 启动
        private void checkBox_memcached_CheckedChanged(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_memcached_status);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _memcached_status(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart("memcached") && checkBox_memcached.Checked)
            {
                return;
            }
            try
            {
                string mdir = BaseDir + @"bin\Memcached\";
                //System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("memcached Server");
                if (checkBox_memcached.Checked)
                {
                    //Wcmd(BaseDir + @"bin/Memcached/memcached.exe -d stop");
                    //Wcmd(BaseDir + @"bin/Memcached/memcached.exe -d uninstall");
                    Wcmd(mdir + @"memcached.exe -d install");
                    Thread.Sleep(1000);
                    Wcmd(mdir + @"memcached.exe -d start");
                    Wstatus("memcached 启动成功!!!");
                    //Wcmd(str + @"bin/Memcached/memcached.exe -d runservice -m 64 -c 2048 -p 11211");
                }
                else
                {
                    Wcmd(mdir + @"memcached.exe -d stop");
                    Thread.Sleep(2000);
                    Wcmd(mdir + @"memcached.exe -d uninstall");
                    Wstatus("memcached 停止成功!!!");
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

            if (WQueryServiceIsStart("MongoDB") && checkBox_MongoDB.Checked)
            {
                return;
            }
            try
            {
                string mdir = BaseDir + @"bin\Mongo\";
                string mdir_Exe = BaseDir + @"bin\Mongo\bin\mongod.exe";
                if (checkBox_MongoDB.Checked)
                {

                    Wcmd_Exe(mdir_Exe, @" --logpath " + mdir + @"log\log.txt --dbpath " + mdir + @"data --serviceName MongoDB --install ");
                    Thread.Sleep(1000);
                    WStart("MongoDB");
                    Wstatus("MongoDB 启动成功!!!");
                }
                else
                {
                    WStop("MongoDB");
                    System.Threading.Thread.Sleep(1000);
                    Wcmd_Exe(mdir_Exe, @" --logpath " + mdir + @"log\log.txt --dbpath " + mdir + @"data --serviceName MongoDB --remove");
                    Wstatus("MongoDB 停止成功!!!");
                }
            }
            catch (Exception ex)
            {
                Wstatus("MongoDB " + ex.Message);
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
                    //Wcmd(mdir_Exe + " " + mdir + @"redis.conf");
                    WStart(MD_Redis);
                    Thread.Sleep(1000);
                    Wstatus("Redis 启动成功!!!");
                }
                else
                {
                    WStop(MD_Redis);
                    Thread.Sleep(1000);
                    //WUninstall(MD_Redis);
                    Wcmd_Exe(mdir_Exe, " --service-uninstall " + mdir + @"redis.conf --service-name " + MD_Redis);
                    Wstatus("Redis 停止成功!!!");
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
            string[] conf = { 
                @"bin/Apache/conf/httpd.conf",
                @"bin/Nginx/conf/nginx.conf",
                @"bin/MySQL/my.ini",
                @"bin/PHP/php.ini"};
            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace("MD:/", BaseDir);
                _WriteContent(BaseDir + i, r);
            }
        }

        //清除记录文件
        private void _clear_record()
        {
            string[] file = { 
                BaseDir + @"bin\Apache\logs\access.log",
                BaseDir + @"bin\Apache\logs\error.log",
                BaseDir + @"bin\Nginx\logs\access.log",
                BaseDir + @"bin\Nginx\logs\error.log",
            };

            foreach (string f in file)
            {
                _clear_file_log(f);
            }


            __Delete(BaseDir + @"bin\Nginx\myapp.err.log");
            __Delete(BaseDir + @"bin\Nginx\myapp.err.log.old");
            __Delete(BaseDir + @"bin\Nginx\myapp.out.log");
            __Delete(BaseDir + @"bin\Nginx\myapp.out.log.old");
            __Delete(BaseDir + @"bin\Nginx\myapp.wrapper.log");
            __Delete(BaseDir + @"bin\Nginx\myapp.xml");
            //_clear_file_log(BaseDir + @"bin\Apache\logs\access.log");
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
            //log(Environment.CurrentDirectory);
            //log(Application.ExecutablePath);
            Wstatus("正在启动中...");
            pre_start_SERVICE();
            _clear_record();
            _start_webserver();
            _start_mysql();

            this.button_start.Enabled = false;
            this.ini.WriteInteger("MDSERVER", "MD_RUN", 1);
            this.ini.WriteString("MDSERVER", "RUN_DIR", BaseDir.Replace("/", "\\"));
        }

        //启动web服务器
        private void _start_webserver()
        {
            //apache单选框状态
            bool apache = radioButton_Apache.Checked;
            bool nginx = radioButton_Nginx.Checked;
            if (apache)
            {
                _start_Apache();
            }
            else if (nginx)
            {
                _start_Nginx();
            }
        }

        //启动apache
        private void _start_Apache()
        {
            string apache = BaseDir + @"bin\Apache\bin\httpd.exe";
            string arg = "-k install -n " + MD_ApacheName;
            log(apache + " " + arg);
            Wcmd(apache + " " + arg);
            Thread.Sleep(1000);
            Wcmd("net start " + MD_ApacheName);
            //延迟执行
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Apache_lazy);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _start_Apache_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart(MD_ApacheName))
            {
                Wstatus_add("," + MD_ApacheName + "启动成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_ApacheName + "启动失败!!!");
            }
        }

        //启动nginx
        private void _start_Nginx()
        {
            string nginx_dir = BaseDir + @"bin\Nginx\";
            string php_dir = BaseDir + @"bin\PHP5.5.8\";
            string nginx_xml = nginx_dir + "myapp.xml";

            //安装过程
            string nginx_xml_content = "<service>\r\n" +
                "<id>" + MD_NginxName + "</id>\r\n" +
                "<name>" + MD_NginxName + "</name>\r\n" +
                "<description>" + MD_NginxName + "</description>\r\n" +
                "<executable>" + nginx_dir + "nginx.exe</executable>\r\n" +
                "<logpath>" + nginx_dir + "</logpath>\r\n" +
                "<logmode>roll</logmode>\r\n" +
                "<depend></depend>\r\n" +
                "<startargument>-p " + nginx_dir + "</startargument>\r\n" +
                "<stopargument>-p " + nginx_dir + " -s stop</stopargument>\r\n" +
                "</service>";
            _WriteContent(nginx_xml, nginx_xml_content);
            Wcmd(nginx_dir + "myapp.exe install");

            Thread.Sleep(3000);
            WStart(MD_NginxName);

            //延迟执行
            System.Timers.Timer nginx_timer = new System.Timers.Timer(3000);
            nginx_timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_Nginx_lazy);
            nginx_timer.Enabled = true;
            nginx_timer.AutoReset = false;
        }

        private void _start_Nginx_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            //执行PHP_CGI
            int php_cgi_num = this.ini.ReadInteger("MDSERVER", "PHP_RUN", 1);
            int pi = 0;
            for (; pi < php_cgi_num; ++pi)
            {
                _start_PHP_CGI();
            }

            Thread.Sleep(1000);
            if (WQueryServiceIsStart(MD_NginxName))
            {
                Wstatus_add("," + MD_NginxName + "启动成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_NginxName + "启动失败!!!");
            }

        }

        //启动PHP-CGI
        private void _start_PHP_CGI()
        {
            int port = this.ini.ReadInteger("MDSERVER", "PHP_PORT", 9000);
            string nginx_dir = BaseDir + @"bin\Nginx\";
            string php_ver = this.ini.ReadString("MDSERVER", "PHP_DIR", "PHP");
            string php_dir = BaseDir + @"bin\" + php_ver + @"\";
            Wcmd(nginx_dir + "RunHiddenConsole.exe " + php_dir + @"php-cgi.exe -b 127.0.0.1:" + port + " -c " + php_dir + @"php.ini");
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
            Thread.Sleep(4000);
            Wcmd("net start " + MD_MySQL);

            //延迟执行
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_start_mysql_lazy);
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void _start_mysql_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WQueryServiceIsStart(MD_MySQL))
            {
                Wstatus_add("," + MD_MySQL + "启动成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_MySQL + "启动失败!!!");
            }
        }

        //停止服务
        private void button_stop_Click(object sender, EventArgs e)
        {
            _stop_webserver();
            _stop_mysql();
            after_stop_SERVICE();
            this.button_start.Enabled = true;

            this.ini.WriteInteger("MDSERVER", "MD_RUN", 0);
            this.ini.WriteString("MDSERVER", "RUN_DIR", "");
        }

        //停止web服务
        private void _stop_webserver()
        {
            Wstatus("正在停止中...");
            _stop_apache();
            _stop_nginx();
        }

        //停止后配置恢复原状
        private void after_stop_SERVICE()
        {
            string[] conf = { 
                @"bin/Apache/conf/httpd.conf",
                @"bin/Nginx/conf/nginx.conf",
                @"bin/MySQL/my.ini",
                @"bin/PHP/php.ini"};
            foreach (string i in conf)
            {
                string r = _ReadContent(BaseDir + i);
                r = r.Replace(BaseDir, "MD:/");
                _WriteContent(BaseDir + i, r);
            }
        }

        //停止卸载Apache
        private void _stop_apache()
        {
            if (WServiceIsExisted(MD_ApacheName))
            {
                Wcmd("net stop " + MD_ApacheName);
                Thread.Sleep(1000);
                string apache = BaseDir + @"bin\Apache\bin\httpd.exe";
                string arg = "-k uninstall -n " + MD_ApacheName;
                Wcmd(apache + " " + arg);

                //延迟执行
                System.Timers.Timer timer = new System.Timers.Timer(3000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_stop_apache_lazy);
                timer.Enabled = true;
                timer.AutoReset = false;
            }
        }

        private void _stop_apache_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!WServiceIsExisted(MD_ApacheName) || !WQueryServiceIsStart(MD_ApacheName))
            {
                Wstatus_add("," + MD_ApacheName + "停止成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_ApacheName + "停止失败!!!");
            }
        }


        //停止nginx
        private void _stop_nginx()
        {
            if (WServiceIsExisted(MD_NginxName))
            {
                string nginx_dir = BaseDir + @"bin\Nginx\";
                Wcmd("net stop " + MD_NginxName);
                //Wcmd("taskkill /F /IM nginx.exe > nul");
                Wcmd("taskkill /F /IM php-cgi.exe > nul");
                Thread.Sleep(1000);
                //Wcmd("sc delete " + MD_NginxName);
                WUninstall(MD_NginxName);


                //延迟执行
                System.Timers.Timer timer = new System.Timers.Timer(3000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_stop_nginx_lazy);
                timer.Enabled = true;
                timer.AutoReset = false;
            }
        }

        private void _stop_nginx_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!WServiceIsExisted(MD_NginxName) || !WQueryServiceIsStart(MD_NginxName))
            {
                Wstatus_add("," + MD_NginxName + "停止成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_NginxName + "停止失败!!!");
            }
        }

        //停止mysql
        private void _stop_mysql()
        {
            string dir = BaseDir + @"bin\MySQL\";
            if (WServiceIsExisted(MD_MySQL))
            {
                WStop( MD_MySQL);
                Thread.Sleep(1000);
                //string installName = dir + @"bin\mysqld " + "remove " + MD_MySQL;
                //Wcmd(installName);
                WUninstall(MD_MySQL);


                //延迟执行
                System.Timers.Timer timer = new System.Timers.Timer(5000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(_stop_mysql_lazy);
                timer.Enabled = true;
                timer.AutoReset = false;
            }
        }

        private void _stop_mysql_lazy(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!WServiceIsExisted(MD_MySQL) || !WQueryServiceIsStart(MD_MySQL))
            {
                Wstatus_add("," + MD_MySQL + "停止成功!!!");
            }
            else
            {
                Wstatus_add("," + MD_MySQL + "停止失败!!!");
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
            if (WQueryServiceIsStart(MD_ApacheName))
            {
                System.Diagnostics.Process.Start("http://127.0.0.1/phpMyAdmin");
            }
            else if (WQueryServiceIsStart(MD_NginxName))
            {
                System.Diagnostics.Process.Start("http://127.0.0.1:8888");
            }
            else
            {
                MessageBox.Show("尚未开启服务!!!");
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
            System.Diagnostics.Process.Start(BaseDir.Replace("/","\\") + @"tool\kcachegrind\kcachegrind.exe");
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

                    Wstatus_add("," + serviceName + "已经启动成功!!!");
                    return true;
                }
                else
                {
                    service.Start();
                    for (int i = 0; i < 60; i++)
                    {
                        service.Start();
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                        {

                            Wstatus_add("," + serviceName + "启动成功!!!");
                            break;
                        }
                        if (i == 59)
                        {
                            Wstatus_add("," + serviceName + "启动失败!!!");
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Wstatus_add(ex.Message);
                return false;
            }
        }


        //暂停服务
        private void WStop_S(string serviceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    service.Stop();
                    Wstatus_add("," + serviceName + "停止成功!!!");
                }
                else
                {
                    Wstatus_add("," + serviceName + "已经停止!!!");
                    service.Refresh();
                }
            }
            catch (Exception ex)
            {
                Wstatus(ex.Message);
            }
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
        private bool _WriteContent(string path, string content)
        {
            bool ok = false;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
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
            string str = File.ReadAllText(path);
            return str;
        }

        //删除文件
        private void __Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
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


    }
}