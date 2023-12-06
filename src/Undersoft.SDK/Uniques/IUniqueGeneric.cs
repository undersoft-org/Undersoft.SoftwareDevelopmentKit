namespace Undersoft.SDK.Uniques
{
    public interface IUnique<V> : IUnique
    {
        V UniqueObject { get; set; }

        int[] UniqueOrdinals();

        long CompactKey();

        object[] UniqueValues();
    }
}
