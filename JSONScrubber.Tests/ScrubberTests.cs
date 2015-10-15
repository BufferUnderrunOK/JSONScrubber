using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JSONScrubber.Tests
{
    [TestClass]
    public class ScrubberTests
    {
        private string _samplePayload = @"{
  ""wholesaler"":""US Foods"",
  ""delivered"":""2015-06-19T05:15:00-0500"",
  ""contacts"": [
    {
      ""wholesaler"":""US Foods"",
      ""name"":""John Lederer""
    },
    {
      ""wholesaler"":""Sysco"",
      ""name"":""Bill Delaney""
    }
  ]
}";
        private Scrubber _scrubber;

        [TestInitialize]
        public void SetUp() 
        {
            _scrubber = new Scrubber();
        }

        [TestMethod]
        public void ScrubUsingHoeggPayload()
        {
            var results = _scrubber.Scrub(_samplePayload);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.contacts.Count);
            Assert.IsFalse(((IDictionary<string,object>) results.contacts[1]).ContainsKey("wholesaler"));
        }
    }
}
