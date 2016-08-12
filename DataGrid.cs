using DevExpress.Data;

namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using DevExpress.Xpf.Grid;

    public class DataGrid : GridControl
    {
        static DataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }

        public DataGrid()
        {
            this.Loaded += (sender, args) =>
            {
                this.AutoGenerateColumns = AutoGenerateColumnsMode.None;
                this.AutoExpandAllGroups = true;
                this.SelectionMode = MultiSelectMode.MultipleRow;

                if (DataGridRepository != null && this.ItemsSource == null)
                {
                    this.ItemsSource = new DataGridDataSource(DataGridRepository);
                    this.CreateCustomColumns();
                    this.RefreshData();
                    this.Focus(); 
                }
            };
        }

        public static readonly DependencyProperty DataGridRepositoryProperty = DependencyProperty.Register(
            "DataGridRepository",
            typeof(IDataGridRepository),
            typeof(DataGrid),
            new PropertyMetadata(null));

        public IDataGridRepository DataGridRepository
        {
            get { return (IDataGridRepository)this.GetValue(DataGridRepositoryProperty); }
            set
            {
                this.SetValue(DataGridRepositoryProperty, value);
                this.ItemsSource = new DataGridDataSource(DataGridRepository);
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
            var columnInformations = this.DataGridRepository.ColumnInformation;

            foreach (var columnInformation in columnInformations)
            {
                var column = new GridColumn()
                {
                    Header = columnInformation.Name,
                    Width = new GridColumnWidth(200),
                    Binding = new Binding("[" + columnInformation.Name + "]"),
                    UnboundExpression = "[" + columnInformation.Name + "]",
                    UnboundType = this.GetDevExpColumnType(columnInformation.Type),
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