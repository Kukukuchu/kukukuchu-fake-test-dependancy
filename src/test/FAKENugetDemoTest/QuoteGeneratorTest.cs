using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FAKENugetDemo
{
    [TestClass]
    public class QuoteGeneratorTest
    {
        [TestMethod]
        public void QuoteStartsWithQuotes()
        {
            var quotegen = new QuoteGenerator();
            string quote = quotegen.GiveMeRandomQuote();
            StringAssert.StartsWith(quote, "\"");
        }

        [TestMethod]
        public void QuoteEndsWithQuotes()
        {
            var quotegen = new QuoteGenerator();
            string quote = quotegen.GiveMeRandomQuote();
            StringAssert.EndsWith(quote, "\"");
        }
    }
}
