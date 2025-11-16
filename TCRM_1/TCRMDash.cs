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

            flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
            LoadAllSorted();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit(); // ensures everything ends
        }

        // Class-level reference
        private Options optionsForm = null;

        private void button4_Click(object sender, EventArgs e)
        {
            // Close the old instance if it exists
            if (optionsForm != null && !optionsForm.IsDisposed)
            {
                optionsForm.Close();
                optionsForm = null;
            }

            // Create a new instance
            optionsForm = new Options(this);

            // Position it near the button
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
                        flowLayoutPanel1.Controls.Clear();
                        LoadAllSorted();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save to database:\n" + ex.Message);
                    }
                }
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
            flowLayoutPanel1.Controls.Clear();
            LoadAllSorted();
        }

        private void LoadWebNotes()
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Link FROM webnote WHERE AccId = @AccId AND IsArchived = 0 ORDER BY Id";
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
                                            LoadAllSorted();
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

                                        flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
                                        LoadNotes();
                                        LoadWebNotes();
                                    };
                                };

                                // RIGHT CLICK → Delete + Archive
                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                    {
                                        ContextMenuStrip cms = new ContextMenuStrip();
                                        cms.Items.Add("Delete").Name = "Delete";
                                        cms.Items.Add("Archive").Name = "Archive";

                                        cms.ItemClicked += (s2, ev2) =>
                                        {
                                            if (ev2.ClickedItem.Name == "Delete")
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
                                                        LoadNotes();
                                                        LoadWebNotes();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show("Failed to delete web note:\n" + ex.Message);
                                                    }
                                                }
                                            }
                                            else if (ev2.ClickedItem.Name == "Archive")
                                            {
                                                UpdateArchiveStatus(noteId, "WebNote", true);
                                            }
                                        };

                                        cms.Show(newBtn, ev.Location);
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
        private void LoadNotes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT NoteId, Title, Note FROM Notes WHERE AccId = @AccId AND IsArchived = 0 ORDER BY NoteId";
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
                                        ContextMenuStrip cms = new ContextMenuStrip();
                                        cms.Items.Add("Delete").Name = "Delete";
                                        cms.Items.Add("Archive").Name = "Archive";

                                        cms.ItemClicked += (s2, ev2) =>
                                        {
                                            if (ev2.ClickedItem.Name == "Delete")
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
                                                        flowLayoutPanel1.Controls.Clear();
                                                        LoadNotes();
                                                        LoadWebNotes();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show("Failed to delete note:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
                                            }
                                            else if (ev2.ClickedItem.Name == "Archive")
                                            {
                                                UpdateArchiveStatus(id, "Note", true);
                                            }
                                        };

                                        cms.Show(newBtn, ev.Location);
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
        private void label1_Click(object sender, EventArgs e)
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
            LoadNotes();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
            LoadWebNotes();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
            LoadAllSorted();
        }

        public void LoadAllSorted()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT NoteId AS Id, Title, Note AS Content, Created, 'note' AS Type
                    FROM Notes
                    WHERE AccId = @AccId AND IsArchived = 0
                    UNION ALL
                    SELECT Id, Title, Link AS Content, Created, 'web' AS Type
                    FROM webnote
                    WHERE AccId = @AccId AND IsArchived = 0
                    ORDER BY Created DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            flowLayoutPanel1.Controls.Clear();

                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.IsDBNull(1) ? "Untitled" : reader.GetString(1);
                                string content = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                string type = reader.GetString(4);

                                Button newBtn = new Button
                                {
                                    Text = title,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = new { Id = id, Type = type, Title = title, Content = content },
                                    BackColor = type == "note"
                                        ? Color.FromArgb(255, 245, 204)
                                        : Color.FromArgb(204, 229, 255),
                                    TextAlign = ContentAlignment.MiddleCenter
                                };

                                // LEFT CLICK → open editor
                                if (type == "note")
                                    AttachNoteHandlersClean(newBtn, id, title, content);
                                else
                                    AttachWebNoteHandlersClean(newBtn, id, title, content);

                                // RIGHT CLICK → context menu (delete / archive)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Delete").Name = "Delete";
                                cms.Items.Add("Archive").Name = "Archive";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Delete")
                                    {
                                        if (type == "note")
                                            DeleteNoteDialog(id, title);
                                        else
                                            DeleteWebNoteDialog(id, title);
                                    }
                                    else if (ev2.ClickedItem.Name == "Archive")
                                    {
                                        UpdateArchiveStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Archived {type}: {title}");
                                        LoadAllSorted();
                                    }
                                };

                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                        cms.Show(newBtn, ev.Location); 
                                };

                                flowLayoutPanel1.Controls.Add(newBtn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load sorted notes:\n" + ex.Message);
            }
        }
        private void UpdateArchiveStatus(int id, string type, bool isArchived)
        {
            string query = type == "note" ? "UPDATE Notes SET IsArchived=@IsArchived WHERE NoteId=@Id" :
                                             "UPDATE webnote SET IsArchived=@IsArchived WHERE Id=@Id";

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
                flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
                LoadAllSorted(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update archive status:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AttachNoteHandlersClean(Button btn, int id, string title, string content)
        {
            btn.Click += (s, ev) =>
            {
                OpenNoteEditor(id, btn, title, content);
            };
        }
        private void OpenNoteEditor(int id, Button btn, string title, string content)
        {
            flowLayoutPanel1.Visible = false;
            panel2.Visible = false;
            contentPanel.Controls.Clear();
            contentPanel.Visible = true;

            // --- Back Button ---
            Button backBtn = new Button
            {
                Text = "← Back",
                Size = new Size(100, 40),
                Location = new Point(10, 10)
            };

            // --- Letter Count Label ---
            Label lblCount = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // --- Title TextBox ---
            TextBox txtTitle = new TextBox
            {
                Text = title,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 60),
                Width = contentPanel.Width - 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // --- Note TextBox ---
            TextBox txtBox = new TextBox
            {
                Text = content,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 16),
                Location = new Point(10, txtTitle.Bottom + 10),
                Size = new Size(contentPanel.Width - 20, contentPanel.Height - txtTitle.Bottom - 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Update letter count dynamically
            txtBox.TextChanged += (s, ev) => { lblCount.Text = $"Letters: {txtBox.Text.Length}"; };
            lblCount.Text = $"Letters: {txtBox.Text.Length}";

            // Keep count label top-right on resize
            contentPanel.Resize += (s, ev) =>
            {
                lblCount.Location = new Point(contentPanel.Width - 140, 20);
            };

            // --- Back Button Click: Save and return ---
            backBtn.Click += (s, ev) =>
            {
                string newTitle = txtTitle.Text.Trim();
                string newContent = txtBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(newTitle)) newTitle = "Note";

                btn.Text = newTitle;

                try
                {
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Notes SET Title=@Title, Note=@Note, Modified=GETDATE() WHERE NoteId=@Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Title", newTitle);
                            cmd.Parameters.AddWithValue("@Note", newContent);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Updated note: {newTitle}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                contentPanel.Visible = false;
                flowLayoutPanel1.Visible = true;
                panel2.Visible = true;
            };

            // Add controls to content panel
            contentPanel.Controls.Add(backBtn);
            contentPanel.Controls.Add(lblCount);
            contentPanel.Controls.Add(txtTitle);
            contentPanel.Controls.Add(txtBox);

            // Position count label after adding controls
            lblCount.Location = new Point(contentPanel.Width - 140, 20);
        }
        private void AttachWebNoteHandlersClean(Button btn, int id, string title, string url)
        {
            btn.Click += async (s, ev) =>
            {
                await OpenWebNoteEditor(id, btn, title, url);
            };
        }
        private async Task OpenWebNoteEditor(int id, Button btn, string title, string url)
        {
            flowLayoutPanel1.Visible = false;
            panel2.Visible = false;
            contentPanel.Controls.Clear();
            contentPanel.Visible = true;

            // --- Top Panel ---
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(102, 102, 204)
            };
            contentPanel.Controls.Add(topPanel);

            // --- Back Button ---
            Button backBtn = new Button
            {
                Text = "← Back",
                Size = new Size(80, 30),
                Location = new Point(10, 20),
                FlatStyle = FlatStyle.Flat
            };
            topPanel.Controls.Add(backBtn);

            // --- Title TextBox ---
            TextBox txtTitle = new TextBox
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                Width = 180,
                Location = new Point(100, 22)
            };
            topPanel.Controls.Add(txtTitle);

            // --- URL TextBox ---
            TextBox txtUrl = new TextBox
            {
                Text = url,
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
                webView.CoreWebView2.Navigate(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load URL:\n" + ex.Message);
            }

            // --- Save & Return Logic ---
            Action saveAndReturn = () =>
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
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        string updateQuery = "UPDATE webnote SET Title=@Title, Link=@Link WHERE Id=@Id";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Title", string.IsNullOrWhiteSpace(newTitle) ? "Web Note" : newTitle);
                            cmd.Parameters.AddWithValue("@Link", newUrl);
                            cmd.Parameters.AddWithValue("@Id", id);
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
                LoadAllSorted(); // Use combined sorted load for both Notes & WebNotes
            };

            // --- ENTER Key → Save & Return ---
            txtUrl.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    ev.SuppressKeyPress = true; // prevent beep
                    saveAndReturn();
                }
            };

            // --- Back Button Click ---
            backBtn.Click += (s, ev) => saveAndReturn();
        }

        // Delete helpers
        private void DeleteNoteDialog(int id, string title)
        {
            if (MessageBox.Show($"Are you sure you want to delete this note?", "Delete Note", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Notes WHERE NoteId=@Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Log deletion
                    AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Deleted note: {title}");

                    flowLayoutPanel1.Controls.Clear();
                    LoadAllSorted();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete note:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteWebNoteDialog(int id, string title)
        {
            if (MessageBox.Show($"Delete this web note?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM webnote WHERE Id=@Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Log deletion
                    AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Deleted WebNote: {title}");

                    flowLayoutPanel1.Controls.Clear();
                    LoadAllSorted();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete web note:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void optionButton1(object sender, EventArgs e)
        {

        }
    }
}
