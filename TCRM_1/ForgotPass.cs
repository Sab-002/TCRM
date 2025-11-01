using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TCRM_1
{
    public partial class ForgotPass : Form
    {
        private string generatedCode = "";
        private string userEmail = "";

        // Gmail API credentials
        private readonly string[] Scopes = { GmailService.Scope.GmailSend };
        private readonly string ApplicationName = "TCRM Gmail Reset";

        public ForgotPass()
        {
            InitializeComponent();
        }

        private void ForgotPass_Load(object sender, EventArgs e)
        {
            panelReset.Visible = false;
        }

        private void btnSendCode_Click(object sender, EventArgs e)
        {
            userEmail = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(userEmail))
            {
                MessageBox.Show("Please enter your registered email.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM acc WHERE email=@Email", conn);
                cmd.Parameters.AddWithValue("@Email", userEmail);
                int count = (int)cmd.ExecuteScalar();

                if (count == 0)
                {
                    MessageBox.Show("Email not found in system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                generatedCode = new Random().Next(100000, 999999).ToString();
                SendVerificationEmail(userEmail, generatedCode);
                MessageBox.Show("A verification code has been sent to your email.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (txtCode.Text.Trim() == generatedCode)
            {
                MessageBox.Show("Code verified successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panelReset.Visible = true;
            }
            else
            {
                MessageBox.Show("Invalid verification code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string newPass = txtNewPassword.Text.Trim();

            if (string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Please enter a new password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE acc SET password=@Password WHERE email=@Email", conn);
                cmd.Parameters.AddWithValue("@Password", newPass);
                cmd.Parameters.AddWithValue("@Email", userEmail);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Password reset successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AppConfig.LogEvent(null, "Password Reset", $"Password reset for {userEmail}");
            Close();
        }

        // ---------------- Gmail API email sending method ----------------
        private void SendVerificationEmail(string toEmail, string code)
        {
            try
            {
                // Load Gmail API credentials from file
                UserCredential credential;
                using (FileStream stream = new FileStream(Application.StartupPath + @"\credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/gmail-tcrm.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    ).Result;
                }

                // Create Gmail service
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });

                // Compose message
                string message =
                    $"To: {toEmail}\r\n" +
                    $"Subject: Password Reset Verification Code\r\n" +
                    "Content-Type: text/html; charset=utf-8\r\n\r\n" +
                    $"<p>Your password reset verification code is:</p><h2>{code}</h2>";

                // Convert to Base64 URL
                var msg = new Google.Apis.Gmail.v1.Data.Message
                {
                    Raw = Base64UrlEncode(message)
                };

                // Send message
                service.Users.Messages.Send(msg, "me").Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail API send failed: " + ex.Message);
            }
        }

        private string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        // ---------------- UI Buttons ----------------
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void close(object sender, FormClosingEventArgs e)
        {
        }
    }
}
