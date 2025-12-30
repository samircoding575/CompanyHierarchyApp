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

namespace CompanyHierarchyApp
{
    public partial class Form1 : Form
    {
        // 1. Connection String
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = new SqlConnection(connString);

        // 2. Design Colors
        private Color primaryColor = Color.FromArgb(41, 128, 185); // Professional Blue
        private Color secondaryColor = Color.FromArgb(52, 152, 219); // Lighter Blue
        private Color backgroundColor = Color.White;
        private Color textColor = Color.FromArgb(64, 64, 64); // Dark Gray text

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyLoginTheme();
        }

        // --- UI & DESIGN SECTION (FIXED) ---

        void ApplyLoginTheme()
        {
            // Form Style
            this.Text = "Company Hierarchy System";
            this.BackColor = backgroundColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            // Style Inputs
            // NOTE: We pass 'true' for the password field to indicate it needs masking logic
            StyleInputBox(txtEmail, "Email Address", false);
            StyleInputBox(txtPassword, "Password", true);

            // Style Buttons
            StylePrimaryButton(button1, "LOGIN");
            StyleSecondaryButton(button2, "Create New Account");

            // Style Link
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;
            button3.ForeColor = Color.Gray;
            button3.Font = new Font("Segoe UI", 9, FontStyle.Underline);
            button3.Cursor = Cursors.Hand;
            button3.BackColor = Color.Transparent;

            // Remove tabstop focus rectangles
            button1.TabStop = false;
            button2.TabStop = false;
        }

        void StyleInputBox(TextBox txt, string placeholder, bool isPasswordBox)
        {
            // Reset Styles
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = backgroundColor;
            txt.Font = new Font("Segoe UI", 12);
            txt.AutoSize = false;
            txt.Height = 30;

            // Apply the Safe Placeholder Logic
            ApplySafePlaceholder(txt, placeholder, isPasswordBox);

            // Add the decorative underline
            Panel underline = new Panel();
            underline.Size = new Size(txt.Width, 2);
            underline.Location = new Point(txt.Location.X, txt.Location.Y + txt.Height);
            underline.BackColor = Color.LightGray;
            this.Controls.Add(underline);
            underline.BringToFront();

            // Animation for underline focus
            txt.GotFocus += (s, e) => { underline.BackColor = primaryColor; };
            txt.LostFocus += (s, e) => { underline.BackColor = Color.LightGray; };
        }

        void ApplySafePlaceholder(TextBox txt, string placeholder, bool isPassword)
        {
            // Initialize
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            if (isPassword)
            {
                // CRITICAL FIX: Disable SystemPasswordChar to prevent handle crashes.
                // We will use PasswordChar property instead.
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '\0'; // '\0' means visible text
            }

            txt.GotFocus += (s, e) =>
            {
                // Check if the box contains the placeholder
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = textColor;

                    if (isPassword)
                    {
                        // Use the Bullet character (●) for password masking
                        // This does NOT recreate the window handle, preventing the crash.
                        txt.PasswordChar = '●';
                    }
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;

                    if (isPassword)
                    {
                        // Reset to visible text so user can read "Password"
                        txt.PasswordChar = '\0';
                    }
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

        void StyleSecondaryButton(Button btn, string text)
        {
            btn.Text = text;
            btn.BackColor = Color.White;
            btn.ForeColor = primaryColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = primaryColor;
            btn.Cursor = Cursors.Hand;
            btn.Height = 40;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.WhiteSmoke;
            btn.MouseLeave += (s, e) => btn.BackColor = Color.White;
        }

        // --- LOGIC SECTION (Identical Logic, formatting checks updated) ---

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // Update validation to check for Placeholders too
            if (email == "" || email == "Email Address" || password == "" || password == "Password")
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

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

                if (!reader.Read())
                {
                    reader.Close();
                    MessageBox.Show("Invalid email or password.");
                    return;
                }

                int employeeId = reader.GetInt32(0);
                bool isVerified = reader.GetBoolean(1);
                bool isActive = reader.GetBoolean(2);
                int roleId = reader.GetInt32(3);

                reader.Close();

                if (!isActive)
                {
                    MessageBox.Show("Your account is inactive. Please contact HR.");
                    return;
                }

                if (!isVerified)
                {
                    MessageBox.Show("Your email is not verified.");
                    return;
                }

                // Role Navigation
                if (roleId == 1) // HR
                {
                     HRDashboardForm hrForm = new HRDashboardForm(employeeId);
                     hrForm.Show();
                    MessageBox.Show("HR Login Successful (Add Form Here)");
                }
                else if (roleId == 2) // Manager
                {
                     ManagerDashboardForm managerForm = new ManagerDashboardForm(employeeId);
                     managerForm.Show();
                    MessageBox.Show("Manager Login Successful (Add Form Here)");
                }
                else if (roleId == 3) // Employee
                {
                    EmployeeDashboardForm employeeForm = new EmployeeDashboardForm(employeeId);
                     employeeForm.Show();
                    MessageBox.Show("Employee Login Successful (Add Form Here)");
                }
                else
                {
                    MessageBox.Show("Unknown role. Access denied.");
                    return;
                }

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

        private void button2_Click(object sender, EventArgs e)
        {
            new RegisterForm().Show();
            this.Hide();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email == "" || email == "Email Address")
            {
                MessageBox.Show("Please enter your email first.");
                return;
            }

            string checkUserQuery = "SELECT EmployeeId FROM Employees WHERE Email = @e;";
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

                string code = new Random().Next(100000, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(10);

                using (SqlCommand cmdInsert = new SqlCommand(insertCodeQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@id", employeeId);
                    cmdInsert.Parameters.AddWithValue("@c", code);
                    cmdInsert.Parameters.AddWithValue("@x", expiry);
                    cmdInsert.ExecuteNonQuery();
                }

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

        // Unused events generated by designer can be left empty
        private void button3_Click(object sender, EventArgs e) { }
        private void Form1_Load_1(object sender, EventArgs e) { }
    }
}