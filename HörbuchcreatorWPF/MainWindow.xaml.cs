using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using HörbuchgreatorWPF.threads;
using System.IO.Compression;
using System.IO;
using HtmlAgilityPack;

namespace HörbuchgreatorWPF
{
    public partial class MainWindow : Window
    {
        SpeechConfig config;
        AudioConfig audioconfig;

        public MainWindow()
        {
            InitializeComponent();
            LabelFinished.Visibility = Visibility.Hidden;
            LabelFinished.Background = System.Windows.Media.Brushes.Green;

            config = SpeechConfig.FromSubscription("678ac92517fc40ffa012d83204f2e979", "germanywestcentral");
        }

        private async void ButtonGreate_Click(object sender, RoutedEventArgs e)
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
                
                LabelFinished.Visibility = Visibility.Hidden;
                ButtonGreate.IsEnabled = false;

                // Get the selected file path
                string filePath = dialog.FileName;

                // Save the file with the specified name
                // (You would need to implement your own code for this)
                audioconfig = AudioConfig.FromWavFileOutput(filePath);

                var worker = new MyBackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.speechConfig = config;
                worker.audioConfig = audioconfig;
                worker.text = TextBoxInput.Text;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_Progress;
                worker.RunWorkerCompleted += worker_Finished;
                worker.RunWorkerAsync();
                                
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //sender ist dere Worker von dem ich diese Methode aus aufrufe
            var ss = new SpeechSynthesizer(((MyBackgroundWorker)sender).speechConfig, ((MyBackgroundWorker)sender).audioConfig);

            var paragraphs = SplitIntoParagraphs(((MyBackgroundWorker) sender).text);
            sendRequests(paragraphs, ss, sender);

            ss.Dispose();
        }

        void worker_Progress(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarMain.Value = e.ProgressPercentage;
        }

        void worker_Finished(object sender, RunWorkerCompletedEventArgs e) 
        {
            LabelFinished.Background = System.Windows.Media.Brushes.Green;
            LabelFinished.Visibility = Visibility.Visible;
            ButtonGreate.IsEnabled = true;
        }

        private static List<string> SplitIntoParagraphs(string text)
        {
            // Liste, in der die Absätze gespeichert werden
            List<string> paragraphs = new List<string>();

            // Aufteilen des Texts in Absätze
            string[] substrings = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Hinzufügen jedes Unterstrings zur Liste der Absätze
            foreach (string substring in substrings)
            {
                string trimmedSubstring = substring.Trim();
                if (!string.IsNullOrEmpty(trimmedSubstring))
                {
                    paragraphs.Add(trimmedSubstring);
                }
            }

            return paragraphs;
        }

        private void sendRequests(List<string> requests, SpeechSynthesizer SS, object sender)
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

        private void Button_Click(object sender, RoutedEventArgs e)
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

                TextBoxInput.Text = text;
            }
        }
    }
}
