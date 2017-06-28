﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace porker
{
    public partial class frmMain : Form
    {
        private ToolStrip pk_tool_bra;
        private ToolStripButton pk_tool_btn_back;
        private ToolStripButton pk_tool_btn_forward;
        private ToolStripSeparator pk_tool_sep1;
        private ToolStripTextBox pk_tool_txt_url;
        private ToolStripButton pk_tool_btn_refresh;
        private ToolStripSeparator pk_tool_sep2;
        private ToolStripButton pk_tool_btn_login;
        private ToolStripButton pk_tool_btn_run;
        private ToolStripSeparator pk_tool_sep3;
        private ToolStripButton pk_tool_btn_time;
        private ToolStripButton pk_tool_btn_showlog;

        private ListView pk_lv_log;

        private TabControl pk_tab;
        private ImageList pk_imglist_log;
        private ImageList pk_imglist_tab;

        private StatusStrip pk_status_bra;
        private ToolStripStatusLabel pk_status_txt_time;
        private ToolStripStatusLabel pk_status_txt_stat;
        private ToolStripStatusLabel pk_status_txt_green_bulb;
        private ToolStripStatusLabel pk_status_txt_red_bulb;

        ExtendedWebBrowser pk_browser_front;
        WebHelper web_helper;
        System.Threading.Timer timer_clock;

        private const int PK_LV_LOG_WIDTH = 260;  //px
        private const int PK_LV_LOG_TIME_WIDTH = 70;
        private const int PK_LV_LOG_TXT_WIDTH = 185;

        private const int TIMER_CLOCK_REFRESH_INTERVAL = 1000;

        private void create_form_components()
        {
            // form
            this.Icon = Properties.Resources.PK_ICO_APP;
            this.Text = Properties.Resources.PK_STR_APP_NAME + " " +
                Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

            // tool bar
            this.pk_tool_bra = new ToolStrip();
            this.pk_tool_btn_back = new ToolStripButton();
            this.pk_tool_btn_forward = new ToolStripButton();
            this.pk_tool_sep1 = new ToolStripSeparator();
            this.pk_tool_txt_url = new ToolStripTextBox();
            this.pk_tool_btn_refresh = new ToolStripButton();
            this.pk_tool_sep2 = new ToolStripSeparator();
            this.pk_tool_btn_login = new ToolStripButton();
            this.pk_tool_btn_run = new ToolStripButton();
            this.pk_tool_sep3 = new ToolStripSeparator();
            this.pk_tool_btn_time = new ToolStripButton();
            this.pk_tool_btn_showlog = new ToolStripButton();
            
            this.pk_tool_bra.Dock = DockStyle.Top;
            this.pk_tool_bra.ImageScalingSize = new Size(24, 24);

            this.pk_tool_btn_back.Image = Properties.Resources.PK_ICO_LEFT_24.ToBitmap();
            this.pk_tool_btn_back.Text = Properties.Resources.PK_STR_BACK;
            this.pk_tool_btn_back.Click += new System.EventHandler(this.pk_tool_btn_back_Click);

            this.pk_tool_btn_forward.Image = Properties.Resources.PK_ICO_RIGHT_24.ToBitmap();
            this.pk_tool_btn_forward.Click += new System.EventHandler(this.pk_tool_btn_forward_Click);

            this.pk_tool_txt_url.AutoSize = false;
            this.pk_tool_txt_url.Width = 650;
            this.pk_tool_txt_url.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pk_took_txt_url_KeyDown);

            this.pk_tool_btn_refresh.Image = Properties.Resources.PK_ICO_REFRESH_24.ToBitmap();
            this.pk_tool_btn_refresh.Click += new System.EventHandler(this.pk_tool_btn_refresh_Click);

            this.pk_tool_btn_login.Image = Properties.Resources.PK_ICO_KEY_24.ToBitmap();
            this.pk_tool_btn_login.Text = Properties.Resources.PK_STR_LOGIN;
            this.pk_tool_btn_login.Click += new System.EventHandler(this.pk_tool_btn_login_Click);

            this.pk_tool_btn_run.Image = Properties.Resources.PK_ICO_EXEC_24.ToBitmap();
            this.pk_tool_btn_run.Text = Properties.Resources.PK_STR_RUN;
            this.pk_tool_btn_run.Click += new System.EventHandler(this.pk_tool_btn_run_Click);

            this.pk_tool_btn_time.Image = Properties.Resources.PK_ICO_TIMESYNC_24.ToBitmap();
            this.pk_tool_btn_time.Text = Properties.Resources.PK_STR_TIME;
            this.pk_tool_btn_time.Click += new System.EventHandler(this.pk_tool_btn_time_Click);

            this.pk_tool_btn_showlog.Image = Properties.Resources.PK_ICO_LOG_24.ToBitmap();
            this.pk_tool_btn_showlog.Text = Properties.Resources.PK_STR_SHOWLOG;
            this.pk_tool_btn_showlog.Click += new System.EventHandler(this.pk_tool_btn_showlog_Click);

            this.pk_tool_bra.SuspendLayout();
            this.pk_tool_bra.Items.AddRange(new ToolStripItem[] {
                this.pk_tool_btn_back,
                this.pk_tool_btn_forward,
                this.pk_tool_sep1,
                this.pk_tool_txt_url,
                this.pk_tool_btn_refresh,
                this.pk_tool_sep2,
                this.pk_tool_btn_login,
                this.pk_tool_btn_run,
                this.pk_tool_sep3,
                this.pk_tool_btn_time,
                this.pk_tool_btn_showlog
            });

            this.pk_tool_bra.ResumeLayout();

            // listview
            this.pk_imglist_log = new ImageList();
            this.pk_lv_log = new ListView();

            this.pk_imglist_log.ColorDepth = ColorDepth.Depth32Bit;
            this.pk_imglist_log.Images.Add(Properties.Resources.PK_ICO_INFO_16);
            this.pk_imglist_log.Images.Add(Properties.Resources.PK_ICO_WARN_16);
            this.pk_imglist_log.Images.Add(Properties.Resources.PK_ICO_ERR_16);

            this.pk_lv_log.Dock = DockStyle.Left;
            this.pk_lv_log.SmallImageList = this.pk_imglist_log;
            this.pk_lv_log.FullRowSelect = true;
            this.pk_lv_log.View = View.Details;
            this.pk_lv_log.Width = PK_LV_LOG_WIDTH;
            this.pk_lv_log.Columns.Add("time", PK_LV_LOG_TIME_WIDTH);
            this.pk_lv_log.Columns.Add("log", PK_LV_LOG_TXT_WIDTH);

            // tab
            this.pk_imglist_tab = new ImageList();
            this.pk_tab = new TabControl();

            this.pk_imglist_tab.ColorDepth = ColorDepth.Depth32Bit;
            this.pk_imglist_tab.Images.Add(Properties.Resources.PK_ICO_IEPAGE_16);
            this.pk_imglist_tab.Images.Add(Properties.Resources.PK_ICO_EXEC_16);

            this.pk_tab.Dock = DockStyle.Fill;
            this.pk_tab.ImageList = this.pk_imglist_tab;
            this.pk_tab.SelectedIndexChanged += new System.EventHandler(this.pk_tab_SelectedIndexChanged);
            this.pk_tab.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pk_tab_MouseDoubleClick);

            // status bar
            this.pk_status_bra = new StatusStrip();
            this.pk_status_txt_time = new ToolStripStatusLabel();
            this.pk_status_txt_stat = new ToolStripStatusLabel();
            this.pk_status_txt_green_bulb = new ToolStripStatusLabel();
            this.pk_status_txt_red_bulb = new ToolStripStatusLabel();
            this.pk_status_txt_stat.BorderSides = ToolStripStatusLabelBorderSides.Left;
            this.pk_status_txt_stat.BorderStyle = Border3DStyle.SunkenOuter;
            this.pk_status_txt_stat.Text = "Ready.";
            this.pk_status_txt_stat.TextAlign = ContentAlignment.MiddleLeft;
            this.pk_status_txt_stat.Spring = true;
            set_status_busy(false);

            this.pk_status_bra.AutoSize = false;
            this.pk_status_bra.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.pk_status_txt_time,
                this.pk_status_txt_stat,
                this.pk_status_txt_green_bulb,
                this.pk_status_txt_red_bulb
            });

            this.Controls.Add(this.pk_tab);
            this.Controls.Add(this.pk_status_bra);
            this.Controls.Add(this.pk_lv_log);
            this.Controls.Add(this.pk_tool_bra);

            // timer
            timer_clock = new System.Threading.Timer(timer_clock_cb, null, 0, 1000);

            // other class
            web_helper = new WebHelper();

            show_log(true);
        }

        public frmMain()
        {
            InitializeComponent();

            // create all components using code:
            create_form_components();
        }

        private object poker_add_page(string url = "")
        {
            TabPage ex_tab = new TabPage();
            ex_tab.Text = "Loading..";
            ex_tab.ImageIndex = 0;
//            ex_tab.Click += new System.EventHandler(this.pk_tabpage_Click);

            ExtendedWebBrowser ex_browser = new ExtendedWebBrowser();
            ex_browser.Name = "ex";
            ex_browser.Parent = ex_tab;
            ex_browser.Dock = DockStyle.Fill;
            ex_browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.pk_browser_Navigated);
            ex_browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.pk_browser_DocumentCompleted);
            ex_browser.NewWindow2 += new EventHandler<NewWindow2EventArgs>(pk_browser_NewWindow2);
            ex_browser.StatusTextChanged += new System.EventHandler(this.wb_StatusTextChanged);

            Program.log(ex_browser.Version.ToString());

            if (url != string.Empty)
            {
                ex_browser.Navigate(url);
            }

            this.pk_tab.TabPages.Add(ex_tab);

            return ex_browser.Application;
        }

        void pk_browser_NewWindow2(object sender, NewWindow2EventArgs e)
        {
            e.PPDisp = poker_add_page();
        }

        private void pk_tool_btn_back_Click(object sender, EventArgs e)
        {
            this.pk_browser_front.GoBack();
        }

        private void pk_tool_btn_forward_Click(object sender, EventArgs e)
        {
            this.pk_browser_front.GoForward();
        }

        private void pk_took_txt_url_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.pk_browser_front.Navigate(this.pk_tool_txt_url.Text);
            }
        }

        private void pk_tool_btn_refresh_Click(object sender, EventArgs e)
        {
            this.pk_browser_front.Refresh(WebBrowserRefreshOption.Completely);
        }

        private void pk_tool_btn_login_Click(object sender, EventArgs e)
        {
            Program.log(Properties.Resources.PK_STR_LOG_LOGIN);
            this.web_helper.pkh_login(this.pk_browser_front);
        }

        private void pk_tool_btn_run_Click(object sender, EventArgs e)
        {
            Program.log(Properties.Resources.PK_STR_LOG_LOGIN);
            this.web_helper.pkh_login(this.pk_browser_front);
        }

        private void pk_tool_btn_time_Click(object sender, EventArgs e)
        {
            update_time_caller();
            //update_app_caller();
        }

        private void pk_tool_btn_showlog_Click(object sender, EventArgs e)
        {
            show_log(!this.pk_tool_btn_showlog.Checked);
            this.web_helper.pkh_post_amend(pk_browser_front);
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            poker_add_page(Properties.Resources.PK_STR_HOMEPAGE);
            update_front();

            // debug for company
            this.pk_browser_front.Navigate("http://127.0.0.1/p/fsl-securityvalidation.html");

            frmLogin frm_login = new frmLogin();
            frm_login.ShowDialog(this);
        }

        private void pk_browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            ExtendedWebBrowser pk_browser_current = sender as ExtendedWebBrowser;

            // if current browser is active
            if (pk_browser_current == pk_browser_front)
            {
                update_front();
            }

            Program.log(Properties.Resources.PK_STR_LOG_NAV + pk_browser_current.Url.ToString());
        }

        private void pk_browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ExtendedWebBrowser pk_browser_current = sender as ExtendedWebBrowser;

            pk_browser_current.Parent.Text = pk_browser_current.Document.Title;
            this.web_helper.pkh_post_amend(pk_browser_current);
        }

        private void wb_StatusTextChanged(object sender, EventArgs e)
        {
            ExtendedWebBrowser pk_browser_current = sender as ExtendedWebBrowser;

            this.pk_status_txt_stat.Text = pk_browser_current.StatusText;
        }

        private void pk_tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            update_front();
        }

        private void pk_tab_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int index = 0; index < pk_tab.TabCount; ++index)
            {
                // keep the last tab
                if (pk_tab.TabCount <= 1)
                    return ;

                if (pk_tab.GetTabRect(index).Contains(e.Location))
                {
                    pk_tab.TabPages[index].Controls["ex"].Dispose();
                    pk_tab.TabPages[index].Dispose();

                    break;
                }
            }
        }

        private void update_front()
        {
            pk_browser_front = (ExtendedWebBrowser)this.pk_tab.SelectedTab.Controls["ex"];
            this.pk_tool_btn_back.Enabled = pk_browser_front.CanGoBack;
            this.pk_tool_btn_forward.Enabled = pk_browser_front.CanGoForward;

            this.pk_browser_front.Focus();

            if (pk_browser_front.Url != null)
            {
                this.pk_tool_txt_url.Text = pk_browser_front.Url.ToString();
            }
        }

        private void set_status_busy(bool is_busy)
        {
            if (is_busy)
            {
                this.pk_status_txt_green_bulb.Image = Properties.Resources.PK_ICO_GREY.ToBitmap();
                this.pk_status_txt_red_bulb.Image = Properties.Resources.PK_ICO_RED.ToBitmap();
            }
            else
            {
                this.pk_status_txt_green_bulb.Image = Properties.Resources.PK_ICO_GREEN.ToBitmap();
                this.pk_status_txt_red_bulb.Image = Properties.Resources.PK_ICO_GREY.ToBitmap();
            }
        }

        private void update_app_caller()
        {
            // read the version file
            using (var client = new WebClient())
            {
                string ver_str = client.DownloadString(Properties.Resources.PK_STR_URL_UPDATE);

                Version ver_new = new Version(ver_str);

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                Version ver_current = new Version(fvi.FileVersion);

                if (ver_new > ver_current)
                {
                    // create a temp copy of application
                    string tmp_file = Path.GetTempFileName() + ".exe";
                    File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, tmp_file, true);

                    // call application with parameter
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.FileName = tmp_file;
                    proc.UseShellExecute = true;
                    proc.Arguments = "update_app";

                    try
                    {
                        Process.Start(proc);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void update_time_caller()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            proc.Arguments = "update_time";
            proc.UseShellExecute = true;
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                proc.Verb = "runas";    // require admin rights
            }

            try
            {
                Process.Start(proc);
            }
            catch (Exception ex)
            {

            }
        }

        private void show_log(bool is_shown)
        {
            this.pk_lv_log.Visible = is_shown;
            this.pk_tool_btn_showlog.Checked = is_shown;
        }

        delegate void log_cb(string txt, int err_level = 0);

        public void log(string txt, int err_level = 0)
        {
            ListViewItem log_item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            log_item.SubItems.Add(txt);
            log_item.ImageIndex = err_level;

            if (this.pk_lv_log.InvokeRequired)
            {
                log_cb log_inv = new log_cb(log);
                this.Invoke(log_inv, new object[] { txt, err_level });
            }
            else
            {
                this.pk_lv_log.Items.Insert(0, log_item);
            }
        }

        private void timer_clock_cb(Object o)
        {
            this.pk_status_txt_time.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}