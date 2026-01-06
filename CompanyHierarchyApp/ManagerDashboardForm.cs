using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Required for Styling
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CompanyHierarchyApp
{
    public partial class ManagerDashboardForm : Form
    {
        // =========================
        // Database Connection
        // =========================
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = new SqlConnection(connString);
        int LoggedInManagerId;

        // =========================
        // DESIGN THEME COLORS
        // =========================
        private Color primaryColor = Color.FromArgb(41, 128, 185);   // Blue
        private Color successColor = Color.FromArgb(39, 174, 96);    // Green
        private Color dangerColor = Color.FromArgb(192, 57, 43);     // Red
        private Color bgColor = Color.WhiteSmoke;

        public ManagerDashboardForm(int employeeId)
        {
            InitializeComponent();
            LoggedInManagerId = employeeId;
        }

        private void ManagerDashboardForm_Load(object sender, EventArgs e)
        {
            // 1. Apply UI Styles before loading data
            ApplyManagerTheme();

            // 2. Load Data
            LoadEmployeesComboBox();
            LoadPendingSubmissions();
        }

        // =========================
        // STYLING LOGIC
        // =========================
        private void ApplyManagerTheme()
        {
            // 1. Form Setup
            this.Text = "Manager Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = bgColor;
            this.Font = new Font("Segoe UI", 10);

            // 2. MenuStrip Styling
            if (this.MainMenuStrip != null)
            {
                this.MainMenuStrip.BackColor = primaryColor;
                this.MainMenuStrip.ForeColor = Color.White;
                this.MainMenuStrip.RenderMode = ToolStripRenderMode.System;
            }

            // 3. TabControl Styling
            foreach (Control c in this.Controls)
            {
                if (c is TabControl tc)
                {
                    tc.Appearance = TabAppearance.FlatButtons;
                    tc.SizeMode = TabSizeMode.Fixed;
                    tc.ItemSize = new Size(150, 40);
                    tc.Font = new Font("Segoe UI", 11, FontStyle.Bold);

                    foreach (TabPage page in tc.TabPages)
                    {
                        page.BackColor = Color.WhiteSmoke;
                        page.Padding = new Padding(0);

                        // Auto-style the panels we added in the designer
                        foreach (Control child in page.Controls)
                        {
                            if (child is Panel pnl)
                            {
                                // If it's the "Assign Card" (Centered Panel)
                                if (pnl.Name == "pnlAssignCard")
                                {
                                    pnl.BackColor = Color.White;
                                    pnl.BorderStyle = BorderStyle.FixedSingle; // Thin border for the card
                                    CenterControl(pnl); // Force it to center
                                }
                                // If it's the Bottom Action Bar
                                else if (pnl.Dock == DockStyle.Bottom)
                                {
                                    pnl.BackColor = Color.White;
                                    pnl.Padding = new Padding(15);
                                }
                            }
                        }
                    }
                }
            }

            // 4. Style Grid
            StyleGrid(dgvSubmissions);

            // 5. Style Buttons
            StyleButton(button1, primaryColor); // Assign Task
            StyleButton(button7, successColor); // Approve
            StyleButton(button6, dangerColor);  // Reject

            // 6. Style Inputs
            StyleInput(txtTaskTitle);
            StyleInput(rtbTaskDescription);
            StyleInput(cbEmployees);

            // Hook the Resize event to keep the card centered if user resizes window
            this.Resize += (s, e) => {
                // Find the panel again to re-center it
                var pnl = this.Controls.Find("pnlAssignCard", true).FirstOrDefault();
                if (pnl != null) CenterControl(pnl);
            };
        }

        // ✅ NEW HELPER: Centers a control within its parent
        private void CenterControl(Control ctrl)
        {
            if (ctrl.Parent != null)
            {
                ctrl.Location = new Point(
                    (ctrl.Parent.ClientSize.Width - ctrl.Width) / 2,
                    (ctrl.Parent.ClientSize.Height - ctrl.Height) / 2
                );
            }
        }

        // (Keep your StyleGrid, StyleButton, StyleInput methods exactly as they were in the previous step)
        // Just ensure StyleGrid has "grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;"

        private void StyleGrid(DataGridView grid)
        {
            if (grid == null) return;

            // 1. Visuals
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;

            // 2. Header
            grid.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 3. Rows
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 234, 248); // Light Blue
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.Padding = new Padding(5);
            grid.RowTemplate.Height = 35;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            // 4. THE FIX: Behavior & Selection
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;        // Hide left selector
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Click anywhere to select row
            grid.MultiSelect = false;              // Only allow one row at a time
            grid.ReadOnly = true;                  // Prevent typing
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

        // =========================
        // BUSINESS LOGIC (Unchanged)
        // =========================

        void LoadEmployeesComboBox()
        {
            string query =
                "SELECT EmployeeId, FullName FROM Employees WHERE RoleId = 3 AND IsActive = 1";

            DataTable dt = new DataTable();

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
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
                MessageBox.Show("Error loading employees: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        void LoadPendingSubmissions()
        {
            string query = @"
SELECT s.SubmissionId,
       s.EmployeeId,
       t.TaskId,
       e.FullName,
       t.Title,
       s.SubmissionMessage
FROM TaskSubmissions s
INNER JOIN Tasks t ON t.TaskId = s.TaskId
INNER JOIN Employees e ON e.EmployeeId = s.EmployeeId
WHERE s.ReviewStatus = 'Pending'
AND t.AssignedBy = @managerId";

            DataTable dt = new DataTable();

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@managerId", LoggedInManagerId);

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
                if (conn.State == ConnectionState.Open)
                    conn.Close();

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
                        ? "Approved by Manager"
                        : "Rejected by Manager. Please revise.");
                cmd1.Parameters.AddWithValue("@id", submissionId);
                cmd1.ExecuteNonQuery();

                // 2️⃣ Update task status
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE Tasks SET Status = @st WHERE TaskId = @tid", conn);

                cmd2.Parameters.AddWithValue("@tid", taskId);
                cmd2.Parameters.AddWithValue("@st",
                    status == "Approved" ? "Completed" : "Rejected");
                cmd2.ExecuteNonQuery();

                // 3️⃣ Insert notification
                SqlCommand cmd3 = new SqlCommand(@"
INSERT INTO Notifications (EmployeeId, Message)
VALUES (@e, @msg)", conn);

                cmd3.Parameters.AddWithValue("@e", employeeId);
                cmd3.Parameters.AddWithValue("@msg",
                    status == "Approved"
                        ? $"Your task '{taskTitle}' was approved by your manager."
                        : $"Your task '{taskTitle}' was rejected by your manager.");
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

            LoadPendingSubmissions();
        }
        private void button1_Click(object sender, EventArgs e)
        {
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
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t", txtTaskTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@d", rtbTaskDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@to", cbEmployees.SelectedValue);
                cmd.Parameters.AddWithValue("@by", LoggedInManagerId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error assigning task: " + ex.Message);
                return;
            }
            finally
            {
                conn.Close();
            }

            MessageBox.Show("Task assigned successfully.");

            txtTaskTitle.Clear();
            rtbTaskDescription.Clear();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            UpdateSubmissionStatus("Approved");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UpdateSubmissionStatus("Rejected");
        }

        private void logoutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();

            // Close employee dashboard
            this.Close();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void dgvSubmissions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}