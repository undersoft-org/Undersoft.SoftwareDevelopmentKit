using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undersoft.SDK.Service.Data.Store.Relation;

namespace Undersoft.SDK.Service.Data.Store;

using Entity;
using Undersoft.SDK.Service.Data.Identifier;
using Undersoft.SDK.Service.Data.Object;

public abstract class EntityTypeMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    protected ModelBuilder modelBuilder;

    public virtual void SetBuilder(ModelBuilder modelBuilder)
    {
        this.modelBuilder = modelBuilder;
    }

    public abstract void Configure(EntityTypeBuilder<TEntity> builder);
}

public static class DataStoreModelBuilderExtensions
{
    public static ModelBuilder ApplyIdentity<TContext>(this ModelBuilder builder)
    {
        foreach (var type in builder.Model.GetEntityTypes().ToList())
        {
            var clr = type.ClrType;
            if (clr != null && clr.GetInterfaces().Contains(typeof(IEntity)))
            {
                var model = builder.Entity(clr);
                model.HasKey("Id");
                model.HasIndex("Id").HasMethod("hash");
                model.HasIndex("Ordinal").IsUnique();
                model.Property("Ordinal").UseIdentityColumn();
                model.Property("CodeNo").HasMaxLength(32).IsConcurrencyToken(true);
            }            
        }
        return builder;
    }

    public static ModelBuilder ApplyMapping<TEntity>(
        this ModelBuilder builder,
        EntityTypeMapping<TEntity> entityBuilder
    ) where TEntity : class
    {
        entityBuilder.SetBuilder(builder);
        builder.ApplyConfiguration(entityBuilder);
        return builder;
    }

    public static ModelBuilder ApplyIdentifiers(this ModelBuilder builder, Type type)
    {
        return new IdentifiersMapping(type, builder).Configure();
    }


    public static ModelBuilder ApplyIdentifiers<TEntity>(this ModelBuilder builder)
        where TEntity : class, IDataObject
    {
        return new IdentifiersMapping<TEntity>(builder).Configure();
    }

    public static ModelBuilder LinkSetToSet<TLeft, TRight>(
        this ModelBuilder builder,
        ExpandSite expandSite = ExpandSite.None,
        bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToSet<TLeft, TRight>(
            builder,
            expandSite,
            dbSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkSetToSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string rightName,
        ExpandSite expandSite = ExpandSite.None,
        bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToSet<TLeft, TRight>(
            builder,
            leftName,
            rightName,
            expandSite,
            dbSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkSetToSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string leftTableName,
        string rightName,
        string rightTableName,
        ExpandSite expandSite = ExpandSite.None,
        bool autoinclude = false,
        string parentSchema = null,
        string childSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToSet<TLeft, TRight>(
            builder,
            leftName,
            leftTableName,
            rightName,
            rightTableName,
            expandSite,
            parentSchema,
            childSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkSetToRemoteSet<TLeft, TRight>(
        this ModelBuilder builder,
        ExpandSite expandSite = ExpandSite.None,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToLinkedSet<TLeft, TRight>(
            builder,
            expandSite,
            dbSchema
        ).Configure();
    }

    public static ModelBuilder LinkSetToRemoteSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string rightName,
        ExpandSite expandSite = ExpandSite.None,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToLinkedSet<TLeft, TRight>(
            builder,
            leftName,
            rightName,
            expandSite,
            dbSchema
        ).Configure();
    }

    public static ModelBuilder LinkSetToRemoteSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string leftTableName,
        string rightName,
        string rightTableName,
        ExpandSite expandSite = ExpandSite.None,
        string parentSchema = null,
        string childSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedSetToLinkedSet<TLeft, TRight>(
            builder,
            leftName,
            leftTableName,
            rightName,
            rightTableName,
            expandSite,
            parentSchema,
            childSchema
        ).Configure();
    }

    public static ModelBuilder LinkOneToSet<TLeft, TRight>(
        this ModelBuilder builder,
        ExpandSite expandSite = ExpandSite.None,
          bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToSet<TLeft, TRight>(builder, expandSite).Configure(autoinclude);
    }

    public static ModelBuilder LinkOneToSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string rightName,
        ExpandSite expandSite = ExpandSite.None,
          bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToSet<TLeft, TRight>(
            builder,
            leftName,
            rightName,
            expandSite,
            dbSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkOneToSet<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string leftTableName,
        string rightName,
        string rightTableName,
        ExpandSite expandSite = ExpandSite.None,
          bool autoinclude = false,
        string parentSchema = null,
        string childSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToSet<TLeft, TRight>(
            builder,
            leftName,
            leftTableName,
            rightName,
            rightTableName,
            expandSite
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkOneToOne<TLeft, TRight>(
        this ModelBuilder builder,
        ExpandSite expandSite = ExpandSite.None,
        bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToOne<TLeft, TRight>(
            builder,
            expandSite,
            dbSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkOneToOne<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string rightName,
        ExpandSite expandSite = ExpandSite.None,
           bool autoinclude = false,
        string dbSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToOne<TLeft, TRight>(
            builder,
            leftName,
            rightName,
            expandSite,
            dbSchema
        ).Configure(autoinclude);
    }

    public static ModelBuilder LinkOneToOne<TLeft, TRight>(
        this ModelBuilder builder,
        string leftName,
        string leftTableName,
        string rightName,
        string rightTableName,
        ExpandSite expandSite = ExpandSite.None,
           bool autoinclude = false,
        string parentSchema = null,
        string childSchema = null
    )
        where TLeft : class, IDataObject
        where TRight : class, IDataObject
    {
        return new RelatedOneToOne<TLeft, TRight>(
            builder,
            leftName,
            leftTableName,
            rightName,
            rightTableName,
            expandSite,
            parentSchema,
            childSchema
        ).Configure(autoinclude);
    }
}
