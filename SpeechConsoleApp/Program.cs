using System;

namespace SpeechConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string InputWavFile = "WavToTranslate.wav";

            var usageText = "Add the WAV you want to convert to the same folder as this exe. You must use the following name (WavToTranslate.wav) for the file\n";
            var prompt = "\nSelect a language to translate from (0: Stop): ";

            Console.WriteLine(usageText);

            // List of supported languages:
            // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices

            Console.WriteLine("1. French");
            Console.WriteLine("2. German");
            Console.WriteLine("3. Dutch");
            Console.WriteLine("4. Spanish");

            Console.Write(prompt);

            ConsoleKeyInfo x;

            do
            {
                x = Console.ReadKey();
                Console.WriteLine("");
                switch (x.Key)
                {
                    case ConsoleKey.D1:
                        Translator.TranslateFromWav(InputWavFile, "fr-FR").Wait();
                        break;
                    case ConsoleKey.D2:
                        Translator.TranslateFromWav(InputWavFile, "de-DE").Wait();
                        break;
                    case ConsoleKey.D3:
                        Translator.TranslateFromWav(InputWavFile, "nl-NL").Wait();
                        break;
                    case ConsoleKey.D4:
                        Translator.TranslateFromWav(InputWavFile, "es-ES").Wait();
                        break;
                    case ConsoleKey.D0:
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            } while (x.Key != ConsoleKey.D0);
        }
    }
}
