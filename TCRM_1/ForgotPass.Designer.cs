using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace TCRM_1
{
    partial class ForgotPass
    {
        private Label lblTitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Button btnSendCode;
        private Label lblCode;
        private TextBox txtCode;
        private Button btnVerify;
        private Panel panelReset;
        private Label lblNewPass;
        private TextBox txtNewPassword;
        private Button btnReset;
        private Button button1;
        private Button button2;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForgotPass));
            lblTitle = new Label();
            lblEmail = new Label();
            txtEmail = new TextBox();
            btnSendCode = new Button();
            lblCode = new Label();
            txtCode = new TextBox();
            btnVerify = new Button();
            panelReset = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            lblNewPass = new Label();
            txtNewPassword = new TextBox();
            btnReset = new Button();
            button1 = new Button();
            button2 = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panelReset.SuspendLayout();
            panel5.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Gadugi", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(19, 31);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(304, 67);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "Forgot Password";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.Location = new Point(18, 100);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(112, 19);
            lblEmail.TabIndex = 6;
            lblEmail.Text = "Enter your email:";
            // 
            // txtEmail
            // 
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.Font = new Font("Segoe UI", 10F);
            txtEmail.Location = new Point(18, 122);
            txtEmail.Multiline = true;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(300, 27);
            txtEmail.TabIndex = 5;
            // 
            // btnSendCode
            // 
            btnSendCode.BackColor = Color.FromArgb(128, 128, 255);
            btnSendCode.FlatAppearance.BorderSize = 0;
            btnSendCode.FlatStyle = FlatStyle.Flat;
            btnSendCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSendCode.ForeColor = Color.White;
            btnSendCode.Location = new Point(93, 164);
            btnSendCode.Name = "btnSendCode";
            btnSendCode.Size = new Size(150, 35);
            btnSendCode.TabIndex = 4;
            btnSendCode.Text = "Send Code";
            btnSendCode.UseVisualStyleBackColor = false;
            btnSendCode.Click += btnSendCode_Click;
            // 
            // lblCode
            // 
            lblCode.AutoSize = true;
            lblCode.Font = new Font("Segoe UI", 10F);
            lblCode.Location = new Point(18, 206);
            lblCode.Name = "lblCode";
            lblCode.Size = new Size(115, 19);
            lblCode.TabIndex = 3;
            lblCode.Text = "Verification Code:";
            // 
            // txtCode
            // 
            txtCode.BorderStyle = BorderStyle.None;
            txtCode.Font = new Font("Segoe UI", 10F);
            txtCode.Location = new Point(18, 228);
            txtCode.Multiline = true;
            txtCode.Name = "txtCode";
            txtCode.Size = new Size(300, 28);
            txtCode.TabIndex = 2;
            // 
            // btnVerify
            // 
            btnVerify.BackColor = Color.FromArgb(34, 197, 94);
            btnVerify.FlatAppearance.BorderSize = 0;
            btnVerify.FlatStyle = FlatStyle.Flat;
            btnVerify.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnVerify.ForeColor = Color.White;
            btnVerify.Location = new Point(93, 267);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new Size(150, 35);
            btnVerify.TabIndex = 1;
            btnVerify.Text = "Verify Code";
            btnVerify.UseVisualStyleBackColor = false;
            btnVerify.Click += btnVerify_Click;
            // 
            // panelReset
            // 
            panelReset.BorderStyle = BorderStyle.FixedSingle;
            panelReset.Controls.Add(panel5);
            panelReset.Controls.Add(lblNewPass);
            panelReset.Controls.Add(txtNewPassword);
            panelReset.Controls.Add(btnReset);
            panelReset.Location = new Point(18, 311);
            panelReset.Name = "panelReset";
            panelReset.Size = new Size(300, 130);
            panelReset.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Black;
            panel5.Controls.Add(panel6);
            panel5.Location = new Point(18, 57);
            panel5.Name = "panel5";
            panel5.Size = new Size(257, 1);
            panel5.TabIndex = 15;
            // 
            // panel6
            // 
            panel6.BackColor = Color.Black;
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(257, 1);
            panel6.TabIndex = 0;
            // 
            // lblNewPass
            // 
            lblNewPass.AutoSize = true;
            lblNewPass.Font = new Font("Segoe UI", 10F);
            lblNewPass.Location = new Point(15, 15);
            lblNewPass.Name = "lblNewPass";
            lblNewPass.Size = new Size(101, 19);
            lblNewPass.TabIndex = 0;
            lblNewPass.Text = "New Password:";
            // 
            // txtNewPassword
            // 
            txtNewPassword.BorderStyle = BorderStyle.None;
            txtNewPassword.Font = new Font("Segoe UI", 10F);
            txtNewPassword.Location = new Point(18, 37);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '•';
            txtNewPassword.Size = new Size(260, 18);
            txtNewPassword.TabIndex = 1;
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.FromArgb(128, 128, 255);
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(70, 80);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(150, 35);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset Password";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Black;
            button1.Location = new Point(12, 10);
            button1.Name = "button1";
            button1.Size = new Size(34, 34);
            button1.TabIndex = 8;
            button1.Text = "<";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.Transparent;
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Black;
            button2.Location = new Point(294, 10);
            button2.Name = "button2";
            button2.Size = new Size(34, 34);
            button2.TabIndex = 12;
            button2.Text = "x";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(panel2);
            panel1.Location = new Point(18, 151);
            panel1.Name = "panel1";
            panel1.Size = new Size(300, 1);
            panel1.TabIndex = 13;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Black;
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(300, 1);
            panel2.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Black;
            panel3.Controls.Add(panel4);
            panel3.Location = new Point(18, 258);
            panel3.Name = "panel3";
            panel3.Size = new Size(300, 1);
            panel3.TabIndex = 15;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Black;
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(300, 1);
            panel4.TabIndex = 0;
            // 
            // ForgotPass
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(340, 460);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(panelReset);
            Controls.Add(btnVerify);
            Controls.Add(txtCode);
            Controls.Add(lblCode);
            Controls.Add(btnSendCode);
            Controls.Add(txtEmail);
            Controls.Add(lblEmail);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ForgotPass";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TCRM";
            FormClosing += close;
            Load += ForgotPass_Load;
            panelReset.ResumeLayout(false);
            panelReset.PerformLayout();
            panel5.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
