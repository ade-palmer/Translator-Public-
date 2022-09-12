using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpeechConsoleApp
{
    public class Translator
    {
        public static async Task TranslateFromWav(string wavToTranslate, string language)
        {
            // TODO: Use Azure Key Vault to store KEY as this is unsafe and could be abused
            const string KEY = "60ea--------------------c65c"; // Key from your Azure Speech Service Instance
            const string REGION = "uksouth"; // Region selected when creating Azure Resource

            SpeechTranslationConfig config = SpeechTranslationConfig.FromSubscription(KEY, REGION);

            string translatedText = await TranslateWavToText(wavToTranslate, language, config);

            // TODO: Output translated text to file
            Console.WriteLine(translatedText);

            await ConvertTextToWav(translatedText, config);
        }

        private static async Task<string> TranslateWavToText(string wavToTranslate, string language, SpeechTranslationConfig config)
        {
            string fromLanguage = language;
            string toLanguage = "en";
            string translatedText = string.Empty;

            TaskCompletionSource<int> stopTranslation = new TaskCompletionSource<int>();

            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage(toLanguage);

            using var audioInput = AudioConfig.FromWavFileInput(wavToTranslate);

            using (var recognizer = new TranslationRecognizer(config, audioInput))
            {
                // Subscribes to events.
                recognizer.Recognized += (s, e) => {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                        foreach (var element in e.Result.Translations)
                        {
                            //Console.Write(element.Value);
                            translatedText += element.Value + "\n";

                        }
                    }
                    else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine($"Speech not translated.");
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"Speech could not be recognized.");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"\nCANCELED: Reason = {e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    stopTranslation.TrySetResult(0);
                };

                recognizer.SpeechStartDetected += (s, e) => {
                    Console.WriteLine("\nTranslation started. This may take some time....");
                };

                recognizer.SpeechEndDetected += (s, e) => {
                    Console.WriteLine("\nEnd of translation");
                };

                recognizer.SessionStopped += (s, e) => {
                    stopTranslation.TrySetResult(0);
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopTranslation.Task });

                // Stops translation.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }

            return translatedText;
        }

        private static async Task ConvertTextToWav(string translatedText, SpeechTranslationConfig config)
        {
            string outputAudioWav = "outputaudio.wav";

            config.SpeechSynthesisLanguage = "en-GB";

            using (var fileOutput = AudioConfig.FromWavFileOutput(outputAudioWav))
            using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
            {
                using (var result = await synthesizer.SpeakTextAsync(translatedText))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"\nTranslated text saved to [{outputAudioWav}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}
