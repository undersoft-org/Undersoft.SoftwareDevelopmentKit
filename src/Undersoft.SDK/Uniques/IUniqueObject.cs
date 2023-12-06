

namespace Undersoft.SDK.Uniques
{
    public interface IUniqueObject : IUniqueCode
    {
        int Ordinal { get; set; }

        string Label { get; set; }   

        DateTime Modified { get; set; }

        string Modifier { get; set; }

        DateTime Created { get; set; }

        string Creator { get; set; }

        string CodeNumber { get; set; }
    }
}
