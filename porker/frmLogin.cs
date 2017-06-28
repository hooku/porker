using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace porker
{
    public partial class frmLogin : Form
    {
        private bool login_success = false;

        public frmLogin()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.PK_ICO_APP;
            this.Text = Properties.Resources.PK_STR_LOGIN_ING;

            this.pk_pic_login.Image = Properties.Resources.PK_ICO_USER_128.ToBitmap();

            this.pk_lbl_user.Text = Properties.Resources.PK_STR_LOGIN_USER;
            this.pk_lbl_pass.Text = Properties.Resources.PK_STR_LOGIN_PASS;
            this.pk_btn_login.Text = Properties.Resources.PK_STR_LOGIN_CMD;
            this.pk_lbl_descript.Text = Properties.Resources.PK_STR_LOGIN_DESCRIPT;

            this.pk_txt_user.Text = Properties.Settings.Default.PK_DEBUG_USER;
            this.pk_txt_pass.Text = Properties.Settings.Default.PK_DEBUG_PASS;

            this.AcceptButton = this.pk_btn_login;
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (login_success == false)
            {
                Application.Exit();
            }
        }

        private void pk_btn_login_Click(object sender, EventArgs e)
        {
            if (this.pk_txt_user.Text == Properties.Settings.Default.PK_DEBUG_USER &&
                this.pk_txt_pass.Text == Properties.Settings.Default.PK_DEBUG_PASS)
            { // simple check
                Properties.Settings.Default.PK_DEBUG_USER = this.pk_txt_user.Text;
                Properties.Settings.Default.PK_DEBUG_PASS = this.pk_txt_pass.Text;
                Properties.Settings.Default.Save();

                login_success = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.PK_STR_LOGIN_FAIL, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
