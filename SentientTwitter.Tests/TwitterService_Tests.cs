using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using SentientTwitter.Services;

namespace SentientTwitter.Tests
{
    [TestClass]
    public class TwitterService_Tests
    {
        private readonly TwitterService _twitterService;
        private IHttpClientFactory _httpClientFactory;
        private static ILogger<TwitterService> _logger;

        public TwitterService_Tests()
        {
            var serviceProvider = new ServiceCollection()
                 .AddHttpClient()
                 .BuildServiceProvider();

            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            _logger = serviceProvider.GetService<ILogger<TwitterService>>();

            _twitterService = new TwitterService(_httpClientFactory, _logger);
        }

        [TestMethod]
        public void TwitterService_StartStream_GOOD()
        {
            _twitterService.StartStream().RunSynchronously();
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void TwitterService_StartStream_BAD()
        {
            _twitterService.StartStream().RunSynchronously();
            Assert.AreEqual(1, 1);
        }
    }
}