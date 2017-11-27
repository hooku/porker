using System;
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
using System.Runtime.InteropServices;
using System.Configuration;

namespace porker
{
    public enum EXEC_ICON
    {
        EXEC_IE = 0,
        EXEC_RED = 1,
        EXEC_GREEN = 2,
        EXEC_GREY = 3,
    }

    class ToolStripSpringTextBox : ToolStripTextBox
    {
        // docs.microsoft.com/en-us/dotnet/framework/winforms/controls/stretch-a-toolstriptextbox-to-fill-the-remaining-width-of-a-toolstrip-wf
        public override Size GetPreferredSize(Size constrainingSize)
        {
            // Use the default size if the text box is on the overflow menu
            // or is on a vertical ToolStrip.
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
            {
                return DefaultSize;
            }

            // Declare a variable to store the total available width as 
            // it is calculated, starting with the display width of the 
            // owning ToolStrip.
            Int32 width = Owner.DisplayRectangle.Width;

            // Subtract the width of the overflow button if it is displayed. 
            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width -
                    Owner.OverflowButton.Margin.Horizontal;
            }

            // Declare a variable to maintain a count of ToolStripSpringTextBox 
            // items currently displayed in the owning ToolStrip. 
            Int32 springBoxCount = 0;

            foreach (ToolStripItem item in Owner.Items)
            {
                // Ignore items on the overflow menu.
                if (item.IsOnOverflow) continue;

                if (item is ToolStripSpringTextBox)
                {
                    // For ToolStripSpringTextBox items, increment the count and 
                    // subtract the margin width from the total available width.
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                {
                    // For all other items, subtract the full width from the total
                    // available width.
                    width = width - item.Width - item.Margin.Horizontal;
                }
            }

            // If there are multiple ToolStripSpringTextBox items in the owning
            // ToolStrip, divide the total available width between them. 
            if (springBoxCount > 1) width /= springBoxCount;

            // If the available width is less than the default width, use the
            // default width, forcing one or more items onto the overflow menu.
            if (width < DefaultSize.Width) width = DefaultSize.Width;

            // Retrieve the preferred size from the base class, but change the
            // width to the calculated width. 
            Size size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }
    }

    class ListViewNF : ListView
    {
        public ListViewNF()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }

    public class INIParser
    {
        string ini_path;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpDefault,
           StringBuilder lpReturnedString,
           uint nSize,
           string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WritePrivateProfileString(string lpAppName,
           string lpKeyName, string lpString, string lpFileName);

        public INIParser(string path)
        {
            ini_path = path;
        }

        public string read(string key)
        {
            string section = Properties.Resources.PK_STR_APP_NAME;
            var result = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", result, 255, ini_path);
            return result.ToString();
        }

        public void write(string key, string value)
        {
            string section = Properties.Resources.PK_STR_APP_NAME;
            WritePrivateProfileString(section, key, value, ini_path);
        }
    }

    public partial class frmBrowser : Form
    {
        private ToolStrip pk_tool_bra;
        private ToolStripButton pk_tool_btn_back;
        private ToolStripButton pk_tool_btn_forward;
        private ToolStripSeparator pk_tool_sep1;
        private ToolStripSpringTextBox pk_tool_txt_url;
        private ToolStripButton pk_tool_btn_refresh;
        private ToolStripSeparator pk_tool_sep2;
        private ToolStripButton pk_tool_btn_login;
        private ToolStripButton pk_tool_btn_run;
        private ToolStripButton pk_tool_btn_config;
        private ToolStripSeparator pk_tool_sep3;
        private ToolStripButton pk_tool_btn_help;
        private ToolStripButton pk_tool_btn_showlog;
        private ToolStripButton pk_tool_btn_updatetime;
        private ToolStripButton pk_tool_btn_updateapp;

        private ListViewNF pk_lv_log;

        private TabControl pk_tab;
        private ImageList pk_imglist_log;
        private ImageList pk_imglist_tab;

        private StatusStrip pk_status_bra;
        private ToolStripStatusLabel pk_status_txt_time;
        private ToolStripStatusLabel pk_status_txt_stat;
        private ToolStripStatusLabel pk_status_txt_green_bulb;
        private ToolStripStatusLabel pk_status_txt_red_bulb;

        private Timer pk_timer_play;

        ExtendedWebBrowser pk_browser_front;
        clsWebHelper web_helper;
        System.Threading.Timer timer_clock;

        private const int PK_LV_LOG_WIDTH = 260;    //px
        private const int PK_LV_LOG_TIME_WIDTH = 70;
        private const int PK_LV_LOG_TXT_WIDTH = 185;

        private const int TIMER_CLOCK_REFRESH_INTERVAL = 1000;

        private bool pk_play = false;

        private void create_form_components()
        {
            // form
            this.Icon = Properties.Resources.PK_ICO_APP;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text = Properties.Resources.PK_STR_APP_NAME + " " + fvi.FileVersion;

            // tool bar
            this.pk_tool_bra = new ToolStrip();
            this.pk_tool_btn_back = new ToolStripButton();
            this.pk_tool_btn_forward = new ToolStripButton();
            this.pk_tool_sep1 = new ToolStripSeparator();
            this.pk_tool_txt_url = new ToolStripSpringTextBox();
            this.pk_tool_btn_refresh = new ToolStripButton();
            this.pk_tool_sep2 = new ToolStripSeparator();
            this.pk_tool_btn_login = new ToolStripButton();
            this.pk_tool_btn_run = new ToolStripButton();
            this.pk_tool_btn_config = new ToolStripButton();
            this.pk_tool_sep3 = new ToolStripSeparator();
            this.pk_tool_btn_help = new ToolStripButton();
            this.pk_tool_btn_showlog = new ToolStripButton();
            this.pk_tool_btn_updatetime = new ToolStripButton();
            this.pk_tool_btn_updateapp = new ToolStripButton();

            this.pk_tool_bra.Dock = DockStyle.Top;
            this.pk_tool_bra.ImageScalingSize = new Size(24, 24);

            this.pk_tool_btn_back.Image = Properties.Resources.PK_ICO_LEFT_24.ToBitmap();
            this.pk_tool_btn_back.Text = Properties.Resources.PK_STR_BACK;
            this.pk_tool_btn_back.Click += new System.EventHandler(this.pk_tool_btn_back_Click);

            this.pk_tool_btn_forward.Image = Properties.Resources.PK_ICO_RIGHT_24.ToBitmap();
            this.pk_tool_btn_forward.Click += new System.EventHandler(this.pk_tool_btn_forward_Click);

            //this.pk_tool_txt_url.AutoSize = false;
            this.pk_tool_txt_url.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pk_took_txt_url_KeyDown);

            this.pk_tool_btn_refresh.Image = Properties.Resources.PK_ICO_REFRESH_24.ToBitmap();
            this.pk_tool_btn_refresh.Click += new System.EventHandler(this.pk_tool_btn_refresh_Click);

            this.pk_tool_btn_login.Image = Properties.Resources.PK_ICO_KEY_24.ToBitmap();
            this.pk_tool_btn_login.Text = Properties.Resources.PK_STR_LOGIN;
            this.pk_tool_btn_login.Click += new System.EventHandler(this.pk_tool_btn_login_Click);

            this.pk_tool_btn_run.Image = Properties.Resources.PK_ICO_EXEC_24.ToBitmap();
            this.pk_tool_btn_run.Text = Properties.Resources.PK_STR_RUN;
            this.pk_tool_btn_run.Click += new System.EventHandler(this.pk_tool_btn_run_Click);

            this.pk_tool_btn_config.Image = Properties.Resources.PK_ICO_CONFIG_24.ToBitmap();
            this.pk_tool_btn_config.Text = Properties.Resources.PK_STR_CONFIG;
            this.pk_tool_btn_config.Click += new System.EventHandler(this.pk_tool_btn_config_Click);

            this.pk_tool_btn_help.Image = Properties.Resources.PK_ICO_HELP_24.ToBitmap();
            this.pk_tool_btn_help.Text = Properties.Resources.PK_STR_HELP;
            this.pk_tool_btn_help.Click += new System.EventHandler(this.pk_tool_btn_help_Click);

            this.pk_tool_btn_showlog.Image = Properties.Resources.PK_ICO_LOG_24.ToBitmap();
            this.pk_tool_btn_showlog.Text = Properties.Resources.PK_STR_SHOWLOG;
            this.pk_tool_btn_showlog.Click += new System.EventHandler(this.pk_tool_btn_showlog_Click);

            this.pk_tool_btn_updatetime.Image = Properties.Resources.PK_ICO_TIMESYNC_24.ToBitmap();
            this.pk_tool_btn_updatetime.Text = Properties.Resources.PK_STR_UPDATETIME;
            this.pk_tool_btn_updatetime.Click += new System.EventHandler(this.pk_tool_btn_updatetime_Click);

            this.pk_tool_btn_updateapp.Image = Properties.Resources.PK_ICO_UPDATE_24.ToBitmap();
            this.pk_tool_btn_updateapp.Text = Properties.Resources.PK_STR_UPDATEAPP;
            this.pk_tool_btn_updateapp.Click += new System.EventHandler(this.pk_tool_btn_updateapp_Click);

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
                this.pk_tool_btn_config,
                this.pk_tool_sep3,
                this.pk_tool_btn_help,
                this.pk_tool_btn_showlog,
                this.pk_tool_btn_updatetime,
                this.pk_tool_btn_updateapp
            });

            this.pk_tool_bra.ResumeLayout();

            // listview
            this.pk_imglist_log = new ImageList();
            this.pk_lv_log = new ListViewNF();

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
            this.pk_imglist_tab.Images.Add(Properties.Resources.PK_ICO_EXECGREEN_16);
            this.pk_imglist_tab.Images.Add(Properties.Resources.PK_ICO_EXECGREY_16);

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

            this.pk_timer_play = new Timer();
            this.pk_timer_play.Interval = Properties.Settings.Default.PK_PLAY_REQ;
            this.pk_timer_play.Tick += new System.EventHandler(this.pk_timer_play_Tick);

            // timer
            timer_clock = new System.Threading.Timer(timer_clock_cb, null, 0, TIMER_CLOCK_REFRESH_INTERVAL);

            // other class
            web_helper = new clsWebHelper();

            show_log(false);
        }

        public frmBrowser()
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

            ExtendedWebBrowser ex_browser = new ExtendedWebBrowser();
            ex_browser.Name = "ex";
            ex_browser.Parent = ex_tab;
            ex_browser.Dock = DockStyle.Fill;
            ex_browser.ScriptErrorsSuppressed = true;
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
            WebBrowserNavigatedEventArgs web_e = new WebBrowserNavigatedEventArgs(
                this.pk_browser_front.Url);

            this.pk_browser_front.Refresh(WebBrowserRefreshOption.Completely);
            pk_browser_Navigated(this.pk_browser_front, web_e);
        }

        private void pk_tool_btn_login_Click(object sender, EventArgs e)
        {
            Program.log(Properties.Resources.PK_STR_LOG_LOGIN);
            this.web_helper.pkh_login(this.pk_browser_front);
        }

        private void pk_tool_btn_run_Click(object sender, EventArgs e)
        {
            play_porker();
        }

        private void pk_tool_btn_config_Click(object sender, EventArgs e)
        {
            Program.log(Properties.Resources.PK_STR_LOG_CONFIG);
            edit_config();
        }

        private void pk_tool_btn_help_Click(object sender, EventArgs e)
        {
            this.pk_browser_front.Navigate(Properties.Settings.Default.PK_URL_HELP);
        }

        private void pk_tool_btn_showlog_Click(object sender, EventArgs e)
        {
            show_log(!this.pk_tool_btn_showlog.Checked);
            this.web_helper.pkh_post_amend(pk_browser_front);
        }

        private void pk_tool_btn_updatetime_Click(object sender, EventArgs e)
        {
            Program.update_time_caller();
        }

        private void pk_tool_btn_updateapp_Click(object sender, EventArgs e)
        {
            Program.update_app_caller();
        }

        private void frmBrowser_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            poker_add_page(Properties.Settings.Default.PK_HOMEPAGE);
            update_front();

            frmLogin frm_login = new frmLogin();
            frm_login.ShowDialog(this);
        }

        private void pk_browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            ExtendedWebBrowser pk_browser_current = sender as ExtendedWebBrowser;
            TabPage pk_tab_parent;

            // if current browser is active
            if (pk_browser_current == pk_browser_front)
            {
                update_front();
            }

            pk_tab_parent = (TabPage)pk_browser_current.Parent;
            if (pk_tab_parent.ImageIndex != (int)EXEC_ICON.EXEC_IE)
            {
                pk_tab_parent.ImageIndex = (int)EXEC_ICON.EXEC_IE;
            }

            Program.log(Properties.Resources.PK_STR_LOG_NAV + pk_browser_current.Url.ToString());
        }

        private void pk_browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ExtendedWebBrowser pk_browser_current = sender as ExtendedWebBrowser;

            pk_browser_current.Parent.Text = pk_browser_current.Document.Title;
            this.web_helper.pkh_post_amend(pk_browser_current);
            pk_browser_current.Document.Body.MouseDown += new HtmlElementEventHandler(pk_play_mousedown_hook);
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
                    return;

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
            string url = "";

            pk_browser_front = (ExtendedWebBrowser)this.pk_tab.SelectedTab.Controls["ex"];
            this.pk_tool_btn_back.Enabled = pk_browser_front.CanGoBack;
            this.pk_tool_btn_forward.Enabled = pk_browser_front.CanGoForward;

            this.pk_browser_front.Focus();

            if (pk_browser_front.Url != null)
            {
                if (!pk_browser_front.Url.ToString().Contains(Properties.Resources.PK_STR_HIDE_URL))
                {
                    url = pk_browser_front.Url.ToString();
                }
                this.pk_tool_txt_url.Text = url;
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

        private void play_porker()
        {
            if (pk_play == false)
            {
                Program.log(Properties.Resources.PK_STR_LOG_PLAY);
                pk_play = true;
                this.pk_timer_play.Start();
            }
            else
            {
                pk_play = false;
                this.pk_timer_play.Stop();
            }
            this.pk_tool_btn_run.Checked = pk_play;
            set_status_busy(pk_play);
        }

        private void edit_config()
        {
            string tmp_file = Path.GetTempFileName();
            INIParser ini_parser = new INIParser(tmp_file);

            foreach (SettingsProperty current_prop in Properties.Settings.Default.Properties)
            {
                ini_parser.write(current_prop.Name, Properties.Settings.Default[current_prop.Name].ToString());
            }

            var editor = Process.Start(Properties.Resources.PK_STR_CONFIGAPP, tmp_file);
            editor.WaitForExit();

            foreach (SettingsProperty current_prop in Properties.Settings.Default.Properties)
            {
                string value = ini_parser.read(current_prop.Name);

                if (current_prop.PropertyType == typeof(int))
                {
                    Properties.Settings.Default[current_prop.Name] = Int32.Parse(value);
                }
                else
                {
                    Properties.Settings.Default[current_prop.Name] = value;
                }
            }

            Properties.Settings.Default.Save();
            MessageBox.Show(Properties.Resources.PK_STR_CONFIGOK);
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

        private void pk_play_mousedown_hook(Object sender, HtmlElementEventArgs e)
        {
            this.web_helper.pkh_post_amend(pk_browser_front);
        }

        private void pk_timer_play_Tick(object sender, EventArgs e)
        {
            TabPage pk_tab_play;
            ExtendedWebBrowser pk_browser_play;
            int icon_exec_index;

            // iterate all webbrowser
            foreach (Control pk_control_play in this.pk_tab.Controls)
            {
                pk_tab_play = (TabPage)pk_control_play;
                pk_browser_play = (ExtendedWebBrowser)pk_tab_play.Controls["ex"];

                if ((pk_browser_play.ReadyState == WebBrowserReadyState.Complete) &&
                    (pk_tab_play.ImageIndex != (int)EXEC_ICON.EXEC_GREY))
                {
                    icon_exec_index = web_helper.pkh_play(pk_browser_play);

                    if (pk_tab_play.ImageIndex != icon_exec_index)
                    {
                        pk_tab_play.ImageIndex = icon_exec_index;
                    }
                }
            }
        }
    }
}
