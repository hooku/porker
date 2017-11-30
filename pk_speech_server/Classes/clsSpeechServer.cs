using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace pk_speech_server
{
    enum SPEECH_ENGINE
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

    enum SERVER_MODE
    {
        SRV_FILE_INPUT,
        SRV_SOCKET_DAEMON,
        SRV_MIC_DEBUG,
    };

    enum ENGINE_MODE
    {
        ENG_STREAM_INPUT,
        ENG_MIC_DEBUG,
    };

    class clsEngine
    {
        public virtual string do_recog(string wave_file)
        {
            return "";
        }
    }

    class clsSpeechServer
    {
#if true
        private const SERVER_MODE DEBUG_SERVER_MODE = SERVER_MODE.SRV_FILE_INPUT;
        private const string DEBUG_AUDIO_FILE = "data\\audio_test.wav";
        private const string DEBUG_ENG_MS_GRAMMA_FILE = "data\\speech_gramma.txt";
#else
        private const SERVER_MODE DEBUG_SERVER_MODE = SERVER_MODE.SRV_MIC_DEBUG;
#endif

        clsEngine g_engine;

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

        private bool engine_init(ENGINE_MODE mode)
        {
            return engine_init(mode, DEBUG_ENG_MS_GRAMMA_FILE);
        }

        private bool engine_init(ENGINE_MODE mode, string gramma_file_path)
        {
            bool result = false;

            Program.log("Recognize Engine Init..");
            get_available_engine();

            // only ms engine is available now
            g_engine = new clsEngineMS(mode, gramma_file_path);

            try
            {
            }
            catch (Exception ex)
            {
                Program.log(ex.Message, ERR_LEVEL.ERR_FATAL);
            }

            return result;
        }

        private bool socket_server_init()
        {
            bool result = false;

            // create a tcp server socket

            return result;
        }

        public clsSpeechServer(string[] args)
        {
            SERVER_MODE mode = DEBUG_SERVER_MODE;
            string gramma_file = "", wave_file = "";

            switch (mode)
            {
                case SERVER_MODE.SRV_SOCKET_DAEMON:
                    Program.log("Socket daemon mode");
                    engine_init(ENGINE_MODE.ENG_STREAM_INPUT);
                    socket_server_init();
                    break;
                case SERVER_MODE.SRV_FILE_INPUT:
                    Program.log("File input mode");

                    if (args.Length == 2)
                    {
                        gramma_file = args[0];
                        wave_file = args[1];

                        gramma_file = Path.GetFullPath(gramma_file);
                        wave_file = Path.GetFullPath(wave_file);
                    }
                    else
                    {
                        Program.help();
                        Environment.Exit(1);
                    }

                    engine_init(ENGINE_MODE.ENG_STREAM_INPUT, gramma_file);
                    break;
                case SERVER_MODE.SRV_MIC_DEBUG:
                default:
                    Program.log("Mic Debug mode");
                    engine_init(ENGINE_MODE.ENG_MIC_DEBUG);
                    break;
            }

            g_engine.do_recog(wave_file);
        }
    }
}
