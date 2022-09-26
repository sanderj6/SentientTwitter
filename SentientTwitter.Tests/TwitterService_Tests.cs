using Microsoft.Extensions.DependencyInjection;
using SentientTwitter.Services;

namespace SentientTwitter.Tests
{
    [TestClass]
    public class TwitterService_Tests
    {
        private readonly TwitterService _twitterService;
        private IHttpClientFactory _httpClientFactory;

        public TwitterService_Tests()
        {
            var serviceProvider = new ServiceCollection()
                 .AddHttpClient()
                 .BuildServiceProvider();

            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _twitterService = new TwitterService(_httpClientFactory);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _twitterService.StartStream().RunSynchronously();
        }
    }
}