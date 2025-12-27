using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CompanyHierarchyApp
{
    public partial class Form1 : Form
    {
        static string connString =
    "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);
    

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // 1. Basic validation
            if (email == "" || password == "")
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            // 2. Hash password
            string passwordHash = password;

            // 3. SQL query to validate login and get role
            string query = @"
    SELECT TOP 1 
        e.EmployeeId,
        e.IsVerified,
        e.IsActive,
        r.RoleName
    FROM Employees e
    INNER JOIN Roles r ON r.RoleId = e.RoleId
    WHERE e.Email = @email AND e.PasswordHash = @passwordHash";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // 4. Invalid credentials
                if (!reader.Read())
                {
                    reader.Close();
                    MessageBox.Show("Invalid email or password.");
                    return;
                }

                // 5. Read values
                int employeeId = reader.GetInt32(0);
                bool isVerified = reader.GetBoolean(1);
                bool isActive = reader.GetBoolean(2);
                string roleName = reader.GetString(3);

                reader.Close();

                // 6. Check active status
                if (!isActive)
                {
                    MessageBox.Show("Your account is inactive. Please contact HR.");
                    return;
                }

                // 7. Check verification
                if (!isVerified)
                {
                    MessageBox.Show("Your email is not verified. Please verify your account.");
                    return;
                }

                // 8. Login success
                MessageBox.Show("Login successful!\nRole: " + roleName);

                // 9. (Optional for now) Role-based navigation
                // You can add dashboard forms later
                // Example:
                /*
                if (roleName == "HR")
                    new HRDashboardForm(employeeId).Show();
                else if (roleName == "Manager")
                    new ManagerDashboardForm(employeeId).Show();
                else
                    new EmployeeDashboardForm(employeeId).Show();

                this.Hide();
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }






        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            new RegisterForm().Show();
            this.Hide(); // or keep it Show() if you prefer
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
