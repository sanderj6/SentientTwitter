namespace SentientTwitter.Data
{
    public class TweetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InServiceDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsLive { get; set; }
        public Guid OwnerId { get; set; }
        public Guid LastKnownBattery { get; set; }
        public HealthStatus Status { get; set; }
        public BatteryModel Battery { get; set; }
    }

    public class BatteryModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public int Capacity { get; set; }
        public int Cycles { get; set; }
        public int Charge { get; set; }
        public HealthStatus Status { get; set; }
    }

    public class FlightRecord
    {
        public Guid Id { get; set; }
        public Guid TweetId { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal LocationX { get; set; }
        public decimal LocationY { get; set; }
        public string Message { get; set; }
        public HealthStatus Status { get; set; }
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
}