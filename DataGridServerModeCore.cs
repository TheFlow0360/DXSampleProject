﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;

namespace DevExpressGridInconsistencyDemo
{
    using System.Collections.Generic;
    using DevExpress.Data.Filtering;
    using DevExpress.Data.Helpers;

    public class DataGridServerModeCore : ServerModeCore, IServerModeCore
    {
        private readonly IEntityManager _entityManager;

        public DataGridServerModeCore(IEntityManager entityManager) : this(entityManager, BuildKeyOperator(entityManager)) { }

        private static CriteriaOperator[] BuildKeyOperator(IEntityManager entityManager)
        {
            return CriteriaOperator.ParseList(entityManager.IdColumn);
        }

        public DataGridServerModeCore(IEntityManager entityManager, CriteriaOperator[] key) : base(key)
        {
            _entityManager = entityManager;
        }

        private List<DataInformation> _columnsInformation;
        public List<DataInformation> ColumnsInformation
        {
            get
            {
                if (_columnsInformation == null)
                {
                    _columnsInformation = _entityManager.GetColumnInformation();
                }
                return _columnsInformation;
            }
        }

        private PropertyDescriptorCollection _pdc;
        private PropertyDescriptorCollection Pdc => _pdc ?? (_pdc = CreatePropertyDescripterCollection());

        private PropertyDescriptorCollection CreatePropertyDescripterCollection()
        {
            List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();
            foreach (var column in ColumnsInformation)
            {
                descriptors.Add(new DataGridColumnPropertyDescriptor(column));
            }
            return new PropertyDescriptorCollection(descriptors.ToArray());
        }

        protected override ServerModeCore DXCloneCreate()
        {
            return new DataGridServerModeCore(_entityManager, this.KeysCriteria);
        }
        protected override ServerModeCache CreateCacheCore()
        {
            DataGridServerModeCache cache = new DataGridServerModeCache(_entityManager, FilterClause, KeysCriteria, SortInfo, GroupCount, SummaryInfo, TotalSummaryInfo);
            cache.IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria = FilterClause;
            return cache;
        }
        protected override object[] GetUniqueValues(CriteriaOperator groupByExpression, int maxCount, CriteriaOperator filter)
        {
            try
            {
                IEnumerable<DataFieldList> src = _entityManager.GetEntities(CreateQueryParams(filter, null, groupByExpression));
                if (maxCount > 0)
                    src = src.Take(maxCount);
                List<object> objectList = new List<object>();
                foreach (DataFieldList row in src)
                {
                    objectList.Add(row[0].Value);
                }
                objectList.Sort();
                return objectList.ToArray();
            }
            catch
            {
                return new object[0];
            }
        }

        public static object EvaluateOnInstanceStatic(object rowObj, CriteriaOperator filter)
        {
            DataFieldList row = rowObj as DataFieldList;
            return row?[filter.ToString().Replace("[", "").Replace("]", "")];
        } 

        protected override object EvaluateOnInstance(object rowObj, CriteriaOperator filter)
        {
            return EvaluateOnInstanceStatic(rowObj, filter);
        }
        protected override bool EvaluateOnInstanceLogical(object rowObj, CriteriaOperator filter)
        {
            bool? nullable = (bool?) EvaluateOnInstanceStatic(rowObj, filter);
            return nullable.HasValue && nullable.GetValueOrDefault();
        }

        public override IList GetAllFilteredAndSortedRows()
        {
            return _entityManager.GetEntities(CreateQueryParams(this.FilterClause, this.SortInfo)).ToList();
        }
        public override void Refresh()
        {
            base.Refresh();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public static QueryParameter CreateQueryParams(CriteriaOperator filter, ServerModeOrderDescriptor[] sortInfo = null, CriteriaOperator groupBy = null, ServerModeSummaryDescriptor[] summary = null)
        {
            QueryParameter queryParams = new QueryParameter();
            queryParams.Filter = filter;
            queryParams.GroupBy = groupBy;
            queryParams.SortBy = sortInfo;
            queryParams.Summary = summary;
            return queryParams;       
        }

        #region IBindingList Members
        public object AddNew()
        {
            throw new NotSupportedException();
        }
        public void AddIndex(PropertyDescriptor property)
        {
        }
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
        }
        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }
        public void RemoveIndex(PropertyDescriptor property)
        {
        }
        public void RemoveSort()
        {
        }
        public bool AllowNew => false;
        public bool AllowEdit => false;
        public bool AllowRemove => false;
        public bool SupportsChangeNotification => true;
        public bool SupportsSearching => false;
        public bool SupportsSorting => false;
        public bool IsSorted => false;
        public PropertyDescriptor SortProperty
        {
            get { throw new NotSupportedException(); }
        }
        public ListSortDirection SortDirection
        {
            get { throw new NotSupportedException(); }
        }
        public event ListChangedEventHandler ListChanged;
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }
        #endregion

        #region ITypedList Members
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(DataFieldList).Name;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return Pdc;
        }
        #endregion
    }

    public class DataGridColumnPropertyDescriptor : PropertyDescriptor
    {
        private readonly DataInformation _column;

        public DataGridColumnPropertyDescriptor(DataInformation column) : base(column.Name, null)
        {
            _column = column;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Object GetValue(object component)
        {
            return ((DataFieldList)component)?[_column.Name];
        }

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => typeof(DataFieldList);

        public override bool IsReadOnly => true;

        public override Type PropertyType => _column.Type;
    }
}