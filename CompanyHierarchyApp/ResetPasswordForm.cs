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
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace CompanyHierarchyApp
{
    public partial class ResetPasswordForm : Form
    {
        static string connString =
  "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connString);
        string _email;

        string _code;

        public ResetPasswordForm(string email, string code)
        {
            InitializeComponent();
            _email = email;
            _code = code;

            // Display the code directly on the form
            lblVerificationCode.Text = "Your verification code is: " + _code;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string newPassword = txtNewPassword.Text;

            if (code == "" || newPassword == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            string getUserQuery = "SELECT EmployeeId FROM Employees WHERE Email = @e;";
            string checkCodeQuery = @"
SELECT TOP 1 VerificationId
FROM EmailVerifications
WHERE EmployeeId = @id AND Code = @c AND IsUsed = 0 AND ExpiresAt >= GETDATE()
ORDER BY CreatedAt DESC;";

            string updatePasswordQuery =
                "UPDATE Employees SET PasswordHash = @p WHERE EmployeeId = @id;";

            string markUsedQuery =
                "UPDATE EmailVerifications SET IsUsed = 1 WHERE VerificationId = @v;";

            try
            {
                conn.Open();

                int employeeId;
                using (SqlCommand cmdUser = new SqlCommand(getUserQuery, conn))
                {
                    cmdUser.Parameters.AddWithValue("@e", _email);
                    employeeId = (int)cmdUser.ExecuteScalar();
                }

                int verificationId;
                using (SqlCommand cmdCheck = new SqlCommand(checkCodeQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id", employeeId);
                    cmdCheck.Parameters.AddWithValue("@c", code);
                    object result = cmdCheck.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Invalid or expired code.");
                        return;
                    }

                    verificationId = (int)result;
                }

                using (SqlCommand cmdUpdate = new SqlCommand(updatePasswordQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@p", newPassword);
                    cmdUpdate.Parameters.AddWithValue("@id", employeeId);
                    cmdUpdate.ExecuteNonQuery();
                }

                using (SqlCommand cmdUsed = new SqlCommand(markUsedQuery, conn))
                {
                    cmdUsed.Parameters.AddWithValue("@v", verificationId);
                    cmdUsed.ExecuteNonQuery();
                }

                MessageBox.Show("Password reset successfully. You can now log in.");
                new Form1().Show();
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

        private void ResetPasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void lblVerificationCode_Click(object sender, EventArgs e)
        {

        }
    }
}
