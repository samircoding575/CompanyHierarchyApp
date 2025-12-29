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
    public partial class NotificationsForm : Form
    {
        int LoggedInEmployeeId;
        static string connString =
            "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public NotificationsForm(int employeeId)
        {
            LoggedInEmployeeId = employeeId;
            InitializeComponent();
        }

        private void NotificationsForm_Load(object sender, EventArgs e)
        {
            LoadNotifications();
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
            }
        }

    }
}
