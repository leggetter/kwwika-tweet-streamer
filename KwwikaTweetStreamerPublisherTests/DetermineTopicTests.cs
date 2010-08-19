using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KwwikaTweetStreamerPublisher;
using TweetStreamer;

namespace KwwikaTweetStreamerPublisherTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DetermineTopicTests
    {
        public DetermineTopicTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void SearchDefinition_with_trackfor_matches_screenname_and_returns_the_topic()
        {
            var text = "RT:@leggetter No way!";
            var def = CreateSearchDefinition("/TEST", "leggetter", text);

            var update = new Dictionary<string, string>();
            update["Text"] = text;
            IStatusMessage msg = new StatusMessage(text);
            string[] topics = Publisher.DetermineTopicFromTwitterMessage(update, msg, new SearchDefinition[] { def });

            Assert.AreEqual(1, topics.Length);
        }
        
        [TestMethod]
        public void SearchDefinition_with_Retweet_using_colon_Ignore_match_defined_does_not_add_the_topic()
        {
            TestIgnore("RT:@leggetter No way!");
        }

        [TestMethod]
        public void SearchDefinition_with_Retweet_with_space_Ignore_match_defined_does_not_add_the_topic()
        {
            TestIgnore("RT @leggetter No way!");
        }

        [TestMethod]
        public void SearchDefinition_with_Retweet_preceeded_by_nonwhitespace_char_with_space_in_middle_of_tweet_Ignore_match_defined_does_not_add_the_topic()
        {
            TestIgnore( "He's right >RT @leggetter No way!" );
        }

        [TestMethod]
        public void SearchDefinition_with_Retweet_with_space_in_middle_of_tweet_Ignore_match_defined_does_not_add_the_topic()
        {
            TestIgnore("He's right > RT @leggetter No way!");
        }

        [TestMethod]
        public void Only_count_field_should_be_present_if_all_but_one_SearchDefinition_ignore_tweet()
        {
            var text = "He's right > RT @leggetter No way!";
            var ignoreDef = CreateSearchDefinition("/TEST", "leggetter", text, "Text", @"(\s|\W|^)RT(\s|\W|$)", false);

            var countOnlyDef = new SearchDefinition();
            countOnlyDef.FieldsToSend = "RetweetCount";
            countOnlyDef.CountFields = new CountField[1];
            countOnlyDef.CountFields[0] = new CountField();
            countOnlyDef.CountFields[0].Match = new MatchExpression();
            countOnlyDef.CountFields[0].Match.FieldToMatch = "Text";
            countOnlyDef.CountFields[0].Match.Expression = @"(\s|\W|^)RT(\s|\W|$)";

            var update = new Dictionary<string, string>();
            update["Text"] = text;
            update["RetweetCount"] = "1";

            string[] fields = Publisher.DetermineFieldsToSend(update, new SearchDefinition[] { ignoreDef, countOnlyDef });

            Assert.AreEqual(1, fields.Length);
            Assert.AreEqual("RetweetCount", fields[0]);
        }

        // Helper methods
        public void TestIgnore(string text)
        {
            var def = CreateSearchDefinition("/TEST", "leggetter", text, "Text", @"(\s|\W|^)RT(\s|\W|$)", false);

            var update = new Dictionary<string, string>();
            update["Text"] = text;
            IStatusMessage msg = new StatusMessage(text);
            string[] topics = Publisher.DetermineTopicFromTwitterMessage(update, msg, new SearchDefinition[] { def });

            Assert.AreEqual(0, topics.Length, "RT should have been ignored");
        }


        private SearchDefinition CreateSearchDefinition(string publishTo, string trackFor, string tweetMsg)
        {
            return CreateSearchDefinition(publishTo, trackFor, tweetMsg, null, null, false);
        }
        private SearchDefinition CreateSearchDefinition(string publishTo, string trackFor, string tweetMsg, string fieldToIgnore, string ignoreExpression, bool ignoreCase)
        {
            SearchDefinition def = new SearchDefinition();
            def.PublishTo = publishTo;
            def.TrackFor = trackFor;

            if (string.IsNullOrEmpty(fieldToIgnore) == false)
            {
                def.Ignore = new MatchExpression[1];
                def.Ignore[0] = new MatchExpression();
                def.Ignore[0].FieldToMatch = fieldToIgnore;
                def.Ignore[0].Expression = ignoreExpression;
                def.Ignore[0].IgnoreCase = ignoreCase;
            }
            return def;
        }
    }
}
