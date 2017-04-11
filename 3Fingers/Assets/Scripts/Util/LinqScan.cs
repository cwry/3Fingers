using System.Collections.Generic;

namespace System.Linq {
    public static partial class LinqExtensions {
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, Func<U, T, U> next, U state) {
            yield return state;
            foreach (var item in input) {
                state = next(state, item);
                yield return state;
            }
        }
    }
}