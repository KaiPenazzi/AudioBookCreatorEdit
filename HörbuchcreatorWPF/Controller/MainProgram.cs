using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using System.Windows.Controls;
using HtmlAgilityPack;
using System.Linq;
//using System.Windows.Forms;

namespace HörbuchcreatorWPF.Controller
{
    public class MainProgram
    {
        public SpeechConfig speechconfig { get; set; }
        public AudioConfig audioconfig { get; set; }

        public MainProgram()
        {
            speechconfig = SpeechConfig.FromSubscription("678ac92517fc40ffa012d83204f2e979", "germanywestcentral");
            audioconfig = AudioConfig.FromWavFileOutput("");
        }

        public bool CreateAudioFile(Button ButtonGreate, string Language, string VoiceName)
        {
            // Create a new common save file dialog
            CommonSaveFileDialog dialog = new CommonSaveFileDialog();

            // Set the file name and filter
            dialog.DefaultFileName = "AudioBook.wav";
            dialog.Filters.Add(new CommonFileDialogFilter("Audio Files", "*.wav"));

            // Show the dialog
            CommonFileDialogResult result = dialog.ShowDialog();

            // Check if the user clicked the "OK" button
            if (result == CommonFileDialogResult.Ok)
            {
                ButtonGreate.IsEnabled = false;

                // Get the selected file path
                string filePath = dialog.FileName;

                // Save the file with the specified name
                // (You would need to implement your own code for this)
                audioconfig = AudioConfig.FromWavFileOutput(filePath);
                speechconfig.SpeechSynthesisLanguage = LanguageNameToShort(Language);
                var voiceinfo = GetVoiceInfo(VoiceName, Language);
                speechconfig.SpeechSynthesisVoiceName = voiceinfo.Name;
                return true;
            }
            return false;
        }

        public void sendRequests(List<string> requests, SpeechSynthesizer SS, object sender)
        {
            int max = requests.Count;
            int count = 1;
            foreach (string request in requests)
            {
                sendRequest(request, SS);
                var progressPercentage = ((double)count / max) * 100;
                ((BackgroundWorker)sender).ReportProgress((int)progressPercentage);
                count++;
            }
        }

        private void sendRequest(string request, SpeechSynthesizer SS)
        {
            var res = SS.SpeakTextAsync(request);
            res.Wait();
        }

        public string ImportEpub()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("E-book Files", "*.epub"));

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {

                ZipArchive archiv = ZipFile.OpenRead(dialog.FileName);

                var Names = new List<string>();
                foreach (var entry in archiv.Entries)
                {
                    if (entry.FullName.EndsWith(".html"))
                    {
                        // Ausgabe des Dateinamens
                        Names.Add(entry.FullName);
                    }
                }

                string text = "";
                foreach (var name in Names)
                {
                    var htmlEntry = archiv.GetEntry(name);
                    if (htmlEntry != null)
                    {
                        var reader = new StreamReader(htmlEntry.Open());
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.Load(reader);
                        text += htmlDoc.DocumentNode.InnerText;
                    }
                    else
                    {
                        text += "A page Failed \n";
                    }

                }

                return text;
            }

            return "";
        }

        private string LanguageNameToShort(string language)
        {
            string ret;
            switch (language)
            {
                case "Deutsch":
                    ret = "de-DE";
                    break;
                case "Arabic":
                    ret = "ar-EG";
                    break;
                case "Chinese":
                    ret = "zh-CN";
                    break;
                case "Dutch":
                    ret = "nl-NL";
                    break;
                case "English":
                    ret = "en-US";
                    break;
                case "French":
                    ret = "fr-FR";
                    break;
                case "Hindi":
                    ret = "hi-IN";
                    break;
                case "Italian":
                    ret = "it-IT";
                    break;
                case "Japanese":
                    ret = "ja-JP";
                    break;
                case "Korean":
                    ret = "ko-KR";
                    break;
                case "Portuguese":
                    ret = "pt-BR";
                    break;
                case "Russian":
                    ret = "ru-RU";
                    break;
                case "Spanish":
                    ret = "es-ES";
                    break;
                default:
                    ret = "en-US";
                    break;
            }

            return ret;
        }

        public List<VoiceInfo> GetVoices(string language)
        {
            var synthesizer = new SpeechSynthesizer(speechconfig);

            var ret = synthesizer.GetVoicesAsync(LanguageNameToShort(language));
            ret.Wait();

            List<VoiceInfo> voices = new List<VoiceInfo>();

            for (int i = 0; i < ret.Result.Voices.Count; i++)
            {
                voices.Add(ret.Result.Voices[i]);
            }

            return voices;
        }

        private VoiceInfo GetVoiceInfo(string VoiceName, string Language)
        {
            var voices = GetVoices(Language);

            foreach (var voice in voices)
            {
                if (voice.LocalName == VoiceName)
                {
                    return voice;
                }
            }

            return null;
        }
    }
}
