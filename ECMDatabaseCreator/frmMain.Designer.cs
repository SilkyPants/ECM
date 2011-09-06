namespace ECMDatabaseCreator
{
    partial class frmMain
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
            this.grpServerType = new System.Windows.Forms.GroupBox();
            this.rdoSQLite = new System.Windows.Forms.RadioButton();
            this.rdoMySql = new System.Windows.Forms.RadioButton();
            this.rdoMSSQL = new System.Windows.Forms.RadioButton();
            this.grpServerDetails = new System.Windows.Forms.GroupBox();
            this.btnOpenSQLiteDB = new System.Windows.Forms.Button();
            this.chkIntegratedSec = new System.Windows.Forms.CheckBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pgbProgress = new System.Windows.Forms.ProgressBar();
            this.lblRecords = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.grpServerType.SuspendLayout();
            this.grpServerDetails.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpServerType
            // 
            this.grpServerType.Controls.Add(this.rdoSQLite);
            this.grpServerType.Controls.Add(this.rdoMySql);
            this.grpServerType.Controls.Add(this.rdoMSSQL);
            this.grpServerType.Location = new System.Drawing.Point(12, 12);
            this.grpServerType.Name = "grpServerType";
            this.grpServerType.Size = new System.Drawing.Size(82, 100);
            this.grpServerType.TabIndex = 0;
            this.grpServerType.TabStop = false;
            this.grpServerType.Text = "Server Type";
            // 
            // rdoSQLite
            // 
            this.rdoSQLite.AutoSize = true;
            this.rdoSQLite.Enabled = false;
            this.rdoSQLite.Location = new System.Drawing.Point(6, 65);
            this.rdoSQLite.Name = "rdoSQLite";
            this.rdoSQLite.Size = new System.Drawing.Size(57, 17);
            this.rdoSQLite.TabIndex = 2;
            this.rdoSQLite.TabStop = true;
            this.rdoSQLite.Text = "SQLite";
            this.rdoSQLite.UseVisualStyleBackColor = true;
            this.rdoSQLite.CheckedChanged += new System.EventHandler(this.ServerTypeChanged);
            // 
            // rdoMySql
            // 
            this.rdoMySql.AutoSize = true;
            this.rdoMySql.Checked = true;
            this.rdoMySql.Location = new System.Drawing.Point(6, 42);
            this.rdoMySql.Name = "rdoMySql";
            this.rdoMySql.Size = new System.Drawing.Size(54, 17);
            this.rdoMySql.TabIndex = 1;
            this.rdoMySql.TabStop = true;
            this.rdoMySql.Text = "MySql";
            this.rdoMySql.UseVisualStyleBackColor = true;
            this.rdoMySql.CheckedChanged += new System.EventHandler(this.ServerTypeChanged);
            // 
            // rdoMSSQL
            // 
            this.rdoMSSQL.AutoSize = true;
            this.rdoMSSQL.Location = new System.Drawing.Point(6, 19);
            this.rdoMSSQL.Name = "rdoMSSQL";
            this.rdoMSSQL.Size = new System.Drawing.Size(65, 17);
            this.rdoMSSQL.TabIndex = 0;
            this.rdoMSSQL.TabStop = true;
            this.rdoMSSQL.Text = "MS SQL";
            this.rdoMSSQL.UseVisualStyleBackColor = true;
            this.rdoMSSQL.CheckedChanged += new System.EventHandler(this.ServerTypeChanged);
            // 
            // grpServerDetails
            // 
            this.grpServerDetails.Controls.Add(this.txtPort);
            this.grpServerDetails.Controls.Add(this.lblPort);
            this.grpServerDetails.Controls.Add(this.btnOpenSQLiteDB);
            this.grpServerDetails.Controls.Add(this.btnStart);
            this.grpServerDetails.Controls.Add(this.chkIntegratedSec);
            this.grpServerDetails.Controls.Add(this.txtPass);
            this.grpServerDetails.Controls.Add(this.label3);
            this.grpServerDetails.Controls.Add(this.txtUser);
            this.grpServerDetails.Controls.Add(this.label2);
            this.grpServerDetails.Controls.Add(this.txtSource);
            this.grpServerDetails.Controls.Add(this.label1);
            this.grpServerDetails.Location = new System.Drawing.Point(100, 12);
            this.grpServerDetails.Name = "grpServerDetails";
            this.grpServerDetails.Size = new System.Drawing.Size(255, 129);
            this.grpServerDetails.TabIndex = 1;
            this.grpServerDetails.TabStop = false;
            this.grpServerDetails.Text = "Server Details";
            // 
            // btnOpenSQLiteDB
            // 
            this.btnOpenSQLiteDB.Location = new System.Drawing.Point(225, 19);
            this.btnOpenSQLiteDB.Name = "btnOpenSQLiteDB";
            this.btnOpenSQLiteDB.Size = new System.Drawing.Size(24, 23);
            this.btnOpenSQLiteDB.TabIndex = 4;
            this.btnOpenSQLiteDB.Text = "...";
            this.btnOpenSQLiteDB.UseVisualStyleBackColor = true;
            // 
            // chkIntegratedSec
            // 
            this.chkIntegratedSec.Location = new System.Drawing.Point(175, 53);
            this.chkIntegratedSec.Name = "chkIntegratedSec";
            this.chkIntegratedSec.Size = new System.Drawing.Size(74, 34);
            this.chkIntegratedSec.TabIndex = 6;
            this.chkIntegratedSec.Text = "Integrated Security";
            this.chkIntegratedSec.UseVisualStyleBackColor = true;
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(65, 71);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(104, 20);
            this.txtPass.TabIndex = 5;
            this.txtPass.Text = "zn645szn";
            this.txtPass.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(65, 45);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(104, 20);
            this.txtUser.TabIndex = 3;
            this.txtUser.Text = "root";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(65, 19);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(154, 20);
            this.txtSource.TabIndex = 1;
            this.txtSource.Text = "localhost";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(175, 100);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(74, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.StartWorker);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pgbProgress);
            this.groupBox1.Controls.Add(this.lblRecords);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Location = new System.Drawing.Point(18, 147);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 62);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // pgbProgress
            // 
            this.pgbProgress.Location = new System.Drawing.Point(6, 45);
            this.pgbProgress.Name = "pgbProgress";
            this.pgbProgress.Size = new System.Drawing.Size(325, 10);
            this.pgbProgress.TabIndex = 7;
            // 
            // lblRecords
            // 
            this.lblRecords.AutoSize = true;
            this.lblRecords.Location = new System.Drawing.Point(6, 29);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(0, 13);
            this.lblRecords.TabIndex = 6;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(6, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(52, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Waiting...";
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CreateDatabase);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UpdateProgress);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WorkCompleted);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(65, 97);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(104, 20);
            this.txtPort.TabIndex = 8;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 100);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 7;
            this.lblPort.Text = "Port";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 224);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpServerDetails);
            this.Controls.Add(this.grpServerType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmMain";
            this.Text = "ECM Database Creator";
            this.grpServerType.ResumeLayout(false);
            this.grpServerType.PerformLayout();
            this.grpServerDetails.ResumeLayout(false);
            this.grpServerDetails.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpServerType;
        private System.Windows.Forms.RadioButton rdoSQLite;
        private System.Windows.Forms.RadioButton rdoMySql;
        private System.Windows.Forms.RadioButton rdoMSSQL;
        private System.Windows.Forms.GroupBox grpServerDetails;
        private System.Windows.Forms.Button btnOpenSQLiteDB;
        private System.Windows.Forms.CheckBox chkIntegratedSec;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar pgbProgress;
        private System.Windows.Forms.Label lblRecords;
        private System.Windows.Forms.Label lblStatus;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
    }
}

