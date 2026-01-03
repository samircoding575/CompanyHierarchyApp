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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", LoggedInEmployeeId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvNotifications.DataSource = dt;

                // Optional: Formatting columns after data load
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
        }

        private void dgvNotifications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}