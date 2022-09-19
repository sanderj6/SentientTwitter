
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;
using SentientTwitter.Data;
using System.Linq;

namespace SentientTwitter.CosmosDB;
public class TwitterRecordDbContext : DbContext
{
    public TwitterRecordDbContext()
    {

    }

    public DbSet<TweetModel> TweetRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("coinworld-db");

        modelBuilder.Entity<TweetModel>(entity =>
        {
            entity.ToContainer("sentimentrecord")
            .HasPartitionKey(x => x.Id);

            entity.Property(x => x.Id);

            entity.HasNoDiscriminator();
        }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCosmos("AccountEndpoint=https://mood-users.documents.azure.com:443/;AccountKey=MXEkdkP6kvzk3FM1CNjLwHHWLQc5Fk06w0g2WkEtaWVRcIgP2PwXkSr4wZEgCvBmkYNXsG3Zh3iq9iMiiOLG0A==;", "coinworld-db");
        base.OnConfiguring(optionsBuilder);
    }
}
