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

        // Option button
        private void button4_Click(object sender, EventArgs e)
        {
            Options optionsForm = null;
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
        // Webnote add
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
        // Note add
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

        // Useless methods but dont delete
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void flowLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        // Category buttons
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

        // Load methods
        private void LoadWebNotes()
        {
            try
            {
                // Pinned
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                Id,
                Title,
                Link AS Content,
                Created,
                ReminderDateTime,
                'web' AS Type
            FROM webnote
            WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 1
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

                                // ReminderDateTime is at index 4
                                string reminder = "(yyyy-MM-dd HH:mm)";
                                if (!reader.IsDBNull(4))
                                    reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                // Type is at index 5
                                string type = reader.GetString(5);

                                Color buttonColor = Color.FromArgb(142, 170, 252); // Default for webnotes

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }

                                Button newBtn = new Button
                                {
                                    Text = title,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                    BackColor = buttonColor,
                                    TextAlign = ContentAlignment.MiddleCenter
                                };

                                // LEFT CLICK → open editor with reminder
                                AttachWebNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadWebNotes());

                                // RIGHT CLICK → context menu (delete / archive / pinned)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Delete").Name = "Delete";
                                cms.Items.Add("Archive").Name = "Archive";
                                cms.Items.Add("Pinned").Name = "Pinned";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Delete")
                                    {
                                        DeleteWebNoteDialog(id, title);
                                    }
                                    else if (ev2.ClickedItem.Name == "Archive")
                                    {
                                        UpdateArchiveStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Archived {type}: {title}");
                                        LoadWebNotes();
                                    }
                                    else if (ev2.ClickedItem.Name == "Pinned")
                                    {
                                        UpdatePinnedStatus(id, type, false);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Pinned", $"Unpinned {type}: {title}");
                                        LoadWebNotes();
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

                // Not Pinned
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                Id,
                Title,
                Link AS Content,
                Created,
                ReminderDateTime,
                'web' AS Type
            FROM webnote
            WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 0
            ORDER BY Created DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.IsDBNull(1) ? "Untitled" : reader.GetString(1);
                                string content = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                // ReminderDateTime is at index 4
                                string reminder = "Reminder (yyyy-MM-dd HH:mm)";
                                if (!reader.IsDBNull(4))
                                    reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                // Type is at index 5
                                string type = reader.GetString(5);

                                Color buttonColor = Color.FromArgb(142, 170, 252); // Default for webnotes

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }

                                Button newBtn = new Button
                                {
                                    Text = title,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                    BackColor = buttonColor,
                                    TextAlign = ContentAlignment.MiddleCenter
                                };

                                AttachWebNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadWebNotes());

                                // RIGHT CLICK → context menu (delete / archive / pin)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Delete").Name = "Delete";
                                cms.Items.Add("Archive").Name = "Archive";
                                cms.Items.Add("Pin").Name = "Pin";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Delete")
                                    {
                                        DeleteWebNoteDialog(id, title);
                                    }
                                    else if (ev2.ClickedItem.Name == "Archive")
                                    {
                                        UpdateArchiveStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Archived {type}: {title}");
                                        LoadWebNotes();
                                    }
                                    else if (ev2.ClickedItem.Name == "Pin")
                                    {
                                        UpdatePinnedStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Pin", $"Pinned {type}: {title}");
                                        LoadWebNotes();
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
                MessageBox.Show("Failed to load web notes:\n" + ex.Message);
            }
        }

        private void LoadNotes()
        {
            try
            {
                // Pinned
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                NoteId AS Id,
                Title,
                Note AS Content,
                Created,
                ReminderDateTime,
                'note' AS Type
            FROM Notes
            WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 1
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

                                // ReminderDateTime is at index 4
                                string reminder = "Reminder (yyyy-MM-dd HH:mm)";
                                if (!reader.IsDBNull(4))
                                    reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                // Type is at index 5
                                string type = reader.GetString(5);

                                Color buttonColor = Color.FromArgb(137, 255, 89); // Default for notes

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }

                                Button newBtn = new Button
                                {
                                    Text = title,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                    BackColor = buttonColor,
                                    TextAlign = ContentAlignment.MiddleCenter
                                };

                                // LEFT CLICK → open editor with reminder
                                AttachNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadNotes());

                                // RIGHT CLICK → context menu (delete / archive / pinned)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Delete").Name = "Delete";
                                cms.Items.Add("Archive").Name = "Archive";
                                cms.Items.Add("Pinned").Name = "Pinned";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Delete")
                                    {
                                        DeleteNoteDialog(id, title);
                                    }
                                    else if (ev2.ClickedItem.Name == "Archive")
                                    {
                                        UpdateArchiveStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Archived {type}: {title}");
                                        LoadNotes();
                                    }
                                    else if (ev2.ClickedItem.Name == "Pinned")
                                    {
                                        UpdatePinnedStatus(id, type, false);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Pinned", $"Unpinned {type}: {title}");
                                        LoadNotes();
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

                // Not Pinned
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                NoteId AS Id,
                Title,
                Note AS Content,
                Created,
                ReminderDateTime,
                'note' AS Type
            FROM Notes
            WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 0
            ORDER BY Created DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.IsDBNull(1) ? "Untitled" : reader.GetString(1);
                                string content = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                // ReminderDateTime is at index 4
                                string reminder = "(yyyy-MM-dd HH:mm)";
                                if (!reader.IsDBNull(4))
                                    reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                // Type is at index 5
                                string type = reader.GetString(5);

                                Color buttonColor = Color.FromArgb(137, 255, 89); // Default for notes

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }

                                Button newBtn = new Button
                                {
                                    Text = title,
                                    Size = new Size(672, 59),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                    Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                    BackColor = buttonColor,
                                    TextAlign = ContentAlignment.MiddleCenter
                                };

                                AttachNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadNotes());

                                // RIGHT CLICK → context menu (delete / archive / pin)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                cms.Items.Add("Delete").Name = "Delete";
                                cms.Items.Add("Archive").Name = "Archive";
                                cms.Items.Add("Pin").Name = "Pin";

                                cms.ItemClicked += (s2, ev2) =>
                                {
                                    if (ev2.ClickedItem.Name == "Delete")
                                    {
                                        DeleteNoteDialog(id, title);
                                    }
                                    else if (ev2.ClickedItem.Name == "Archive")
                                    {
                                        UpdateArchiveStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"Archived {type}: {title}");
                                        LoadNotes();
                                    }
                                    else if (ev2.ClickedItem.Name == "Pin")
                                    {
                                        UpdatePinnedStatus(id, type, true);
                                        AppConfig.LogEvent(AppConfig.CurrentAccId, "Pin", $"Pinned {type}: {title}");
                                        LoadNotes();
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
                MessageBox.Show("Failed to load notes:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadAllSorted()
            {
                try
                {
                    //Pinned
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();

                        string query = @"
                        SELECT 
                            NoteId AS Id,
                            Title,
                            Note AS Content,
                            Created,
                            ReminderDateTime,
                            'note' AS Type
                        FROM Notes
                        WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 1
                        UNION ALL
                        SELECT 
                            Id,
                            Title,
                            Link AS Content,
                            Created,
                            ReminderDateTime,
                            'web' AS Type
                        FROM webnote
                        WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 1
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

                                    // ReminderDateTime is at index 4
                                    string reminder = "(yyyy-MM-dd HH:mm)";
                                    if (!reader.IsDBNull(4))
                                        reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                    // Type is at index 5
                                    string type = reader.GetString(5);

                                Color buttonColor;

                                // Default color depending on type
                                if (type == "note")
                                    buttonColor = Color.FromArgb(137, 255, 89);
                                else
                                    buttonColor = Color.FromArgb(142, 170, 252);

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }
                                Button newBtn = new Button
                                    {
                                        Text = title,
                                        Size = new Size(672, 59),
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                        Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                        BackColor = buttonColor,
                                        TextAlign = ContentAlignment.MiddleCenter
                                    };
                                    

                                    // LEFT CLICK → open editor with reminder
                                    if (type == "note")
                                        AttachNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadAllSorted());
                                    else
                                        AttachWebNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadAllSorted());
                            
                                // RIGHT CLICK → context menu (delete / archive / pinned)
                                ContextMenuStrip cms = new ContextMenuStrip();
                                    cms.Items.Add("Delete").Name = "Delete";
                                    cms.Items.Add("Archive").Name = "Archive";
                                    cms.Items.Add("Pinned").Name = "Pinned";

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
                                        else if (ev2.ClickedItem.Name == "Pinned")
                                        {
                                            UpdatePinnedStatus(id, type, false);
                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "Pinned", $"Pinned {type}: {title}");
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
                    // Not Pinned
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();

                        string query = @"
                        SELECT 
                            NoteId AS Id,
                            Title,
                            Note AS Content,
                            Created,
                            ReminderDateTime,
                            'note' AS Type
                        FROM Notes
                        WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 0
                        UNION ALL
                        SELECT 
                            Id,
                            Title,
                            Link AS Content,
                            Created,
                            ReminderDateTime,
                            'web' AS Type
                        FROM webnote
                        WHERE AccId = @AccId AND IsArchived = 0 AND IsPinned = 0
                        ORDER BY Created DESC";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccId", AppConfig.CurrentAccId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    string title = reader.IsDBNull(1) ? "Untitled" : reader.GetString(1);
                                    string content = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                    // ReminderDateTime is at index 4
                                    string reminder = "Reminder (yyyy-MM-dd HH:mm)";
                                    if (!reader.IsDBNull(4))
                                        reminder = ((DateTime)reader.GetValue(4)).ToString("yyyy-MM-dd HH:mm");

                                    // Type is at index 5
                                    string type = reader.GetString(5);

                                Color buttonColor;

                                // Default color depending on type
                                if (type == "note")
                                    buttonColor = Color.FromArgb(137, 255, 89);
                                else
                                    buttonColor = Color.FromArgb(142, 170, 252);

                                // Change color based on reminder urgency
                                if (DateTime.TryParse(reminder, out DateTime reminderDate))
                                {
                                    TimeSpan diff = reminderDate - DateTime.Now;

                                    if (diff.TotalHours <= 12)            // Less than 12 hours
                                        buttonColor = Color.Red;
                                    else if (diff.TotalDays <= 2)          // Within 2 days
                                        buttonColor = Color.Orange;
                                }
                                Button newBtn = new Button
                                    {
                                        Text = title,
                                        Size = new Size(672, 59),
                                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                                        Tag = new { Id = id, Type = type, Title = title, Content = content, Reminder = reminder },
                                        BackColor = buttonColor,
                                        TextAlign = ContentAlignment.MiddleCenter
                                    };

                                    if (type == "note")
                                        AttachNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadAllSorted());
                                    else
                                        AttachWebNoteHandlersClean(newBtn, id, title, content, reminder, () => LoadAllSorted());
                                    // RIGHT CLICK → context menu (delete / archive)
                                    ContextMenuStrip cms = new ContextMenuStrip();
                                    cms.Items.Add("Delete").Name = "Delete";
                                    cms.Items.Add("Archive").Name = "Archive";
                                    cms.Items.Add("Pin").Name = "Pin";

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
                                        else if (ev2.ClickedItem.Name == "Pin")
                                        {
                                            UpdatePinnedStatus(id, type, true);
                                            AppConfig.LogEvent(AppConfig.CurrentAccId, "Pin", $"Pinned {type}: {title}");
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

        // Update and UI methods
        private void UpdatePinnedStatus(int id, string type, bool IsPinned)
        {
            string query = type == "note" ? "UPDATE Notes SET IsPinned=@IsPinned WHERE NoteId=@Id" :
                                             "UPDATE webnote SET IsPinned=@IsPinned WHERE Id=@Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@IsPinned", IsPinned);
                        cmd.ExecuteNonQuery();
                    }
                }

                AppConfig.LogEvent(AppConfig.CurrentAccId, "Archive", $"{(IsPinned ? "Archived" : "Unarchived")} {type}: {id}");
                flowLayoutPanel1.Controls.Clear(); // clear existing buttons before reloading
                LoadAllSorted();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update pin status:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void AttachNoteHandlersClean(Button btn, int id, string title, string content, string reminder, Action reloadAction)
        {
            btn.Click += (s, ev) =>
            {
                OpenNoteEditor(id, btn, title, content, reminder, reloadAction);
            };
        }

        private void AttachWebNoteHandlersClean(Button btn, int id, string title, string url, string reminder, Action reloadAction)
        {
            btn.Click += async (s, ev) =>
            {
                await OpenWebNoteEditor(id, btn, title, url, reminder, reloadAction);
            };
        }
        private async Task OpenWebNoteEditor(int id, Button btn, string title, string url, string reminder, Action reloadAction)
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
                Location = new Point(backBtn.Right + 200, 22)
            };
            topPanel.Controls.Add(txtTitle);

            // --- URL TextBox ---
            TextBox txtUrl = new TextBox
            {
                Text = url,
                Font = new Font("Segoe UI", 10),
                Width = 300,
                Location = new Point(txtTitle.Right + 10, 22)
            };
            topPanel.Controls.Add(txtUrl);

            // --- Reminder TextBox ---
            TextBox txtReminder = new TextBox
            {
                Text = string.IsNullOrWhiteSpace(reminder) ? "(yyyy-MM-dd HH:mm)" : reminder,
                Font = new Font("Segoe UI", 10),
                Width = 180,
                Location = new Point(backBtn.Right + 10, 22),
                ForeColor = string.IsNullOrWhiteSpace(reminder) ? Color.Gray : Color.Black
            };
            topPanel.Controls.Add(txtReminder);

            // --- WebView2 ---
            var webView = new Microsoft.Web.WebView2.WinForms.WebView2 { Dock = DockStyle.Fill };
            contentPanel.Controls.Add(webView);
            webView.BringToFront();
            topPanel.SendToBack();

            try { await webView.EnsureCoreWebView2Async(null); webView.CoreWebView2.Navigate(url); }
            catch (Exception ex) { MessageBox.Show("Failed to load URL:\n" + ex.Message); }

            // --- Save and Return Action ---
            Action saveAndReturn = () =>
            {
                string newTitle = txtTitle.Text.Trim();
                string newUrl = txtUrl.Text.Trim();
                string reminderText = txtReminder.Text.Trim();

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
                        string updateQuery = "UPDATE webnote SET Title=@Title, Link=@Link, ReminderDateTime=@Reminder WHERE Id=@Id";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Title", string.IsNullOrWhiteSpace(newTitle) ? "Web Note" : newTitle);
                            cmd.Parameters.AddWithValue("@Link", newUrl);
                            cmd.Parameters.AddWithValue("@Reminder", (reminderText == "(yyyy-MM-dd HH:mm)" || string.IsNullOrWhiteSpace(reminderText)) ? DBNull.Value : (object)DateTime.Parse(reminderText));
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    AppConfig.LogEvent(AppConfig.CurrentAccId, "WebNote", $"Updated WebNote: {newTitle}");
                }
                catch (Exception ex) { MessageBox.Show("Failed to update database:\n" + ex.Message); }

                try { webView.Dispose(); } catch { }

                contentPanel.Visible = false;
                flowLayoutPanel1.Visible = true;
                panel2.Visible = true;
                flowLayoutPanel1.Controls.Clear();
                reloadAction();
            };

            // --- Events ---
            txtReminder.GotFocus += (s, e) =>
            {
                if (txtReminder.Text == "(yyyy-MM-dd HH:mm)")
                {
                    txtReminder.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    txtReminder.ForeColor = Color.Black;
                }
            };
            txtReminder.Leave += (s, e) => saveAndReturn(); // now works correctly

            txtUrl.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) { ev.SuppressKeyPress = true; saveAndReturn(); } };
            backBtn.Click += (s, ev) => saveAndReturn();
        }
        private void OpenNoteEditor(int id, Button btn, string title, string content, string reminder, Action reloadAction)
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
            contentPanel.Controls.Add(backBtn);

            // --- Reminder TextBox ---
            TextBox txtReminder = new TextBox
            {
                Text = string.IsNullOrWhiteSpace(reminder) ? "(yyyy-MM-dd HH:mm)" : reminder,
                Font = new Font("Segoe UI", 10),
                Width = 180,
                Location = new Point(backBtn.Right + 10, 15),
                ForeColor = string.IsNullOrWhiteSpace(reminder) ? Color.Gray : Color.Black
            };
            contentPanel.Controls.Add(txtReminder);

            // --- Letter Count Label ---
            Label lblCount = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(contentPanel.Width - 140, 20)

            };
            contentPanel.Controls.Add(lblCount);

            // --- Title TextBox ---
            TextBox txtTitle = new TextBox
            {
                Text = title,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 60),
                Width = contentPanel.Width - 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            contentPanel.Controls.Add(txtTitle);

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
            contentPanel.Controls.Add(txtBox);

            txtBox.TextChanged += (s, ev) => { lblCount.Text = $"Letters: {txtBox.Text.Length}"; };
            lblCount.Text = $"Letters: {txtBox.Text.Length}";
            contentPanel.Resize += (s, ev) => { lblCount.Location = new Point(contentPanel.Width - 140, 20); };

            // --- Save and Return Action ---
            Action saveAndReturn = () =>
            {
                string newTitle = txtTitle.Text.Trim();
                string newContent = txtBox.Text.Trim();
                string reminderText = txtReminder.Text.Trim();
                if (string.IsNullOrWhiteSpace(newTitle)) newTitle = "Note";

                btn.Text = newTitle;

                try
                {
                    using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Notes SET Title=@Title, Note=@Note, Modified=GETDATE(), ReminderDateTime=@Reminder WHERE NoteId=@Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Title", newTitle);
                            cmd.Parameters.AddWithValue("@Note", newContent);
                            cmd.Parameters.AddWithValue("@Reminder", (reminderText == "(yyyy-MM-dd HH:mm)" || string.IsNullOrWhiteSpace(reminderText)) ? DBNull.Value : (object)DateTime.Parse(reminderText));
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
                flowLayoutPanel1.Controls.Clear();
                reloadAction();
            };

            // --- Events ---
            txtReminder.GotFocus += (s, e) =>
            {
                if (txtReminder.Text == "(yyyy-MM-dd HH:mm)")
                {
                    txtReminder.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    txtReminder.ForeColor = Color.Black;
                }
            };
            txtReminder.Leave += (s, e) => saveAndReturn(); // auto-save on exit
            backBtn.Click += (s, ev) => saveAndReturn();
        }

        // Delete methods
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

        
    }
}
