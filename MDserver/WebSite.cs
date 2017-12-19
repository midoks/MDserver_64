using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace MDserver
{
    public partial class WebSite : Form
    {
        private static string BaseDir = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");

        private SystemXml iniXml;
        public MDserv mainUI;

        //port总长度
        private int PORTLEN = 10;
        //域名长度
        private int DOMAINLEN = 30;

        public WebSite()
        {
            InitializeComponent();
            this.iniXml = new SystemXml(BaseDir + "host.xml");
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        //设置为有一定
        public string setLenValue(string value, int len)
        {
            int vlen = value.Length;
            int padLen = len - vlen;
            this.mainUI.log(padLen.ToString() + ":");
            if (padLen > 0)
            {
                value = value.PadRight(padLen, ' ');
            }
            return value;
        }
        public string getRealValue(string value)
        {
            return value.Trim();
        }


        private void WebSite_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        public void InitUI()
        {
            domainList.SelectionMode = DataGridViewSelectionMode.CellSelect;

            //禁止排序
            for (int i = 0; i < this.domainList.Columns.Count; i++)
            {
                this.domainList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.domainList.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }

            string hostname = setLenValue("localhost", DOMAINLEN);
            string port = setLenValue("80", PORTLEN);
            string root_dir = BaseDir + "www/htdocs";

            DataGridViewRow Row = new DataGridViewRow();
            int index = domainList.Rows.Add(Row);
            domainList.Rows[index].Cells[0].Value = hostname;
            domainList.Rows[index].Cells[1].Value = port;

            textBox_hostname.Text = getRealValue(hostname);
            textBox_Port.Text = getRealValue(port);
            textBox_rootDir.Text = root_dir;



            XmlNode r = this.iniXml.rootNode();
            foreach (XmlNode r_one in r)
            {
                DataGridViewRow Rt = new DataGridViewRow();
                int t = domainList.Rows.Add(Rt);
                domainList.Rows[t].Cells[0].Value = setLenValue(r_one.Attributes["name"].Value, DOMAINLEN);
                domainList.Rows[t].Cells[1].Value = setLenValue(r_one.Attributes["port"].Value, PORTLEN);

                //this.mainUI.log(domainList.Rows[t].Cells[0].Value+ ":");
                //this.mainUI.log(domainList.Rows[t].Cells[1].Value + ":");
            }

        }


        private void domainList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = domainList.CurrentRow.Index;

            string hostname = domainList.Rows[index].Cells[0].Value.ToString();
            string port = domainList.Rows[index].Cells[1].Value.ToString();
            textBox_hostname.Text = getRealValue(hostname);
            textBox_Port.Text = getRealValue(port);

            if (index > 0)
            {
                XmlNode ch = this.iniXml.selectedNode(index - 1);

                string v = ch.Attributes["root_dir"].Value;
                textBox_rootDir.Text = v;
            }
            else
            {
                string root_dir = BaseDir + "www/htdocs";
                textBox_rootDir.Text = root_dir;
            }

        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int count = domainList.RowCount;

            DataGridViewRow Row = new DataGridViewRow();
            //domainList.RowHeadersWidth = 45;

            string hostname = Guid.NewGuid().ToString().Substring(0, 8) + ".com"; ;
            string port = "80";

            hostname = setLenValue(hostname, 30);
            port = setLenValue(port, 10);

            string root_dir = BaseDir + "www/htdocs";

            int index = domainList.Rows.Add(Row);
            domainList.Rows[index].Cells[0].Value = hostname;
            domainList.Rows[index].Cells[1].Value = port;

            this.iniXml.addNode(hostname, "port", getRealValue(port));
            this.iniXml.updateNode(index - 1, "root_dir", getRealValue(root_dir));


            restart();

        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            int index = domainList.CurrentRow.Index;
            if (index > 0)
            {
                domainList.Rows.RemoveAt(index);
                this.iniXml.removeNode(index - 1);

                restart();
            }
            else
            {
                MessageBox.Show("localhost不可修改");
            }

        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            var ok = checkFunc();
            if (ok)
            {
                int index = domainList.CurrentRow.Index;

                string hostname = getRealValue(domainList.Rows[index].Cells[0].Value.ToString());
                string port = getRealValue(domainList.Rows[index].Cells[1].Value.ToString());

                string root_dir = BaseDir + "www/htdocs";
                if (index > 0)
                {
                    XmlNode ch = this.iniXml.selectedNode(index - 1);
                    root_dir = ch.Attributes["root_dir"].Value.ToString();
                }

                string php_tmp = "tmp_" + Guid.NewGuid().ToString().Replace("-", "_") + ".php";
                var cbool = this.mainUI._WriteContent(root_dir + "/" + php_tmp, "<?php phpinfo(); ?>");
                if (cbool)
                {
                    System.Diagnostics.Process.Start("http://" + hostname + ":" + port + "/" + php_tmp);


                    Thread delfile = new Thread(() =>
                    {
                        Thread.Sleep(3000);
                        //删除临时文件
                        File.Delete(root_dir + "/" + php_tmp);
                    });
                    delfile.Start();
                }
                else
                {
                    MessageBox.Show("创建临时文件失败!");
                }
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            var ok = checkFunc();
            if (ok)
            {
                int index = domainList.CurrentRow.Index;
                string hostname = getRealValue(domainList.Rows[index].Cells[0].Value.ToString());
                string port = getRealValue(domainList.Rows[index].Cells[1].Value.ToString());

                System.Diagnostics.Process.Start("http://" + hostname + ":" + port + "/");
            }
        }

        private void buttonSelectDir_Click(object sender, EventArgs e)
        {
            int index = domainList.CurrentRow.Index;
            if (index > 0)
            {
                FolderBrowserDialog dir = new FolderBrowserDialog();
                dir.RootFolder = Environment.SpecialFolder.Desktop;
                if (dir.ShowDialog() == DialogResult.OK)
                {
                    if (index > 0)
                    {
                        this.iniXml.updateNode(index - 1, "root_dir", dir.SelectedPath);
                        textBox_rootDir.Text = dir.SelectedPath;
                    }
                }

                restart();
            }
            else
            {
                MessageBox.Show("localhost不可修改");
            }
        }

        private void buttonRootGo_Click(object sender, EventArgs e)
        {
            string dir = textBox_rootDir.Text;
            //dir = getDirPath(dir);
            if (Directory.Exists(dir))
            {
                System.Diagnostics.Process.Start(dir);
            }
            else
            {
                MessageBox.Show(dir + "目录不存在!");
            }

        }

        private void textBox_hostname_TextChanged(object sender, EventArgs e)
        {
            int index = domainList.CurrentRow.Index;
            string conf_hostname;
            string hostname = textBox_hostname.Text.Trim();

            if (index > 0)
            {
                bool isRepeat = false;

                //检查是否为空
                if (hostname.Equals(""))
                {
                    MessageBox.Show("端口不能为空!!");
                    textBox_hostname.Text = Guid.NewGuid().ToString().Substring(0, 8) + ".com";
                    return;
                }

                //检查是否重复
                XmlNode hostXml = this.iniXml.rootNode();
                int hostIndex = 1;
                foreach (XmlNode host in hostXml)
                {
                    conf_hostname = host.Attributes["name"].Value;
                    
                    if (hostname.Equals(conf_hostname) && index != hostIndex)
                    {
                        isRepeat = true;
                        break;
                    }
                    hostIndex++;
                }

                if (isRepeat)
                {
                    MessageBox.Show("域名不要重复!!");
                    textBox_hostname.Text = Guid.NewGuid().ToString().Substring(0, 8) + ".com";
                    return;
                }

                domainList.Rows[index].Cells[0].Value = setLenValue(textBox_hostname.Text.Trim(), DOMAINLEN);
                this.iniXml.updateNode(index - 1, "name", textBox_hostname.Text.Trim());

            }
            else
            {
                if (textBox_hostname.Text == "localhost")
                {
                }
                else
                {
                    MessageBox.Show("localhost不可修改!");
                    textBox_hostname.Text = "localhost";
                }
                return;
            }
        }

        private void textBox_Port_TextChanged(object sender, EventArgs e)
        {
            int index = domainList.CurrentRow.Index;
            string port = textBox_Port.Text;
            if (index > 0)
            {


                if (port.Equals(""))
                {
                    MessageBox.Show("端口不能为空!!");
                    textBox_Port.Text = "80";
                    return;
                }

                int portInt = int.Parse(port);
                if (portInt > 0 && portInt < 65536)
                {
                    textBox_Port.Text = portInt.ToString();
                    port = textBox_Port.Text;
                    domainList.Rows[index].Cells[1].Value = setLenValue(port, PORTLEN);
                    this.iniXml.updateNode(index - 1, "port", textBox_Port.Text);
                }
                else
                {
                    MessageBox.Show("端口范围在0~65536之内");
                    textBox_Port.Text = "80";
                }

            }
            else
            {
                if (port != "80")
                {
                    MessageBox.Show("localhost端口不可修改!");
                    textBox_Port.Text = "80";
                    return;
                }
            }
        }


        //私有方法
        public Boolean checkFunc()
        {
            bool isRun = !this.mainUI.button_start.Enabled;
            int index = domainList.CurrentRow.Index;

            if (!isRun)
            {
                MessageBox.Show("HTTP服务没有启动!!!");
                return false;
            }
            //else if (index == 0) {
            //    MessageBox.Show("localhost不可修改");
            //    return false;
            //}

            return true;
        }

        public void restart()
        {
            var isRun = !this.mainUI.button_start.Enabled;
            if (isRun)
            {
                DialogResult qa = MessageBox.Show("要想立即生效需要重新启动,是否重新启动!!!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (qa == DialogResult.Yes)
                {
                    Thread re = new Thread(() =>
                    {
                        this.mainUI.restart();
                    });
                    re.Start();
                }
            }
        }

        private void button_RestartServer_Click(object sender, EventArgs e)
        {
            restart();
        }
    }
}
