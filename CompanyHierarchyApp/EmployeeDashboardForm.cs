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
        // =========================
        // NEW UI & LAYOUT METHOD
        // =========================
        private void ApplyDashboardTheme()
        {
            // 1. Form Global Style
            this.Text = "Employee Workspace";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10);

            // 2. MenuStrip Styling
            if (this.MainMenuStrip != null)
            {
                this.MainMenuStrip.BackColor = primaryColor;
                this.MainMenuStrip.ForeColor = Color.White;
                this.MainMenuStrip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            // 3. Header Panel (Profile)
            // Finds the panel docked to the TOP (created in Designer) and styles it
            foreach (Control c in this.Controls)
            {
                if (c is Panel pnl)
                {
                    if (pnl.Dock == DockStyle.Top) // The Header Panel
                    {
                        pnl.BackColor = Color.White;
                        pnl.Padding = new Padding(20);

                        // Style the labels inside this panel
                        lblRole.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                        lblRole.ForeColor = primaryColor;

                        lblRole.ForeColor = Color.Gray;
                        lblRole.Font = new Font("Segoe UI", 11);

                        lblEmail.ForeColor = Color.Gray;
                        lblEmail.Font = new Font("Segoe UI", 11);
                    }
                    else if (pnl.Dock == DockStyle.Bottom) // The Action Panel
                    {
                        pnl.BackColor = Color.White;
                        pnl.Padding = new Padding(15);
                        // Add a visual border line at the top of this panel
                        pnl.Paint += (s, paintE) => {
                            paintE.Graphics.DrawLine(Pens.LightGray, 0, 0, pnl.Width, 0);
                        };
                    }
                }
            }

            // 4. Style Grid
            StyleGrid(dgvMyTasks);

            // 5. Style Inputs & Buttons
            StyleButton(button2, accentColor);  // Submit Button (Orange)

            StyleInput(cbPendingTasks);
            StyleInput(nudTimeSpent);
            StyleInput(rtbSubmissionMessage);
        }

        // Helper: Grid Styling
        private void StyleGrid(DataGridView grid)
        {
            if (grid == null) return;
            grid.BackgroundColor = Color.WhiteSmoke;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;

            // Headers
            grid.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Rows
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 234, 248);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.Padding = new Padding(5);
            grid.RowTemplate.Height = 35;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.RowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);

            // Layout
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
        }

        // Helper: Button Styling
        private void StyleButton(Button btn, Color color)
        {
            if (btn == null) return;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Height = 40;
        }

        // Helper: Input Styling
        private void StyleInput(Control ctrl)
        {
            if (ctrl == null) return;
            ctrl.BackColor = Color.WhiteSmoke;
            ctrl.Font = new Font("Segoe UI", 11);
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
                    lblRole.Text = reader.GetString(0);
                    lblEmail.Text = reader.GetString(1);
                    lblFullName.Text = reader.GetString(2);
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
            // NEW: Added 'Description' to the query
            string query = @"
        SELECT TaskId, Title, Description
        FROM Tasks
        WHERE AssignedTo = @id
        AND (Status = 'Pending' OR Status = 'Rejected')";

            DataTable dt = new DataTable();

            // Safety Check: Clear existing connection if open
            if (conn.State == ConnectionState.Open) conn.Close();

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                cbPendingTasks.DataSource = dt;
                cbPendingTasks.DisplayMember = "Title";
                cbPendingTasks.ValueMember = "TaskId";
                cbPendingTasks.SelectedIndex = -1; // Default to nothing selected
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
            if (cbPendingTasks.SelectedIndex != -1)
            {
                // When binding a DataTable, the SelectedItem is a 'DataRowView'
                DataRowView row = (DataRowView)cbPendingTasks.SelectedItem;

                // Extract the description safely
                string desc = row["Description"].ToString();

                // Update the label
                lblTaskDescription.Text = string.IsNullOrEmpty(desc) ? "No description provided." : desc;
            }
            else
            {
                lblTaskDescription.Text = "Select a task to view details...";
            }
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

        private void dgvMyTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}