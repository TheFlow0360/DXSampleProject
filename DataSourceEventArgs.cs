using System;

namespace DevExpressGridInconsistencyDemo
{
    public class DataSourceInconsistencyDetectedEventArgs : EventArgs
    {
        private bool _handled;

        public bool Handled
        {
            get { return this._handled; }
            set { this._handled = value; }
        }
    }

    public class DataSourceExceptionThrownEventArgs : EventArgs
    {
        private Exception _Exception;

        public Exception Exception
        {
            get
            {
                return this._Exception;
            }
        }

        public DataSourceExceptionThrownEventArgs(Exception exception)
        {
            this._Exception = exception;
        }
    }
}