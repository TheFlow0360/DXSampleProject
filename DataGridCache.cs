using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;

namespace DevExpressGridInconsistencyDemo
{
    public class DataGridServerModeCache : ServerModeKeyedCache
    {
        private readonly IEntityManager _entityManager;
        private readonly CriteriaOperator _globalFilter;

        public DataGridServerModeCache(IEntityManager entityManager, CriteriaOperator globalFilter, CriteriaOperator[] keyCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary) 
            : base(keyCriteria, sortInfo, groupCount, summary, totalSummary)
        {
            _entityManager = entityManager;
            _globalFilter = globalFilter;
        }

        protected override ServerModeGroupInfoData[] PrepareChildren(CriteriaOperator groupWhere, CriteriaOperator groupByCriterion, CriteriaOperator orderByCriterion, bool isDesc, ServerModeSummaryDescriptor[] summaries)
        {
            List<ServerModeGroupInfoData> infoDataList = new List<ServerModeGroupInfoData>();
            QueryParameter queryParams = CreateQueryParams(groupWhere, new[] {new ServerModeOrderDescriptor(groupByCriterion, isDesc)}, groupByCriterion, summaries);
            IList<DataFieldList> rows = _entityManager.GetEntities(queryParams);
            foreach (DataFieldList row in rows)
            {
                object[] objArray = new object[summaries.Length];
                for (int index = 0; index < summaries.Length; ++index)
                    objArray[index] = row[index + 2].Value;
                infoDataList.Add(new ServerModeGroupInfoData(row[0].Value, (int)row[1].Value, objArray));
            }
            return infoDataList.ToArray();
        }

        protected override ServerModeGroupInfoData PrepareTopGroupInfo(ServerModeSummaryDescriptor[] summaries)
        {
            QueryParameter queryParams = CreateQueryParams(null, null, null, summaries);
            IList<DataFieldList> rows = _entityManager.GetEntities(queryParams);
            foreach (DataFieldList row in rows)
            {
                object[] objArray = new object[summaries.Length];
                for (int index = 0; index < summaries.Length; ++index)
                    objArray[index] = row[index + 1].Value;
                return new ServerModeGroupInfoData(null, (int)row[0].Value, objArray);
            }
            return new ServerModeGroupInfoData(null, 0, new object[summaries.Length]);
        }

        protected override Type ResolveKeyType(CriteriaOperator singleKeyToResolve)
        {
            return _entityManager.GetColumnInformation().FirstOrDefault(information => information.Name == singleKeyToResolve.ToString().Replace("[", "").Replace("]", ""))?.Type; 
        }

        protected override Type ResolveRowType()
        {
            return typeof(DataFieldList);
        }

        protected override object[] FetchKeys(CriteriaOperator @where, ServerModeOrderDescriptor[] order, int skip, int take)
        {
            QueryParameter queryParams = CreateQueryParams(where, order);
            if (take > 0)
                queryParams.Take = take;
            if (skip > 0)
                queryParams.Skip = skip;
            return _entityManager.GetKeys(queryParams).ToArray();
        }

        protected override object[] FetchRows(CriteriaOperator @where, ServerModeOrderDescriptor[] order, int take)
        {
            QueryParameter queryParams = CreateQueryParams(where, order);
            if (take > 0)
                queryParams.Take = take;
            return _entityManager.GetEntities(queryParams).Cast<object>().ToArray();
        }

        protected override int GetCount(CriteriaOperator criteriaOperator)
        {
            return _entityManager.GetEntityCount(CreateQueryParams(criteriaOperator));
        }

        protected override object EvaluateOnInstance(object rowObj, CriteriaOperator criteriaOperator)
        {
            return DataGridServerModeCore.EvaluateOnInstanceStatic(rowObj, criteriaOperator);
        }

        private QueryParameter CreateQueryParams(CriteriaOperator filter, ServerModeOrderDescriptor[] orderBy = null, CriteriaOperator groupBy = null, ServerModeSummaryDescriptor[] summary = null)
        {
            CriteriaOperator compoundFilter = CriteriaOperator.And(_globalFilter, filter);
            return DataGridServerModeCore.CreateQueryParams(compoundFilter, orderBy, groupBy, summary);
        }
    }
}