using System.Collections.Generic;
using System;

namespace ItchyOwl.General
{
    public static class EnumHelpers
    {
        /// <summary>
        /// Generic and type safe method for getting all the values of an enumeration.
        /// Note: Creates a new HashSet
        /// </summary>
        public static HashSet<T> GetEnumValues<T>() where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("[Helpers] The type of the generic method must be of type Enum", "type");
            }
            var values = new HashSet<T>();
            foreach (var t in Enum.GetValues(type))
            {
                values.Add((T)t);
            }
            return values;
        }
    }
}
