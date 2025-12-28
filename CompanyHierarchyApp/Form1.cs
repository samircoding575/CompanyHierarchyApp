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

            // 2. SQL query (USE RoleId, NOT RoleName)
            string query = @"
SELECT TOP 1 
    e.EmployeeId,
    e.IsVerified,
    e.IsActive,
    e.RoleId
FROM Employees e
WHERE e.Email = @email AND e.PasswordHash = @password";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // 3. Invalid credentials
                if (!reader.Read())
                {
                    reader.Close();
                    MessageBox.Show("Invalid email or password.");
                    return;
                }

                // 4. Read values
                int employeeId = reader.GetInt32(0);
                bool isVerified = reader.GetBoolean(1);
                bool isActive = reader.GetBoolean(2);
                int roleId = reader.GetInt32(3);

                reader.Close();

                // 5. Check active status
                if (!isActive)
                {
                    MessageBox.Show("Your account is inactive. Please contact HR.");
                    return;
                }

                // 6. Check verification
                if (!isVerified)
                {
                    MessageBox.Show("Your email is not verified.");
                    return;
                }

                // 7. Role-based navigation
                if (roleId == 1) // HR
                {
                    HRDashboardForm hrForm = new HRDashboardForm(employeeId);
                    hrForm.Show();
                }
                //else if (roleId == 2) // Manager
               // {
                   // ManagerDashboardForm managerForm = new ManagerDashboardForm(employeeId);
                 //   managerForm.Show();
               // }
                else if (roleId == 3) // Employee
                {
                    EmployeeDashboardForm employeeForm = new EmployeeDashboardForm(employeeId);
                    employeeForm.Show();
                }
                else
                {
                    MessageBox.Show("Unknown role. Access denied.");
                    return;
                }

                // 8. Hide login form
                this.Hide();
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email == "")
            {
                MessageBox.Show("Please enter your email first.");
                return;
            }

            // 1) Check if email exists
            string checkUserQuery = "SELECT EmployeeId FROM Employees WHERE Email = @e;";

            // 2) Insert verification code
            string insertCodeQuery = @"
INSERT INTO EmailVerifications (EmployeeId, Code, ExpiresAt, IsUsed, CreatedAt)
VALUES (@id, @c, @x, 0, GETDATE());";

            try
            {
                conn.Open();

                int employeeId;
                using (SqlCommand cmdCheck = new SqlCommand(checkUserQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@e", email);
                    object result = cmdCheck.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("No account found with this email.");
                        return;
                    }

                    employeeId = (int)result;
                }

                // Generate simple 6-digit code
                string code = new Random().Next(100000, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(10);

                using (SqlCommand cmdInsert = new SqlCommand(insertCodeQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@id", employeeId);
                    cmdInsert.Parameters.AddWithValue("@c", code);
                    cmdInsert.Parameters.AddWithValue("@x", expiry);
                    cmdInsert.ExecuteNonQuery();
                }



                // Open reset password form
                new ResetPasswordForm(email, code).Show();
                this.Hide();

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
    }
}
