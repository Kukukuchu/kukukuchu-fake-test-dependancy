using System;
using NUnit.Framework;
using FAKENugetDemo;

namespace NUnitTest.FAKENugetDemo
{
    [TestFixture]
    public class QuoteGeneratorTest
    {
        [Test]
        public void QuoteStartsWithQuotes()
        {
            var quotegen = new QuoteGenerator();
            string quote = quotegen.GiveMeRandomQuote();
            StringAssert.StartsWith("\"", quote, "The quote should start with quotation mark");
        }

        [Test]
        public void QuoteEndsWithQuotes()
        {
            var quotegen = new QuoteGenerator();
            string quote = quotegen.GiveMeRandomQuote();
            StringAssert.EndsWith("\"", quote, "The quote should end with quotation mark");
        }
    }
}