namespace MDserver
{
    partial class WebSite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebSite));
            this.domainList = new System.Windows.Forms.DataGridView();
            this.groupList = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_rootDir = new System.Windows.Forms.TextBox();
            this.buttonSelectDir = new System.Windows.Forms.Button();
            this.buttonRootGo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonInfo = new System.Windows.Forms.Button();
            this.buttonGo = new System.Windows.Forms.Button();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_hostname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDel = new System.Windows.Forms.Button();
            this.hostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.domainList)).BeginInit();
            this.groupList.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // domainList
            // 
            this.domainList.AllowUserToAddRows = false;
            this.domainList.AllowUserToDeleteRows = false;
            this.domainList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.domainList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.domainList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.domainList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.hostname,
            this.port});
            this.domainList.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.domainList.Location = new System.Drawing.Point(15, 20);
            this.domainList.Name = "domainList";
            this.domainList.ReadOnly = true;
            this.domainList.RowTemplate.Height = 23;
            this.domainList.Size = new System.Drawing.Size(241, 197);
            this.domainList.TabIndex = 0;
            this.domainList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.domainList_CellContentClick);
            // 
            // groupList
            // 
            this.groupList.Controls.Add(this.buttonDel);
            this.groupList.Controls.Add(this.buttonAdd);
            this.groupList.Controls.Add(this.domainList);
            this.groupList.Location = new System.Drawing.Point(12, 12);
            this.groupList.Name = "groupList";
            this.groupList.Size = new System.Drawing.Size(262, 257);
            this.groupList.TabIndex = 1;
            this.groupList.TabStop = false;
            this.groupList.Text = "List";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_rootDir);
            this.groupBox1.Controls.Add(this.buttonSelectDir);
            this.groupBox1.Controls.Add(this.buttonRootGo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.buttonInfo);
            this.groupBox1.Controls.Add(this.buttonGo);
            this.groupBox1.Controls.Add(this.textBox_Port);
            this.groupBox1.Controls.Add(this.textBox_hostname);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(284, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 257);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // textBox_rootDir
            // 
            this.textBox_rootDir.Location = new System.Drawing.Point(14, 143);
            this.textBox_rootDir.Name = "textBox_rootDir";
            this.textBox_rootDir.ReadOnly = true;
            this.textBox_rootDir.Size = new System.Drawing.Size(224, 21);
            this.textBox_rootDir.TabIndex = 9;
            // 
            // buttonSelectDir
            // 
            this.buttonSelectDir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSelectDir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSelectDir.Location = new System.Drawing.Point(164, 114);
            this.buttonSelectDir.Name = "buttonSelectDir";
            this.buttonSelectDir.Size = new System.Drawing.Size(74, 23);
            this.buttonSelectDir.TabIndex = 8;
            this.buttonSelectDir.Text = "SelectDir";
            this.buttonSelectDir.UseVisualStyleBackColor = true;
            this.buttonSelectDir.Click += new System.EventHandler(this.buttonSelectDir_Click);
            // 
            // buttonRootGo
            // 
            this.buttonRootGo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonRootGo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRootGo.Location = new System.Drawing.Point(128, 114);
            this.buttonRootGo.Name = "buttonRootGo";
            this.buttonRootGo.Size = new System.Drawing.Size(28, 23);
            this.buttonRootGo.TabIndex = 7;
            this.buttonRootGo.Text = "Go";
            this.buttonRootGo.UseVisualStyleBackColor = true;
            this.buttonRootGo.Click += new System.EventHandler(this.buttonRootGo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Doucment Root:";
            // 
            // buttonInfo
            // 
            this.buttonInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonInfo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonInfo.Location = new System.Drawing.Point(226, 29);
            this.buttonInfo.Name = "buttonInfo";
            this.buttonInfo.Size = new System.Drawing.Size(28, 23);
            this.buttonInfo.TabIndex = 5;
            this.buttonInfo.Text = "i";
            this.buttonInfo.UseVisualStyleBackColor = true;
            this.buttonInfo.Click += new System.EventHandler(this.buttonInfo_Click);
            // 
            // buttonGo
            // 
            this.buttonGo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonGo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonGo.Location = new System.Drawing.Point(196, 29);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(28, 23);
            this.buttonGo.TabIndex = 4;
            this.buttonGo.Text = "Go";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(91, 56);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(65, 21);
            this.textBox_Port.TabIndex = 3;
            this.textBox_Port.Text = "80";
            this.textBox_Port.TextChanged += new System.EventHandler(this.textBox_Port_TextChanged);
            // 
            // textBox_hostname
            // 
            this.textBox_hostname.Location = new System.Drawing.Point(91, 30);
            this.textBox_hostname.Name = "textBox_hostname";
            this.textBox_hostname.Size = new System.Drawing.Size(100, 21);
            this.textBox_hostname.TabIndex = 2;
            this.textBox_hostname.Text = "localhost";
            this.textBox_hostname.TextChanged += new System.EventHandler(this.textBox_hostname_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port Number:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // buttonAdd
            // 
            this.buttonAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonAdd.Location = new System.Drawing.Point(15, 223);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(28, 28);
            this.buttonAdd.TabIndex = 10;
            this.buttonAdd.Text = " +";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDel
            // 
            this.buttonDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDel.Location = new System.Drawing.Point(47, 223);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(28, 28);
            this.buttonDel.TabIndex = 11;
            this.buttonDel.Text = " -";
            this.buttonDel.UseVisualStyleBackColor = true;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // hostname
            // 
            this.hostname.HeaderText = "hostname";
            this.hostname.Name = "hostname";
            this.hostname.ReadOnly = true;
            this.hostname.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // port
            // 
            this.port.HeaderText = "port";
            this.port.MaxInputLength = 10;
            this.port.Name = "port";
            this.port.ReadOnly = true;
            this.port.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.port.Width = 80;
            // 
            // WebSite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 296);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupList);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebSite";
            this.Text = "Web管理";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WebSite_Load);
            ((System.ComponentModel.ISupportInitialize)(this.domainList)).EndInit();
            this.groupList.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView domainList;
        private System.Windows.Forms.GroupBox groupList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_hostname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Button buttonInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSelectDir;
        private System.Windows.Forms.Button buttonRootGo;
        private System.Windows.Forms.TextBox textBox_rootDir;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDel;
        private System.Windows.Forms.DataGridViewTextBoxColumn hostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn port;
    }
}