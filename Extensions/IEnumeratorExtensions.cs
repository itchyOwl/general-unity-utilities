using System.Collections.Generic;
using System.Linq;

namespace ItchyOwl.Extensions
{
    public static class IEnumeratorExtensions
    {
        /// <summary>
        /// Advances the enumerator/iterator to the next and by default resets when the last item is reached. 
        /// Resetting can be disabled, in which case returns the last item.
        /// Returns the iterator.
        /// Usage: 
        /// var iterator = collection.GetEnumerator();
        /// var nextIteration = iterator.GetNext();
        /// </summary>
        public static IEnumerable<T> GetNext<T>(this IEnumerator<T> iterator, bool resetAfterLast = true)
        {
            if (!iterator.MoveNext())
            {
                if (resetAfterLast)
                {
                    iterator.Reset();
                    iterator.MoveNext();
                }
            }
            yield return iterator.Current;
        }

        /// <summary>
        /// Advances the enumerator/iterator to the next and by default resets when the last item is reached. 
        /// Resetting can be disabled, in which case returns the last item.
        /// var iterator = collection.GetEnumerator();
        /// var nextItem = iterator.GetNextItem();
        /// </summary>
        public static T GetNextItem<T>(this IEnumerator<T> iterator, bool resetAfterLast = true)
        {
            return iterator.GetNext(resetAfterLast).First();
        }
    }
}
