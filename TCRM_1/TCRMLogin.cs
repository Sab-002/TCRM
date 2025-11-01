using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.SqlClient;

namespace TCRM_1
{
    public partial class TCRMLogin : Form
    {
        public TCRMLogin()
        {
            InitializeComponent();
            this.AcceptButton = login;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            user.Select();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = user.Text.Trim();
                string password = pass.Text;

                // Basic validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Login Failed", "Attempted login with empty fields.");
                    return;
                }

                // Optional: prevent SQL injection attempts
                if (username.Contains("'") || username.Contains("--") || username.Contains(";"))
                {
                    MessageBox.Show("Invalid characters in username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.LogEvent(null, "Login Failed", $"Suspicious characters in username '{username}'.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT Id, username FROM acc WHERE username = @username AND password = @password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int accId = reader.GetInt32(0);
                                string accUser = reader.GetString(1);

                                // Store globally
                                AppConfig.CurrentAccId = accId;
                                AppConfig.CurrentUsername = accUser;

                                reader.Close();

                                // Log successful login
                                AppConfig.LogEvent(accId, "Login", $"User '{accUser}' logged in successfully.");

                                // Open dashboard
                                new TCRMDash(accUser).Show();
                                this.Hide();
                            }
                            else
                            {
                                reader.Close();

                                // Try to find account ID if username exists
                                string idQuery = "SELECT Id FROM acc WHERE username = @username";
                                using (SqlCommand idCmd = new SqlCommand(idQuery, conn))
                                {
                                    idCmd.Parameters.AddWithValue("@username", username);
                                    object result = idCmd.ExecuteScalar();

                                    int? accId = result != null ? Convert.ToInt32(result) : (int?)null;
                                    AppConfig.LogEvent(accId, "Login Failed", $"Failed login attempt for username: '{username}'.");
                                }

                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppConfig.LogEvent(null, "Login Error", $"Exception occurred during login: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pass_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            new TCRMReg().Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            new ForgotPass().Show();
        }
    }
}
