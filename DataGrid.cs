using System;

namespace DevExpressGridInconsistencyDemo
{
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
                    if (!IsFirstLoad)
                    {
                        return;
                    }

                    if (ServerModeCore != null)
                    {
                        if (ItemsSource == null)
                        {
                            ItemsSource = new DataGridDataSource(ServerModeCore);
                        }

                        CreateColumns();
                    }

                    IsFirstLoad = false;
                }
            };
        }

        private Boolean IsFirstLoad { get; set; } = true;

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
                this.CreateColumns();
                this.RefreshData();
            }
        }

        public static readonly DependencyProperty ColumnTemplateHelpersProperty = DependencyProperty.Register(
           "ColumnTemplateHelpers",
           typeof(ColumnTemplateHelperList),
           typeof(DataGrid),
           new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ColumnTemplateHelpersPropertyChangedCallback));

        private static void ColumnTemplateHelpersPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var This = (DataGrid)d;
            if (This == null || This.IsFirstLoad)
            {
                return;
            }
            This.CreateColumns();
        }

        public ColumnTemplateHelperList ColumnTemplateHelpers
        {
            get { return (ColumnTemplateHelperList)GetValue(ColumnTemplateHelpersProperty); }
            set { SetValue(ColumnTemplateHelpersProperty, value); }
        }

        protected override DataViewBase CreateDefaultView()
        {
            return new TableView();
        }

        private void CreateColumns()
        {
            this.Columns.Clear();

            var columnInformations = this.ServerModeCore.ColumnsInformation;

            foreach (var columnInformation in columnInformations)
            {
                var column = new GridColumn()
                {
                    Header = columnInformation.Name,
                    FieldName = columnInformation.Name,
                    Width = new GridColumnWidth(200)
                };

                if (this.ColumnTemplateHelpers != null && this.ColumnTemplateHelpers.Count > 0)
                {
                    foreach (var templateHelper in ColumnTemplateHelpers)
                    {
                        if (templateHelper.ColumnFieldName != columnInformation.Name)
                        {
                            continue;
                        }

                        templateHelper.ApplyTemplates(column);
                        break;
                    }
                }

                this.Columns.Add(column);
            }
        }
    }
}