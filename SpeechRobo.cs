using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace Win10_IOTCore_WebBrowser
{
    public class SpeechRobo
    {
        // Grammer File
        private const string SRGS_FILE = "Grammar\\grammar.xml";
        
        // RED Led Pin
        private const int RED_LED_PIN = 5;
        // GREEN Led Pin
        private const int GREEN_LED_PIN = 6;
        // Bedroom Light Pin
        private const int BEDROOM_LIGHT_PIN = 13;
        // Tag TARGET
        private const string TAG_TARGET = "target";
        // Tag CMD
        private const string TAG_CMD = "cmd";
        // Tag Device
        private const string TAG_DEVICE = "device";


        // On State
        private const string STATE_ON = "ON";
        // Off State
        private const string STATE_OFF = "OFF";
        // LED Device
        private const string DEVICE_LED = "LED";
        // Light Device
        private const string DEVICE_LIGHT = "LIGHT";
        // Red Led
        private const string COLOR_RED = "RED";
        // Green Led
        private const string COLOR_GREEN = "GREEN";
        // Bedroom
        private const string TARGET_BEDROOM = "BEDROOM";
        // Porch
        private const string TARGET_PORCH = "PORCH";
        // Youtube
        private const string TARGET_YOUTUBE = "YOUTUBE";
        // Google
        private const string TARGET_GOOGLE = "GOOGLE";

        // Speech Recognizer
        private SpeechRecognizer recognizer;

        public async void Init()
        {
            // Initialize recognizer
            recognizer = new SpeechRecognizer();

            // Set event handlers
            recognizer.StateChanged += RecognizerStateChanged;
            recognizer.ContinuousRecognitionSession.ResultGenerated += RecognizerResultGenerated;

            // Load Grammer file constraint
            string fileName = String.Format(SRGS_FILE);
            StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(fileName);

            SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);

            // Add to grammer constraint
            recognizer.Constraints.Add(grammarConstraint);

            // Compile grammer
            SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();

            Debug.WriteLine("Grammer Compilation Status: " + compilationResult.Status.ToString());

            // If successful, display the recognition result.
            if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                Debug.WriteLine("Grammer Recognition Result: " + compilationResult.ToString());

                await recognizer.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);
            }
            else
            {
                Debug.WriteLine("Grammer Compilation Status: " + compilationResult.Status);
            }
        }

        public async void Stop()
        {
            // Stop recognizing
            await recognizer.ContinuousRecognitionSession.StopAsync();
            recognizer.Dispose();
            recognizer = null;
        }

        // Recognizer generated results
        private void RecognizerResultGenerated(SpeechContinuousRecognitionSession session, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            // Output debug strings
            Debug.WriteLine(args.Result.Status);
            Debug.WriteLine(args.Result.Text);

            int count = args.Result.SemanticInterpretation.Properties.Count;

            Debug.WriteLine("Count: " + count);
            Debug.WriteLine("Tag: " + args.Result.Constraint.Tag);

            // Check for different tags and initialize the variables
            String target = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_TARGET) ?
                            args.Result.SemanticInterpretation.Properties[TAG_TARGET][0].ToString() :
                            "";

            String cmd = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_CMD) ?
                            args.Result.SemanticInterpretation.Properties[TAG_CMD][0].ToString() :
                            "";

            String device = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_DEVICE) ?
                            args.Result.SemanticInterpretation.Properties[TAG_DEVICE][0].ToString() :
                            "";

            // Whether state is on or off
            bool isOn = cmd.Equals(STATE_ON);

            Debug.WriteLine("Target: " + target + ", Command: " + cmd + ", Device: " + device);

            // First check which device the user refers to
            if (device.Equals(DEVICE_LED))
            {
                // Check what color is specified
                if (target.Equals(COLOR_RED))
                {
                    Debug.WriteLine("RED LED " + (isOn ? STATE_ON : STATE_OFF));
                }
                else if (target.Equals(COLOR_GREEN))
                {
                    Debug.WriteLine("GREEN LED " + (isOn ? STATE_ON : STATE_OFF));
                }
                else
                {
                    Debug.WriteLine("Unknown Target");
                }
            }
            else if (device.Equals(DEVICE_LIGHT))
            {
                // Check target location
                if (target.Equals(TARGET_BEDROOM))
                {
                    Debug.WriteLine("BEDROOM LIGHT " + (isOn ? STATE_ON : STATE_OFF));
                }
                else if (target.Equals(TARGET_PORCH))
                {
                    Debug.WriteLine("PORCH LIGHT " + (isOn ? STATE_ON : STATE_OFF));
                }
                else
                {
                    Debug.WriteLine("Unknown Target");
                }
            }
            else if (target.Equals(TARGET_YOUTUBE))
            {
                Debug.WriteLine("YOUTUBE " + (isOn ? STATE_ON : STATE_OFF));
            }
            else if (target.Equals(TARGET_GOOGLE))
            {
                Debug.WriteLine("GOOGLE " + (isOn ? STATE_ON : STATE_OFF));
            }
            else
            {
                Debug.WriteLine("Unknown Device");
            }

            /*foreach (KeyValuePair<String, IReadOnlyList<string>> child in args.Result.SemanticInterpretation.Properties)
            {
                Debug.WriteLine(child.Key + " = " + child.Value.ToString());

                foreach (String val in child.Value)
                {
                    Debug.WriteLine("Value = " + val);
                }
            }*/
        }

        // Recognizer state changed
        private void RecognizerStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            Debug.WriteLine("Speech recognizer state: " + args.State.ToString());
        }

    }
}
