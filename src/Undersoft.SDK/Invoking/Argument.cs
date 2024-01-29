namespace Undersoft.SDK.Invoking
{
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using Undersoft.SDK.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Argument : Identifiable, IArgument
    {
        private Type _type;

        public Argument() { }

        public Argument(IArgument value) { Set(value); }

        public Argument(string name, object value, int position = 0) { Set(name, value); }

        public Argument(string name, Type type, int position = 0) { Set(name, type.Default()); }

        public Argument(string name, string typeName, int position = 0) { Set(name, Assemblies.FindType(typeName)); }

        public Argument(ParameterInfo info) { Set(info); }
         
        public string Name { get; set; }

        public object Value { get; set; }

        public int Position { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Type Type => _type ??= Value.GetType();

        public void Set(ParameterInfo item)
        {
            Name = item.Name;
            Value = item.ParameterType.Default();
            _type = item.ParameterType;
            Position = item.Position;
            TypeName = _type.FullName;

            Id = Name.UniqueKey();
            TypeId = TypeName.UniqueKey();
        }

        public void Set(IArgument item)
        {
            Name = item.Name;
            Value = item.Value;
            _type = item.Type;
            Position = item.Position;
            TypeName = item.TypeName;

            Id = Name.UniqueKey();
            TypeId = TypeName.UniqueKey();
        }

        public void Set(string name, object value, int position = 0)
        {
            Name = name; 
            Value = value;
            _type = value.GetType();
            TypeName = _type.FullName;
            Position = position;

            Id = Name.UniqueKey();
            TypeId = TypeName.UniqueKey();
        }

        public void Set(string name, Type type, int position = 0)
        {
            Name = name;
            Value = type.Default();
            _type = type;
            TypeName = _type.FullName;
            Position = position;

            Id = Name.UniqueKey();
            TypeId = TypeName.UniqueKey();
        }
    }
}
