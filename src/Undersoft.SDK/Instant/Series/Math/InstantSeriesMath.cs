namespace Undersoft.SDK.Instant.Series.Math;

using SDK.Series;
using Uniques;
using System.Linq;
using Rubrics;
using Set;
using Instant.Rubrics;

public class InstantSeriesMath : IInstantSeriesMath
{
    private MathRubrics computation;

    public InstantSeriesMath(IInstantSeries data)
    {
        computation = new MathRubrics(data);
        serialcode.Id = (long)DateTime.Now.ToBinary();
        if (data.Computations == null)
            data.Computations = new Catalog<IInstantSeriesMath>();
        data.Computations.Put(this);
    }

    public MathSet this[int id]
    {
        get { return GetMathSet(id); }
    }
    public MathSet this[string name]
    {
        get { return GetMathSet(name); }
    }
    public MathSet this[MemberRubric rubric]
    {
        get { return GetMathSet(rubric); }
    }

    public MathSet GetMathSet(int id)
    {
        MemberRubric rubric = computation.Rubrics[id];
        if (rubric != null)
        {
            MathRubric mathrubric = null;
            if (computation.MathsetRubrics.TryGet(rubric.Name, out mathrubric))
                return mathrubric.GetMathset();
            return computation
                .Put(rubric.Name, new MathRubric(computation, rubric))
                .Value.GetMathset();
        }
        return null;
    }

    public MathSet GetMathSet(string name)
    {
        MemberRubric rubric = null;
        if (computation.Rubrics.TryGet(name, out rubric))
        {
            MathRubric mathrubric = null;
            if (computation.MathsetRubrics.TryGet(name, out mathrubric))
                return mathrubric.GetMathset();
            return computation
                .Put(rubric.Name, new MathRubric(computation, rubric))
                .Value.GetMathset();
        }
        return null;
    }

    public MathSet GetMathSet(MemberRubric rubric)
    {
        return GetMathSet(rubric.Name);
    }

    public bool ContainsFirst(MemberRubric rubric)
    {
        return computation.First.Value.RubricName == rubric.Name;
    }

    public bool ContainsFirst(string rubricName)
    {
        return computation.First.Value.RubricName == rubricName;
    }

    public IInstantSeries Compute()
    {
        computation.Combine();
        computation
            .AsValues()
            .Where(p => !p.PartialMathset)
            .OrderBy(p => p.ComputeOrdinal)
            .Select(p => p.Compute())
            .ToArray();
        return computation.Data;
    }

    private Uscn serialcode;
    public Uscn SerialCode
    {
        get => serialcode;
        set => serialcode = value;
    }
    public IUnique Empty => Uscn.Empty;

    public long Id
    {
        get => serialcode.Id;
        set => serialcode.Id = value;
    }

    public int CompareTo(IUnique other)
    {
        return serialcode.CompareTo(other);
    }

    public bool Equals(IUnique other)
    {
        return serialcode.Equals(other);
    }

    public byte[] GetBytes()
    {
        return serialcode.GetBytes();
    }

    public byte[] GetIdBytes()
    {
        return serialcode.GetIdBytes();
    }

    public long TypeId
    {
        get => serialcode.TypeId;
        set => serialcode.TypeId = value;
    }

    public string CodeNo {
        get => serialcode.CodeNo;
        set => serialcode.CodeNo = value;
    }
}
