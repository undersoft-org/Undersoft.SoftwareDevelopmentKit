namespace Undersoft.SDK.Instant.Updating;

using Proxies;
using Rubrics;

public interface IUpdater : IInstant
{
    //IProxy Preset { get; }
    IProxy Source { get; }

    IRubrics Rubrics { get; set; }

    object Clone();

    E Patch<E>() where E : class;
    E Patch<E>(E item) where E : class;

    object PatchSelf();

    E Put<E>() where E : class;
    E Put<E>(E item) where E : class;

    object PutSelf();

    UpdaterItem[] Detect<E>(E item) where E : class;
}
