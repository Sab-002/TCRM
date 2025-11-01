namespace TCRM_1
{
    partial class TCRMLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TCRMLogin));
            login = new Button();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            label2 = new Label();
            user = new TextBox();
            pass = new TextBox();
            panel1 = new Panel();
            panel2 = new Panel();
            label3 = new Label();
            linkLabel2 = new LinkLabel();
            button2 = new Button();
            label5 = new Label();
            SuspendLayout();
            // 
            // login
            // 
            login.BackColor = Color.FromArgb(128, 128, 255);
            login.FlatStyle = FlatStyle.Flat;
            login.Font = new Font("Tahoma", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            login.ForeColor = Color.White;
            login.Location = new Point(38, 343);
            login.Name = "login";
            login.Size = new Size(233, 40);
            login.TabIndex = 0;
            login.Text = "Login";
            login.UseVisualStyleBackColor = false;
            login.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(37, 160);
            label1.Name = "label1";
            label1.Size = new Size(82, 20);
            label1.TabIndex = 1;
            label1.Text = "Username ";
            label1.Click += label1_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(108, 325);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(95, 15);
            linkLabel1.TabIndex = 2;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Forgot password";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(37, 233);
            label2.Name = "label2";
            label2.Size = new Size(73, 20);
            label2.TabIndex = 3;
            label2.Text = "Password";
            // 
            // user
            // 
            user.BorderStyle = BorderStyle.None;
            user.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            user.Location = new Point(37, 183);
            user.Multiline = true;
            user.Name = "user";
            user.Size = new Size(233, 32);
            user.TabIndex = 5;
            user.TextChanged += textBox1_TextChanged;
            // 
            // pass
            // 
            pass.BorderStyle = BorderStyle.None;
            pass.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            pass.Location = new Point(37, 256);
            pass.Multiline = true;
            pass.Name = "pass";
            pass.PasswordChar = '*';
            pass.Size = new Size(233, 34);
            pass.TabIndex = 6;
            pass.TextChanged += pass_TextChanged;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaptionText;
            panel1.Location = new Point(37, 214);
            panel1.Name = "panel1";
            panel1.Size = new Size(232, 1);
            panel1.TabIndex = 7;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaptionText;
            panel2.Location = new Point(38, 289);
            panel2.Name = "panel2";
            panel2.Size = new Size(232, 1);
            panel2.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlText;
            label3.Location = new Point(38, 126);
            label3.Name = "label3";
            label3.Size = new Size(69, 30);
            label3.TabIndex = 9;
            label3.Text = "Login";
            label3.Click += label3_Click;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(94, 386);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(128, 15);
            linkLabel2.TabIndex = 10;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Dont have an account?";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // button2
            // 
            button2.Location = new Point(266, 12);
            button2.Name = "button2";
            button2.Size = new Size(34, 34);
            button2.TabIndex = 11;
            button2.Text = "X";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Gadugi", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(76, 46);
            label5.Name = "label5";
            label5.Size = new Size(160, 57);
            label5.TabIndex = 12;
            label5.Text = "TCRM";
            // 
            // TCRMLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(312, 431);
            Controls.Add(label5);
            Controls.Add(button2);
            Controls.Add(linkLabel2);
            Controls.Add(label3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pass);
            Controls.Add(user);
            Controls.Add(label2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Controls.Add(login);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TCRMLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button login;
        private Label label1;
        private LinkLabel linkLabel1;
        private Label label2;
        private TextBox user;
        private TextBox pass;
        private Panel panel1;
        private Panel panel2;
        private Label label3;
        private LinkLabel linkLabel2;
        private Button button2;
        private Label label5;
    }
}
