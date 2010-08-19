using TweetStreamer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KwwikaTweetStreamerPublisherTests
{
    
    
    /// <summary>
    ///This is a test class for StatusMessageTest and is intended
    ///to contain all StatusMessageTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatusMessageTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CreateMessageFromJsonString
        ///</summary>
        [TestMethod()]
        public void CreateMessageFromJsonString_serilaizes_Geo_coordinates_Test()
        {
            string messageData = "{\"profile_image_url\":\"http://a1.twimg.com/profile_images/600362977/image_normal.jpg\",\"created_at\":\"Wed, 18 Aug 2010 20:12:53 +0000\",\"from_user\":\"AMonkster\",\"metadata\":{\"result_type\":\"recent\"},\"to_user_id\":null,\"text\":\"#mlc 15191 Old Street to Appold Street\",\"id\":21517146090,\"from_user_id\":5558215,\"geo\":{\"type\":\"Point\",\"coordinates\":[51.5214,-0.081]},\"iso_language_code\":\"en\",\"place\":{\"id\":\"ece3664c1b6462f7\",\"type\":\"neighborhood\",\"full_name\":\"Bishopsgate, London\"},\"source\":\"<a href=\\\"http://twitter.com/\\\" rel=\\\"nofollow\\\">Twitter for iPhone</a>\"}";
            
            IStatusMessage message = StatusMessage.CreateMessageFromJsonString(messageData);
            Assert.AreEqual(new Decimal(51.5214), message.Geo.Coordinates[0]);
            Assert.AreEqual(new Decimal(-0.081), message.Geo.Coordinates[1]);
        }
    }
}
