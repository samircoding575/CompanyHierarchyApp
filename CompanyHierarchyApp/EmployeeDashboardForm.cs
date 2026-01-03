using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Added for UI Styling
using System.Windows.Forms;

namespace CompanyHierarchyApp
{
    public partial class EmployeeDashboardForm : Form
    {
        // =========================
        // Database Connection
        // =========================
        int LoggedInEmployeeId;
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);
        SqlCommand cmd = new SqlCommand();
        // DESIGN: Corporate Palette
        private Color primaryColor = Color.FromArgb(41, 128, 185); // Professional Blue
        private Color secondaryColor = Color.FromArgb(52, 152, 219); // Lighter Blue
        private Color accentColor = Color.FromArgb(243, 156, 18); // Orange for "Pending"/Action
        private Color backgroundColor = Color.WhiteSmoke;

        public EmployeeDashboardForm(int employeeId)
        {
            InitializeComponent();
            LoggedInEmployeeId = employeeId;
        }

        private void EmployeeDashboardForm_Load(object sender, EventArgs e)
        {
            // Apply the new UI layout before loading data
            ApplyDashboardTheme();

            LoadEmployeeInfo();
            LoadMyTasks();
            LoadPendingTasks();
        }

        // =========================
        // NEW UI & LAYOUT METHOD
        // =========================
        private void ApplyDashboardTheme()
        {
            // 1. Form Global Style
            this.Text = "Employee Dashboard";
            this.WindowState = FormWindowState.Maximized; // Dashboards look best full screen
            this.BackColor = backgroundColor;
            this.Font = new Font("Segoe UI", 10);

            // 2. Style MenuStrip (Assuming it's named menuStrip1 based on toolstrip items)
            if (this.MainMenuStrip != null)
            {
                this.MainMenuStrip.BackColor = primaryColor;
                this.MainMenuStrip.ForeColor = Color.White;
                this.MainMenuStrip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                this.MainMenuStrip.Padding = new Padding(10, 5, 10, 5);
            }

            // --- LAYOUT STRATEGY: Docking ---
            // We anchor the Profile to Top, Submission to Bottom, and Tasks to Fill the center.

            // 3. GroupBox 1: Personal Info (The Top Banner)
            groupBox1.Text = "  My Profile  ";
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Height = 110; // Fixed height for banner
            groupBox1.BackColor = Color.White;
            groupBox1.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            groupBox1.ForeColor = primaryColor;

            // Organize Labels inside GB1 (Flow logic)
            lblFullName.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblFullName.ForeColor = Color.Black;
            lblFullName.Location = new Point(30, 40);

            lblRole.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblRole.ForeColor = Color.Gray;
            lblRole.Location = new Point(30, 70);

            lblEmail.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblEmail.ForeColor = Color.Gray;
            lblEmail.Location = new Point(300, 70); // spaced out

            // 4. GroupBox 3: Task Submission (The Bottom Action Area)
            groupBox3.Text = "  Submit Update / Complete Task  ";
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.Height = 180; // Enough space for text box
            groupBox3.BackColor = Color.White;
            groupBox3.ForeColor = primaryColor;

            // Layout controls inside GB3
            // Label for dropdown
            if (cbPendingTasks.Parent != null)
            {
                // Find the label associated with the combo box (assuming there is one, or we position relative)
                cbPendingTasks.Location = new Point(30, 50);
                cbPendingTasks.Width = 300;
            }

            rtbSubmissionMessage.Location = new Point(350, 40);
            rtbSubmissionMessage.Height = 100;
            rtbSubmissionMessage.Width = 400;
            rtbSubmissionMessage.BorderStyle = BorderStyle.FixedSingle;

            // Submit Button
            button2.Text = "SUBMIT UPDATE";
            button2.BackColor = accentColor; // Orange for action
            button2.ForeColor = Color.White;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.Location = new Point(780, 40);
            button2.Size = new Size(150, 40);
            button2.Cursor = Cursors.Hand;

            // 5. GroupBox 2: Task List (The Main Content)
            groupBox2.Text = "  My Assigned Tasks  ";
            groupBox2.Dock = DockStyle.Fill; // Fills the space between Top and Bottom
            groupBox2.BackColor = Color.White;
            groupBox2.ForeColor = primaryColor;

            // Fix Z-Order so Fill works correctly between Top and Bottom
            groupBox2.BringToFront();

            // Style DataGridView
            dgvMyTasks.BackgroundColor = Color.White;
            dgvMyTasks.BorderStyle = BorderStyle.None;
            dgvMyTasks.Dock = DockStyle.Fill; // Fill the group box
            dgvMyTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMyTasks.EnableHeadersVisualStyles = false;
            dgvMyTasks.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvMyTasks.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvMyTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMyTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMyTasks.ColumnHeadersHeight = 40;
            dgvMyTasks.DefaultCellStyle.SelectionBackColor = Color.AliceBlue;
            dgvMyTasks.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMyTasks.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvMyTasks.RowHeadersVisible = false;

            // Style Refresh Button (button1) inside GB2
            // We need to dock it or float it. Let's dock it to the top of GB2 or float it top-right
            Panel pnlGridHeader = new Panel(); // Create a small header panel for the Refresh button
            pnlGridHeader.Height = 50;
            pnlGridHeader.Dock = DockStyle.Top;
            pnlGridHeader.Parent = groupBox2;

            button1.Parent = pnlGridHeader; // Move button to this panel
            button1.Text = "Refresh List";
            button1.BackColor = secondaryColor;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Size = new Size(120, 35);
            button1.Location = new Point(20, 10); // Left align in the header panel
            button1.Cursor = Cursors.Hand;
        }
        void LoadEmployeeInfo()
        {
            string query = @"
SELECT e.FullName, e.Email, r.RoleName
FROM Employees e
INNER JOIN Roles r ON r.RoleId = e.RoleId
WHERE e.EmployeeId = @id";

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblFullName.Text = reader.GetString(0);
                    lblEmail.Text = reader.GetString(1);
                    lblRole.Text = reader.GetString(2);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee info: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        void LoadMyTasks()
        {
            string query = @"
SELECT 
    t.TaskId,
    t.Title,
    t.AssignedDate,
    t.Status,
    e.FullName AS AssignedBy
FROM Tasks t
INNER JOIN Employees e ON e.EmployeeId = t.AssignedBy
WHERE t.AssignedTo = @id";

            DataTable dt = new DataTable();

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                dgvMyTasks.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tasks: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        void LoadPendingTasks()
        {
            string query = @"
        SELECT TaskId, Title
        FROM Tasks
        WHERE AssignedTo = @id
        AND (Status = 'Pending' OR Status = 'Rejected')";

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                cbPendingTasks.DataSource = dt;
                cbPendingTasks.DisplayMember = "Title";
                cbPendingTasks.ValueMember = "TaskId"; 
                cbPendingTasks.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending tasks: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cbPendingTasks_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cbPendingTasks.SelectedIndex == -1 ||
                rtbSubmissionMessage.Text.Trim() == "" ||
                nudTimeSpent.Value <= 0)
            {
                MessageBox.Show("Please select a task, enter time spent, and write a message.");
                return;
            }

            int taskId = Convert.ToInt32(cbPendingTasks.SelectedValue);
            string message = rtbSubmissionMessage.Text.Trim();
            int timeSpentMinutes = (int)nudTimeSpent.Value;

            try
            {
                // 🔒 Safety check (VERY IMPORTANT)
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                // 1️⃣ Insert submission WITH time spent
                SqlCommand cmd1 = new SqlCommand(@"
INSERT INTO TaskSubmissions
(TaskId, EmployeeId, SubmissionMessage, TimeSpentMinutes, ReviewStatus, ReviewMessage)
VALUES (@t, @e, @m, @time, 'Pending', '')", conn);

                cmd1.Parameters.AddWithValue("@t", taskId);
                cmd1.Parameters.AddWithValue("@e", LoggedInEmployeeId);
                cmd1.Parameters.AddWithValue("@m", message);
                cmd1.Parameters.AddWithValue("@time", timeSpentMinutes);
                cmd1.ExecuteNonQuery();

                // 2️⃣ Update task status
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE Tasks SET Status = 'Submitted' WHERE TaskId = @id", conn);
                cmd2.Parameters.AddWithValue("@id", taskId);
                cmd2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting task: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            MessageBox.Show("Task submitted successfully.");

            rtbSubmissionMessage.Clear();
            nudTimeSpent.Value = nudTimeSpent.Minimum;

            LoadMyTasks();
            LoadPendingTasks();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            LoadMyTasks();
            LoadPendingTasks();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();

            // Close employee dashboard
            this.Close();
        }

        private void viewNotificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotificationsForm nf = new NotificationsForm(LoggedInEmployeeId);
            nf.ShowDialog();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void rtbSubmissionMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}