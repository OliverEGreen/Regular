using System.ComponentModel;

namespace Regular.Enums
{
    public enum CaseSensitivity
    {
        [Description("UPPER CASE")]
        UpperCase = 0,
        [Description("Any Case")]
        AnyCase = 1,
        [Description("lower case")]
        LowerCase = 2,
        [Description("")]
        None = 3
    }
}
