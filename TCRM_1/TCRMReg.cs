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
using System.Text.RegularExpressions;

namespace TCRM_1
{
    public partial class TCRMReg : Form
    {
        public TCRMReg()
        {
            InitializeComponent();
            this.AcceptButton = button1; 
        }

        private void TCRMReg_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit(); // ensures everything ends
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            new TCRMLogin().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = textBox1.Text.Trim();
                string email = textBox2.Text.Trim();
                string password = textBox3.Text;
                string confirmPassword = textBox4.Text;

                // ✅ Validation checks
                if (string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Registration Failed", "Attempted registration with empty fields.");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Registration Failed", $"Invalid email format for '{email}'.");
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Registration Failed", $"Passwords did not match for username '{username}'.");
                    return;
                }

                if (password.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Registration Failed", $"Password too short for username '{username}'.");
                    return;
                }

                // ✅ Database operations
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    // Check if username or email already exists
                    string checkQuery = "SELECT COUNT(*) FROM acc WHERE username = @username OR email = @email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        checkCmd.Parameters.AddWithValue("@email", email);

                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            MessageBox.Show("Username or email already exists.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            AppConfig.LogEvent(null, "Registration Failed", $"Duplicate username or email attempted: '{username}' / '{email}'.");
                            return;
                        }
                    }

                    // Insert the new user record
                    string insertQuery = "INSERT INTO acc (username, email, password) OUTPUT INSERTED.Id VALUES (@username, @email, @password)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@username", username);
                        insertCmd.Parameters.AddWithValue("@email", email);
                        insertCmd.Parameters.AddWithValue("@password", password); // 🔒 TODO: hash later

                        int newAccId = (int)insertCmd.ExecuteScalar();

                        // Store user globally
                        AppConfig.CurrentAccId = newAccId;
                        AppConfig.CurrentUsername = username;

                        // Log success
                        AppConfig.LogEvent(newAccId, "Registration", $"User '{username}' registered successfully.");

                        MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        new TCRMLogin().Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppConfig.LogEvent(null, "Registration Error", $"Exception occurred during registration: {ex.Message}");
            }
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
