namespace Undersoft.SDK.Instant.Updating
{
    using Invoking;
    using Proxies;

    public class Updater<T> : Updater, IUpdater<T> where T : class
    {
        public Updater() { }

        public Updater(T item) : base(item) { }

        public Updater(T item, IInvoker traceChanges) : base(item, traceChanges) { }

        public T Patch(T item)
        {
            base.Patch<T>(item);
            return item;
        }

        public T PatchFrom(T source)
        {
            ((object)this).PatchFrom(source);
            base.PatchSelf();
            return (T)(base.source.Target);
        }

        public new T PatchSelf()
        {
            base.PatchSelf();
            return (T)(source.Target);
        }

        public T Put(T item)
        {
            base.Put<T>(item);
            return item;
        }

        public T PutFrom(T source)
        {
            ((object)this).PutFrom(source);
            base.PutSelf();
            return (T)(base.source.Target);
        }

        public new T PutSelf()
        {
            base.PutSelf();
            return (T)(source.Target);
        }

        public UpdaterItem[] Detect(T item)
        {
            return base.Detect<T>(item);
        }

        public new T Clone()
        {
            var clone = typeof(T).New<T>();
            var _clone = creator.Create(clone);
            _clone.PutFrom(Devisor);
            return clone;
        }

        public IProxy EntryProxy => source;
        public IProxy PresetProxy => (IProxy)Preset;

        public new T Entry => (T)(preset.Target);
        public new T Preset => (T)((preset == null) ? preset = creator.Create(source) : preset);

        public new T Devisor
        {
            get => (T)(source.Target);
            set => source.Target = value;
        }
    }
}
