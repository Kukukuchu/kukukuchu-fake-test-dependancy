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
            var quotes = File.ReadAllLines(@".\Quotes.txt");
            var rangen = new Random();
            int index = rangen.Next(quotes.Length - 1);
            return string.Format("\"{0}\"", quotes[index]);
        }
    }
}
