namespace porker
{
    partial class frmLogin
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
            this.pk_txt_user = new System.Windows.Forms.TextBox();
            this.pk_lbl_user = new System.Windows.Forms.Label();
            this.pk_txt_pass = new System.Windows.Forms.TextBox();
            this.pk_lbl_pass = new System.Windows.Forms.Label();
            this.pk_btn_login = new System.Windows.Forms.Button();
            this.pk_pic_login = new System.Windows.Forms.PictureBox();
            this.pk_lbl_descript = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pk_pic_login)).BeginInit();
            this.SuspendLayout();
            // 
            // pk_txt_user
            // 
            this.pk_txt_user.Location = new System.Drawing.Point(200, 70);
            this.pk_txt_user.Name = "pk_txt_user";
            this.pk_txt_user.Size = new System.Drawing.Size(180, 20);
            this.pk_txt_user.TabIndex = 0;
            // 
            // pk_lbl_user
            // 
            this.pk_lbl_user.AutoSize = true;
            this.pk_lbl_user.Location = new System.Drawing.Point(145, 75);
            this.pk_lbl_user.Name = "pk_lbl_user";
            this.pk_lbl_user.Size = new System.Drawing.Size(35, 13);
            this.pk_lbl_user.TabIndex = 1;
            this.pk_lbl_user.Text = "####";
            // 
            // pk_txt_pass
            // 
            this.pk_txt_pass.Location = new System.Drawing.Point(200, 115);
            this.pk_txt_pass.Name = "pk_txt_pass";
            this.pk_txt_pass.PasswordChar = '*';
            this.pk_txt_pass.Size = new System.Drawing.Size(180, 20);
            this.pk_txt_pass.TabIndex = 2;
            // 
            // pk_lbl_pass
            // 
            this.pk_lbl_pass.AutoSize = true;
            this.pk_lbl_pass.Location = new System.Drawing.Point(145, 120);
            this.pk_lbl_pass.Name = "pk_lbl_pass";
            this.pk_lbl_pass.Size = new System.Drawing.Size(35, 13);
            this.pk_lbl_pass.TabIndex = 3;
            this.pk_lbl_pass.Text = "####";
            // 
            // pk_btn_login
            // 
            this.pk_btn_login.Location = new System.Drawing.Point(200, 170);
            this.pk_btn_login.Name = "pk_btn_login";
            this.pk_btn_login.Size = new System.Drawing.Size(120, 30);
            this.pk_btn_login.TabIndex = 4;
            this.pk_btn_login.Text = "####";
            this.pk_btn_login.UseVisualStyleBackColor = true;
            this.pk_btn_login.Click += new System.EventHandler(this.pk_btn_login_Click);
            // 
            // pk_pic_login
            // 
            this.pk_pic_login.Location = new System.Drawing.Point(10, 10);
            this.pk_pic_login.Name = "pk_pic_login";
            this.pk_pic_login.Size = new System.Drawing.Size(128, 128);
            this.pk_pic_login.TabIndex = 5;
            this.pk_pic_login.TabStop = false;
            // 
            // pk_lbl_descript
            // 
            this.pk_lbl_descript.AutoSize = true;
            this.pk_lbl_descript.Location = new System.Drawing.Point(155, 30);
            this.pk_lbl_descript.Name = "pk_lbl_descript";
            this.pk_lbl_descript.Size = new System.Drawing.Size(35, 13);
            this.pk_lbl_descript.TabIndex = 6;
            this.pk_lbl_descript.Text = "####";
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 225);
            this.Controls.Add(this.pk_lbl_descript);
            this.Controls.Add(this.pk_pic_login);
            this.Controls.Add(this.pk_btn_login);
            this.Controls.Add(this.pk_lbl_pass);
            this.Controls.Add(this.pk_txt_pass);
            this.Controls.Add(this.pk_lbl_user);
            this.Controls.Add(this.pk_txt_user);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
            this.Shown += new System.EventHandler(this.frmLogin_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pk_pic_login)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pk_txt_user;
        private System.Windows.Forms.Label pk_lbl_user;
        private System.Windows.Forms.TextBox pk_txt_pass;
        private System.Windows.Forms.Label pk_lbl_pass;
        private System.Windows.Forms.Button pk_btn_login;
        private System.Windows.Forms.PictureBox pk_pic_login;
        private System.Windows.Forms.Label pk_lbl_descript;
    }
}