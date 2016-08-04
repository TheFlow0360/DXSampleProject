namespace DevExpressGridInconsistencyDemo
{
    using DevExpress.Data;
    using DevExpress.Data.Filtering;

    public class QueryParameter
    {
        public QueryParameter()
        {
            PagingActive = false;
        }

        public bool PagingActive { get; set; }

        private int _skip;

        public int Skip
        {
            get { return _skip; }
            set
            {
                _skip = value;
                PagingActive = true;
            }
        }

        private int _take;

        public int Take
        {
            get { return _take; }
            set
            {
                _take = value;
                PagingActive = true;
            }
        }

        public CriteriaOperator Filter { get; set; }

        public CriteriaOperator GroupBy { get; set; }

        public ServerModeOrderDescriptor[] SortBy { get; set; }

        public ServerModeSummaryDescriptor[] Summary { get; set; }
    }
}
