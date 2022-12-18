using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using HörbuchcreatorWPF.threads;
using System.IO.Compression;
using System.IO;
using HtmlAgilityPack;
using HörbuchcreatorWPF.Controller;
using HörbuchcreatorWPF.tools;

namespace HörbuchcreatorWPF
{
    public partial class MainWindow : Window
    {
        MainProgram Programm;

        public MainWindow()
        {
            InitializeComponent();

            Programm = new MainProgram();

            LabelFinished.Visibility = Visibility.Hidden;
            LabelFinished.Background = System.Windows.Media.Brushes.Green;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            LabelFinished.Visibility = Visibility.Hidden;
            
            if (Programm.CreateAudioFile(ButtonCreate, ComboBoxLanguage.Text))
            {
                var worker = new MyBackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.speechConfig = Programm.speechconfig;
                worker.audioConfig = Programm.audioconfig;
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

            var paragraphs = Tools.SplitIntoParagraphs(((MyBackgroundWorker)sender).text);
            Programm.sendRequests(paragraphs, ss, sender);

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
            ButtonCreate.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = Programm.ImportEpub();
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Programm != null)
            {
                var voices = Programm.GetVoices("Deutsch");

                ComboBoxVoices.Items.Clear();

                foreach (var voice in voices)
                {
                    ComboBoxVoices.Items.Add(voice);
                }
                
            }
        }
    }
}
