using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.Size = new System.Drawing.Size(200, 160);
            this.Text = "Account Options";

            // Logs Button
            Button btnLogs = new Button();
            btnLogs.Text = "View Logs";
            btnLogs.Size = new System.Drawing.Size(160, 30);
            btnLogs.Location = new System.Drawing.Point(20, 20);
            btnLogs.Click += BtnLogs_Click;
            this.Controls.Add(btnLogs);

            // Change Credentials Button
            Button btnChange = new Button();
            btnChange.Text = "Change Credentials";
            btnChange.Size = new System.Drawing.Size(160, 30);
            btnChange.Location = new System.Drawing.Point(20, 60);
            btnChange.Click += BtnChange_Click;
            this.Controls.Add(btnChange);

            // Logout Button
            Button btnLogout = new Button();
            btnLogout.Text = "Logout";
            btnLogout.Size = new Size(160, 30);
            btnLogout.Location = new Point(20, 100);

            // Styling (reddish theme)
            btnLogout.BackColor = Color.FromArgb(220, 53, 69); // Soft Bootstrap red
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 35, 51); // Darker on hover
            btnLogout.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);


        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            using (var changeForm = new ChangeCredentialForm(AppConfig.CurrentAccId))
            {
                changeForm.ShowDialog(this); // modal
                _dashboard.RefreshUsername(); // update dashboard after closing
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Restart(); // or navigate back to login form
            }
        }
        private void BtnLogs_Click(object sender, EventArgs e)
        {
            LogsForm logsForm = new LogsForm();
            logsForm.ShowDialog();
        }

        private void Options_Load(object sender, EventArgs e)
        {

        }

    }
}
