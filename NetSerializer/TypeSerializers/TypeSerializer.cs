using System;
using NetSerializer.V5.Storage;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Clase abstracta de la que deriven tots els serializadors de tipus.
    /// </summary>
    /// 
    public abstract class TypeSerializer: ITypeSerializer {

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// 
        public TypeSerializer() {
        }

        /// <inheritdoc/>
        /// 
        public abstract bool CanSerialize(Type type);

        /// <inheritdoc/>
        /// 
        public virtual void Initialize() {
        }

        /// <inheritdoc/>
        /// 
        public abstract void Serialize(StorageWriter write, string name, Type type, object obj);

        /// <inheritdoc/>
        /// 
        public abstract void Deserialize(StorageReader reader, string name, Type type, out object obj);
    }
}
