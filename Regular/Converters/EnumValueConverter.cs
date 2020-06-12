using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Regular.Converters
{
    public class EnumValueConverter : IValueConverter
    {
        private static string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attributeArray = fieldInfo.GetCustomAttributes(false);

            if (attributeArray.Length == 0) { return enumObj.ToString(); }
            DescriptionAttribute attribute = attributeArray[0] as DescriptionAttribute;
            return attribute?.Description;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
