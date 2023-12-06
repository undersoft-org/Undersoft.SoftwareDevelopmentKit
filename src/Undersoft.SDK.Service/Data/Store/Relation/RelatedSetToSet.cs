using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Undersoft.SDK.Service.Data.Store.Relation;

using Entity;
using Undersoft.SDK.Service.Data.Object;

public class RelatedSetToSet<TLeft, TRight>
    where TLeft : class, IDataObject
    where TRight : class, IDataObject
{
    private readonly string RELATION_TABLE_NAME;
    private readonly string LEFT_TABLE_NAME = typeof(TLeft).Name + "s";
    private readonly string RIGHT_TABLE_NAME = typeof(TRight).Name + "s";
    private readonly string LEFT_NAME = typeof(TLeft).Name + "s";
    private readonly string RIGHT_NAME =
        typeof(TRight).Name != typeof(TLeft).Name
            ? typeof(TRight).Name.Replace(typeof(TLeft).Name, "") + "s"
            : typeof(TRight).Name + "s";
    private readonly string LEFT_SCHEMA = null;
    private readonly string RIGHT_SCHEMA = null;

    private readonly ExpandSite _expandSite;
    private readonly ModelBuilder _modelBuilder;
    private readonly EntityTypeBuilder<TLeft> _firstBuilder;
    private readonly EntityTypeBuilder<TRight> _secondBuilder;
    private readonly EntityTypeBuilder<RelationNode<TLeft, TRight>> _relationBuilder;

    public RelatedSetToSet(
        ModelBuilder modelBuilder,
        ExpandSite expandSite = ExpandSite.None,
        string dbSchema = null
    ) : this(modelBuilder, null, null, null, null, expandSite, dbSchema, dbSchema) { }

    public RelatedSetToSet(
        ModelBuilder modelBuilder,
        string leftName,
        string rightName,
        ExpandSite expandSite = ExpandSite.None,
        string dbSchema = null
    )
        : this(
            modelBuilder,
            leftName,
            null,
            rightName,
            null,
            expandSite,
            dbSchema,
            dbSchema
        )
    { }

    public RelatedSetToSet(
        ModelBuilder modelBuilder,
        string leftName,
        string leftTableName,
        string rightName,
        string rightTableName,
        ExpandSite expandSite = ExpandSite.None,
        string parentSchema = null,
        string childSchema = null
    )
    {
        _modelBuilder = modelBuilder;
        _firstBuilder = _modelBuilder.Entity<TLeft>();
        _secondBuilder = _modelBuilder.Entity<TRight>();
        _relationBuilder = _modelBuilder.Entity<RelationNode<TLeft, TRight>>();
        _expandSite = expandSite;

        if (leftTableName != null)
            LEFT_TABLE_NAME = leftTableName;
        if (rightTableName != null)
            RIGHT_TABLE_NAME = rightTableName;
        if (leftName != null)
            LEFT_NAME = leftName;
        if (rightName != null)
            RIGHT_NAME = rightName;
        if (parentSchema != null)
            LEFT_SCHEMA = parentSchema;
        if (childSchema != null)
            RIGHT_SCHEMA = childSchema;

        RELATION_TABLE_NAME = LEFT_TABLE_NAME + "And" + RIGHT_TABLE_NAME;
    }

    public ModelBuilder Configure(bool autoinclude = false)
    {
        //if (LEFT_SCHEMA != null && RIGHT_NAME != null)
        //{
        //    _firstBuilder.ToTable(LEFT_TABLE_NAME, LEFT_SCHEMA);
        //    _secondBuilder.ToTable(RIGHT_TABLE_NAME, RIGHT_SCHEMA);
        //}
        _relationBuilder.ToTable(RELATION_TABLE_NAME, DataStoreSchema.RelationSchema);

        _firstBuilder
            .HasMany<TRight>(RIGHT_NAME)
            .WithMany(LEFT_NAME)
            .UsingEntity<RelationNode<TLeft, TRight>>(
                j => j.HasOne(a => a.RightEntity).WithMany(),
                //.WithMany(RELATION_TABLE_NAME)
                //.HasForeignKey(a => a.RightEntityId),

                j => j.HasOne(a => a.LeftEntity).WithMany(),
                //.WithMany(RELATION_TABLE_NAME)
                //.HasForeignKey(a => a.LeftEntityId),

                j =>
                {
                    j.HasKey(k => new { k.LeftEntityId, k.RightEntityId });
                }
            );

        if (_expandSite != ExpandSite.None)
        {
            if ((_expandSite & (ExpandSite.OnRight | ExpandSite.WithMany)) > 0)
            {
                if (!autoinclude)
                {
                    _firstBuilder.Navigation(RIGHT_NAME);
                }
                else
                {
                    _firstBuilder.Navigation(RIGHT_NAME).AutoInclude();
                }
            }
            if ((_expandSite & (ExpandSite.OnLeft | ExpandSite.WithMany)) > 0)
            {
                if (!autoinclude)
                {
                    _secondBuilder.Navigation(LEFT_NAME);
                }
                else
                {
                    _secondBuilder.Navigation(LEFT_NAME).AutoInclude();
                }
            }
        }

        return _modelBuilder;
    }
}
