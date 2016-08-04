namespace DevExpressGridInconsistencyDemo
{
    using System;

    public class DataField : DataInformation
    {
        public DataField(Type type, String name, Object value)
            :this(type, name, name, value)
        {
        }

        public DataField(Type type, String name, String viewText, Object value)
            : base(type, name, viewText)
        {
            this.Value = value;
        }

        public Object Value { get; }
    }
}