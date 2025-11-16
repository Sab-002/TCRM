using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCRM_1
{
    public partial class LogsForm : Form
    {
        public LogsForm()
        {
            InitializeComponent();
            SetupForm();
            LoadLogs();
        }

        private void SetupForm()
        {
            this.Text = "System Logs";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Button btnBack = new Button();
            btnBack.Text = "Back";
            btnBack.Size = new Size(80, 30);
            btnBack.Location = new Point(20, 20);
            btnBack.Click += BtnBack_Click;
            this.Controls.Add(btnBack);

            DataGridView dgvLogs = new DataGridView();
            dgvLogs.Name = "dgvLogs";
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.Location = new Point(20, 70);
            dgvLogs.Size = new Size(640, 360);
            dgvLogs.ReadOnly = true;
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 👇 Add these for bigger message area
            dgvLogs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvLogs.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            this.Controls.Add(dgvLogs);
        }


        private void LoadLogs()
        {
            string query = "SELECT LogId, Type, Message, Timestamp FROM Logs WHERE AccId = @AccId ORDER BY timestamp DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            DataGridView dgvLogs = this.Controls["dgvLogs"] as DataGridView;
                            if (dgvLogs != null)
                                dgvLogs.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            this.Close(); // Closes when user clicks outside the form
        }

        private void LogsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
