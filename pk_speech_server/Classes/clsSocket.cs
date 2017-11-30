using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace pk_speech_server.Classes
{
    class clsSocket
    {
        private const string SOCKET_IP_ADDR = "0.0.0.0";
        private const int SOCKET_IP_PORT = 5222;

        private TcpListener tcp_server;

        public clsSocket()
        {
            socket_init(SOCKET_IP_ADDR, SOCKET_IP_PORT);
        }

        private bool socket_init(string ip_addr_str, int ip_port)
        {
            bool result = false;

            try
            {
                IPAddress ip_addr = IPAddress.Parse(ip_addr_str);
                tcp_server = new TcpListener(ip_addr, ip_port);


                tcp_server.Start();
            }
            catch (SocketException e)
            {
                Program.log(e.Message, ERR_LEVEL.ERR_FATAL);
                tcp_server.Stop();
            }

            return result;
        }

        private bool socket_destory()
        {
            bool result = false;

            tcp_server.Stop();

            return result;
        }
    }
}
