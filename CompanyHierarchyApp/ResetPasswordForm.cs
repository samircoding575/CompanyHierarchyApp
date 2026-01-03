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
    public partial class ResetPasswordForm : Form
    {
        // 1. Connection String
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = new SqlConnection(connString);

        // 2. Design Colors (Consistent with other forms)
        private Color primaryColor = Color.FromArgb(41, 128, 185); // Professional Blue
        private Color secondaryColor = Color.FromArgb(52, 152, 219); // Lighter Blue
        private Color backgroundColor = Color.White;
        private Color textColor = Color.FromArgb(64, 64, 64); // Dark Gray

        string _email;
        string _code;

        public ResetPasswordForm(string email, string code)
        {
            InitializeComponent();
            _email = email;
            _code = code;

            // Display the code (We will style this label in ApplyResetTheme)
            lblVerificationCode.Text = "Your verification code is: " + _code;
        }

        private void ResetPasswordForm_Load(object sender, EventArgs e)
        {
            ApplyResetTheme();
        }

        // --- UI & DESIGN SECTION ---

        void ApplyResetTheme()
        {
            // Form Styling
            this.Text = "Reset Password";
            this.BackColor = backgroundColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            // Style the Instruction Label
            lblVerificationCode.ForeColor = primaryColor;
            lblVerificationCode.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblVerificationCode.TextAlign = ContentAlignment.MiddleCenter;
            // Optional: Ensure the label is wide enough or docked to center if not set in designer
            // lblVerificationCode.AutoSize = false; 
            // lblVerificationCode.Width = this.ClientSize.Width; 

            // Style Inputs
            StyleInputBox(txtCode, "Verification Code", false);
            StyleInputBox(txtNewPassword, "New Password", true);

            // Style Button
            StylePrimaryButton(button2, "RESET PASSWORD");

            // Remove tabstop focus rectangles
            button2.TabStop = false;
        }

        void StyleInputBox(TextBox txt, string placeholder, bool isPasswordBox)
        {
            // Respect Designer-defined size & position
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = backgroundColor;
            txt.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            // Placeholder logic (unchanged)
            ApplySafePlaceholder(txt, placeholder, isPasswordBox);

            // Create underline safely (no layout shift)
            Panel underline = new Panel
            {
                Height = 2,
                Width = txt.Width,
                BackColor = Color.LightGray,
                Left = txt.Left,
                Top = txt.Bottom + 2
            };

            // IMPORTANT: attach underline to the same parent
            txt.Parent.Controls.Add(underline);
            underline.BringToFront();

            // Visual-only focus behavior
            txt.GotFocus += (s, e) =>
            {
                underline.BackColor = primaryColor;
            };

            txt.LostFocus += (s, e) =>
            {
                underline.BackColor = Color.LightGray;
            };

            // Keep underline synced if Designer layout changes
            txt.LocationChanged += (s, e) =>
            {
                underline.Left = txt.Left;
                underline.Top = txt.Bottom + 2;
            };

            txt.SizeChanged += (s, e) =>
            {
                underline.Width = txt.Width;
                underline.Top = txt.Bottom + 2;
            };
        }


        void ApplySafePlaceholder(TextBox txt, string placeholder, bool isPassword)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            if (isPassword)
            {
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '\0';
            }

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = textColor;
                    if (isPassword) txt.PasswordChar = '●';
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                    if (isPassword) txt.PasswordChar = '\0';
                }
            };
        }

        void StylePrimaryButton(Button btn, string text)
        {
            btn.Text = text;
            btn.BackColor = primaryColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Height = 45;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            btn.MouseEnter += (s, e) => btn.BackColor = secondaryColor;
            btn.MouseLeave += (s, e) => btn.BackColor = primaryColor;
        }

        // --- LOGIC SECTION (Functionality Preserved) ---

        private void button2_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string newPassword = txtNewPassword.Text;

            if (code == "" || code == "Verification Code" ||
                newPassword == "" || newPassword == "New Password")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            string getUserQuery =
                "SELECT EmployeeId, PasswordHash FROM Employees WHERE Email = @e;";

            string checkCodeQuery = @"
SELECT TOP 1 VerificationId
FROM EmailVerifications
WHERE EmployeeId = @id
  AND Code = @c
  AND IsUsed = 0
  AND ExpiresAt >= GETDATE()
ORDER BY CreatedAt DESC;";

            string updatePasswordQuery =
                "UPDATE Employees SET PasswordHash = @p WHERE EmployeeId = @id;";

            string markUsedQuery =
                "UPDATE EmailVerifications SET IsUsed = 1 WHERE VerificationId = @v;";

            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                // 1️⃣ Get employee ID AND current password
                SqlCommand cmdUser = new SqlCommand(getUserQuery, conn);
                cmdUser.Parameters.AddWithValue("@e", _email);

                SqlDataReader reader = cmdUser.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    MessageBox.Show("User not found.");
                    return;
                }

                int employeeId = reader.GetInt32(0);
                string oldPassword = reader.GetString(1);
                reader.Close();

                // 2️⃣ Check if new password is same as old
                if (oldPassword == newPassword)
                {
                    MessageBox.Show("New password must be different from the old password.");
                    return;
                }

                // 3️⃣ Validate verification code
                SqlCommand cmdCheck = new SqlCommand(checkCodeQuery, conn);
                cmdCheck.Parameters.AddWithValue("@id", employeeId);
                cmdCheck.Parameters.AddWithValue("@c", code);

                object result = cmdCheck.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Invalid or expired code.");
                    return;
                }

                int verificationId = Convert.ToInt32(result);

                // 4️⃣ Update password
                SqlCommand cmdUpdate = new SqlCommand(updatePasswordQuery, conn);
                cmdUpdate.Parameters.AddWithValue("@p", newPassword);
                cmdUpdate.Parameters.AddWithValue("@id", employeeId);
                cmdUpdate.ExecuteNonQuery();

                // 5️⃣ Mark verification code as used
                SqlCommand cmdUsed = new SqlCommand(markUsedQuery, conn);
                cmdUsed.Parameters.AddWithValue("@v", verificationId);
                cmdUsed.ExecuteNonQuery();

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





        private void lblVerificationCode_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}