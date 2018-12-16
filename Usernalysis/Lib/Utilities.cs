using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usernalysis.Lib
{
    public class Utilities
    {
        public static IList<KeyValuePair<T, U>> GetTopSortedFromDictionary<T, U>(IDictionary<T, U> dictionary, int topN = 10)
        {
            var result = new List<KeyValuePair<T, U>>();
            var query = (from entry in dictionary
                         orderby entry.Value descending
                         select entry).Take(topN);
            foreach (var entry in query)
            {
                result.Add(new KeyValuePair<T, U>(entry.Key, entry.Value));
            }
            return result;
        }

    }
}
