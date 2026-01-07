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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pnlAction = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.nudTimeSpent = new System.Windows.Forms.NumericUpDown();
            this.cbPendingTasks = new System.Windows.Forms.ComboBox();
            this.rtbSubmissionMessage = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvMyTasks = new System.Windows.Forms.DataGridView();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTaskDescription = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.pnlAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSpent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyTasks)).BeginInit();
            this.pnlHeader.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pnlAction);
            this.groupBox2.Controls.Add(this.dgvMyTasks);
            this.groupBox2.Controls.Add(this.pnlHeader);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1168, 482);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // pnlAction
            // 
            this.pnlAction.Controls.Add(this.lblTaskDescription);
            this.pnlAction.Controls.Add(this.label3);
            this.pnlAction.Controls.Add(this.label1);
            this.pnlAction.Controls.Add(this.button2);
            this.pnlAction.Controls.Add(this.nudTimeSpent);
            this.pnlAction.Controls.Add(this.cbPendingTasks);
            this.pnlAction.Controls.Add(this.rtbSubmissionMessage);
            this.pnlAction.Controls.Add(this.label2);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAction.Location = new System.Drawing.Point(3, 289);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Size = new System.Drawing.Size(1162, 190);
            this.pnlAction.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(390, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Description:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(217, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Time Spent:";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(599, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Submit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // nudTimeSpent
            // 
            this.nudTimeSpent.Location = new System.Drawing.Point(220, 33);
            this.nudTimeSpent.Name = "nudTimeSpent";
            this.nudTimeSpent.Size = new System.Drawing.Size(120, 22);
            this.nudTimeSpent.TabIndex = 6;
            // 
            // cbPendingTasks
            // 
            this.cbPendingTasks.FormattingEnabled = true;
            this.cbPendingTasks.Location = new System.Drawing.Point(9, 31);
            this.cbPendingTasks.Name = "cbPendingTasks";
            this.cbPendingTasks.Size = new System.Drawing.Size(150, 24);
            this.cbPendingTasks.TabIndex = 3;
            this.cbPendingTasks.SelectedIndexChanged += new System.EventHandler(this.cbPendingTasks_SelectedIndexChanged);
            // 
            // rtbSubmissionMessage
            // 
            this.rtbSubmissionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSubmissionMessage.Location = new System.Drawing.Point(393, 30);
            this.rtbSubmissionMessage.Name = "rtbSubmissionMessage";
            this.rtbSubmissionMessage.Size = new System.Drawing.Size(150, 92);
            this.rtbSubmissionMessage.TabIndex = 4;
            this.rtbSubmissionMessage.Text = "";
            this.rtbSubmissionMessage.TextChanged += new System.EventHandler(this.rtbSubmissionMessage_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Select a task:";
            // 
            // dgvMyTasks
            // 
            this.dgvMyTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMyTasks.Location = new System.Drawing.Point(3, 118);
            this.dgvMyTasks.Name = "dgvMyTasks";
            this.dgvMyTasks.RowHeadersWidth = 51;
            this.dgvMyTasks.RowTemplate.Height = 24;
            this.dgvMyTasks.Size = new System.Drawing.Size(1162, 361);
            this.dgvMyTasks.TabIndex = 3;
            this.dgvMyTasks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMyTasks_CellContentClick);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.label5);
            this.pnlHeader.Controls.Add(this.label6);
            this.pnlHeader.Controls.Add(this.lblEmail);
            this.pnlHeader.Controls.Add(this.lblFullName);
            this.pnlHeader.Controls.Add(this.lblRole);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(3, 18);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1162, 100);
            this.pnlHeader.TabIndex = 6;
            this.pnlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 26);
            this.label5.TabIndex = 5;
            this.label5.Text = "Email:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 26);
            this.label6.TabIndex = 4;
            this.label6.Text = "role:";
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(91, 74);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(136, 23);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "email";
            // 
            // lblFullName
            // 
            this.lblFullName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.8F);
            this.lblFullName.Location = new System.Drawing.Point(3, 0);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(167, 31);
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Text = "fullname";
            // 
            // lblRole
            // 
            this.lblRole.Location = new System.Drawing.Point(91, 46);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(124, 26);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "role";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageToolStripMenuItem,
            this.notificationsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1168, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pageToolStripMenuItem
            // 
            this.pageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logoutToolStripMenuItem});
            this.pageToolStripMenuItem.Name = "pageToolStripMenuItem";
            this.pageToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
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
            this.notificationsToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.notificationsToolStripMenuItem.Text = "Notifications";
            // 
            // viewNotificationsToolStripMenuItem
            // 
            this.viewNotificationsToolStripMenuItem.Name = "viewNotificationsToolStripMenuItem";
            this.viewNotificationsToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.viewNotificationsToolStripMenuItem.Text = "View Notifications";
            this.viewNotificationsToolStripMenuItem.Click += new System.EventHandler(this.viewNotificationsToolStripMenuItem_Click);
            // 
            // lblTaskDescription
            // 
            this.lblTaskDescription.Location = new System.Drawing.Point(12, 78);
            this.lblTaskDescription.Name = "lblTaskDescription";
            this.lblTaskDescription.Size = new System.Drawing.Size(203, 63);
            this.lblTaskDescription.TabIndex = 12;
            this.lblTaskDescription.Text = "label4";
            // 
            // EmployeeDashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 510);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.Name = "EmployeeDashboardForm";
            this.Text = "ol";
            this.Load += new System.EventHandler(this.EmployeeDashboardForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.pnlAction.ResumeLayout(false);
            this.pnlAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSpent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyTasks)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvMyTasks;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox rtbSubmissionMessage;
        private System.Windows.Forms.ComboBox cbPendingTasks;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewNotificationsToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown nudTimeSpent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Panel pnlAction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTaskDescription;
    }
}