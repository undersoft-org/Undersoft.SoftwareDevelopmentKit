namespace Undersoft.SDK
{ 
    public interface IOperation
    {
        public object Input { get; }

        public object Output { get; }

        public Delegate Processings { get; set; }
    }
}
