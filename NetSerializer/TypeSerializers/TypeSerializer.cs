using System;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Clase abstracta de la que deriven tots els serializadors de tipus.
    /// </summary>
    /// 
    public abstract class TypeSerializer: ITypeSerializer {

        /// <inheritdoc/>
        /// 
        public abstract bool CanProcess(Type type);

        /// <inheritdoc/>
        /// 
        public abstract void Serialize(SerializationContext context, string name, Type type, object obj);

        /// <inheritdoc/>
        /// 
        public abstract void Deserialize(DeserializationContext context, string name, Type type, out object obj);
    }
}
