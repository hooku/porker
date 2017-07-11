using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace porker
{
    [StructLayout(LayoutKind.Sequential)]
    struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public class WebClient : System.Net.WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest local_webrequest = base.GetWebRequest(uri);
            local_webrequest.Timeout = Properties.Settings.Default.PK_WEB_TIMEOUT;
            ((HttpWebRequest)local_webrequest).ReadWriteTimeout = local_webrequest.Timeout;
            return local_webrequest;
        }
    }

    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        static frmBrowser frm_inst;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if false
            update_time();
#endif
#if false
            update_app();
#endif

            if (args.Length >= 1)
            {
                if (args[0] == "update_time")
                {
                    update_time();
                }
                else if (args[0] == "update_app")
                {
                    update_app();
                }
                return;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frm_inst = new frmBrowser();
                Application.Run(frm_inst);
            }
        }

        // stackoverflow.com/questions/1193955
        private static DateTime GetNetworkTime()
        {
            string ntp_server = Properties.Settings.Default.PK_NTP_SERVER;

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntp_data = new byte[48];

            var network_date_time = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            //Setting the Leap Indicator, Version Number and Mode values
            ntp_data[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntp_server).AddressList;

            //The UDP port number assigned to NTP is 123
            var ip_end_point = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ip_end_point);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            try
            {
                socket.Send(ntp_data);
                socket.Receive(ntp_data);
                socket.Close();
            }
            catch (Exception ex)
            {
                return network_date_time;
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte server_reply_time = 40;

            //Get the seconds part
            ulong int_part = BitConverter.ToUInt32(ntp_data, server_reply_time);

            //Get the seconds fraction
            ulong fract_part = BitConverter.ToUInt32(ntp_data, server_reply_time + 4);

            //Convert From big-endian to little-endian
            int_part = SwapEndianness(int_part);
            fract_part = SwapEndianness(fract_part);

            var milliseconds = (int_part * 1000) + ((fract_part * 1000) / 0x100000000L);

            //**UTC** time
            return network_date_time.AddMilliseconds((long)milliseconds);
        }

        // stackoverflow.com/a/3294698/162671
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        private static void update_time()
        {
            DateTime ntp_time = GetNetworkTime();
            if (ntp_time.Year == 1900)
            {
                MessageBox.Show(Properties.Resources.PK_STR_LOG_SYNCERR);
            }
            else
            {
                SYSTEMTIME st = new SYSTEMTIME();
                st.wYear = (ushort)ntp_time.Year;
                st.wMonth = (ushort)ntp_time.Month;
                st.wDay = (ushort)ntp_time.Day;
                st.wHour = (ushort)ntp_time.Hour;
                st.wMinute = (ushort)ntp_time.Minute;
                st.wSecond = (ushort)ntp_time.Second;

                SetSystemTime(ref st);

                MessageBox.Show(Properties.Resources.PK_STR_LOG_SYNCOK);
            }
        }

        private static void update_app()
        {
            string tmp_file = Path.GetTempFileName();
            string original_file;

            FileVersionInfo porker_ver_info = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

            try
            {
                using (var client = new WebClient())
                {
                    // download latest file
                    client.DownloadFile(Properties.Resources.PK_STR_URL_UPDATE + porker_ver_info.OriginalFilename, tmp_file);
                }
            }
            catch
            {
                MessageBox.Show(Properties.Resources.PK_STR_UPDATEERR);
                return ;
            }

            // find process path
            Process[] pname = Process.GetProcessesByName(porker_ver_info.ProductName);
            if (pname.Length > 0)
            {
                original_file = pname[0].MainModule.FileName;

                // kill process
                pname[0].Kill();
                pname[0].WaitForExit();

                // remove old file
                File.Delete(original_file);

                // switch to new file
                File.Move(tmp_file, original_file);

                MessageBox.Show(Properties.Resources.PK_STR_UPDATEOK);

                // start new file
                Process.Start(original_file);
            }
            else
            {
                MessageBox.Show(Properties.Resources.PK_STR_UPDATEERR);
            }
        }

        public static void update_time_caller()
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

        public static void update_app_caller(bool prompt_update_not_found = true)
        {
            // read the version file
            using (var client = new WebClient())
            {
                string ver_str = "";
                Version ver_new = null;
                try
                {
                    ver_str = client.DownloadString(Properties.Resources.PK_STR_URL_UPDATE);
                    ver_new = new Version(ver_str);
                }
                catch (Exception ex)
                {

                }

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                Version ver_current = new Version(fvi.FileVersion);

                if (ver_new > ver_current)
                {
                    if (MessageBox.Show(Properties.Resources.PK_STR_UPDATEFOUND + ver_str, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.Yes)
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
                else if (prompt_update_not_found)
                {
                    MessageBox.Show(Properties.Resources.PK_STR_UPDATENOTFOUND, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void log(string txt, int err_level = 0)
        {
            frm_inst.log(txt, err_level);
        }
    }
}
