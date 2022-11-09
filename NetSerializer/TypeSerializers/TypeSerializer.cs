using System;
using NetSerializer.V5.Storage;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Clase abstracta de la que deriven tots els serializadors de tipus.
    /// </summary>
    /// 
    public abstract class TypeSerializer: ITypeSerializer {

        private readonly ITypeSerializerProvider _typeSerializerProvider;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="typeSerializerProvider">Un objecte proveidor de serialitzadors.</param>
        /// 
        public TypeSerializer(ITypeSerializerProvider typeSerializerProvider) {

            _typeSerializerProvider = typeSerializerProvider ?? throw new ArgumentNullException(nameof(typeSerializerProvider));
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

        /// <summary>
        /// Obte el proveidor de serialitzadors
        /// </summary>
        /// 
        protected ITypeSerializer GetSerializer(Type type) =>
            _typeSerializerProvider.GetSerializer(type);
    }
}
