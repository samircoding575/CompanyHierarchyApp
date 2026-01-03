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
            StylePrimaryButton(button6, "LOGIN");
            StyleSecondaryButton(button5, "Create New Account");

            // Style Link
            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.ForeColor = Color.Gray;
            button4.Font = new Font("Segoe UI", 9, FontStyle.Underline);
            button4.Cursor = Cursors.Hand;
            button4.BackColor = Color.Transparent;

            // Remove tabstop focus rectangles
            button6.TabStop = false;
            button5.TabStop = false;
        }
        void StyleInputBox(TextBox txt, string placeholder, bool isPasswordBox)
        {
            // DO NOT touch Location, Width, or Height
            // Respect Designer layout completely

            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = backgroundColor;
            txt.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            // Placeholder logic (safe)
            ApplySafePlaceholder(txt, placeholder, isPasswordBox);

            // Create underline WITHOUT affecting layout
            Panel underline = new Panel
            {
                Height = 2,
                Width = txt.Width,
                BackColor = Color.LightGray,
                Left = txt.Left,
                Top = txt.Bottom + 2   // visually under, no overlap
            };

            // Add underline to SAME parent (important)
            txt.Parent.Controls.Add(underline);
            underline.BringToFront();

            // Focus animation (no layout impact)
            txt.GotFocus += (s, e) =>
            {
                underline.BackColor = primaryColor;
            };

            txt.LostFocus += (s, e) =>
            {
                underline.BackColor = Color.LightGray;
            };

            // Keep underline synced if textbox moves (Designer-safe)
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

   

        // Unused events generated by designer can be left empty
        private void button3_Click(object sender, EventArgs e) { }
        private void Form1_Load_1(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //this is the login button
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
                    MessageBox.Show("Manager Login Successful ");
                }
                else if (roleId == 3) // Employee
                {
                    EmployeeDashboardForm employeeForm = new EmployeeDashboardForm(employeeId);
                    employeeForm.Show();
                    MessageBox.Show("Employee Login Successful ");
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

        private void button5_Click(object sender, EventArgs e)
        {
            //this is the create new account button
            new RegisterForm().Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Forgot password button
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
                // 🔒 Safety check
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                // 1️⃣ Check if user exists
                SqlCommand cmdCheck = new SqlCommand(checkUserQuery, conn);
                cmdCheck.Parameters.AddWithValue("@e", email);

                object result = cmdCheck.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("No account found with this email.");
                    return;
                }

                int employeeId = Convert.ToInt32(result);

                // 2️⃣ Generate verification code
                string code = new Random().Next(100000, 999999).ToString();
                DateTime expiry = DateTime.Now.AddMinutes(10);

                // 3️⃣ Insert verification code
                SqlCommand cmdInsert = new SqlCommand(insertCodeQuery, conn);
                cmdInsert.Parameters.AddWithValue("@id", employeeId);
                cmdInsert.Parameters.AddWithValue("@c", code);
                cmdInsert.Parameters.AddWithValue("@x", expiry);
                cmdInsert.ExecuteNonQuery();

                // 4️⃣ Navigate to reset password form
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


        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}