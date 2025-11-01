namespace TCRM_1
{
    partial class ChangePasswordForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtCurrentPass;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.TextBox txtConfirmPass;
        private System.Windows.Forms.Button btnSavePassword;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblCurrent = new Label();
            lblNew = new Label();
            lblConfirm = new Label();
            txtCurrentPass = new TextBox();
            txtNewPass = new TextBox();
            txtConfirmPass = new TextBox();
            btnSavePassword = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblCurrent
            // 
            lblCurrent.AutoSize = true;
            lblCurrent.Location = new Point(20, 20);
            lblCurrent.Name = "lblCurrent";
            lblCurrent.Size = new Size(103, 15);
            lblCurrent.TabIndex = 0;
            lblCurrent.Text = "Current Password:";
            // 
            // lblNew
            // 
            lblNew.AutoSize = true;
            lblNew.Location = new Point(20, 70);
            lblNew.Name = "lblNew";
            lblNew.Size = new Size(87, 15);
            lblNew.TabIndex = 1;
            lblNew.Text = "New Password:";
            // 
            // lblConfirm
            // 
            lblConfirm.AutoSize = true;
            lblConfirm.Location = new Point(20, 120);
            lblConfirm.Name = "lblConfirm";
            lblConfirm.Size = new Size(107, 15);
            lblConfirm.TabIndex = 2;
            lblConfirm.Text = "Confirm Password:";
            // 
            // txtCurrentPass
            // 
            txtCurrentPass.Location = new Point(180, 17);
            txtCurrentPass.Name = "txtCurrentPass";
            txtCurrentPass.Size = new Size(180, 23);
            txtCurrentPass.TabIndex = 3;
            txtCurrentPass.UseSystemPasswordChar = true;
            // 
            // txtNewPass
            // 
            txtNewPass.Location = new Point(180, 67);
            txtNewPass.Name = "txtNewPass";
            txtNewPass.Size = new Size(180, 23);
            txtNewPass.TabIndex = 4;
            txtNewPass.UseSystemPasswordChar = true;
            // 
            // txtConfirmPass
            // 
            txtConfirmPass.Location = new Point(180, 117);
            txtConfirmPass.Name = "txtConfirmPass";
            txtConfirmPass.Size = new Size(180, 23);
            txtConfirmPass.TabIndex = 5;
            txtConfirmPass.UseSystemPasswordChar = true;
            // 
            // btnSavePassword
            // 
            btnSavePassword.BackColor = Color.FromArgb(52, 152, 219);
            btnSavePassword.FlatStyle = FlatStyle.Flat;
            btnSavePassword.ForeColor = Color.White;
            btnSavePassword.Location = new Point(180, 165);
            btnSavePassword.Name = "btnSavePassword";
            btnSavePassword.Size = new Size(80, 30);
            btnSavePassword.TabIndex = 6;
            btnSavePassword.Text = "Save";
            btnSavePassword.UseVisualStyleBackColor = false;
            btnSavePassword.Click += btnSavePassword_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.LightGray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(280, 165);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // ChangePasswordForm
            // 
            BackColor = SystemColors.ControlLight;
            ClientSize = new Size(390, 220);
            Controls.Add(lblCurrent);
            Controls.Add(lblNew);
            Controls.Add(lblConfirm);
            Controls.Add(txtCurrentPass);
            Controls.Add(txtNewPass);
            Controls.Add(txtConfirmPass);
            Controls.Add(btnSavePassword);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ChangePasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change Password";
            Load += ChangePasswordForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
