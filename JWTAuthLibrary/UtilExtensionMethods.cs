using System.Collections.Generic;
using System.Linq;

namespace JWTAuthLibrary
{
    internal static class UtilExtensionMethods
    {
        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> input)
            => input ?? Enumerable.Empty<T>();
    }
}