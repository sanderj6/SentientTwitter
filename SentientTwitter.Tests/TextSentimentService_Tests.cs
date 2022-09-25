using SentientTwitter.Services;

namespace SentientTwitter.Tests
{
    [TestClass]
    public class TextSentimentService_Tests
    {
        private readonly TextSentimentService _textSentimentService;
        private string testText = "Life is good.";

        public TextSentimentService_Tests()
        {
            _textSentimentService = new TextSentimentService();
        }

        [TestMethod]
        public void AnalyzeText_GOOD()
        {
            var result = _textSentimentService.AnalyzeText(testText);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AnalyzeText_BAD()
        {
            testText = string.Empty;
            var result = _textSentimentService.AnalyzeText(testText);
            Assert.IsNotNull(result);
        }
    }
}