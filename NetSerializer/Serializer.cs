using System;
using NetSerializer.V5.Storage;
using NetSerializer.V5.TypeSerializers;

namespace NetSerializer.V5 {

    /// <summary>
    /// Serializador.
    /// </summary>
    /// 
    public sealed class Serializer {

        private readonly StorageWriter _writer;
        private readonly TypeSerializerProvider _typeManager = TypeSerializerProvider.Instance;
        private readonly int _version;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="writer">Objecte escriptor de dades.</param>
        /// <param name="version">Numero de versio.</param>
        /// <seealso cref="StorageWriter"/>
        /// 
        public Serializer(StorageWriter writer, int version) {

            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _version = version;
        }

        /// <summary>
        /// Tanca el serializador.
        /// </summary>
        /// 
        public void Close() {

            _writer.Close();
        }

        /// <summary>
        /// Afegeix un serialitzador.
        /// </summary>
        /// <param name="serializer">El serialitzador a afeigir.</param>
        /// 
        public void AddSerializer(ITypeSerializer serializer) {

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _typeManager.AddSerializer(serializer);
        }

        /// <summary>
        /// Serialitza un objecte.
        /// </summary>
        /// <param name="obj">El objecte a serialitzar.</param>
        /// <param name="name">Identificador del objecte.</param>
        /// 
        public void Serialize(object obj, string name) {

            _writer.Initialize(_version);
            _typeManager.Initialize();

            var serializer = _typeManager.GetSerializer(obj.GetType());
            serializer.Serialize(_writer, name, obj.GetType(), obj);
        }
    }
}
