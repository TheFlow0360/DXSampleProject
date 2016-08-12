namespace DevExpressGridInconsistencyDemo
{
    using System.Collections.Generic;

    public interface IEntityManager
    {
        List<DataInformation> GetColumnInformation();

        List<DataFieldList> GetEntities(QueryParameter queryParameter);

        List<object> GetKeys(QueryParameter queryParameter);

        int GetEntityCount(QueryParameter queryParameter);

        string IdColumn { get; }
       
    }
}