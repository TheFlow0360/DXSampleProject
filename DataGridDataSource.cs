namespace DevExpressGridInconsistencyDemo
{
    using System.Collections;
    using System.ComponentModel;
    using DevExpress.Data.Helpers;

    public delegate void DataSourceExceptionThrownEventHandler(object sender, DataSourceExceptionThrownEventArgs e);
    public delegate void DataSourceInconsistencyDetectedEventHandler(object sender, DataSourceInconsistencyDetectedEventArgs e);

    public class DataGridDataSource : IListSource
    {
        public IServerModeCore DataFrontend { get; }

        public event DataSourceExceptionThrownEventHandler ExceptionThrown;

        public event DataSourceInconsistencyDetectedEventHandler InconsistencyDetected;

        public DataGridDataSource(IServerModeCore frontend)
        {
            DataFrontend = frontend;
            DataFrontend.InconsistencyDetected += ListOnInconsistencyDetected;
            DataFrontend.ExceptionThrown += ListOnExceptionThrown;
        }

        private void Reload()
        {
            DataFrontend.Refresh();
        }

        private void ListOnExceptionThrown(object sender, DevExpress.Data.ServerModeExceptionThrownEventArgs e)
        {
            OnExceptionThrown(new DataSourceExceptionThrownEventArgs(e.Exception));
        }

        private void ListOnInconsistencyDetected(object sender, DevExpress.Data.ServerModeInconsistencyDetectedEventArgs e)
        {
            DataSourceInconsistencyDetectedEventArgs e2 = new DataSourceInconsistencyDetectedEventArgs();
            e2.Handled = e.Handled;
            OnInconsistencyDetected(e2);
            e.Handled = e2.Handled;
            if (e.Handled)
                return;
            e.Handled = true;
            InconsistentHelper.PostponedInconsistent(Reload, null);
        }

        protected virtual void OnExceptionThrown(DataSourceExceptionThrownEventArgs e)
        {
            ExceptionThrown?.Invoke(this, e);
        }

        protected virtual void OnInconsistencyDetected(DataSourceInconsistencyDetectedEventArgs e)
        {
            InconsistencyDetected?.Invoke(this, e);
        }

        IList IListSource.GetList()
        {
            return DataFrontend;
        }

        public bool ContainsListCollection => false;
    }
}