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
}