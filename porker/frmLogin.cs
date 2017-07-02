#define ENABLE_LOGIN_AUTH

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
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
            {

#if ENABLE_LOGIN_AUTH
                login_success = login_auth(this.pk_txt_user.Text, this.pk_txt_pass.Text);
#else
                // simple check
                Properties.Settings.Default.PK_DEBUG_USER = this.pk_txt_user.Text;
                Properties.Settings.Default.PK_DEBUG_PASS = this.pk_txt_pass.Text;
                Properties.Settings.Default.Save();
                login_success = true;
#endif
                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.PK_STR_LOGIN_FAIL, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool login_auth(string user, string pass)
        {
            bool auth_result = false;

            const short CODE_SUCCESS = 200;
            const short CODE_FAIL = 201;

            var request = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.PK_LOGIN_AUTH_URL);

            var post_data = "account=" + user + "&password=" + pass;
            var data = Encoding.ASCII.GetBytes(post_data);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            var response_str = "";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                response_str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {

            }

            if (response_str != "")
            {
                if (response_str.Contains(CODE_SUCCESS.ToString()))
                {
                    auth_result = true;
                }
            }

            return auth_result;
        }
    }
}
