﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace JSONScrubber.Tests
{
    [TestClass]
    public class ScrubberTests
    {
        private string _samplePayload = @"{
  ""wholesaler"":""US Foods"",
  ""delivered"":""2015-06-19T05:15:00-05:00"",
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

        private string _expectedPayload = @"{
  ""wholesaler"":""US Foods"",
  ""delivered"":""2015-06-19T05:15:00-05:00"",
  ""contacts"": [
    {
      ""name"":""John Lederer""
    },
    {
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
        public void ScrubToExpando_MatchesExpected_UsingSuppliedPayload()
        {
            var results = _scrubber.ScrubToExpando(_samplePayload);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.contacts.Count);
            Assert.IsFalse(((IDictionary<string,object>) results.contacts[1]).ContainsKey("wholesaler"));
        }

        [TestMethod]
        public void ScrubToString_MatchesExpected_UsingSuppliedPayload()
        {
            var results = _scrubber.ScrubToString(_samplePayload);

            Assert.IsNotNull(results);

            // The JSONConverter will make the whitespace different between the payloads
            var expectedStripped = Regex.Replace(_expectedPayload, @"\s+", "");
            var resultsStripped = Regex.Replace(results, @"\s+", "");            

            Assert.AreEqual(expectedStripped, resultsStripped);
        }

        [TestMethod]
        public void ScrubToString_DoesNotExplode_OnEmptyJSON()
        {
            var results = _scrubber.ScrubToString("{}");

            Assert.IsNotNull(results);
            Assert.AreEqual("{}", results);
        }

        [TestMethod]
        public void ScrubToString_WorksAsExpected_ForThreeLevels()
        {
            var payload = @"{
              ""foo"":""bar"",
              ""baz"": [
                {
                  ""ack"": [
                    {
                        ""baz"" : ""boing""
                    }
                    ]                
                }
              ]
            }";

            var expected = @"{""foo"":""bar"",""baz"":[{""ack"":[{}]}]}";

            var results = _scrubber.ScrubToString(payload);

            Assert.IsNotNull(results);
            Assert.AreEqual(expected, results);
        }

    }
}
