namespace DevExpressGridInconsistencyDemo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DevExpress.Data;
    using DevExpress.Data.Helpers;

    public interface IDataGridRepository: IBindingList, IList, ICollection, IEnumerable, IListServer, IListServerHints
    {
        List<DataInformation> ColumnInformation { get; }
    }
}