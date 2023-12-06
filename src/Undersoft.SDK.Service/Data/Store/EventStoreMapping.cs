using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Undersoft.SDK.Service.Data.Store;

using Event;

public class EventStoreMapping : EntityTypeMapping<Event>
{
    private const string TABLE_NAME = "EventStore";

    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable(TABLE_NAME, "EventNode");

        builder.Property(p => p.PublishTime)
            .HasColumnType("timestamp");
    }
}