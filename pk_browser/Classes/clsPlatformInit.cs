using System;
using System.Collections.Generic;
using System.Text;

namespace porker
{
    class clsPlatformInit
    {
        private const int OS_MIN_VER_MAJOR = 6; // At least Windows 7
        private const int OS_MIN_VER_MINOR = 1;
        //private const int 

        public static bool check_os()
        {
            bool result = false;

            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;

            if (os.Platform == PlatformID.Win32NT)
            {
                if ((ver.Major >= OS_MIN_VER_MAJOR) &&
                    (ver.Minor >= OS_MIN_VER_MINOR))
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool check_virtual_audio()
        {
            bool result = false;

            return result;
        }

        public static bool check_speech_rt()
        {
            bool result = false;

            // check the dll first?

            return result;
        }

        public static bool check_python()
        {
            bool result = false;

            return result;
        }

        public static void install_virtual_audio()
        {

        }

        public static void install_speech_rt()
        {

        }

        public static void install_python()
        {

        }
    }
}
