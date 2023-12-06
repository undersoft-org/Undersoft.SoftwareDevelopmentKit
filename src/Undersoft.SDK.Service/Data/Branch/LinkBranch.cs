namespace Undersoft.SDK.Service.Data.Branch
{
    public class LinkBranch : Branch
    {
        public virtual long LeftEntityId { get; set; }

        public virtual long RightEntityId { get; set; }
    }
}