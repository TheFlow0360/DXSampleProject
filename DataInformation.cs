namespace DevExpressGridInconsistencyDemo
{
    using System;

    public class DataInformation
    {
        public DataInformation(Type type, String name)
            : this(type, name, name)
        {
        }

        public DataInformation(Type type, String name, String viewText)
        {
            Type = type;
            Name = name;
            ViewText = viewText;
        }

        public String Name { get; }

        public Type Type { get; }

        public String ViewText { get; }

        public override String ToString()
        {
            return $"{Name} ({Type.Name})";
        }
    }
}