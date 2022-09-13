using Azure.AI.TextAnalytics;

namespace SentientTwitter.Data
{
    public class SentimentRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public double Score => ConfidenceScores is not null ? Math.Round((ConfidenceScores.Sum(x => x.Score) / 3) * 100, 2) : 0;
        public Mood Sentiment => Score > 0 ? Mood.Positive : Score < 0 ? Mood.Negative : Mood.Neutral;
        public string RawText { get; set; }
        public Mood Mood { get; set; }
        public List<SentenceSentiment> Sentences { get; set; }
        public List<ConfidenceScore> ConfidenceScores { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum Mood
    {
        Positive,
        Negative,
        Neutral,
        Cheerful,
        Reflective,
        Gloomy,
        Humorous,
        Melancholy,
        Idyllic,
        Whimsical,
        Romantic,
        Mysterious,
        Ominous,
        Calm,
        Lighthearted,
        Hopeful,
        Angry,
        Fearful,
        Tense,
        Lonely
    }

    public enum SessionState
    {
        Input,
        Overview,
        History
    }

    public class ConfidenceScore
    {
        public Mood ConfidenceType { get; set; }
        public double Score { get; set; }
    }

    ///// <summary>
    ///// HTTP Post request and response classes
    ///// </summary>

    //// Text Request
    //public class TextAnalyzerRequest
    //{
    //    public DocumentRequest[] documents { get; set; }
    //}

    //public class DocumentRequest
    //{
    //    public string language { get; set; }
    //    public string id { get; set; }
    //    public string text { get; set; }
    //}


    // Text Response
    //public class ConfidenceScores
    //{
    //    public double Positive { get; set; }
    //    public double Neutral { get; set; }
    //    public double Negative { get; set; }
    //}

    //public class DocumentSentiment
    //{
    //    public string id { get; set; }
    //    public string Sentiment { get; set; }
    //    public ConfidenceScores ConfidenceScores { get; set; }
    //    public List<SentenceSentiment> Sentences { get; set; }
    //    public List<object> Warnings { get; set; }
    //}

    //public class TextAnalyzerResponse
    //{
    //    public List<DocumentSentiment> documents { get; set; }
    //    public List<object> errors { get; set; }
    //    public string modelVersion { get; set; }
    //}

    //public class SentenceSentiment
    //{
    //    public string Sentiment { get; set; }
    //    public ConfidenceScores ConfidenceScores { get; set; }
    //    public int offset { get; set; }
    //    public int length { get; set; }
    //    public string Text { get; set; }
    //}

}