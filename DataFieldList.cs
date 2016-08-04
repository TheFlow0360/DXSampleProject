using System.Collections.Generic;

namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Linq;

    public class DataFieldList : List<DataField>
    {
        public Object this[String name]
        {
            get
            {
                return this.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
            }
        }
    }
}