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
            LoadWebNotes();
            LoadNotes();
        }
        public void RefreshUsername()
        {
            label1.Text = "Welcome, " + AppConfig.CurrentUsername;
        }
        private async void TCRM_2_Load(object sender, EventArgs e)
        {
            // await webView21.EnsureCoreWebView2Async(null);
            // webView21.CoreWebView2.Navigate("https://www.youtube.com");

            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Visible = false;  // hidden 
            Controls.Add(contentPanel);
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
                    string title = urlForm.EnteredTitle;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        MessageBox.Show("URL cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        title = "Web Note"; // Default if empty
                    }

                    // Insert into database
                    int newId;
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                        {
                            conn.Open();
                            string query = "INSERT INTO webnote (AccId, Title, Link) VALUES (@AccId, @Title, @Link); SELECT SCOPE_IDENTITY();";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);
                                cmd.Parameters.AddWithValue("@Title", title);
                                cmd.Parameters.AddWithValue("@Link", url);
                                newId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        // Log successful addition
                        AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Added new web note with title: {title}, URL: {url}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save to database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string currentUrl = url;
                    string currentTitle = title;

                    Button newBtn = new Button
                    {
                        Text = currentTitle,
                        Size = new Size(672, 59),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Tag = newId
                    };

                    // Left-click: open WebView (unchanged)
                    newBtn.Click += async (s, ev) =>
                    {
                        if (string.IsNullOrWhiteSpace(currentUrl))
                        {
                            MessageBox.Show("No URL set for this button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        flowLayoutPanel1.Visible = false;
                        panel2.Visible = false;
                        contentPanel.Controls.Clear();
                        contentPanel.Visible = true;

                        // Top panel with back button
                        Panel topPanel = new Panel
                        {
                            Dock = DockStyle.Top,
                            Height = 50,
                            BackColor = Color.Transparent
                        };
                        contentPanel.Controls.Add(topPanel);

                        Button backBtn = new Button
                        {
                            Text = "← Back",
                            Size = new Size(100, 30),
                            Location = new Point(10, 10)
                        };
                        topPanel.Controls.Add(backBtn);

                        // WebView2 fills remaining space
                        var webView21 = new Microsoft.Web.WebView2.WinForms.WebView2
                        {
                            Dock = DockStyle.Fill
                        };
                        contentPanel.Controls.Add(webView21);
                        topPanel.SendToBack();

                        backBtn.Click += (s2, ev2) =>
                        {
                            try { webView21.Dispose(); } catch { }
                            contentPanel.Visible = false;
                            flowLayoutPanel1.Visible = true;
                            panel2.Visible = true;
                        };

                        try
                        {
                            await webView21.EnsureCoreWebView2Async(null);
                            webView21.CoreWebView2.Navigate(currentUrl);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to load URL:\n" + ex.Message, "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            contentPanel.Visible = false;
                            flowLayoutPanel1.Visible = true;
                            panel2.Visible = true;
                        }

                    };

                    // Right-click: change URL and Title
                    newBtn.MouseUp += (s, ev) =>
                    {
                        if (ev.Button == MouseButtons.Right)
                        {
                            using (var changeUrlForm = new UrlInputForm())
                            {
                                changeUrlForm.EnteredTitle = currentTitle; // Pre-set current title
                                changeUrlForm.EnteredUrl = currentUrl; // Pre-set current URL
                                if (changeUrlForm.ShowDialog(this) == DialogResult.OK)
                                {
                                    string newTitle = changeUrlForm.EnteredTitle;
                                    string newUrl = changeUrlForm.EnteredUrl;
                                    if (!string.IsNullOrWhiteSpace(newUrl))
                                    {
                                        if (string.IsNullOrWhiteSpace(newTitle))
                                        {
                                            newTitle = "Web Note"; // Default if empty
                                        }
                                        currentTitle = newTitle;
                                        currentUrl = newUrl;
                                        newBtn.Text = currentTitle; // Update button text
                                                                    // Update DB
                                        int id = (int)newBtn.Tag;
                                        try
                                        {
                                            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                                            {
                                                conn.Open();
                                                string query = "UPDATE webnote SET Title = @Title, Link = @Link WHERE Id = @Id";
                                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@Title", currentTitle);
                                                    cmd.Parameters.AddWithValue("@Link", currentUrl);
                                                    cmd.Parameters.AddWithValue("@Id", id);
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                            // Log successful update
                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Updated web note to title: {currentTitle}, URL: {currentUrl}");
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                            }
                        }
                    };

                    flowLayoutPanel1.Controls.Add(newBtn);
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
                                int id = reader.GetInt32(0);
                                string title = reader.IsDBNull(1) ? "Web Note" : reader.GetString(1);
                                string link = reader.GetString(2);

                                string currentUrl = link;
                                string currentTitle = title;

                                Button newBtn = new Button
                                {
                                    Text = currentTitle,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = id
                                };

                                // Left-click: open WebView
                                newBtn.Click += async (s, ev) =>
                                {
                                    if (string.IsNullOrWhiteSpace(currentUrl))
                                    {
                                        MessageBox.Show("No URL set for this button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

                                    flowLayoutPanel1.Visible = false;
                                    panel2.Visible = false;
                                    contentPanel.Controls.Clear();
                                    contentPanel.Visible = true;

                                    // Top panel with back button
                                    Panel topPanel = new Panel
                                    {
                                        Dock = DockStyle.Top,
                                        Height = 50,
                                        BackColor = Color.Transparent
                                    };
                                    contentPanel.Controls.Add(topPanel);

                                    Button backBtn = new Button
                                    {
                                        Text = "← Back",
                                        Size = new Size(100, 30),
                                        Location = new Point(10, 10)
                                    };
                                    topPanel.Controls.Add(backBtn);

                                    // WebView2 fills remaining space
                                    var webView21 = new Microsoft.Web.WebView2.WinForms.WebView2
                                    {
                                        Dock = DockStyle.Fill
                                    };
                                    contentPanel.Controls.Add(webView21);
                                    topPanel.SendToBack();

                                    backBtn.Click += (s2, ev2) =>
                                    {
                                        try { webView21.Dispose(); } catch { }
                                        contentPanel.Visible = false;
                                        flowLayoutPanel1.Visible = true;
                                        panel2.Visible = true;
                                    };

                                    try
                                    {
                                        await webView21.EnsureCoreWebView2Async(null);
                                        webView21.CoreWebView2.Navigate(currentUrl);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Failed to load URL:\n" + ex.Message, "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        contentPanel.Visible = false;
                                        flowLayoutPanel1.Visible = true;
                                        panel2.Visible = true;
                                    }
                                };

                                // Right-click: change Title and URL
                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                    {
                                        using (var changeUrlForm = new UrlInputForm())
                                        {
                                            changeUrlForm.EnteredTitle = currentTitle; // Pre-set current title
                                            changeUrlForm.EnteredUrl = currentUrl; // Pre-set current URL
                                            if (changeUrlForm.ShowDialog(this) == DialogResult.OK)
                                            {
                                                string newTitle = changeUrlForm.EnteredTitle;
                                                string newUrl = changeUrlForm.EnteredUrl;
                                                if (!string.IsNullOrWhiteSpace(newUrl))
                                                {
                                                    if (string.IsNullOrWhiteSpace(newTitle))
                                                    {
                                                        newTitle = "Web Note"; // Default if empty
                                                    }
                                                    currentTitle = newTitle;
                                                    currentUrl = newUrl;
                                                    newBtn.Text = currentTitle; // Update button text
                                                                                // Update DB
                                                    int btnId = (int)newBtn.Tag;
                                                    try
                                                    {
                                                        using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                                        {
                                                            updateConn.Open();
                                                            string updateQuery = "UPDATE webnote SET Title = @Title, Link = @Link WHERE Id = @Id";
                                                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn))
                                                            {
                                                                updateCmd.Parameters.AddWithValue("@Title", currentTitle);
                                                                updateCmd.Parameters.AddWithValue("@Link", currentUrl);
                                                                updateCmd.Parameters.AddWithValue("@Id", btnId);
                                                                updateCmd.ExecuteNonQuery();
                                                            }
                                                        }
                                                        // Log successful update
                                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Updated web note to title: {currentTitle}, URL: {currentUrl}");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
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
                MessageBox.Show("Failed to load web notes:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            // Directly create with defaults, no modal
            string title = "Note"; // Default title
            string note = ""; // Empty note

            // Insert into database
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
                // Log successful addition
                AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Added new note with title: {title}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save to database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string currentTitle = title;
            string currentNote = note;

            Button newBtn = new Button
            {
                Text = currentTitle,
                Size = new Size(672, 59),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Tag = newId // Store DB ID
            };

            newBtn.Click += (s, ev) =>
            {
                // Hide main panel
                flowLayoutPanel1.Visible = false;
                panel2.Visible = false;
                contentPanel.Visible = true;
                contentPanel.Controls.Clear();

                // Title TextBox (changed from Label)
                TextBox txtTitleBox = new TextBox
                {
                    Text = currentTitle,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Location = new Point(10, 60),
                    Width = contentPanel.Width - 20,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                // Big multiline TextBox for note
                TextBox txtBox = new TextBox
                {
                    Text = currentNote,
                    Multiline = true,
                    Location = new Point(10, txtTitleBox.Bottom + 10),
                    Size = new Size(contentPanel.Width - 20, contentPanel.Height - txtTitleBox.Bottom - 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Segoe UI", 16, FontStyle.Regular)
                };

                // Back button
                Button backBtn = new Button
                {
                    Text = "← Back",
                    Size = new Size(100, 40),
                    Location = new Point(10, 10)
                };
                backBtn.Click += (s2, ev2) =>
                {
                    // Save changes on back
                    string updatedTitle = txtTitleBox.Text.Trim();
                    string updatedNote = txtBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(updatedTitle))
                    {
                        updatedTitle = "Note";
                    }
                    currentTitle = updatedTitle;
                    currentNote = updatedNote;
                    newBtn.Text = currentTitle; // Update button text
                                                // Update DB
                    int btnId = (int)newBtn.Tag;
                    try
                    {
                        using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                        {
                            updateConn.Open();
                            string updateQuery = "UPDATE Notes SET Title = @Title, Note = @Note, Modified = GETDATE() WHERE NoteId = @Id";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn))
                            {
                                updateCmd.Parameters.AddWithValue("@Title", currentTitle);
                                updateCmd.Parameters.AddWithValue("@Note", currentNote);
                                updateCmd.Parameters.AddWithValue("@Id", btnId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        // Log successful update
                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Updated note to title: {currentTitle}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Go back to main view
                    contentPanel.Visible = false;
                    flowLayoutPanel1.Visible = true;
                    panel2.Visible = true;
                };

                // Add controls
                contentPanel.Controls.Add(backBtn);
                contentPanel.Controls.Add(txtTitleBox);
                contentPanel.Controls.Add(txtBox);
            };

            // Right-click: removed modal, now does nothing (or you can add other functionality if needed)
            newBtn.MouseUp += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    // No action, or add something else like delete, pin, etc.
                }
            };

            // Add the button to the panel
            flowLayoutPanel1.Controls.Add(newBtn);

            // Immediately open the note editor (simulate click)
            newBtn.PerformClick();
        }

        private void LoadNotes()
        {
            try
            {
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
                                string note = reader.GetString(2);

                                string currentTitle = title;
                                string currentNote = note;

                                Button newBtn = new Button
                                {
                                    Text = currentTitle,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = id
                                };

                                newBtn.Click += (s, ev) =>
                                {
                                    // Hide main panel
                                    flowLayoutPanel1.Visible = false;
                                    panel2.Visible = false;
                                    contentPanel.Visible = true;
                                    contentPanel.Controls.Clear();

                                    // Title TextBox
                                    TextBox txtTitleBox = new TextBox
                                    {
                                        Text = currentTitle,
                                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                                        Location = new Point(10, 60),
                                        Width = contentPanel.Width - 20,
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                                    };

                                    // Big multiline TextBox for note
                                    TextBox txtBox = new TextBox
                                    {
                                        Text = currentNote,
                                        Multiline = true,
                                        Location = new Point(10, txtTitleBox.Bottom + 10),
                                        Size = new Size(contentPanel.Width - 20, contentPanel.Height - txtTitleBox.Bottom - 20),
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                                        ScrollBars = ScrollBars.Vertical,
                                        Font = new Font("Segoe UI", 16, FontStyle.Regular)
                                    };

                                    // Back button
                                    Button backBtn = new Button
                                    {
                                        Text = "← Back",
                                        Size = new Size(100, 40),
                                        Location = new Point(10, 10)
                                    };
                                    backBtn.Click += (s2, ev2) =>
                                    {
                                        // Save changes on back
                                        string updatedTitle = txtTitleBox.Text.Trim();
                                        string updatedNote = txtBox.Text.Trim();
                                        if (string.IsNullOrWhiteSpace(updatedTitle))
                                        {
                                            updatedTitle = "Note";
                                        }
                                        currentTitle = updatedTitle;
                                        currentNote = updatedNote;
                                        newBtn.Text = currentTitle;
                                        // Update DB
                                        int btnId = (int)newBtn.Tag;
                                        try
                                        {
                                            using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                            {
                                                updateConn.Open();
                                                string updateQuery = "UPDATE Notes SET Title = @Title, Note = @Note, Modified = GETDATE() WHERE NoteId = @Id";
                                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn))
                                                {
                                                    updateCmd.Parameters.AddWithValue("@Title", currentTitle);
                                                    updateCmd.Parameters.AddWithValue("@Note", currentNote);
                                                    updateCmd.Parameters.AddWithValue("@Id", btnId);
                                                    updateCmd.ExecuteNonQuery();
                                                }
                                            }
                                            // Log successful update
                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Updated note to title: {currentTitle}");
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        // Go back to main view
                                        contentPanel.Visible = false;
                                        flowLayoutPanel1.Visible = true;
                                        panel2.Visible = true;
                                    };



                                    // Add controls
                                    contentPanel.Controls.Add(backBtn);
                                    contentPanel.Controls.Add(txtTitleBox);
                                    contentPanel.Controls.Add(txtBox);
                                };

                                // Right-click: edit Title and Note
                                newBtn.MouseUp += (s, ev) =>
                                {
                                    if (ev.Button == MouseButtons.Right)
                                    {
                                        using (var editNoteForm = new NoteInputForm())
                                        {
                                            editNoteForm.EnteredTitle = currentTitle;
                                            editNoteForm.EnteredNote = currentNote;
                                            if (editNoteForm.ShowDialog(this) == DialogResult.OK)
                                            {
                                                string newTitle = editNoteForm.EnteredTitle;
                                                string newNote = editNoteForm.EnteredNote;
                                                if (!string.IsNullOrWhiteSpace(newNote))
                                                {
                                                    if (string.IsNullOrWhiteSpace(newTitle))
                                                    {
                                                        newTitle = "Note";
                                                    }
                                                    currentTitle = newTitle;
                                                    currentNote = newNote;
                                                    newBtn.Text = currentTitle;
                                                    // Update DB
                                                    int btnId = (int)newBtn.Tag;
                                                    try
                                                    {
                                                        using (SqlConnection updateConn = new SqlConnection(AppConfig.ConnectionString))
                                                        {
                                                            updateConn.Open();
                                                            string updateQuery = "UPDATE Notes SET Title = @Title, Note = @Note, Modified = GETDATE() WHERE NoteId = @Id";
                                                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn))
                                                            {
                                                                updateCmd.Parameters.AddWithValue("@Title", currentTitle);
                                                                updateCmd.Parameters.AddWithValue("@Note", currentNote);
                                                                updateCmd.Parameters.AddWithValue("@Id", btnId);
                                                                updateCmd.ExecuteNonQuery();
                                                            }
                                                        }
                                                        // Log successful update
                                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Note", $"Updated note to title: {currentTitle}");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show("Failed to update database:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
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
