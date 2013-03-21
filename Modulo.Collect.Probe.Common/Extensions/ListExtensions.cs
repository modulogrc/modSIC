using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modulo.Collect.Probe.Common.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfUnique(this List<String> list, String itemToAdd)
        {
            if (!list.Contains(itemToAdd))
                list.Add(itemToAdd);
        }
    }
}
