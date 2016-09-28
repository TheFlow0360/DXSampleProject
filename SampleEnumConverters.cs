using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DevExpressGridInconsistencyDemo
{
    public static class SampleEnumConverterHelper
    {
        public static bool IsValidSampleEnum(object value, out SampleEnum property)
        {
            property = SampleEnum.None;
            
            if (value == null)
            {
                return false;
            }

            if (value is SampleEnum)
            {
                property = (SampleEnum)value;
            }
            else if (value is SampleEnumWrapper)
            {
                var adressWrapper = (SampleEnumWrapper)value;
                if (adressWrapper?.Enum != null)
                {
                    property = adressWrapper.Enum.Value;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Int16 numericValue;
                if (value is String)
                {
                    if (!Int16.TryParse((String)value, out numericValue))
                    {
                        return false;
                    }
                }
                else if (value is Int16)
                {
                    numericValue = (Int16)value;
                }
                else
                {
                   System.Console.WriteLine("Tried to convert invalid type: Expected was AdressStatus(Wrapper)/int/string, but was {0}.", value.GetType());
                   return false;    
                }
                property = (SampleEnum)numericValue;
            }
            return true;
        }
    }

    public class SampleEnumToCaptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SampleEnum property;
            if (SampleEnumConverterHelper.IsValidSampleEnum(value, out property))
            {
                switch (property)
                {
                    case SampleEnum.None:
                        return "<None>";
                    case SampleEnum.Low:
                        return "Low";
                    case SampleEnum.Medium:
                        return "Medium";
                        case SampleEnum.High:
                        return "High";
                    case SampleEnum.Locked:
                        return "Locked";
                    case SampleEnum.Deleted:
                        return "Deleted";
                    default:
                        break;
                }
            }
            return "<All>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SampleEnumToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SampleEnum property;
            if (SampleEnumConverterHelper.IsValidSampleEnum(value, out property))
            {
                switch (property)
                {
                    case SampleEnum.Low:
                        return new SolidColorBrush(Colors.CornflowerBlue); ;
                    case SampleEnum.Medium:
                        return new SolidColorBrush(Colors.LimeGreen); ;
                    case SampleEnum.High:
                        return new SolidColorBrush(Colors.Coral); ;
                    case SampleEnum.Locked:
                        return new SolidColorBrush(Colors.Gray); ;
                    case SampleEnum.Deleted:
                        return new SolidColorBrush(Colors.Crimson); ;
                    default:
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
