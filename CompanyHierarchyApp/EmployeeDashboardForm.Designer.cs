namespace CompanyHierarchyApp
{
    partial class EmployeeDashboardForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dgvMyTasks = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.rtbSubmissionMessage = new System.Windows.Forms.RichTextBox();
            this.cbPendingTasks = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudTimeSpent = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyTasks)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSpent)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblFullName);
            this.groupBox1.Controls.Add(this.lblEmail);
            this.groupBox1.Controls.Add(this.lblRole);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(655, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lblFullName
            // 
            this.lblFullName.Location = new System.Drawing.Point(124, 2);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(85, 31);
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Text = "fullname";
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(361, 36);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(56, 16);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "email";
            this.lblEmail.Click += new System.EventHandler(this.label3_Click);
            // 
            // lblRole
            // 
            this.lblRole.Location = new System.Drawing.Point(124, 33);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(92, 34);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "role";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.dgvMyTasks);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(655, 405);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(219, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgvMyTasks
            // 
            this.dgvMyTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyTasks.Location = new System.Drawing.Point(-36, 74);
            this.dgvMyTasks.Name = "dgvMyTasks";
            this.dgvMyTasks.RowHeadersWidth = 51;
            this.dgvMyTasks.RowTemplate.Height = 24;
            this.dgvMyTasks.Size = new System.Drawing.Size(685, 175);
            this.dgvMyTasks.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nudTimeSpent);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.rtbSubmissionMessage);
            this.groupBox3.Controls.Add(this.cbPendingTasks);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(0, 354);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(655, 156);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(297, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Submit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // rtbSubmissionMessage
            // 
            this.rtbSubmissionMessage.Location = new System.Drawing.Point(297, 58);
            this.rtbSubmissionMessage.Name = "rtbSubmissionMessage";
            this.rtbSubmissionMessage.Size = new System.Drawing.Size(150, 36);
            this.rtbSubmissionMessage.TabIndex = 4;
            this.rtbSubmissionMessage.Text = "";
            this.rtbSubmissionMessage.TextChanged += new System.EventHandler(this.rtbSubmissionMessage_TextChanged);
            // 
            // cbPendingTasks
            // 
            this.cbPendingTasks.FormattingEnabled = true;
            this.cbPendingTasks.Location = new System.Drawing.Point(30, 58);
            this.cbPendingTasks.Name = "cbPendingTasks";
            this.cbPendingTasks.Size = new System.Drawing.Size(150, 24);
            this.cbPendingTasks.TabIndex = 3;
            this.cbPendingTasks.SelectedIndexChanged += new System.EventHandler(this.cbPendingTasks_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageToolStripMenuItem,
            this.notificationsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(655, 30);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pageToolStripMenuItem
            // 
            this.pageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logoutToolStripMenuItem});
            this.pageToolStripMenuItem.Name = "pageToolStripMenuItem";
            this.pageToolStripMenuItem.Size = new System.Drawing.Size(55, 26);
            this.pageToolStripMenuItem.Text = "Page";
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(139, 26);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // notificationsToolStripMenuItem
            // 
            this.notificationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewNotificationsToolStripMenuItem});
            this.notificationsToolStripMenuItem.Name = "notificationsToolStripMenuItem";
            this.notificationsToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
            this.notificationsToolStripMenuItem.Text = "Notifications";
            // 
            // viewNotificationsToolStripMenuItem
            // 
            this.viewNotificationsToolStripMenuItem.Name = "viewNotificationsToolStripMenuItem";
            this.viewNotificationsToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.viewNotificationsToolStripMenuItem.Text = "View Notifications";
            this.viewNotificationsToolStripMenuItem.Click += new System.EventHandler(this.viewNotificationsToolStripMenuItem_Click);
            // 
            // nudTimeSpent
            // 
            this.nudTimeSpent.Location = new System.Drawing.Point(30, 108);
            this.nudTimeSpent.Name = "nudTimeSpent";
            this.nudTimeSpent.Size = new System.Drawing.Size(120, 22);
            this.nudTimeSpent.TabIndex = 6;
            // 
            // EmployeeDashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 510);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "EmployeeDashboardForm";
            this.Text = "ol";
            this.Load += new System.EventHandler(this.EmployeeDashboardForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyTasks)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSpent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvMyTasks;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox rtbSubmissionMessage;
        private System.Windows.Forms.ComboBox cbPendingTasks;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewNotificationsToolStripMenuItem;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.NumericUpDown nudTimeSpent;
    }
}