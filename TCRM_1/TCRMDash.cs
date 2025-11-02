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
    public partial class TCRMDash : Form
    {
        public TCRMDash(string user)
        {
            InitializeComponent();
            label1.Text = "Welcome, " + user;
        }
        public void RefreshUsername()
        {
            label1.Text = "Welcome, " + AppConfig.CurrentUsername;
        }
        private async void TCRM_2_Load(object sender, EventArgs e)
        {
            // await webView21.EnsureCoreWebView2Async(null);
            // webView21.CoreWebView2.Navigate("https://www.youtube.com");

            // clear buttons
            //contentPanel.Controls.Clear();

            // Create content panel here first
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Visible = false;
            Controls.Add(contentPanel);

            // Now safe to load notes after contentPanel exists
            LoadNotes();
            LoadWebNotes();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit(); // ensures everything ends
        }

       

        private void button4_Click(object sender, EventArgs e)
        {
            Options optionsForm = new Options(this); 
            
            // Position it near the button (like a context popup)
            var button = (Button)sender;
            var buttonLocation = button.PointToScreen(System.Drawing.Point.Empty);

            optionsForm.StartPosition = FormStartPosition.Manual;
            optionsForm.Location = new System.Drawing.Point(buttonLocation.X, buttonLocation.Y + button.Height);

            optionsForm.Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private async void button2_Click(object sender, EventArgs e)
        {
            using (var urlForm = new UrlInputForm())
            {
                if (urlForm.ShowDialog(this) == DialogResult.OK)
                {
                    string url = urlForm.EnteredUrl;
                    string title = string.IsNullOrWhiteSpace(urlForm.EnteredTitle) ? "Web Note" : urlForm.EnteredTitle;

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        MessageBox.Show("URL cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                        {
                            conn.Open();
                            string query = "INSERT INTO webnote (AccId, Title, Link) VALUES (@AccId, @Title, @Link);";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                                cmd.Parameters.AddWithValue("@Title", title);
                                cmd.Parameters.AddWithValue("@Link", url);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Added new web note: {title}");
                        LoadNotes();
                        LoadWebNotes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save to database:\n" + ex.Message);
                    } 
                }
            }
        }

        private void LoadWebNotes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Link FROM webnote WHERE AccId = @AccId ORDER BY Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int noteId = reader.GetInt32(0);
                                string currentTitle = reader.IsDBNull(1) ? "Web Note" : reader.GetString(1);
                                string currentUrl = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                Button newBtn = new Button
                                {
                                    Text = currentTitle,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    BackColor = Color.FromArgb(204, 229, 255),
                                    Tag = noteId
                                };

                                // LEFT CLICK → open editable web note
                                newBtn.Click += async (s, ev) =>
                                {
                                    flowLayoutPanel1.Visible = false;
                                    panel2.Visible = false;
                                    contentPanel.Controls.Clear();
                                    contentPanel.Visible = true;

                                    // --- Header Panel ---
                                    Panel topPanel = new Panel
                                    {
                                        Dock = DockStyle.Top,
                                        Height = 70,
                                        BackColor = Color.FromArgb(102, 102, 204)
                                    };
                                    contentPanel.Controls.Add(topPanel);

                                    // Back Button
                                    Button backBtn = new Button
                                    {
                                        Text = "← Back",
                                        Size = new Size(80, 30),
                                        Location = new Point(10, 20),
                                        FlatStyle = FlatStyle.Flat
                                    };
                                    topPanel.Controls.Add(backBtn);

                                    // Title TextBox
                                    TextBox txtTitle = new TextBox
                                    {
                                        Text = currentTitle,
                                        Font = new Font("Segoe UI", 10),
                                        Width = 180,
                                        Location = new Point(100, 22)
                                    };
                                    topPanel.Controls.Add(txtTitle);

                                    // URL TextBox
                                    TextBox txtUrl = new TextBox
                                    {
                                        Text = currentUrl,
                                        Font = new Font("Segoe UI", 10),
                                        Width = 300,
                                        Location = new Point(290, 22)
                                    };
                                    topPanel.Controls.Add(txtUrl);

                                    // --- WebView2 ---
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
                                        webView.CoreWebView2.Navigate(currentUrl);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Failed to load URL:\n" + ex.Message);
                                    }

                                    // ENTER KEY → save and reload (same as back)
                                    txtUrl.KeyDown += (s2, ev2) =>
                                    {
                                        if (ev2.KeyCode == Keys.Enter)
                                        {
                                            ev2.SuppressKeyPress = true; // prevent the beep

                                            string newTitle = txtTitle.Text.Trim();
                                            string newUrl = txtUrl.Text.Trim();

                                            if (string.IsNullOrWhiteSpace(newUrl))
                                            {
                                                MessageBox.Show("URL cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                return;
                                            }
                                            else
                                            {
                                                MessageBox.Show("The URL has been updated successfully!", "Web Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return;
                                            }
                                            try
                                                {
                                                    using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                                    {
                                                        updateConn.Open();
                                                        string updateQuery = "UPDATE webnote SET Title=@Title, Link=@Link WHERE Id=@Id";
                                                        using (SqlCommand cmd = new SqlCommand(updateQuery, updateConn))
                                                        {
                                                            cmd.Parameters.AddWithValue("@Title", string.IsNullOrWhiteSpace(newTitle) ? "Web Note" : newTitle);
                                                            cmd.Parameters.AddWithValue("@Link", newUrl);
                                                            cmd.Parameters.AddWithValue("@Id", noteId);
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                    }

                                                    AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Updated WebNote: {newTitle}");
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show("Failed to update database:\n" + ex.Message);
                                                }

                                            try { webView.Dispose(); } catch { }

                                            contentPanel.Visible = false;
                                            flowLayoutPanel1.Visible = true;
                                            panel2.Visible = true;

                                            flowLayoutPanel1.Controls.Clear();
                                            LoadNotes();
                                            LoadWebNotes();
                                        }
                                    };

                                    // BACK BUTTON → auto save and reload
                                    backBtn.Click += (s2, ev2) =>
                                    {
                                        string newTitle = txtTitle.Text.Trim();
                                        string newUrl = txtUrl.Text.Trim();

                                        if (string.IsNullOrWhiteSpace(newUrl))
                                        {
                                            MessageBox.Show("URL cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }

                                        try
                                        {
                                            using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                            {
                                                updateConn.Open();
                                                string updateQuery = "UPDATE webnote SET Title=@Title, Link=@Link WHERE Id=@Id";
                                                using (SqlCommand cmd = new SqlCommand(updateQuery, updateConn))
                                                {
                                                    cmd.Parameters.AddWithValue("@Title", string.IsNullOrWhiteSpace(newTitle) ? "Web Note" : newTitle);
                                                    cmd.Parameters.AddWithValue("@Link", newUrl);
                                                    cmd.Parameters.AddWithValue("@Id", noteId);
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }

                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Updated WebNote: {newTitle}");
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Failed to update database:\n" + ex.Message);
                                        }

                                        try { webView.Dispose(); } catch { }

                                        contentPanel.Visible = false;
                                        flowLayoutPanel1.Visible = true;
                                        panel2.Visible = true;

                                        flowLayoutPanel1.Controls.Clear();
                                        LoadNotes();
                                        LoadWebNotes();
                                    };
                                };

                                // RIGHT CLICK → delete
                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                    {
                                        DialogResult confirm = MessageBox.Show(
                                            "Delete this web note?",
                                            "Confirm Delete",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning
                                        );

                                        if (confirm == DialogResult.Yes)
                                        {
                                            try
                                            {
                                                using (SqlConnection delConn = new SqlConnection(AppConfig.ConnectionString))
                                                {
                                                    delConn.Open();
                                                    string deleteQuery = "DELETE FROM webnote WHERE Id=@Id";
                                                    using (SqlCommand delCmd = new SqlCommand(deleteQuery, delConn))
                                                    {
                                                        delCmd.Parameters.AddWithValue("@Id", noteId);
                                                        delCmd.ExecuteNonQuery();
                                                    }
                                                }

                                                AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Deleted WebNote: {currentTitle}");
                                                flowLayoutPanel1.Controls.Clear();
                                                LoadNotes();
                                                LoadWebNotes();
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Failed to delete web note:\n" + ex.Message);
                                            }
                                        }
                                    }
                                };

                                flowLayoutPanel1.Controls.Add(newBtn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load web notes:\n" + ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string title = "Note";
            string note = "";

            int newId;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Notes (AccId, Title, Note, Created, Modified, IsPinned) VALUES (@AccId, @Title, @Note, GETDATE(), GETDATE(), 0); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Note", note);
                        newId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Added new note with title: {title}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save to database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadNotes();
            LoadWebNotes();
        }


        private void LoadNotes()
        {
            try
            {
                flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading

                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT NoteId, Title, Note FROM Notes WHERE AccId = @AccId ORDER BY NoteId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.IsDBNull(1) ? "Note" : reader.GetString(1);
                                string note = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                string currentTitle = title;
                                string currentNote = note;

                                Button newBtn = new Button
                                {
                                    Text = currentTitle,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    BackColor = Color.FromArgb(255, 245, 204),
                                    Tag = id
                                };

                                // LEFT CLICK — open note editor
                                newBtn.Click += (s, ev) =>
                                {
                                    flowLayoutPanel1.Visible = false;
                                    panel2.Visible = false;
                                    contentPanel.Visible = true;
                                    contentPanel.Controls.Clear();

                                    // Back button
                                    Button backBtn = new Button
                                    {
                                        Text = "← Back",
                                        Size = new Size(100, 40),
                                        Location = new Point(10, 10)
                                    };

                                    // Live count label at top-right
                                    Label lblCount = new Label
                                    {
                                        AutoSize = true,
                                        Font = new Font("Segoe UI", 10, FontStyle.Italic),
                                        ForeColor = Color.Gray,
                                        TextAlign = ContentAlignment.MiddleRight,
                                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                                    };

                                    // Title box
                                    TextBox txtTitleBox = new TextBox
                                    {
                                        Text = currentTitle,
                                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                                        Location = new Point(10, 60),
                                        Width = contentPanel.Width - 20,
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                                    };

                                    // Note box
                                    TextBox txtBox = new TextBox
                                    {
                                        Text = currentNote,
                                        Multiline = true,
                                        ScrollBars = ScrollBars.Vertical,
                                        Font = new Font("Segoe UI", 16),
                                        Location = new Point(10, txtTitleBox.Bottom + 10),
                                        Size = new Size(contentPanel.Width - 20, contentPanel.Height - txtTitleBox.Bottom - 20),
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                                    };

                                    // Update count dynamically
                                    txtBox.TextChanged += (s3, ev3) => { lblCount.Text = $"Letters: {txtBox.Text.Length}"; };
                                    lblCount.Text = $"Letters: {txtBox.Text.Length}";

                                    // Keep count label top-right on resize
                                    contentPanel.Resize += (s2, ev2) =>
                                    {
                                        lblCount.Location = new Point(contentPanel.Width - 140, 20);
                                    };

                                    // Save & return to main view
                                    backBtn.Click += (s2, ev2) =>
                                    {
                                        string updatedTitle = txtTitleBox.Text.Trim();
                                        string updatedNote = txtBox.Text.Trim();
                                        if (string.IsNullOrWhiteSpace(updatedTitle)) updatedTitle = "Note";

                                        currentTitle = updatedTitle;
                                        currentNote = updatedNote;
                                        newBtn.Text = currentTitle;

                                        try
                                        {
                                            using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                            {
                                                updateConn.Open();
                                                string updateQuery = "UPDATE Notes SET Title=@Title, Note=@Note, Modified=GETDATE() WHERE NoteId=@Id";
                                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn))
                                                {
                                                    updateCmd.Parameters.AddWithValue("@Title", currentTitle);
                                                    updateCmd.Parameters.AddWithValue("@Note", currentNote);
                                                    updateCmd.Parameters.AddWithValue("@Id", id);
                                                    updateCmd.ExecuteNonQuery();
                                                }
                                            }
                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Updated note: {currentTitle}");
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }

                                        contentPanel.Visible = false;
                                        flowLayoutPanel1.Visible = true;
                                        panel2.Visible = true;
                                    };

                                    // Add controls
                                    contentPanel.Controls.Add(backBtn);
                                    contentPanel.Controls.Add(lblCount);
                                    contentPanel.Controls.Add(txtTitleBox);
                                    contentPanel.Controls.Add(txtBox);

                                    // Position label after adding
                                    lblCount.Location = new Point(contentPanel.Width - 140, 20);
                                };

                                // RIGHT CLICK — delete note
                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                    {
                                        DialogResult confirm = MessageBox.Show(
                                            "Are you sure you want to delete this note?",
                                            "Delete Note",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning
                                        );

                                        if (confirm == DialogResult.Yes)
                                        {
                                            try
                                            {
                                                using (SqlConnection delConn = new SqlConnection(AppConfig.ConnectionString))
                                                {
                                                    delConn.Open();
                                                    string deleteQuery = "DELETE FROM Notes WHERE NoteId = @Id";
                                                    using (SqlCommand delCmd = new SqlCommand(deleteQuery, delConn))
                                                    {
                                                        delCmd.Parameters.AddWithValue("@Id", id);
                                                        delCmd.ExecuteNonQuery();
                                                    }
                                                }

                                                AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Deleted note: {currentTitle}");
                                                LoadNotes();
                                                LoadWebNotes();
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Failed to delete note:\n" + ex.Message,
                                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                };

                                flowLayoutPanel1.Controls.Add(newBtn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load notes:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
