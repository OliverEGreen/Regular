using System;
using System.ComponentModel;
using System.Reflection;

namespace Regular.Utilities
{
    public static class EnumUtils
    {
        public static string GetEnumDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null) return null;
            FieldInfo field = type.GetField(name);
            if (field == null) return null;
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) return attr.Description;
            return null;
        }
    }
}
