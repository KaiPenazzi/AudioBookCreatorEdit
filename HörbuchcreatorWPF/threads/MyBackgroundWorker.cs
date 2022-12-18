using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HörbuchgreatorWPF.threads
{
    internal class MyBackgroundWorker : BackgroundWorker
    {
        public AudioConfig audioConfig { get; set; }
        public SpeechConfig speechConfig { get; set; }
        public string text { get; set; }

    }
}
