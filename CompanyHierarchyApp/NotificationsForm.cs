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
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(700, 500); // Slightly wider for buttons
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // 2. Create a Bottom Action Panel (Holds the buttons)
            // We create this dynamically so you don't have to drag a panel in the designer
            Panel pnlBottom = new Panel();
            pnlBottom.Height = 80;
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.BackColor = Color.WhiteSmoke;
            pnlBottom.Padding = new Padding(15);
            // Draw a subtle top border line
            pnlBottom.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 0, 0, pnlBottom.Width, 0); };
            this.Controls.Add(pnlBottom);

            // 3. Configure Buttons (Move them into the panel)

            // "Mark as Read" Button (Primary Action - Right Aligned)
            button2.Parent = pnlBottom; // Move inside panel
            button2.Text = "Mark as Read";
            button2.Size = new Size(160, 40);
            // Position: Right side with padding
            button2.Location = new Point(pnlBottom.Width - 180, 20);
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(button2, primaryColor);

            // "Delete" Button (Secondary Action - Left of the Read button)
            button1.Parent = pnlBottom; // Move inside panel
            button1.Text = "Delete Selected";
            button1.Size = new Size(160, 40);
            // Position: To the left of button2
            button1.Location = new Point(pnlBottom.Width - 350, 20);
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(button1, Color.IndianRed); // Red for delete

            // 4. DataGridView Style
            dgvNotifications.Parent = this; // Ensure grid is on the form, not the panel
            dgvNotifications.BackgroundColor = backgroundColor;
            dgvNotifications.BorderStyle = BorderStyle.None;
            dgvNotifications.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNotifications.Dock = DockStyle.Fill; // Fill the REMAINING space above the panel
            dgvNotifications.BringToFront(); // Ensure it doesn't get covered

            // Header Style
            dgvNotifications.EnableHeadersVisualStyles = false;
            dgvNotifications.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvNotifications.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvNotifications.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNotifications.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvNotifications.ColumnHeadersHeight = 45;

            // Row Style
            dgvNotifications.DefaultCellStyle.BackColor = Color.White;
            dgvNotifications.DefaultCellStyle.ForeColor = Color.Black;
            dgvNotifications.DefaultCellStyle.SelectionBackColor = selectionColor;
            dgvNotifications.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvNotifications.DefaultCellStyle.Padding = new Padding(5);
            dgvNotifications.RowTemplate.Height = 40;

            // Grid Behavior
            dgvNotifications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNotifications.RowHeadersVisible = false;
            dgvNotifications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotifications.AllowUserToAddRows = false;
            dgvNotifications.ReadOnly = true;
        }

        // Helper Method for Button Styling
        private void StyleButton(Button btn, Color color)
        {
            if (btn == null) return;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
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