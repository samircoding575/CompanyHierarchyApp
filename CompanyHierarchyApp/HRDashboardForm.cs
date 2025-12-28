using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CompanyHierarchyApp
{
    public partial class HRDashboardForm : Form
    {
        // =========================
        // Database Connection
        // =========================
        static string connString =
            "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);

        // Logged-in HR employee ID (set from login)
        int LoggedInEmployeeId;

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
            LoadEmployees();
            LoadEmployeesComboBox();
            LoadSubmissions();
        }

        // =========================
        // TAB 1 — EMPLOYEES
        // =========================
        void LoadEmployees()
        {
            string query = @"
            SELECT e.EmployeeId, e.FullName, e.Email, r.RoleName, e.IsActive
            FROM Employees e
            INNER JOIN Roles r ON r.RoleId = e.RoleId";

            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
                dgvEmployees.DataSource = dt;
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

            int empId = Convert.ToInt32(
                dgvEmployees.SelectedRows[0].Cells["EmployeeId"].Value
            );

            using (SqlConnection conn = new SqlConnection(connString))
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

            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);

                cbEmployees.DataSource = dt;
                cbEmployees.DisplayMember = "FullName";
                cbEmployees.ValueMember = "EmployeeId";
                cbEmployees.SelectedIndex = -1;
            }
            finally
            {
                conn.Close();
            }
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
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", employeeId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                dgvEmployeeTasks.DataSource = dt;
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
            SELECT s.SubmissionId, e.FullName, t.Title,
                   s.SubmissionMessage, s.ReviewStatus
            FROM TaskSubmissions s
            INNER JOIN Employees e ON e.EmployeeId = s.EmployeeId
            INNER JOIN Tasks t ON t.TaskId = s.TaskId";

            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
                dgvSubmissions.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            int id = Convert.ToInt32(
                dgvSubmissions.SelectedRows[0].Cells["SubmissionId"].Value
            );

            string query =
                "UPDATE TaskSubmissions SET ReviewStatus = @s WHERE SubmissionId = @id";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                LoadSubmissions();
            }
            finally
            {
                conn.Close();
            }
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
    }
}
