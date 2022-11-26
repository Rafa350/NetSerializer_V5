using System;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Extensions de la clase 'Type'.
    /// </summary>
    internal static class TypeExtensions {

        /// <summary>
        /// Identifica les classes que s'utilitzen com a tipus (String, DateTime, etc).
        /// </summary>
        /// <param name="type">This</param>
        /// <returns>True en cas afirmatiu.</returns>
        /// 
        public static bool IsSpecialClass(this Type type) {

            return 
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(TimeSpan)) ||
                (type == typeof(Guid)) ||
                (type == typeof(decimal));
        }
    }
}
