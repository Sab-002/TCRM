namespace TCRM_1
{
    partial class ArchiveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Archived Notes & WebNotes";
            this.Size = new Size(720, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Optional top panel (panel2)
            panel2 = new Panel
            {
                Size = new Size(700, 40),
                Location = new Point(10, 10)
            };
            this.Controls.Add(panel2);

            Button btnBack = new Button
            {
                Text = "Back",
                Size = new Size(80, 30),
                Location = new Point(20, 5)
            };
            btnBack.Click += BtnBack_Click;
            panel2.Controls.Add(btnBack);

            flowLayoutPanel1 = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(660, 380),
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.TopDown
            };
            this.Controls.Add(flowLayoutPanel1);

            contentPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(660, 380),
                Visible = false
            };
            this.Controls.Add(contentPanel);
        }

        #endregion
    }
}