using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Speech.Recognition;
using Microsoft.Speech.Recognition;
using System.IO;

namespace pk_speech_server
{
    class clsEngineMS
    {
        private const bool DEBUG_ENG_MS = true; // enable this option to record from mic directly
        private const string DEBUG_ENG_MS_GRAMMA_FILE = "data\\speech_gramma.txt";
        /* 
         * Gramma Syntax Example:
         * MS_CHOICE: (one short sentence per line)
         *  喂
         *  你好
         *  详细介绍一下
         */

        private const string ENG_MS_CULTURE = "zh-CN";

        enum MS_GRAMMA
        {
            MS_CHOICE,
            MS_SRGS,
            MS_SRGS_XML,
        };

        SpeechRecognitionEngine g_engine = null;

        public clsEngineMS()
        {
            try
            {
                string gramma_file_path = Path.Combine(Directory.GetCurrentDirectory(), DEBUG_ENG_MS_GRAMMA_FILE);

                // create the engine
                g_engine = engine_ms_create(ENG_MS_CULTURE);

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

                if (DEBUG_ENG_MS)
                {
                    // use the system's default microphone
                    g_engine.SetInputToDefaultAudioDevice();

                    // start listening
                    g_engine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
            catch (Exception ex)
            {
                Program.log(ex.Message, ERR_LEVEL.ERR_WARN);
            }
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
            Program.log(e.Result.Text);
        }
    }
}
