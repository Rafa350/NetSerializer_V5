using System;
using NetSerializer.V5.Storage;
using NetSerializer.V5.TypeSerializers;

namespace NetSerializer.V5 {

    /// <summary>
    /// Deserializacion de objetos.
    /// </summary>
    /// 
    public sealed class Deserializer {

        private readonly TypeSerializerProvider _typeManager = TypeSerializerProvider.Instance;
        private readonly StorageReader _reader;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="reader">Objeto StorageReader. Si es nulo dispara una excepcion.</param>
        /// <exception cref="ArgumentNullException">Elgun argumento es nulo.</exception>
        /// 
        public Deserializer(StorageReader reader) {

            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>
        /// Cierra el deserializador.
        /// </summary>
        /// 
        public void Close() {

            _reader.Close();
        }

        public void AddSerializer(ITypeSerializer serializer) {

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _typeManager.AddSerializer(serializer);
        }

        /// <summary>
        /// Deserializa un objeto
        /// </summary>
        /// <param name="type">Tipo del objeto a deserializar. Si es nulo dispara una excepcion.</param>
        /// <param name="name">Nombre del nodo raiz.</param>
        /// <returns>El objeto deserializado.</returns>
        /// <exception cref="ArgumentNullException">Algun argumento es nulo.</exception>
        /// 
        public object Deserialize(Type type, string name) {

            _reader.Initialize();
            _typeManager.Initialize();

            var serializer = _typeManager.GetSerializer(type);
            serializer.Deserialize(_reader, name, type, out object obj);

            return obj;
        }

        /// <summary>
        /// Deserializa un objeto (Version generica).
        /// </summary>
        /// <param name="name">Nombre del nodo raiz.</param>
        /// <returns>El objeto deserializado.</returns>
        /// <exception cref="ArgumentNullException">Algun argumento es nulo.</exception>
        /// 
        public T Deserialize<T>(string name) {

            return (T)Deserialize(typeof(T), name);
        }
    }
}
