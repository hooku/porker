using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using mshtml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace porker
{
    class clsWebHelper
    {
        [DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Int32 URLDownloadToCacheFile(
            [MarshalAs(UnmanagedType.IUnknown)] object pCaller,
            [MarshalAs(UnmanagedType.LPWStr)] string szURL,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szFileName,
            Int32 cchFileName,
            Int32 dwReserved,
            IntPtr lpfnCB);

        private string pkh_get_cached_file(string url)
        {
            string tmp_file = Path.GetTempFileName();
            StringBuilder file_name = new StringBuilder(tmp_file, 260);

            int result = URLDownloadToCacheFile(0, url, file_name, file_name.Capacity, 0, IntPtr.Zero);

            return tmp_file;
        }

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

        private string pkh_code(HtmlDocument doc)
        {
            string result = "";

            string code_url = doc.GetElementById("codeImage").GetAttribute("src");

            //string file = pkh_get_cached_file(code_url);
            string tmp_file = Path.GetTempFileName();

            Program.log(tmp_file);

            IHTMLDocument2 dom_doc = (IHTMLDocument2)doc.DomDocument;
            IHTMLControlRange img_range = (IHTMLControlRange)((HTMLBody)dom_doc.body).createControlRange();

            Program.log("domdoc=" + dom_doc.fileSize.ToString());

            foreach (IHTMLImgElement img in dom_doc.images)
            {
                Program.log(img.nameProp.ToString());

                if (img.nameProp.Contains("Code"))
                {
                    img_range.add((IHTMLControlElement)img);

                    img_range.execCommand("Copy", false, null);

                    try
                    {
                        using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap))
                        {
                            bmp.Save(tmp_file, ImageFormat.Png);
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.log(ex.Message);
                    }
                }
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] response = client.UploadFile(Properties.Settings.Default.PK_OCR_URL, tmp_file);

                    string s = client.Encoding.GetString(response);

                    foreach (Match match in Regex.Matches(s, "\"([^\"]*)\""))
                    {
                        if (match.ToString() != "\"code\"")
                        {
                            result = match.ToString().Replace("\"", "");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.log(ex.Message);
            }

            return result;
        }

        // get the ticket
        public int pkh_play(ExtendedWebBrowser browser)
        {
            int icon_exec_index = 0;

            try
            {
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
                                if ((pk_elem.InnerText.Contains("抢完")) ||    // 亲，很抱歉项目已被抢完了哦～
                                    (pk_elem.InnerText.Contains("很抱歉")))    // 很抱歉，您拥有的资格数量已达到上限值1
                                {

                                    icon_exec_index = (int)EXEC_ICON.EXEC_GREY;
                                }
                                else if ((pk_elem.InnerText.Contains("超时")) ||          // 网络异常或超时，请稍候再试！
                                         (pk_elem.InnerText.Contains("外呼的人员已满")))   // 当前项目参与外呼的人员已满，请稍后再试
                                {
                                    browser.Refresh(WebBrowserRefreshOption.Completely);
                                    icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                                }

                                if (pk_elem.InnerText.Contains("超过10次"))
                                {
                                    // 5分钟内抢单超过10次（含10次）
                                    Program.log("验证码识别!");
                                    string code = pkh_code(browser.Document);

                                    Program.log(code, 1);

                                    HtmlElement pk_elem_code = browser.Document.GetElementById("regcheckcode");
                                    pk_elem_code.SetAttribute("value", code);

                                    Application.DoEvents();
                                    Thread.Sleep(50);
                                    Application.DoEvents();

                                    foreach (HtmlElement pk_elem_ok in browser.Document.All)
                                    {
                                        if (pk_elem_ok.GetAttribute("i-id") == "ok")
                                        {
                                            pk_elem_ok.InvokeMember("Click");
                                            break;
                                        }
                                    }

                                    icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                                }
                                else if (Properties.Settings.Default.PK_CODETEST > 0)
                                {
                                    Program.log("关闭对话框");
//                                     foreach (HtmlElement pk_elem_close in browser.Document.All)
//                                     {
//                                         if (pk_elem_close.GetAttribute("className") == "ui-dialog-close")
//                                         {
//                                             pk_elem_close.InvokeMember("Click");
//                                             break;
//                                         }
//                                     }

                                    icon_exec_index = (int)EXEC_ICON.EXEC_RED;
                                    browser.Refresh(WebBrowserRefreshOption.Normal);
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
            }
            catch (Exception ex)
            {
                Program.log(ex.Message);
            }

            return icon_exec_index;
        }
    }
}
