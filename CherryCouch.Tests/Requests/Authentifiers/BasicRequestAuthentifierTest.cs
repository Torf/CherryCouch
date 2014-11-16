using CherryCouch.Requests.Authentifiers;
using Nancy;
using NUnit.Framework;

namespace CherryCouch.Tests.Requests.Authentifiers
{
    /// <summary>
    /// Description résumée pour AuthentificatedRequestTest
    /// </summary>
    [TestFixture]
    public class BasicRequestAuthentifierTest
    {
        private const string ValidAuthUser = "alice";
        private const string ValidAuthPasskey = "6fds4gs";

        private const string BadAuthUser = "bob";
        private const string BadAuthPasskey = "9sdgjr3";

        [TestCase]
        public void ShouldParseQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"user", ValidAuthUser}, 
                {"passkey", ValidAuthPasskey}
            };

            // Act
            var authentifier = new BasicRequestAuthentifier(ValidAuthUser, ValidAuthPasskey);
            var authorization = authentifier.ParseQuery(queryArgs);

            // Verify
            Assert.AreEqual(ValidAuthUser, authorization.Username);
            Assert.AreEqual(ValidAuthPasskey, authorization.Passkey);
            Assert.AreEqual(true, authorization.IsAuthorized);
        }

        [TestCase(BadAuthUser, ValidAuthPasskey)]
        [TestCase(ValidAuthUser, BadAuthPasskey)]
        [TestCase(BadAuthUser, BadAuthPasskey)]
        public void ShouldNotParseQuery(string username, string passkey)
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"user", username}, 
                {"passkey", passkey}
            };

            // Act
            var authentifier = new BasicRequestAuthentifier(ValidAuthUser, ValidAuthPasskey);
            var authorization = authentifier.ParseQuery(queryArgs);

            // Verify
            Assert.AreEqual(username, authorization.Username);
            Assert.AreEqual(passkey, authorization.Passkey);
            Assert.AreEqual(false, authorization.IsAuthorized);
        }
    }
}
