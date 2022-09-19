using Azure.AI.TextAnalytics;

namespace SentientTwitter.Data
{
    public class TweetModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Domain Domain { get; set; }
        public Entity Entity { get; set; }
        public DocumentSentiment Sentiment { get; set; }
    }

    public enum HealthStatus
    {
        Healthy,
        Unhealthy,
        Degraded,
        Mismatched,
        Failed
    }

    public class ChartData
    {
        public DateTime Date;
        public HealthStatus Status;
        public int Count;
        public decimal AvailabilityCount;
    }

    // API Call Models
    public class TweetSearchResponse
    {
        public Datum[] data { get; set; }
        public Meta meta { get; set; }
    }

    public class Meta
    {
        public string newest_id { get; set; }
        public string oldest_id { get; set; }
        public int result_count { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string text { get; set; }
    }


    public class TweetStreamData
    {
        public Data data { get; set; }
        public DocumentSentiment sentiment { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string text { get; set; }
        public ContextAnnotation[] context_annotations { get; set; }
        public Entities entities { get; set; }
        public string lang { get; set; }
    }

    public class ContextAnnotation
    {
        public Domain domain { get; set; }
        public Entity entity { get; set; }
    }
    public class Domain
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Hashtag[] hashtags { get; set; }
    }

    public class Entities
    {
        public Annotation[] annotations { get; set; }
        public URL[] urls { get; set; }
        public Hashtag[] hashtags { get; set; }
        public Mention[] mentions { get; set; }
        public Cashtag[] cashtags { get; set; }
    }

    public class Annotation
    {
        public int start { get; set; }
        public int end { get; set; }
        public double probability { get; set; }
        public string type { get; set; }
        public string normalized_text { get; set; }
    }

    public class URL
    {
        public int start { get; set; }
        public int end { get; set; }
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public string media_key { get; set; }
    }

    public class Mention
    {
        public int start { get; set; }
        public int end { get; set; }
        public string username { get; set; }
        public string id { get; set; }
    }

    public class Entity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Hashtag[] hashtags { get; set; }
    }

    public class Hashtag
    {
        public string tag { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Cashtag
    {
        public string tag { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }
}