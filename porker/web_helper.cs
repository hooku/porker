using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace porker
{
    class WebHelper
    {
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
                browser.Document.GetElementById("mobile").InnerText = Properties.Settings.Default.PK_DEFAULT_USER;
                browser.Document.GetElementById("password").InnerText = Properties.Settings.Default.PK_DEFAULT_PASS;
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
        public int pkh_play(ExtendedWebBrowser browser)
        {
            int icon_exec_index = 0;

            if (browser.Url.ToString().Contains("project_detail.html"))
            {
                foreach (HtmlElement pk_elem in browser.Document.All)
                {
                    if (pk_elem.GetAttribute("className") == "data-loading")
                    {
                        if (pk_elem.Style == "display:block")
                        {
                            icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                            break;
                        }
                    }

                    switch (pk_elem.GetAttribute("className"))
                    {
                        case "ui-dialog-content":
                            Program.log("对话框");
                            if (pk_elem.InnerText.Contains("抢完"))
                            {
                                // 亲，很抱歉项目已被抢完了哦～
                                icon_exec_index = (int)EXEC_ICON.EXEC_GREY;
                            } 
                            else if (pk_elem.InnerText.Contains("很抱歉"))
                            {
                                // 很抱歉，您拥有的资格数量已达到上限值1
                                icon_exec_index = (int)EXEC_ICON.EXEC_GREY;
                            }
                            else if (pk_elem.InnerText.Contains("超时"))
                            {
                                // 网络异常或超时，请稍候再试！
                                browser.Refresh(WebBrowserRefreshOption.Completely);
                                icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                            }
                            break;
                        case "btn btn-yellow btn-mid J_start_work mt-5":    // "开始工作"
                        case "btn btn-yellow btn-mid J_grab_single mt-5":   // "开始抢单"
                            if (pk_elem.GetAttribute("clicked") != "true")
                            {
                                Program.log("点击按钮");
                                pk_elem.SetAttribute("clicked", "true");
                                pk_elem.InvokeMember("Click");
                                icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                            }
                            break;
                        case "btn btn-yellow btn-disabled btn-mid mt-5":    // button disabled
                            Program.log("刷新");
                            browser.Refresh(WebBrowserRefreshOption.Completely);
                            icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                            break;
                        default:
                            break;
                    }
                }
            }

            return icon_exec_index;
        }
    }
}
