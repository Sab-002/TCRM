using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TCRM_1
{
    public partial class ChangePasswordForm : Form
    {
        private int _accId;

        public ChangePasswordForm(int accId)
        {
            InitializeComponent();
            _accId = accId;
            CancelButton = btnCancel;
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            string currentPass = txtCurrentPass.Text;
            string newPass = txtNewPass.Text;
            string confirmPass = txtConfirmPass.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(currentPass) ||
                string.IsNullOrWhiteSpace(newPass) ||
                string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Please fill in all password fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass.Length < 8)
            {
                MessageBox.Show("New password must be at least 8 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("New passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    // Verify current password
                    string checkQuery = "SELECT COUNT(*) FROM acc WHERE Id = @Id AND password = @Password";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", _accId);
                        checkCmd.Parameters.AddWithValue("@Password", currentPass);

                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists == 0)
                        {
                            MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Update new password
                    string updateQuery = "UPDATE acc SET password = @Password WHERE Id = @Id";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@Password", newPass);
                        updateCmd.Parameters.AddWithValue("@Id", _accId);
                        updateCmd.ExecuteNonQuery();
                    }

                    AppConfig.LogEvent(_accId, "Password Change", "User successfully changed their password.");
                    MessageBox.Show("Password updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while changing password:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
