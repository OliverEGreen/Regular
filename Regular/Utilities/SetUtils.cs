using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace Regular.Utilities
{
    public static class SetUtils
    {
        public static List<Parameter> ConvertParameterSetToList(ParameterSet parameterSet)
        {
            return (from Parameter parameter in parameterSet select parameter).ToList();
        }
    }
}
