using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CompanyHierarchyApp
{
    public partial class EmployeeDashboardForm : Form
    {
        // =========================
        // Database Connection
        // =========================
        int LoggedInEmployeeId;
        static string connString =
            "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);
        public EmployeeDashboardForm(int employeeId)
        {

            InitializeComponent();
            LoggedInEmployeeId = employeeId;
        }

        private void EmployeeDashboardForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeInfo();
            LoadMyTasks();
            LoadPendingTasks();
        }
        void LoadEmployeeInfo()
        {
            string query = @"
    SELECT e.FullName, e.Email, r.RoleName
    FROM Employees e
    INNER JOIN Roles r ON r.RoleId = e.RoleId
    WHERE e.EmployeeId = @id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvMyTasks.DataSource = dt;
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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                cbPendingTasks.DataSource = dt;
                cbPendingTasks.DisplayMember = "Title";
                cbPendingTasks.ValueMember = "TaskId";
                cbPendingTasks.SelectedIndex = -1;
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
       rtbSubmissionMessage.Text.Trim() == "")
            {
                MessageBox.Show("Please select a task and write a message.");
                return;
            }

            int taskId = (int)cbPendingTasks.SelectedValue;
            string message = rtbSubmissionMessage.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // 1️⃣ Insert submission
                SqlCommand cmd1 = new SqlCommand(@"
INSERT INTO TaskSubmissions
(TaskId, EmployeeId, SubmissionMessage, ReviewStatus, ReviewMessage)
VALUES (@t, @e, @m, 'Pending', '')", conn);



                cmd1.Parameters.AddWithValue("@t", taskId);
                cmd1.Parameters.AddWithValue("@e", LoggedInEmployeeId);
                cmd1.Parameters.AddWithValue("@m", message);
                cmd1.ExecuteNonQuery();

                // 2️⃣ Update task status
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE Tasks SET Status = 'Submitted' WHERE TaskId = @id", conn);
                cmd2.Parameters.AddWithValue("@id", taskId);
                cmd2.ExecuteNonQuery();
            }

            MessageBox.Show("Task submitted successfully.");

            rtbSubmissionMessage.Clear();
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
    }
}
