namespace Undersoft.SDK.Instant.Rubrics
{
    using Series;
    using Undersoft.SDK.Series;

    public interface IRubrics : ISeries<MemberRubric>
    {
        int BinarySize { get; }

        int[] BinarySizes { get; }

        IInstantSeries Figures { get; set; }

        IRubrics KeyRubrics { get; set; }

        RubricSqlMappings Mappings { get; set; }

        int[] Ordinals { get; }

        byte[] GetBytes(IInstant figure);

        byte[] GetUniqueBytes(IInstant figure, uint seed = 0);

        void Update();
    }
}
