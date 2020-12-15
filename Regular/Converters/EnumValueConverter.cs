using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Regular.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? DependencyProperty.UnsetValue : GetDescription((Enum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo.Length <= 0) return en.ToString();
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : en.ToString();
        }
    }
}
