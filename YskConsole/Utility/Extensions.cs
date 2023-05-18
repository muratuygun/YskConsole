using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YskConsole.Utility
{

    public static class Extensions
    {
        public static IEnumerable<List<T>> Partition<T>(this List<T> values, int chunkSize)
        {
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                yield return values.GetRange(i, Math.Min(chunkSize, values.Count - i));
            }
        }
    }
}
