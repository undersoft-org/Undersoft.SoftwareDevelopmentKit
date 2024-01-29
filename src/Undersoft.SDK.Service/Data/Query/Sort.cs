using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Query;

using Instant.Proxies;
using Instant.Rubrics;

public class Sort<TEntity>
{
    private SortExpression<TEntity> sortExpression;

    public Sort()
    {
    }
    public Sort(Expression<Func<TEntity, object>> expressionItem, SortDirection direction = SortDirection.Asc)
    {
        ExpressionItem = expressionItem;
        Direction = direction;
    }
    public Sort(MemberRubric sortedRubric, SortDirection direction = SortDirection.Asc)
    {
        Direction = direction;
        Rubric = sortedRubric;
        Property = Rubric.Name;
    }
    public Sort(string rubricName, string direction = "Asc")
    {
        Property = rubricName;
        SortDirection sortDirection;
        Enum.TryParse(direction, true, out sortDirection);
        Direction = sortDirection;
    }
    public Sort(SortItem item) : this(item.Property, item.Direction)
    {
    }
  
    public Expression<Func<TEntity, object>> ExpressionItem { get; set; }

    public SortDirection Direction { get; set; }

    public int Position { get; set; }

    public string Property { get; set; }

    public MemberRubric Rubric { get; set; }

    public void Assign(SortExpression<TEntity> sortExpression)
    {
        var fe = sortExpression;
        this.sortExpression = fe;
        if (fe.Rubrics.TryGet(Property, out MemberRubric rubric))
        {
            Rubric = rubric;
            ExpressionItem = e => e.ValueOf(Property);
        }
    }

    public bool Compare(Sort<TEntity> term)
    {
        if (Property != term.Property || Direction != term.Direction)
            return false;

        return true;
    }

}
