using System;
using NetSerializer.V5.Formatters;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Clase abstracta de la que deriven tots els serializadors de tipus.
    /// </summary>
    /// 
    public abstract class TypeSerializer: ITypeSerializer {

        /// <inheritdoc/>
        /// 
        public abstract bool CanSerialize(Type type);

        /// <inheritdoc/>
        /// 
        public virtual void Initialize() {
        }

        /// <inheritdoc/>
        /// 
        public abstract void Serialize(FormatWriter write, string name, Type type, object obj);

        /// <inheritdoc/>
        /// 
        public abstract object Deserialize(FormatReader reader, string name, Type type);
    }
}
