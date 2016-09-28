using System;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;

namespace DevExpressGridInconsistencyDemo
{
    public class DataGridColumnTemplateHelper
    {
        public String ColumnFieldName { get; set; }

        public DataTemplate CellTemplate { get; set; }

        public DataTemplate GroupValueTemplate { get; set; }

        public Boolean UseCustomFilterPopup => CustomColumnFilterPopupTemplate != null;

        public DataTemplate CustomColumnFilterPopupTemplate { get; set; }

        public BaseEditSettings EditSettings { get; set; }

        public Boolean ImmediateUpdateAutoFilter { get; set; }

        public void ApplyTemplates(GridColumn column)
        {
            column.CellTemplate = CellTemplate;
            column.GroupValueTemplate = GroupValueTemplate;
            column.FilterPopupMode = UseCustomFilterPopup ? FilterPopupMode.Custom : FilterPopupMode.Default;
            column.CustomColumnFilterPopupTemplate = CustomColumnFilterPopupTemplate;
            column.EditSettings = EditSettings;
            column.ImmediateUpdateAutoFilter = ImmediateUpdateAutoFilter;
        }
    }

    public class ColumnTemplateHelperList : List<DataGridColumnTemplateHelper> { }
}