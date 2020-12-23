using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Utilities
{
    public static class ListUtils
    {
        public static IList<T> ConvertListToIList<T>(List<T> inputList)
        {
            IList<T> iList = new List<T>();
            foreach (T input in inputList) { iList.Add(input); }
            return iList;
        }
    }
}
