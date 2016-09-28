using System;
using System.Globalization;
using System.Windows.Data;

namespace DevExpressGridInconsistencyDemo
{
    public enum SampleEnum
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Locked = 4,
        Deleted = 5
    }

    public class SampleEnumWrapper
    {
        public SampleEnum? Enum;
    }

    public class SampleEnumWrapperConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var enumWrapper = new SampleEnumWrapper();
            if (value is SampleEnum?)
            {
                enumWrapper.Enum = (SampleEnum?)value;
            }
            return enumWrapper;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var enumWrapper = value as SampleEnumWrapper;
            if (enumWrapper == null || !enumWrapper.Enum.HasValue)
            {
                return null;
            }
            return enumWrapper.Enum.Value;
        }
    }
}