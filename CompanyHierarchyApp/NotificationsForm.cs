using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Required for Colors and Fonts
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyHierarchyApp
{
    public partial class NotificationsForm : Form
    {
        int LoggedInEmployeeId;
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        // DESIGN: Corporate Palette (Matching Dashboard)
        private Color primaryColor = Color.FromArgb(41, 128, 185); // Professional Blue
        private Color backgroundColor = Color.White;
        private Color selectionColor = Color.AliceBlue;

        public NotificationsForm(int employeeId)
        {
            LoggedInEmployeeId = employeeId;
            InitializeComponent();
        }

        private void NotificationsForm_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadNotifications();
        }

        void ApplyTheme()
        {
            // 1. Form Style
            this.Text = "My Notifications";
            this.BackColor = backgroundColor;
            this.StartPosition = FormStartPosition.CenterParent; // Opens centered over the dashboard
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Clean dialog style
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(600, 500); // Set a reasonable default size
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // 2. DataGridView Style
            // We assume the grid is named 'dgvNotifications'
            dgvNotifications.BackgroundColor = backgroundColor;
            dgvNotifications.BorderStyle = BorderStyle.None;
            dgvNotifications.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Clean horizontal lines
            dgvNotifications.Dock = DockStyle.Fill; // Fill the whole form

            // Header Style
            dgvNotifications.EnableHeadersVisualStyles = false;
            dgvNotifications.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvNotifications.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvNotifications.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNotifications.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvNotifications.ColumnHeadersHeight = 40;

            // Row Style
            dgvNotifications.DefaultCellStyle.BackColor = Color.White;
            dgvNotifications.DefaultCellStyle.ForeColor = Color.Black;
            dgvNotifications.DefaultCellStyle.SelectionBackColor = selectionColor;
            dgvNotifications.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvNotifications.DefaultCellStyle.Padding = new Padding(5);
            dgvNotifications.RowTemplate.Height = 35; // More breathing room

            // Layout
            dgvNotifications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNotifications.RowHeadersVisible = false; // Hide the ugly left selector column
            dgvNotifications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotifications.AllowUserToAddRows = false;
            dgvNotifications.ReadOnly = true;
        }

        void LoadNotifications()
        {
            string query = @"
        SELECT Message, CreatedAt, IsRead
        FROM Notifications
        WHERE EmployeeId = @id
        ORDER BY CreatedAt DESC";

            DataTable dt = new DataTable();

            // Define columns explicitly (important when using DataReader)
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("CreatedAt", typeof(DateTime));
            dt.Columns.Add("IsRead", typeof(bool));

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["Message"] = reader.GetString(0);
                            row["CreatedAt"] = reader.GetDateTime(1);
                            row["IsRead"] = reader.GetBoolean(2);
                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            dgvNotifications.DataSource = dt;

            // Formatting after binding
            if (dgvNotifications.Columns["CreatedAt"] != null)
            {
                dgvNotifications.Columns["CreatedAt"].HeaderText = "Date";
                dgvNotifications.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm";
            }

            if (dgvNotifications.Columns["IsRead"] != null)
            {
                dgvNotifications.Columns["IsRead"].HeaderText = "Read?";
            }
        }


        private void dgvNotifications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvNotifications.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select at least one notification.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgvNotifications.SelectedRows)
                {
                    string message = row.Cells["Message"].Value.ToString();
                    DateTime createdAt = Convert.ToDateTime(row.Cells["CreatedAt"].Value);

                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE Notifications
SET IsRead = 1
WHERE EmployeeId = @eid
AND Message = @msg
AND CreatedAt = @dt", conn))
                    {
                        cmd.Parameters.AddWithValue("@eid", LoggedInEmployeeId);
                        cmd.Parameters.AddWithValue("@msg", message);
                        cmd.Parameters.AddWithValue("@dt", createdAt);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadNotifications();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvNotifications.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a notification to delete.");
                return;
            }

            if (MessageBox.Show("Delete selected notification(s)?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgvNotifications.SelectedRows)
                {
                    string message = row.Cells["Message"].Value.ToString();
                    DateTime createdAt = Convert.ToDateTime(row.Cells["CreatedAt"].Value);

                    SqlCommand cmd = new SqlCommand(@"
DELETE FROM Notifications
WHERE EmployeeId = @eid
AND Message = @msg
AND CreatedAt = @dt", conn);

                    cmd.Parameters.AddWithValue("@eid", LoggedInEmployeeId);
                    cmd.Parameters.AddWithValue("@msg", message);
                    cmd.Parameters.AddWithValue("@dt", createdAt);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadNotifications();
        
}
    }
}