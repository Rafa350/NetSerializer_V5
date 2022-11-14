using System;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Proveidor de serialitzadors
    /// </summary>
    /// 
    public interface ITypeSerializerProvider {

        /// <summary>
        /// Obte el serialitzador pel tipus especificat.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>El serialitzador. Null si no troba cap.</returns>
        /// 
        ITypeSerializer GetTypeSerializer(Type type);
    }
}
