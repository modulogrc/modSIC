using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Modulo.Collect.Probe.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static Boolean HasItems(this IEnumerable<object> array)
        {
            return array != null && array.Count() > 0;
        }

        public static Boolean IsEmpty(this IEnumerable<object> array)
        {
            return array == null || array.Count() == 0;
        }
    }
}
