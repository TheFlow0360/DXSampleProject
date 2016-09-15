namespace DevExpressGridInconsistencyDemo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DevExpress.Data;
    using DevExpress.Data.Helpers;

    public interface IServerModeCore: IBindingList, IList, ITypedList, ICollection, IEnumerable, IListServer, IListServerHints
    {
        List<DataInformation> ColumnsInformation { get; }
    }
}