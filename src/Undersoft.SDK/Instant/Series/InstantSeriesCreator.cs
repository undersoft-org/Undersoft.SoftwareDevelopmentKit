namespace Undersoft.SDK.Instant.Series
{
    using Instant.Proxies;
    using SDK.Uniques;
    using Rubrics;
    using System.Linq;

    public enum InstantSeriesMode
    {
        proxy,
        instant
    }

    public class InstantProxiesCreator<T> : InstantSeriesCreator
    {
        public InstantProxiesCreator() : base(typeof(T)) { }

        public InstantProxiesCreator(string seriesName) : base(typeof(T), seriesName) { }
    }

    public class InstantSeriesCreator<T> : InstantSeriesCreator
    {
        public InstantSeriesCreator(InstantType mode = InstantType.Reference) : base(typeof(T), mode) { }

        public InstantSeriesCreator(string seriesName, InstantType mode = InstantType.Reference)
            : base(typeof(T), seriesName, mode) { }
    }

    public class InstantSeriesCreator : IInstantCreator
    {
        private Type compiledType;
        private InstantCreator instant;
        private ProxyCreator proxy;
        private long key;
        private bool safeThread;
        private InstantSeriesMode mode;

        public InstantSeriesCreator(
            ProxyCreator proxyGenerator,
            string seriesTypeName = null,
            bool safeThread = true
        )
        {
            mode = InstantSeriesMode.proxy;
            if (proxyGenerator.Type == null)
                proxyGenerator.Create();
            this.safeThread = safeThread;
            this.proxy = proxyGenerator;
            Name =
                (seriesTypeName != null && seriesTypeName != "")
                    ? seriesTypeName
                    : proxy.Name + "_F";
        }

        public InstantSeriesCreator(IProxy proxyObject, bool safeThread = true)
            : this(
                new ProxyCreator(proxyObject.GetType(), proxyObject.GetType().Name),
                null,
                safeThread
            )
        { }

        public InstantSeriesCreator(Type proxyModelType, bool safeThread = true)
            : this(new ProxyCreator(proxyModelType), null, safeThread) { }

        public InstantSeriesCreator(Type proxyModelType, string seriesName, bool safeThread = true)
            : this(new ProxyCreator(proxyModelType), seriesName, safeThread) { }

        public InstantSeriesCreator(
            InstantCreator instantGenerator,
            string seriesTypeName = null,
            bool safeThread = true
        )
        {
            mode = InstantSeriesMode.instant;
            if (instantGenerator.Type == null)
                instantGenerator.Create();
            this.safeThread = safeThread;
            this.instant = instantGenerator;
            Name =
                (seriesTypeName != null && seriesTypeName != "")
                    ? seriesTypeName
                    : instant.Name + "s";
        }

        public InstantSeriesCreator(IInstant instantObject, bool safeThread = true)
            : this(
                new InstantCreator(
                    instantObject.GetType(),
                    instantObject.GetType().Name,
                    InstantType.Reference
                ),
                null,
                safeThread
            )
        { }

        public InstantSeriesCreator(
            IInstant instantObject,
            string seriesTypeName,
            InstantType modeType = InstantType.Reference,
            bool safeThread = true
        )
            : this(
                new InstantCreator(instantObject.GetType(), instantObject.GetType().Name, modeType),
                seriesTypeName,
                safeThread
            )
        { }

        public InstantSeriesCreator(
            MemberRubrics instantRubrics,
            string seriesTypeName = null,
            string instantTypeName = null,
            InstantType modeType = InstantType.Reference,
            bool safeThread = true
        ) : this(new InstantCreator(instantRubrics, instantTypeName, modeType), seriesTypeName, safeThread)
        { }

        public InstantSeriesCreator(Type instantModelType, InstantType modeType, bool safeThread = true)
            : this(new InstantCreator(instantModelType, null, modeType), null, safeThread) { }

        public InstantSeriesCreator(
            Type instantModelType,
            string seriesTypeName,
            InstantType modeType,
            bool safeThread = true
        ) : this(new InstantCreator(instantModelType, null, modeType), seriesTypeName, safeThread) { }

        public InstantSeriesCreator(
            Type instantModelType,
            string seriesTypeName,
            string instantTypeName,
            InstantType modeType = InstantType.Reference,
            bool safeThread = true
        ) : this(new InstantCreator(instantModelType, instantTypeName, modeType), seriesTypeName, safeThread)
        { }

        public Type BaseType { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics
        {
            get => (proxy != null) ? proxy.Rubrics : instant.Rubrics;
        }

        public int Size
        {
            get => (proxy != null) ? proxy.Size : instant.Size;
        }

        public Type Type { get; set; }

        public IInstantSeries Combine()
        {
            if (this.Type == null)
            {
                var ifc = new InstantSeriesCompiler(this, safeThread);
                compiledType = ifc.CompileFigureType(Name);
                this.Type = compiledType.New().GetType();
                key = Unique.NewId;
            }
            if (mode == InstantSeriesMode.instant)
                return newinstants();
            else
                return newProxies();
        }

        public object New()
        {
            return (mode == InstantSeriesMode.proxy) ? newProxies() : newinstants();
        }

        private MemberRubrics CloneRubrics()
        {
            var rubrics = new MemberRubrics(Rubrics.Select(r => r.ShalowCopy(null)));
            rubrics.KeyRubrics = new MemberRubrics(
                Rubrics.KeyRubrics.Select(r => r.ShalowCopy(null))
            );
            rubrics.Update();
            return rubrics;
        }

        private IInstantSeries newinstants()
        {
            IInstantSeries newseries = newinstants((IInstantSeries)(Type.New()));
            newseries.Rubrics = CloneRubrics();
            newseries.KeyRubrics = newseries.Rubrics.KeyRubrics;
            newseries.View = newseries.AsQueryable();
            return newseries;
        }

        private IInstantSeries newinstants(IInstantSeries newseries)
        {
            newseries.InstantType = instant.Type;
            newseries.InstantSize = instant.Size;
            newseries.Type = this.Type;
            newseries.Instant = this;
            newseries.Prime = true;
            ((IUnique)newseries).Id = key;
            ((IUnique)newseries).TypeId = Name.UniqueKey64();

            return newseries;
        }

        private IInstantSeries newProxies()
        {
            IInstantSeries newproxies = newProxies((IInstantSeries)(this.Type.New()));
            newproxies.Rubrics = CloneRubrics();
            newproxies.KeyRubrics = newproxies.Rubrics.KeyRubrics;
            newproxies.View = newproxies.AsQueryable();
            return newproxies;
        }

        private IInstantSeries newProxies(IInstantSeries newproxies)
        {
            newproxies.InstantType = instant.Type;
            newproxies.InstantSize = instant.Size;
            newproxies.Type = this.Type;
            newproxies.Instant = this;
            newproxies.Prime = true;
            ((IUnique)newproxies).Id = key;
            ((IUnique)newproxies).TypeId = Name.UniqueKey64();

            return newproxies;
        }

        public static object AsParallel()
        {
            throw new NotImplementedException();
        }
    }
}
