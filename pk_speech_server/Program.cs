using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace pk_speech_server
{
    public enum ERR_LEVEL
    {
        ERR_INFO,
        ERR_WARN,
        ERR_FATAL,
    }

    static class Program
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public const bool PK_SRV_DEBUG = true;
        private const bool PK_SRV_VERBOSE = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                // Startup as service.
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new ServiceWrapper() 
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                // Startup as application
                AllocConsole();
                clsServer server = new clsServer();

                Console.ReadKey();
            }
        }

        public static void log(string txt, ERR_LEVEL err_level = ERR_LEVEL.ERR_INFO)
        {
            switch (err_level)
            {
                case ERR_LEVEL.ERR_INFO:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (PK_SRV_VERBOSE == true)
                    {
                        Console.Write("[INFO] ");
                    }
                    break;
                case ERR_LEVEL.ERR_WARN:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (PK_SRV_VERBOSE == true)
                    {
                        Console.Write("[WARN] ");
                    }
                    break;
                case ERR_LEVEL.ERR_FATAL:
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (PK_SRV_VERBOSE == true)
                    {
                        Console.Write("[FATL] ");
                    }
                    break;
            }

            Console.WriteLine(txt, err_level);
        }
    }
}
