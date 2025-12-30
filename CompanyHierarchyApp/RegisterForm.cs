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
    public partial class RegisterForm : Form
    {
        // 1. Connection String (UNCHANGED)
        static string connString = "Data Source=.\\SQLEXPRESS;Initial Catalog=CompanyDB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = new SqlConnection(connString);

        // 2. Design Colors (Matching Login Form)
        private Color primaryColor = Color.FromArgb(41, 128, 185); // Professional Blue
        private Color secondaryColor = Color.FromArgb(52, 152, 219); // Lighter Blue
        private Color backgroundColor = Color.White;
        private Color textColor = Color.FromArgb(64, 64, 64); // Dark Gray

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            ApplyRegisterTheme();
        }

        // --- UI & DESIGN SECTION ---

        void ApplyRegisterTheme()
        {
            // Form Styling
            this.Text = "Create New Account";
            this.BackColor = backgroundColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            // Style Inputs
            // We pass 'false' for normal text, 'true' for password
            StyleInputBox(txtFullName, "Full Name", false);
            StyleInputBox(txtEmail, "Email Address", false);
            StyleInputBox(txtPassword, "Password", true);

            // Style Buttons
            StylePrimaryButton(button1, "CREATE ACCOUNT");
            StyleSecondaryButton(button2, "Back to Login");

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

            // Apply Safe Placeholder Logic
            ApplySafePlaceholder(txt, placeholder, isPasswordBox);

            // Add Decorative Underline
            Panel underline = new Panel();
            underline.Size = new Size(txt.Width, 2);
            underline.Location = new Point(txt.Location.X, txt.Location.Y + txt.Height);
            underline.BackColor = Color.LightGray;
            this.Controls.Add(underline);
            underline.BringToFront();

            // Focus Animation
            txt.GotFocus += (s, e) => { underline.BackColor = primaryColor; };
            txt.LostFocus += (s, e) => { underline.BackColor = Color.LightGray; };
        }

        void ApplySafePlaceholder(TextBox txt, string placeholder, bool isPassword)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            if (isPassword)
            {
                // Fix for Win32Exception: Disable system char, use PasswordChar property
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '\0'; // Visible text initially
            }

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = textColor;
                    if (isPassword) txt.PasswordChar = '●'; // Mask with dots
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                    if (isPassword) txt.PasswordChar = '\0'; // Show "Password" text
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

        // --- LOGIC SECTION (Functionality Preserved) ---

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // Updated validation: Ensure we don't submit the Placeholders as data
            if (name == "" || name == "Full Name" ||
                email == "" || email == "Email Address" ||
                password == "" || password == "Password")
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