using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyHierarchyApp
{   
    public partial class ManagerDashboardForm : Form
    {
        static string connString =
       "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        int LoggedInManagerId;

        public ManagerDashboardForm(int employeeId)
        {
            InitializeComponent();
            LoggedInManagerId = employeeId;
        }

        private void ManagerDashboardForm_Load(object sender, EventArgs e)
        {
            LoadEmployeesComboBox();
            LoadPendingSubmissions();
        }
        void LoadEmployeesComboBox()
        {
            string query = "SELECT EmployeeId, FullName FROM Employees WHERE RoleId = 3 AND IsActive = 1";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);

                cbEmployees.DataSource = dt;
                cbEmployees.DisplayMember = "FullName";
                cbEmployees.ValueMember = "EmployeeId";
                cbEmployees.SelectedIndex = -1;
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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@managerId", LoggedInManagerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvSubmissions.DataSource = dt;
            }
        }
        void UpdateSubmissionStatus(string status)
        {
            if (dgvSubmissions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a submission.");
                return;
            }

            int submissionId = Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["SubmissionId"].Value);
            int employeeId = Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["EmployeeId"].Value);
            int taskId = Convert.ToInt32(dgvSubmissions.SelectedRows[0].Cells["TaskId"].Value);
            string taskTitle = dgvSubmissions.SelectedRows[0].Cells["Title"].Value.ToString();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Update submission
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

                // Update task
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE Tasks SET Status = @st WHERE TaskId = @tid", conn);
                cmd2.Parameters.AddWithValue("@tid", taskId);
                cmd2.Parameters.AddWithValue("@st",
                    status == "Approved" ? "Completed" : "Rejected");
                cmd2.ExecuteNonQuery();

                // Notification
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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t", txtTaskTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@d", rtbTaskDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@to", cbEmployees.SelectedValue);
                cmd.Parameters.AddWithValue("@by", LoggedInManagerId);
                cmd.ExecuteNonQuery();
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
    }
}
