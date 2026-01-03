using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Required for UI styling
using System.Windows.Forms;

namespace CompanyHierarchyApp
{
    public partial class HRDashboardForm : Form
    {
        // =========================
        // Database Connection
        // =========================
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);

        // Logged-in HR employee ID (set from login)
        int LoggedInEmployeeId;

        // =========================
        // DESIGN THEME COLORS
        // =========================
        private Color primaryColor = Color.FromArgb(41, 128, 185);   // Blue
        private Color successColor = Color.FromArgb(39, 174, 96);    // Green
        private Color dangerColor = Color.FromArgb(192, 57, 43);     // Red
        private Color darkText = Color.FromArgb(44, 62, 80);         // Dark Gray
        private Color bgColor = Color.WhiteSmoke;

        // =========================
        // Constructors
        // =========================
        public HRDashboardForm()
        {
            InitializeComponent();
        }

        public HRDashboardForm(int employeeId)
        {
            InitializeComponent();
            LoggedInEmployeeId = employeeId;
        }

        // =========================
        // FORM LOAD
        // =========================
        private void HRDashboardForm_Load(object sender, EventArgs e)
        {
            // 1. Apply UI Styles before loading data
            ApplyHRTheme();
            LoadEmployeeFilterComboBox(); // ✅ NEW
            // 2. Load Data
            LoadEmployees();
            LoadEmployeesComboBox();
            LoadSubmissions();
        }
        void LoadEmployeeFilterComboBox()
        {
            string query = @"
        SELECT EmployeeId, FullName
        FROM Employees
        WHERE IsActive = 1
        ORDER BY FullName";

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                cbEmployeeFilter.DataSource = dt;
                cbEmployeeFilter.DisplayMember = "FullName";
                cbEmployeeFilter.ValueMember = "EmployeeId";
                cbEmployeeFilter.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee filter list: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }



        // =========================
        // STYLING LOGIC (New)
        // =========================
        private void ApplyHRTheme()
        {
            // Form Setup
            this.Text = "HR Administration Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = bgColor;
            this.Font = new Font("Segoe UI", 10);

            // MenuStrip Styling (if exists)
            if (this.MainMenuStrip != null)
            {
                this.MainMenuStrip.BackColor = primaryColor;
                this.MainMenuStrip.ForeColor = Color.White;
            }

            // Style all DataGridViews
            StyleGrid(dgvEmployees);
            StyleGrid(dgvEmployeeTasks);
            StyleGrid(dgvSubmissions);

            // Style Buttons (Find controls by name or logic)
            StyleButton(button2, primaryColor);
            StyleButton(button3, dangerColor);  // Delete Employee
            StyleButton(button4, successColor);
            StyleButton(button1, primaryColor); // Assign Task
            StyleButton(button7, successColor); // Approve
            StyleButton(button6, dangerColor);  // Reject

            // Style Inputs
            StyleInput(txtTaskTitle);
            StyleInput(rtbTaskDescription);
            StyleInput(cbEmployees);

            // Find and Style TabControl
            foreach (Control c in this.Controls)
            {
                if (c is TabControl tc)
                {
                    tc.Appearance = TabAppearance.FlatButtons;
                    tc.SizeMode = TabSizeMode.Fixed;
                    tc.ItemSize = new Size(180, 45); // Bigger tabs
                    tc.Padding = new Point(10, 5);
                    tc.Font = new Font("Segoe UI", 11, FontStyle.Bold);

                    // Hook draw event to color tabs if needed, but FlatButtons is usually enough for simple style
                }
            }
        }

        private void StyleGrid(DataGridView grid)
        {
            if (grid == null) return;

            // 1. Basic Visuals
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;

            // 2. Header Style
            grid.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 3. Row Style
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 234, 248); // Light Blue selection
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.Padding = new Padding(5);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.RowTemplate.Height = 35;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            // 4. Layout & Interaction (THE FIX)
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;       // Hides the left selector column

            // This line is the fix: It lets you click ANY text in the row to select the whole row
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            grid.MultiSelect = false;             // Forces single row selection (easier for logic)
            grid.ReadOnly = true;                 // Prevents user from typing inside the grid
        }

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

        private void StyleInput(Control ctrl)
        {
            if (ctrl == null) return;
            ctrl.BackColor = Color.White;
            ctrl.Font = new Font("Segoe UI", 11);
        }
        void LoadEmployees()
        {
            string query = @"
        SELECT e.EmployeeId, e.FullName, e.Email, r.RoleName, e.IsActive
        FROM Employees e
        INNER JOIN Roles r ON r.RoleId = e.RoleId";

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                dgvEmployees.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an employee first.");
                return;
            }

            // Confirm deletion
            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this employee? All data will be lost.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (dr != DialogResult.Yes)
                return;

            int empId = Convert.ToInt32(
                dgvEmployees.SelectedRows[0].Cells["EmployeeId"].Value
            );

            try
            {
                conn.Open();

                SqlCommand cmd1 = new SqlCommand(
                    "DELETE FROM EmailVerifications WHERE EmployeeId = @id", conn);
                cmd1.Parameters.AddWithValue("@id", empId);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand(
                    "DELETE FROM TaskSubmissions WHERE EmployeeId = @id", conn);
                cmd2.Parameters.AddWithValue("@id", empId);
                cmd2.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand(
                    "DELETE FROM Tasks WHERE AssignedTo = @id", conn);
                cmd3.Parameters.AddWithValue("@id", empId);
                cmd3.ExecuteNonQuery();

                SqlCommand cmd4 = new SqlCommand(
                    "DELETE FROM Employees WHERE EmployeeId = @id", conn);
                cmd4.Parameters.AddWithValue("@id", empId);
                cmd4.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting employee: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            MessageBox.Show("Employee deleted successfully.");
            LoadEmployees();
        }




        // =========================
        // TAB 2 — ASSIGN TASK
        // =========================
        void LoadEmployeesComboBox()
        {
            string query = "SELECT EmployeeId, FullName FROM Employees WHERE IsActive = 1";

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                cbEmployees.DataSource = dt;
                cbEmployees.DisplayMember = "FullName";
                cbEmployees.ValueMember = "EmployeeId";
                cbEmployees.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees list: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (cbEmployees.SelectedIndex == -1)
            {
                MessageBox.Show("Select an active employee.");
                return;
            }

            if (cbEmployees.SelectedIndex == -1 || txtTaskTitle.Text.Trim() == "")
            {
                MessageBox.Show("Fill all task fields.");
                return;
            }

            string query = @"
            INSERT INTO Tasks (Title, Description, AssignedTo, AssignedBy, Status)
            VALUES (@t, @d, @to, @by, 'Pending')";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t", txtTaskTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@d", rtbTaskDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@to", cbEmployees.SelectedValue);
                cmd.Parameters.AddWithValue("@by", LoggedInEmployeeId);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Task assigned.");

                txtTaskTitle.Clear();
                rtbTaskDescription.Clear();
            }
            finally
            {
                conn.Close();
            }
        }

        // =========================
        // TAB 3 — EMPLOYEE TASKS
        // =========================
        void LoadEmployeeTasks(int employeeId)
        {
            string query = @"
SELECT TaskId, Title, AssignedDate, Status
FROM Tasks
WHERE AssignedTo = @id";

            DataTable dt = new DataTable();

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", employeeId);

                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                dgvEmployeeTasks.DataSource = dt;
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



        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
                return;

            int empId =
                Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["EmployeeId"].Value);

            LoadEmployeeTasks(empId);
        }
        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
                return;

            int empId =
                Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["EmployeeId"].Value);

            LoadEmployeeTasks(empId);
        }


        private void dgvEmployeeTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        // =========================
        // TAB 4 — TASK SUBMISSIONS
        // =========================
        void LoadSubmissions()
        {
            string query = @"
        SELECT s.SubmissionId,
               s.EmployeeId,
               t.TaskId,
               e.FullName,
               t.Title,
               s.SubmissionMessage
        FROM TaskSubmissions s
        INNER JOIN Employees e ON e.EmployeeId = s.EmployeeId
        INNER JOIN Tasks t ON t.TaskId = s.TaskId
        WHERE s.ReviewStatus = 'Pending'";

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                dgvSubmissions.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading submissions: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        void UpdateSubmissionStatus(string status)
        {
            if (dgvSubmissions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a submission.");
                return;
            }

            int submissionId =
                Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["SubmissionId"].Value);

            int employeeId =
                Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["EmployeeId"].Value);

            int taskId =
                Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["TaskId"].Value);

            string taskTitle =
                dgvSubmissions.SelectedRows[0].Cells["Title"].Value.ToString();

            try
            {
                conn.Open();

                // 1️⃣ Update submission status
                SqlCommand cmd1 = new SqlCommand(@"
UPDATE TaskSubmissions
SET ReviewStatus = @s,
    ReviewMessage = @m
WHERE SubmissionId = @id", conn);

                cmd1.Parameters.AddWithValue("@s", status);
                cmd1.Parameters.AddWithValue("@m",
                    status == "Approved"
                        ? "Approved by HR"
                        : "Rejected. Please revise and resubmit.");
                cmd1.Parameters.AddWithValue("@id", submissionId);
                cmd1.ExecuteNonQuery();

                // 2️⃣ Update task status
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE Tasks SET Status = @ts WHERE TaskId = @tid", conn);

                cmd2.Parameters.AddWithValue("@tid", taskId);
                cmd2.Parameters.AddWithValue("@ts",
                    status == "Approved" ? "Completed" : "Rejected");
                cmd2.ExecuteNonQuery();

                // 3️⃣ Insert notification
                SqlCommand cmd3 = new SqlCommand(@"
INSERT INTO Notifications (EmployeeId, Message)
VALUES (@eid, @msg)", conn);

                cmd3.Parameters.AddWithValue("@eid", employeeId);
                cmd3.Parameters.AddWithValue("@msg",
                    status == "Approved"
                        ? $"Your task '{taskTitle}' was approved and closed."
                        : $"Your task '{taskTitle}' was rejected. Please revise and resubmit.");

                cmd3.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating submission: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            LoadSubmissions(); // refresh HR grid
        }




        private void button7_Click(object sender, EventArgs e)
        {
            UpdateSubmissionStatus("Approved");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UpdateSubmissionStatus("Rejected");
        }

        // =========================
        // UNUSED / DESIGNER EVENTS
        // =========================
        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSubmissions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void logoutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Show login form again
            Form1 loginForm = new Form1();
            loginForm.Show();

            // Close current HR dashboard
            this.Close();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        void ToggleEmployeeActiveStatus()
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an employee first.");
                return;
            }

            int employeeId = Convert.ToInt32(
                dgvEmployees.SelectedRows[0].Cells["EmployeeId"].Value
            );

            bool isActive = Convert.ToBoolean(
                dgvEmployees.SelectedRows[0].Cells["IsActive"].Value
            );

            string action = isActive ? "deactivate" : "activate";

            DialogResult dr = MessageBox.Show(
                $"Are you sure you want to {action} this employee?",
                "Confirm Action",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr != DialogResult.Yes)
                return;

            SqlCommand cmd = new SqlCommand(
                "UPDATE Employees SET IsActive = @active WHERE EmployeeId = @id",
                conn
            );

            cmd.Parameters.AddWithValue("@active", !isActive);
            cmd.Parameters.AddWithValue("@id", employeeId);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating employee status: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            MessageBox.Show(
                isActive ? "Employee deactivated." : "Employee activated."
            );

            LoadEmployees(); // refresh grid
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ignore events during data binding
            if (cbEmployeeFilter.SelectedValue == null)
                return;

            // VERY IMPORTANT: protect against DataRowView
            if (cbEmployeeFilter.SelectedValue is DataRowView)
                return;

            int employeeId;

            // Safe conversion (works for int, long, string)
            if (!int.TryParse(cbEmployeeFilter.SelectedValue.ToString(), out employeeId))
                return;

            LoadEmployeeTasks(employeeId);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ToggleEmployeeActiveStatus();
        }
    }
}