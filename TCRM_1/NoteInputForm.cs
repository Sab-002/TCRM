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
    public partial class NoteInputForm : Form
    {
        public string EnteredTitle { get; set; }
        public string EnteredNote { get; set; }

        public NoteInputForm()
        {
            this.Text = "Enter Title and Note";
            this.Size = new Size(500, 400); // Larger for multiline note
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Label for Title
            Label lblTitle = new Label
            {
                Text = "Title:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            // TextBox for Title
            TextBox txtTitle = new TextBox
            {
                Location = new Point(10, 30),
                Width = this.ClientSize.Width - 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Label for Note
            Label lblNote = new Label
            {
                Text = "Note:",
                Location = new Point(10, 60),
                AutoSize = true
            };

            // TextBox for Note
            TextBox txtNote = new TextBox
            {
                Location = new Point(10, 80),
                Width = this.ClientSize.Width - 20,
                Height = 200, // Multiline height
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // OK Button
            Button btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(this.ClientSize.Width - 180, this.ClientSize.Height - 40),
                Size = new Size(75, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(this.ClientSize.Width - 90, this.ClientSize.Height - 40),
                Size = new Size(75, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblNote);
            this.Controls.Add(txtNote);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            // Set default buttons
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // Pre-populate if set
            this.Load += (s, e) =>
            {
                txtTitle.Text = EnteredTitle ?? "";
                txtNote.Text = EnteredNote ?? "";
            };

            btnOk.Click += (s, e) =>
            {
                EnteredTitle = txtTitle.Text.Trim();
                EnteredNote = txtNote.Text.Trim();
            };
        }
    



        private void NoteInputForm_Load(object sender, EventArgs e)
        {

        }
    }
}
