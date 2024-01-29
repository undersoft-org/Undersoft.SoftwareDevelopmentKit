using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Workflows
{
    public class Workflow<TCase> : Workflow where TCase : class
    {
        public Workflow() : base(typeof(TCase).FullName) { }
    }

    public class Workflow : WorkCase
    {
        public Workflow() : this(Unique.NewId.ToString()) { }

        public Workflow(string name) : base(new WorkAspects(name)) { }

        public WorkAspect<TAspect> Aspect<TAspect>() where TAspect : class
        {
            if (!TryGet(typeof(TAspect).FullName, out WorkAspect aspect))
            {
                aspect = new WorkAspect<TAspect>();
                Add(aspect);
            }
            return aspect as WorkAspect<TAspect>;
        }
    }
}
