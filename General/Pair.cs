using System;

namespace ItchyOwl.General
{
    /// <summary>
    /// Generic pair struct. In .Net 4.0+, you can use generic Tuples instead.
    /// </summary>
    [Serializable]
    public struct Pair<T1, T2>
    {
        public T1 first;
        public T2 second;

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
