using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Regular.Enums;

namespace Regular.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumeration)
        {
            FieldInfo fieldInfo = enumeration.GetType().GetField(enumeration.ToString());
            object[] attributeArray = fieldInfo.GetCustomAttributes(false);
            DescriptionAttribute descriptionAttribute = null;

            for (var i = 0; i < attributeArray.Length; i++)
            {
                if (attributeArray[i] is DescriptionAttribute)
                {
                    descriptionAttribute = attributeArray[i] as DescriptionAttribute;
                }
            }
            return descriptionAttribute.Description;
        }

        private RuleType GetRuleTypeByName(string name)
        {
            return (RuleType) Enum.Parse(typeof(RuleType), name);
        }
        
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) => GetEnumDescription((Enum)value);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => GetRuleTypeByName(value.ToString());
    }
}
