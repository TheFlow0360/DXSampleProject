namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using DevExpress.Xpf.Grid;

    public class DataGridCellTemplateSelector : DataTemplateSelector
    {
        public TemplateList Templates { get; set; }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            var data = item as EditGridCellData;
            return data != null ? Templates.FirstOrDefault(t => t.ColumnFieldName == data.Column?.FieldName) : null;
        }
    }

    public class DataGridCellTemplate : DataTemplate
    {
        public String ColumnFieldName { get; set; }
    }

    public class TemplateList : List<DataGridCellTemplate>
    {

    }
}