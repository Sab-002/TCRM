namespace TCRM_1
{
    partial class TCRMDash
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
        private Panel contentPanel;  // Covers entire window when a button is clicked

        private int dynamicButtonCount = 0 ; // buttoncount for adding buttons

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TCRMDash));
            button3 = new Button();
            button2 = new Button();
            button4 = new Button();
            panel2 = new Panel();
            button6 = new Button();
            button5 = new Button();
            button1 = new Button();
            label1 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.BackColor = Color.FromArgb(147, 147, 255);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(639, 15);
            button3.Name = "button3";
            button3.Size = new Size(46, 43);
            button3.TabIndex = 2;
            button3.Text = "+";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click_1;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.FromArgb(147, 147, 255);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(587, 15);
            button2.Name = "button2";
            button2.Size = new Size(46, 43);
            button2.TabIndex = 1;
            button2.Text = "±";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(147, 147, 255);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(12, 15);
            button4.Name = "button4";
            button4.Size = new Size(46, 43);
            button4.TabIndex = 3;
            button4.Text = "≡";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.BackColor = Color.FromArgb(102, 102, 204);
            panel2.Controls.Add(button6);
            panel2.Controls.Add(button5);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(button4);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(697, 61);
            panel2.TabIndex = 1;
            panel2.Paint += panel2_Paint;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button6.BackColor = Color.FromArgb(147, 147, 255);
            button6.FlatStyle = FlatStyle.Flat;
            button6.Location = new Point(357, 22);
            button6.Name = "button6";
            button6.Size = new Size(62, 29);
            button6.TabIndex = 7;
            button6.Text = "All";
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.BackColor = Color.FromArgb(147, 147, 255);
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(493, 22);
            button5.Name = "button5";
            button5.Size = new Size(73, 29);
            button5.TabIndex = 6;
            button5.Text = "Webnotes";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.FromArgb(147, 147, 255);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(425, 22);
            button1.Name = "button1";
            button1.Size = new Size(62, 29);
            button1.TabIndex = 5;
            button1.Text = "Notes";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(64, 15);
            label1.Name = "label1";
            label1.Size = new Size(173, 32);
            label1.TabIndex = 4;
            label1.Text = "Welcome, ----";
            label1.Click += label1_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.GhostWhite;
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 61);
            flowLayoutPanel1.MinimumSize = new Size(697, 429);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(697, 429);
            flowLayoutPanel1.TabIndex = 3;
            flowLayoutPanel1.Paint += flowLayoutPanel1_Paint_1;
            // 
            // TCRMDash
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(697, 487);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(panel2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(697, 522);
            Name = "TCRMDash";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TCRM";
            Load += TCRM_2_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button3;
        private Button button2;
        private Button button4;
        private Panel panel2;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label1;
        private Button button5;
        private Button button1;
        private Button button6;
    }
}