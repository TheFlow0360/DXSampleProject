using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Grid;

namespace DevExpressGridInconsistencyDemo
{
    class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object item, Type targetType, object parameter, CultureInfo culture)
        {
            var data = item as EditGridCellData;
            if (data == null || data.Value == null)
            {
                return null;
            }

            var boolean = (Boolean) data.Value;
            switch (boolean)
            {
                case true:
                    return new SolidColorBrush(Colors.Green);
                case false:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Blue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
