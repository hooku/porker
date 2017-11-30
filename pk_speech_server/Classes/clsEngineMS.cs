using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Speech.Recognition;
using Microsoft.Speech.Recognition;
using System.IO;

namespace pk_speech_server
{
    class clsEngineMS : clsEngine
    {
        /* 
         * Gramma Syntax Example:
         * MS_CHOICE: (one short sentence per line)
         *  喂
         *  你好
         *  详细介绍一下
         */
        private const bool ENG_MS_USE_FILE = true;
        private const string ENG_MS_CULTURE = "zh-CN";
        private const int ENG_MS_SLIENCE_TIMEOUT = 1;   // sec

        enum MS_GRAMMA
        {
            MS_CHOICE,
            MS_SRGS,
            MS_SRGS_XML,
        };

        SpeechRecognitionEngine g_engine = null;
        ENGINE_MODE g_mode = ENGINE_MODE.ENG_MIC_DEBUG;

        public clsEngineMS(ENGINE_MODE mode, string gramma_file_path)
        {
            try
            {
                // create the engine
                g_engine = engine_ms_create(ENG_MS_CULTURE);

                g_mode = mode;

                // hook to events
                g_engine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(engine_ms_audiolevel_updated);
                g_engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_ms_speech_recognized);

                // load dictionary
                if (File.Exists(gramma_file_path))
                {
                    engine_ms_load_gramma(gramma_file_path, MS_GRAMMA.MS_CHOICE);
                }
                else
                {
                    Program.log("Gramma file not exist", ERR_LEVEL.ERR_FATAL);
                }
            }
            catch (Exception ex)
            {
                Program.log(ex.Message, ERR_LEVEL.ERR_WARN);
            }
        }

        public override string do_recog(string wave_file)
        {
            string result = "";

            switch (g_mode)
            {
                case ENGINE_MODE.ENG_STREAM_INPUT:
                    if (ENG_MS_USE_FILE)
                    {
                        g_engine.SetInputToWaveFile(wave_file);
                        g_engine.EndSilenceTimeout = new TimeSpan(0, 0, ENG_MS_SLIENCE_TIMEOUT);
                        g_engine.Recognize();
                    }
                    else 
                    {
                        // we use audio stream as the input
                        //g_engine.SetInputToAudioStream();
                    }
                    break;
                case ENGINE_MODE.ENG_MIC_DEBUG:
                default:
                    // use the system's default microphone
                    g_engine.SetInputToDefaultAudioDevice();
                    g_engine.RecognizeAsync(RecognizeMode.Multiple);
                    break;
            }

            return result;
        }

        private SpeechRecognitionEngine engine_ms_create(string preferredCulture)
        {
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (config.Culture.ToString() == preferredCulture)
                {
                    g_engine = new SpeechRecognitionEngine(config);
                    break;
                }
            }

            // if the desired culture is not found, then load default
            if (g_engine == null)
            {
                Program.log("The desired culture is not installed on this machine, the speech-engine will continue using "
                    + SpeechRecognitionEngine.InstalledRecognizers()[0].Culture.ToString() + " as the default culture.", ERR_LEVEL.ERR_WARN);
                g_engine = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers()[0]);
            }

            return g_engine;
        }

        private void engine_ms_load_gramma(string gramma_file, MS_GRAMMA gramma_type)
        {
            switch (gramma_type)
            {
                case MS_GRAMMA.MS_CHOICE:
                    try
                    {
                        Choices choices = new Choices();
                        string[] lines = File.ReadAllLines(gramma_file);
                        foreach (string line in lines)
                        {
                            // add the text to the known choices of speechengine
                            choices.Add(line);
                        }
                        GrammarBuilder gram_builder = new GrammarBuilder(choices);
                        gram_builder.Culture = new System.Globalization.CultureInfo(ENG_MS_CULTURE);
                        Grammar choice_list = new Grammar(gram_builder);
                        g_engine.LoadGrammar(choice_list);
                    }
                    catch (Exception ex)
                    {
                        Program.log(ex.Message, ERR_LEVEL.ERR_FATAL);
                    }
                    break;
                case MS_GRAMMA.MS_SRGS:
                    break;
                case MS_GRAMMA.MS_SRGS_XML:
                    break;
            }
        }

        private void engine_ms_audiolevel_updated(object sender, AudioLevelUpdatedEventArgs e)
        {
            Program.log(e.AudioLevel.ToString());
        }

        private void engine_ms_speech_recognized(object sender, SpeechRecognizedEventArgs e)
        {
            Program.log("Result=" + e.Result.Text);
        }
    }
}
