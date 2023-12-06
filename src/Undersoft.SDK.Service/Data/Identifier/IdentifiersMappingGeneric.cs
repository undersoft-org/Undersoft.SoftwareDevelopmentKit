using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Undersoft.SDK.Service.Data.Identifier;

using Data.Store;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Object;

public class IdentifiersMapping<TObject> : IIdentifiersMapping where TObject : class, IDataObject
{
    private string TABLE_NAME = typeof(TObject).Name + "Identifiers";
    private readonly ModelBuilder _modelBuilder;
    private readonly EntityTypeBuilder<TObject> _entityBuilder;
    private readonly EntityTypeBuilder<Identifier<TObject>> _identifierBuilder;

    public IdentifiersMapping(ModelBuilder builder)
    {
        _modelBuilder = builder;
        _entityBuilder = _modelBuilder.Entity<TObject>();
        _identifierBuilder = _modelBuilder.Entity<Identifier<TObject>>();
    }

    public ModelBuilder Configure()
    {
        _identifierBuilder.ToTable(TABLE_NAME, DataStoreSchema.IdentifierSchema);

        _identifierBuilder.HasIndex(k => k.Key).HasMethod("hash");
        _identifierBuilder.HasIndex(k => k.ObjectId).HasMethod("hash");

        _identifierBuilder.HasOne(a => a.Object)
                          .WithMany("Identifiers")
                          .HasForeignKey(k => k.ObjectId)
                          .IsRequired()
                          .OnDelete(DeleteBehavior.Restrict);

        _entityBuilder.HasMany("Identifiers")
                      .WithOne(nameof(Identifier<TObject>.Object))
                      .HasForeignKey(nameof(Identifier<TObject>.ObjectId))
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

        _entityBuilder.Navigation("Identifiers");

        return _modelBuilder;
    }
}