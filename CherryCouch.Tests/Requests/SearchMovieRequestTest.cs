using CherryCouch.Common.Protocol.Request;
using CherryCouch.Exceptions;
using CherryCouch.Requests;
using CherryCouch.Requests.Authentifiers;
using Moq;
using Nancy;
using NUnit.Framework;

namespace CherryCouch.Tests.Requests
{
    [TestFixture]
    public class SearchMovieRequestTest
    {
        private const string ImdbId = "tt6534263";
        private const string SearchMovieName = "Lets%20Be%20Cops%202014";

        [TestCase]
        public void ShouldParseImdbIdQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"imdbid", ImdbId}
            };

            var authentifier = new Mock<IRequestAuthentifier>();

            // Act
            var request = new SearchMovieRequest().Parse(authentifier.Object, queryArgs);

            // Verify
            Assert.AreEqual(ImdbId, request.ImdbId);
            Assert.AreEqual(null, request.Terms);
        }

        [TestCase]
        public void ShouldParseSearchQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"search", SearchMovieName}
            };

            var authentifier = new Mock<IRequestAuthentifier>();

            // Act
            var request = new SearchMovieRequest().Parse(authentifier.Object, queryArgs);

            // Verify
            Assert.AreEqual(null, request.ImdbId);
            Assert.AreEqual(SearchMovieName, request.Terms);
        }

        [TestCase]
        public void ShouldNotParseQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary();

            var authentifier = new Mock<IRequestAuthentifier>();

            // Act & Verify
            Assert.Catch<MissingParameterRequestException>(() => 
            {
                var request = new SearchMovieRequest().Parse(authentifier.Object, queryArgs);
            });
        }

        [TestCase]
        public void ShouldNotParseNotAuthentificatedQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"imdbid", ImdbId}
            };

            var authentifier = new Mock<IRequestAuthentifier>();
            authentifier.Setup(e => e.ParseQuery(It.IsAny<DynamicDictionary>()))
                .Throws(new MissingParameterRequestException("user"));

            // Act & Verify
            Assert.Catch<MissingParameterRequestException>(() =>
            {
                var request = new SearchMovieRequest().Parse(authentifier.Object, queryArgs);
            });
        }

        [TestCase]
        public void ShouldNotParseBadAuthentificatedQuery()
        {
            // Prepare
            var queryArgs = new DynamicDictionary
            {
                {"imdbid", ImdbId}
            };

            var authorization = new Mock<IRequestAuthorization>();
            authorization.SetupGet(e => e.IsAuthorized).Returns(false);

            var authentifier = new Mock<IRequestAuthentifier>(); 
            authentifier.Setup(e => e.ParseQuery(It.IsAny<DynamicDictionary>()))
                         .Returns(authorization.Object);

            // Act
            var request = new SearchMovieRequest().Parse(authentifier.Object, queryArgs);

            // Verify
            Assert.AreEqual(ImdbId, request.ImdbId);
            Assert.AreEqual(null, request.Terms);
            Assert.AreEqual(false, request.Authorization.IsAuthorized);
        }
    }
}
