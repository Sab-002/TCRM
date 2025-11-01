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
    public partial class UrlInputForm : Form
    {
        public string EnteredUrl { get; set; }
        public string EnteredTitle { get; set; }

        public UrlInputForm()
        {
            this.Text = "Enter Title and URL";
            this.Size = new Size(400, 180); // Increased height to fit both text boxes
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
                Width = this.ClientSize.Width - 20, // keeps 10px margin on both sides
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Label for URL
            Label lblUrl = new Label
            {
                Text = "URL:",
                Location = new Point(10, 60),
                AutoSize = true
            };

            // TextBox for URL
            TextBox txtUrl = new TextBox
            {
                Location = new Point(10, 80),
                Width = this.ClientSize.Width - 20, // keeps 10px margin on both sides
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // OK Button
            Button btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(this.ClientSize.Width - 180, 110),
                Size = new Size(75, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(this.ClientSize.Width - 90, 110),
                Size = new Size(75, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblUrl);
            this.Controls.Add(txtUrl);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            // Optional: set default buttons
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // Pre-populate if set
            this.Load += (s, e) =>
            {
                txtTitle.Text = EnteredTitle ?? "";
                txtUrl.Text = EnteredUrl ?? "";
            };

            btnOk.Click += (s, e) =>
            {
                EnteredTitle = txtTitle.Text.Trim();
                EnteredUrl = txtUrl.Text.Trim();
            };
        }

        private void UrlInputForm_Load(object sender, EventArgs e)
        {

        }
    }
}