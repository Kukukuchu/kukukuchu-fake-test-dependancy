using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAKENugetDemo
{
    public class QuoteGenerator
    {
        public string GiveMeRandomQuote()
        {
            var a = 1;
            var quotes = new List<string>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var reader = new StreamReader(assembly.GetManifestResourceStream("FAKENugetDemo.Quotes.txt"), Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    quotes.Add(line);
            }

            var rangen = new Random();
            int index = rangen.Next(quotes.Count - 1);
            return string.Format("\"{0}\"", quotes[index]);
        }
    }
}
