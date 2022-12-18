using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HörbuchcreatorWPF.threads
{
    internal class WorkerParams
    {
        SpeechSynthesizer ss;
        string text;

        public WorkerParams(SpeechSynthesizer ss, string text)
        {
            this.ss = ss;
            this.text = text;
        }

    }
}
