using DevExpress.Data;

namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Windows;
    using DevExpress.Xpf.Grid;

    public class DataGrid : GridControl
    {
        public DataGrid()
        {
            this.Loaded += (sender, args) =>
            {
                this.AutoGenerateColumns = AutoGenerateColumnsMode.None;
                this.AutoExpandAllGroups = true;
                this.SelectionMode = MultiSelectMode.Row;

                if (ServerModeCore != null && this.ItemsSource == null)
                {
                    this.ItemsSource = new DataGridDataSource(ServerModeCore);
                    this.CreateCustomColumns();
                    this.RefreshData();
                    this.Focus(); 
                }
            };
        }

        public static readonly DependencyProperty ServerModeCoreProperty = DependencyProperty.Register(
            "ServerModeCore",
            typeof(IServerModeCore),
            typeof(DataGrid),
            new PropertyMetadata(null));

        public IServerModeCore ServerModeCore
        {
            get { return (IServerModeCore)this.GetValue(ServerModeCoreProperty); }
            set
            {
                this.SetValue(ServerModeCoreProperty, value);
                this.ItemsSource = new DataGridDataSource(ServerModeCore);
                this.CreateCustomColumns();
                this.RefreshData();
            }
        }

        protected override DataViewBase CreateDefaultView()
        {
            return new TableView();
        }

        private void CreateCustomColumns()
        {
            var columnInformations = this.ServerModeCore.ColumnsInformation;

            foreach (var columnInformation in columnInformations)
            {
                var column = new GridColumn()
                {
                    Header = columnInformation.Name,
                    FieldName = columnInformation.Name,
                    Width = new GridColumnWidth(200),
                    //Binding = new Binding("[" + columnInformation.Name + "]"),
                    //UnboundExpression = "[" + columnInformation.Name + "]",
                    //UnboundType = this.GetDevExpColumnType(columnInformation.Type),
                };
                this.Columns.Add(column);
            }
        }

        private UnboundColumnType GetDevExpColumnType(Type columnType)
        {
            System.TypeCode typeCode = Type.GetTypeCode(columnType);
            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.Char:
                    return UnboundColumnType.String;
                case TypeCode.Boolean:
                    return UnboundColumnType.Boolean;
                case TypeCode.DateTime:
                    return UnboundColumnType.DateTime;
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return UnboundColumnType.Integer;
                case TypeCode.Single:
                case TypeCode.Decimal:
                case TypeCode.Double:
                    return UnboundColumnType.Decimal;
                default:
                    return UnboundColumnType.Object;
            }
        }
    }
}