using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TCRM_1
{
    public partial class Options : Form
    {
        private TCRMDash _dashboard;

        public Options(TCRMDash dashboard)
        {
            InitializeComponent();
            _dashboard = dashboard;
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(200, 200); // increased height for the new button
            this.Text = "Account Options";
            this.Deactivate += (s, e) =>
            {
                this.Hide();  // or this.Close() if you want to destroy it
            };

            // Logs Button
            Button btnLogs = new Button
            {
                Text = "View Logs",
                Size = new Size(160, 30),
                Location = new Point(20, 20)
            };
            btnLogs.Click += BtnLogs_Click;
            this.Controls.Add(btnLogs);

            // Change Credentials Button
            Button btnChange = new Button
            {
                Text = "Change Credentials",
                Size = new Size(160, 30),
                Location = new Point(20, 60)
            };
            btnChange.Click += BtnChange_Click;
            this.Controls.Add(btnChange);

            // Archive Button
            Button btnArchive = new Button
            {
                Text = "View Archive",
                Size = new Size(160, 30),
                Location = new Point(20, 100)
            };
            btnArchive.Click += BtnArchive_Click;
            this.Controls.Add(btnArchive);

            // Logout Button
            Button btnLogout = new Button
            {
                Text = "Logout",
                Size = new Size(160, 30),
                Location = new Point(20, 140),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 35, 51);
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);
        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            using (var changeForm = new ChangeCredentialForm(AppConfig.CurrentAccId))
            {
                changeForm.ShowDialog(this); 
                _dashboard.RefreshUsername();
            }
        }

        private void BtnLogs_Click(object sender, EventArgs e)
        {
            LogsForm logsForm = new LogsForm();
            logsForm.ShowDialog();
        }

        private void BtnArchive_Click(object sender, EventArgs e)
        {
            ArchiveForm archiveForm = new ArchiveForm(_dashboard);
            archiveForm.Show();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
        private void Options_Load(object sender, EventArgs e)
        {

        }
    }
}
