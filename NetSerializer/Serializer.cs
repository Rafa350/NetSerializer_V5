using System;
using NetSerializer.V5.Formatters;
using NetSerializer.V5.TypeSerializers;

namespace NetSerializer.V5 {

    /// <summary>
    /// Serializador.
    /// </summary>
    /// 
    public sealed class Serializer {

        private readonly TypeSerializerProvider _typeSerializerProvider;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        public Serializer() {

            _typeSerializerProvider = new TypeSerializerProvider();
        }

        /// <summary>
        /// Serialitza un objecte.
        /// </summary>
        /// <param name="obj">El objecte a serialitzar.</param>
        /// <param name="name">Identificador del objecte.</param>
        /// 
        public void Serialize(FormatWriter writer, object obj, string name = "root", int version = 0) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            writer.Initialize(version);
            try {
                var context = new SerializationContext(writer, _typeSerializerProvider);

                var typeSerializer = context.GetTypeSerializer(obj.GetType());
                typeSerializer.Serialize(context, name, obj.GetType(), obj);
            }
            finally {
                writer.Close();
            }
        }

        /// <summary>
        /// Deserializa un objeto
        /// </summary>
        /// <param name="type">Tipo del objeto a deserializar. Si es nulo dispara una excepcion.</param>
        /// <param name="name">Nombre del nodo raiz.</param>
        /// <returns>El objeto deserializado.</returns>
        /// <exception cref="ArgumentNullException">Algun argumento es nulo.</exception>
        /// 
        public object Deserialize(FormatReader reader, Type type, string name) {

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            reader.Initialize();
            try {
                var context = new DeserializationContext(reader, _typeSerializerProvider);

                var typeSerializer = context.GetTypeSerializer(type);
                typeSerializer.Deserialize(context, name, type, out object obj);

                return obj;
            }
            finally {
                reader.Close();
            }
        }

        /// <summary>
        /// Deserializa un objeto (Version generica).
        /// </summary>
        /// <param name="name">Nombre del nodo raiz.</param>
        /// <returns>El objeto deserializado.</returns>
        /// <exception cref="ArgumentNullException">Algun argumento es nulo.</exception>
        /// 
        public T Deserialize<T>(FormatReader reader, string name) {

            return (T)Deserialize(reader, typeof(T), name);
        }

        /// <summary>
        /// Obte el proveidor de serialitzadors.
        /// </summary>
        /// 
        public TypeSerializerProvider TypeSerializerProvider =>
            _typeSerializerProvider;
    }
}
