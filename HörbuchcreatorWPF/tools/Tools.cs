using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HörbuchcreatorWPF.tools
{
    internal class Tools
    {
        public static List<string> SplitIntoParagraphs(string text)
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
    }
}
