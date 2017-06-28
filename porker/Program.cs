﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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

    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        static frmMain frm_inst;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
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
                frm_inst = new frmMain();
                Application.Run(frm_inst);
            }
        }

        // stackoverflow.com/questions/1193955
        private static DateTime GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";    //"time.pool.aliyun.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        static void update_time()
        {
            DateTime ntp_time = GetNetworkTime();

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

        static void update_app()
        {
            if (true)
            {

                // download latest file


                MessageBox.Show("OK!");

                // remove old file

                // switch to newest file

            }
        }

        public static void log(string txt, int err_level = 0)
        {
            frm_inst.log(txt, err_level);
        }
    }
}