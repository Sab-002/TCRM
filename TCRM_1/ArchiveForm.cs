using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCRM_1
{
    public partial class ArchiveForm : Form
    {
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel contentPanel;
        private Panel panel2;
        private TCRMDash _dashboard;

        public ArchiveForm(TCRMDash dashboard)
        {
            InitializeComponent();
            LoadArchivedItems();
            _dashboard = dashboard;
        }
        private void LoadArchivedItems()
        {
            flowLayoutPanel1.Controls.Clear();

            string queryNotes = "SELECT NoteId AS Id, Title, Note AS Content, 'Note' AS Type, Created, Modified FROM Notes WHERE AccId=@AccId AND IsArchived=1 ORDER BY Modified DESC";
            string queryWebNotes = "SELECT Id, Title, Link AS Content, 'WebNote' AS Type, Created, Created AS Modified FROM webnote WHERE AccId=@AccId AND IsArchived=1 ORDER BY Created DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    DataTable dtAll = new DataTable();

                    using (SqlCommand cmdNotes = new SqlCommand(queryNotes, conn))
                    {
                        cmdNotes.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdNotes))
                        {
                            adapter.Fill(dtAll);
                        }
                    }

                    using (SqlCommand cmdWebNotes = new SqlCommand(queryWebNotes, conn))
                    {
                        cmdWebNotes.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdWebNotes))
                        {
                            DataTable dtWeb = new DataTable();
                            adapter.Fill(dtWeb);
                            dtAll.Merge(dtWeb);
                        }
                    }

                    // Show placeholder if no archived items
                    if (dtAll.Rows.Count == 0)
                    {
                        flowLayoutPanel1.Controls.Clear();

                        Label lblEmpty = new Label
                        {
                            Text = "No archived items yet. Archived items will appear here.",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Segoe UI", 12, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            Size = new Size(flowLayoutPanel1.ClientSize.Width - 10, 50),
                            Location = new Point(5, 5)
                        };

                        flowLayoutPanel1.Controls.Add(lblEmpty);
                        return;
                    }

                    foreach (DataRow row in dtAll.Rows)
                    {
                        int id = Convert.ToInt32(row["Id"]);
                        string title = row["Title"] == DBNull.Value ? (row["Type"].ToString() == "Note" ? "Note" : "Web Note") : row["Title"].ToString();
                        string content = row["Content"].ToString();
                        string type = row["Type"].ToString();

                        Button btn = new Button
                        {
                            Text = title,
                            Size = new Size(640, 60),
                            Tag = new { Id = id, Type = type, Title = title, Content = content },
                            BackColor = type == "Note" ? Color.FromArgb(153, 245, 128) : Color.FromArgb(204, 229, 255),
                            TextAlign = ContentAlignment.MiddleLeft
                        };

                        btn.Click += async (s, ev) => await ShowItemDetail(id, type, title, content);

                        btn.MouseUp += (s, ev) =>
                        {
                            if (ev.Button == MouseButtons.Right)
                            {
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Unarchive").Name = "Unarchive";
                                cms.Items.Add("Delete").Name = "Delete";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Unarchive")
                                        UpdateArchiveStatus(id, type, false);
                                    else if (ev2.ClickedItem.Name == "Delete")
                                        DeleteArchivedItem(id, type, title);
                                };

                                cms.Show(Cursor.Position);
                            }
                        };

                        flowLayoutPanel1.Controls.Add(btn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load archived items:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ShowItemDetail(int id, string type, string title, string content)
        {
            flowLayoutPanel1.Visible = false;
            panel2.Visible = false;
            contentPanel.Controls.Clear();
            contentPanel.Visible = true;
            contentPanel.Dock = DockStyle.Fill;

            if (type == "Note")
            {
                Button backBtn = new Button
                {
                    Text = "← Back",
                    Size = new Size(100, 40),
                    Location = new Point(10, 10)
                };

                Label lblCount = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleRight,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                TextBox txtTitle = new TextBox
                {
                    Text = title,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Location = new Point(10, 60),
                    Width = contentPanel.Width - 20,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    ReadOnly = true,
                    BackColor = Color.White
                };

                TextBox txtBox = new TextBox
                {
                    Text = content,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Segoe UI", 16),
                    Location = new Point(10, txtTitle.Bottom + 10),
                    Size = new Size(contentPanel.Width - 20, contentPanel.Height - txtTitle.Bottom - 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    ReadOnly = true,
                    BackColor = Color.White
                };

                lblCount.Text = $"Letters: {txtBox.Text.Length}";
                txtBox.TextChanged += (s, ev) => lblCount.Text = $"Letters: {txtBox.Text.Length}";

                contentPanel.Resize += (s, ev) =>
                {
                    lblCount.Location = new Point(contentPanel.Width - 140, 20);
                };

                backBtn.Click += (s, ev) =>
                {
                    contentPanel.Visible = false;
                    flowLayoutPanel1.Visible = true;
                    panel2.Visible = true;
                };

                contentPanel.Controls.Add(backBtn);
                contentPanel.Controls.Add(lblCount);
                contentPanel.Controls.Add(txtTitle);
                contentPanel.Controls.Add(txtBox);

                lblCount.Location = new Point(contentPanel.Width - 140, 20);
            }
            else // WebNote
            {
                Panel topPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    BackColor = Color.FromArgb(102, 102, 204)
                };
                contentPanel.Controls.Add(topPanel);

                Button backBtn = new Button
                {
                    Text = "← Back",
                    Size = new Size(80, 30),
                    Location = new Point(10, 20),
                    FlatStyle = FlatStyle.Flat
                };
                topPanel.Controls.Add(backBtn);

                TextBox txtTitle = new TextBox
                {
                    Text = title,
                    Font = new Font("Segoe UI", 10),
                    Width = 180,
                    Location = new Point(100, 22),
                    ReadOnly = true,
                    BackColor = Color.White
                };
                topPanel.Controls.Add(txtTitle);

                TextBox txtUrl = new TextBox
                {
                    Text = content,
                    Font = new Font("Segoe UI", 10),
                    Width = 300,
                    Location = new Point(290, 22),
                    ReadOnly = true,
                    BackColor = Color.White
                };
                topPanel.Controls.Add(txtUrl);

                var webView = new Microsoft.Web.WebView2.WinForms.WebView2
                {
                    Dock = DockStyle.Fill
                };
                contentPanel.Controls.Add(webView);
                webView.BringToFront();
                topPanel.SendToBack();

                try
                {
                    await webView.EnsureCoreWebView2Async(null);
                    webView.CoreWebView2.Navigate(content);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load URL:\n" + ex.Message);
                }

                backBtn.Click += (s, ev) =>
                {
                    try { webView.Dispose(); } catch { }
                    contentPanel.Visible = false;
                    flowLayoutPanel1.Visible = true;
                    panel2.Visible = true;
                };
            }
        }

        private void UpdateArchiveStatus(int id, string type, bool isArchived)
        {
            string query = type == "Note" ? "UPDATE Notes SET IsArchived=@IsArchived WHERE NoteId=@Id" : "UPDATE webnote SET IsArchived=@IsArchived WHERE Id=@Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@IsArchived", isArchived);
                        cmd.ExecuteNonQuery();
                    }
                }

                AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"{(isArchived ? "Archived" : "Unarchived")} {type}: {id}");
                LoadArchivedItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update archive status:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteArchivedItem(int id, string type, string title)
        {
            if (MessageBox.Show($"Are you sure you want to permanently delete this {type}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string query = type == "Note" ? "DELETE FROM Notes WHERE NoteId=@Id" : "DELETE FROM webnote WHERE Id=@Id";

                try
                {
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Deleted archived {type}: {title}");
                    LoadArchivedItems();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            _dashboard.LoadAllSorted();
            this.Close();
        }
        private void ArchiveForm_Load(object sender, EventArgs e)
        {

        }
    }
}
