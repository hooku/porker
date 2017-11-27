using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pk_speech_server
{
    class clsServer
    {
        private enum SPEECH_ENGINE
        {
            UNKNOWN_ENGINE,
            SYSTEM_SPEECH_WIN7,
            SYSTEM_SPEECH_WIN8,     // not supported
            SYSTEM_SPEECH_WIN10,
            MS_SPEECH_11,
            MS_CORTANA,
            ONLINE_BIDU,
            ONLINE_IFLY,
        };

        private SPEECH_ENGINE get_available_engine()
        {
            SPEECH_ENGINE result = SPEECH_ENGINE.UNKNOWN_ENGINE;

            System.Version os_ver = System.Environment.OSVersion.Version;
            switch (os_ver.Major)
            {
                case 6:
                    switch (os_ver.Minor)
                    {
                        case 0:
                            // vista/2008
                            result = SPEECH_ENGINE.UNKNOWN_ENGINE;
                            break;
                        case 1:
                            // win7/2008 r2
                            result = SPEECH_ENGINE.SYSTEM_SPEECH_WIN7;
                            break;
                        case 2:
                            // win8/8.1/2012
                            result = SPEECH_ENGINE.SYSTEM_SPEECH_WIN8;
                            break;
                    }
                    break;
                case 10:
                    // win10/2016
                    result = SPEECH_ENGINE.SYSTEM_SPEECH_WIN10;
                    break;
            }

            return result;
        }

        private bool engine_init()
        {
            bool result = false;

            Program.log("Recognize Engine Init..");
            get_available_engine();

            clsEngineMS eng_ms = new clsEngineMS();

            try
            {
            }
            catch (Exception ex)
            {
                Program.log(ex.Message, ERR_LEVEL.ERR_FATAL);
            }

            return result;
        }

        public clsServer()
        {
            engine_init();
        }
    }
}
