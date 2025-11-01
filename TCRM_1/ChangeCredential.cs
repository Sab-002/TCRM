using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TCRM_1
{
    public partial class ChangeCredentialForm : Form
    {
        private int _accId;

        public ChangeCredentialForm(int accId)
        {
            InitializeComponent();
            _accId = accId;
            CancelButton = btnCancel;
        }

        private void ChangeCredentialForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            LoadCurrentCredentials();
        }

        private void LoadCurrentCredentials()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT email, username FROM acc WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _accId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmail.Text = reader["email"].ToString();
                                txtUsername.Text = reader["username"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading credentials: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newEmail = txtEmail.Text.Trim();
            string newUsername = txtUsername.Text.Trim();

            if (string.IsNullOrWhiteSpace(newEmail) || string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("Please fill in both email and username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string updateQuery = "UPDATE acc SET email = @Email, username = @Username WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", newEmail);
                        cmd.Parameters.AddWithValue("@Username", newUsername);
                        cmd.Parameters.AddWithValue("@Id", _accId);
                        cmd.ExecuteNonQuery();
                    }
                }
                AppConfig.CurrentUsername = txtUsername.Text.Trim();
                AppConfig.LogEvent(_accId, "Credential Change", "User updated their email and username.");
                MessageBox.Show("Credentials updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating credentials: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            using (var passForm = new ChangePasswordForm(_accId))
            {
                passForm.ShowDialog(this);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
