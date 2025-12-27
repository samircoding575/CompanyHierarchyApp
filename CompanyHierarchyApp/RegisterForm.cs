using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace CompanyHierarchyApp
{
    public partial class RegisterForm : Form
    {
        static string connString =
  "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (name == "" || email == "" || password == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            // 1) Get RoleId for Employee (default role)
            string roleQuery = "SELECT RoleId FROM Roles WHERE RoleName='Employee';";

            // 2) Insert user (NO verification step)
            string insertQuery = @"
INSERT INTO Employees (FullName, Email, PasswordHash, RoleId, IsVerified, IsActive, CreatedAt)
VALUES (@n, @e, @p, @r, 1, 1, GETDATE());";

            try
            {
                conn.Open();

                int roleId;
                using (SqlCommand cmdRole = new SqlCommand(roleQuery, conn))
                {
                    object r = cmdRole.ExecuteScalar();
                    if (r == null)
                    {
                        MessageBox.Show("Employee role not found in Roles table.");
                        return;
                    }
                    roleId = (int)r;
                }

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.Parameters.AddWithValue("@r", roleId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Account created successfully. You can now log in.");

                // Go back to login
                new Form1().Show();
                this.Hide();
            }
            catch (SqlException ex)
            {
                // Common case: duplicate email (unique constraint)
                MessageBox.Show("Database error: " + ex.Message);
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


        private void button2_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Hide();
        }
    }
}
