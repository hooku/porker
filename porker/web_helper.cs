using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace porker
{
    class WebHelper
    {
        private const int WEB_TIMEOUT = 3000;   // 3sec

        // all data in this file are hardcoded
        public void pkh_login(ExtendedWebBrowser browser)
        {
            try
            {
                browser.Navigate("http://zhongbao.10085.cn:18080/module/public/reg/user_login.html");

                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    // TODO: need add timeout
                    Application.DoEvents();
                }

                browser.Document.GetElementById("mobile").Focus();
                browser.Document.GetElementById("mobile").InnerText = Properties.Settings.Default.PK_DEBUG_USER;
                browser.Document.GetElementById("password").InnerText = Properties.Settings.Default.PK_DEBUG_PASS;
                browser.Document.GetElementById("regcheckcode").Focus();
            }
            catch (Exception ex)
            {

            }
        }

        private void pkh_post_amend_thread(ExtendedWebBrowser browser)
        {
            for (int tries = 0; tries < 3; tries++)
            {
                if (browser != null)
                {
                    foreach (HtmlElement pk_elem in browser.Document.All)
                    {
                        if (pk_elem.GetAttribute("className") == "btn btn-yellow")
                        {
                            Program.log("_blank set");
                            pk_elem.SetAttribute("target", "_blank");
                            tries = 3;  // exit outer loop
                        }
                    }
                    Application.DoEvents();
                    Thread.Sleep(250);
                }
            }
        }

        public void pkh_post_amend(ExtendedWebBrowser browser)
        {
            // amend the a href to open in new tab
            try
            {
                if (browser.Url.ToString().Contains("home.html"))
                {
                    //Thread thread = new Thread(() => pkh_post_amend_thread(browser));
                    //thread.Start();
                    pkh_post_amend_thread(browser); // cross-thread isn't working
                }
            }
            catch (Exception ex)
            {

            }
        }

        // get the ticket
        public void pkh_play(ExtendedWebBrowser browser)
        {
            Program.log("hi");
        }
    }
}
