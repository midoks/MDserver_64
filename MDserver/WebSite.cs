using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace MDserver
{
    public partial class WebSite : Form
    {
        private static string BaseDir = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        private SystemXml iniXml;
        public MDserv mainUI;

        public WebSite()
        {
            InitializeComponent();
            this.iniXml = new SystemXml(BaseDir + "host.xml");
        }

        //设置为有一定
        public string setLenValue(string value, int len)
        {
            int vlen = value.Length;
            if (len - vlen > 0)
            {
                value = value.PadRight(len - vlen, ' ');
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

            string hostname = setLenValue("localhost", 30);
            string port = setLenValue("80", 10);
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
                domainList.Rows[t].Cells[0].Value = setLenValue(r_one.Attributes["name"].Value, 30);
                domainList.Rows[t].Cells[1].Value = setLenValue(r_one.Attributes["port"].Value, 10);

            }

        }


        private void domainList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // System.Diagnostics.Debug.WriteLine("信息");
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


        private void domainList_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("信r息");

        }

        private void domainList_CellEenter(object sender, DataGridViewCellEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("domainList_CellCenter");

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int count = domainList.RowCount;

            DataGridViewRow Row = new DataGridViewRow();
            //domainList.RowHeadersWidth = 45;

            string hostname = "tmp" + count.ToString() + ".com";
            string port = "80";

            hostname = setLenValue(hostname, 30);
            port = setLenValue(port, 10);

            string root_dir = BaseDir + "www/htdocs";

            int index = domainList.Rows.Add(Row);
            domainList.Rows[index].Cells[0].Value = hostname;
            domainList.Rows[index].Cells[1].Value = port;

            this.iniXml.addNode(hostname, "port", getRealValue(port));
            this.iniXml.updateNode(index - 1, "root_dir", getRealValue(root_dir));

        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            int index = domainList.CurrentRow.Index;
            if (index > 0)
            {
                domainList.Rows.RemoveAt(index);
                this.iniXml.removeNode(index - 1);
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
                if (index > 1)
                {
                    XmlNode ch = this.iniXml.selectedNode(index - 1);
                    root_dir = ch.Attributes["root_dir"].Value.ToString();
                }


                string php_tmp = "tmp_" + Guid.NewGuid().ToString().Replace("-", "_") + ".php";

                this.mainUI._WriteContent(root_dir + "/" + php_tmp, "<?php phpinfo(); ?>");

                Console.WriteLine(php_tmp);


                System.Diagnostics.Process.Start("http://" + hostname + ":" + port + "/" + php_tmp);
                int a = 123132;
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Enabled = false;

            }
        }



        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
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
            if (index > 0)
            {
                System.Diagnostics.Debug.WriteLine(index.ToString());

                domainList.Rows[index].Cells[0].Value = setLenValue(textBox_hostname.Text, 30);
                this.iniXml.updateNode(index - 1, "name", textBox_hostname.Text);

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
            if (index > 0)
            {
                domainList.Rows[index].Cells[1].Value = setLenValue(textBox_Port.Text, 10);
                this.iniXml.updateNode(index - 1, "port", textBox_Port.Text);
            }
            else
            {

                if (textBox_Port.Text == "80")
                {
                }
                else
                {
                    MessageBox.Show("localhost不可修改!");
                    textBox_Port.Text = "80";
                }

                return;
            }

        }


        //私有方法
        public Boolean checkFunc()
        {

            var isRun = !this.mainUI.button_start.Enabled;

            if (!isRun)
            {
                MessageBox.Show("HTTP服务没有启动!!!");
                return false;

                int index = domainList.CurrentRow.Index;
                if (index > 0)
                {

                }
                else
                {
                    MessageBox.Show("localhost不可修改");
                    return false;
                }
            }
            return true;
        }
    }
}
