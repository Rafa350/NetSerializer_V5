using System;
using System.ComponentModel;
using NetSerializer.V5.Storage;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serializa i deserializa tipos primitivos, enumeradores, DateTime y TimeStamp.
    /// </summary>
    /// 
    public class ValueSerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public ValueSerializer(ITypeSerializerProvider typeSerializerProvider) :
            base(typeSerializerProvider) {

        }

        /// <summary>
        /// Comprueba si puede serializar o deserializar el tipo de objeto especificado.
        /// </summary>
        /// <param name="type">El tipo de objeto a verificar. Si es nulo, dispara una excepcion.</param>
        /// <returns>True si es posible serializar o deserializar el tipo de objeto, false en caso contrario.</returns>
        /// <exception cref="ArgumentnullException">Algun argumento requerido es nulo.</exception>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                (type.IsValueType && TypeDescriptor.GetConverter(type) != null);
        }

        /// <summary>
        /// Serializa un objeto.
        /// </summary>
        /// <param name="writer">Objeto StorageWriter. Si es nulo, dispara una excepcion.</param>
        /// <param name="name">El nombre del nodo.</param>
        /// <param name="type">El tipo de objeto a serializar. Si es nulo genera una excepcion.</param>
        /// <param name="obj">El objeto a serializar.</param>
        /// <exception cref="InvalidOperationException">No es posible serializar el objeto.</exception>
        /// <exception cref="ArgumentNullException">Algun argumento requerido es nulo.</exception>
        /// 
        public override void Serialize(StorageWriter writer, string name, Type type, object obj) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));

            if (obj == null)
                writer.WriteNull(name);
            else
                writer.WriteValue(name, obj);
        }

        /// <summary>
        /// Deserializa un objeto.
        /// </summary>
        /// <param name="reader">El objeto StorageReader. Si es nulo, dispara una excepcion.</param>
        /// <param name="name">El nombre del nodo.</param>
        /// <param name="type">El tipo de objeto a deserializar. Si es nulo dispara una excepcion.</param>
        /// <param name="obj">El objeto deserializado.</param>
        /// <exception cref="InvalidOperationException">No es posible deserializar el objeto.</exception>
        /// <exception cref="ArgumentNullException">Algun argumento requerido es nulo.</exception>
        /// 
        public override void Deserialize(StorageReader reader, string name, Type type, out object obj) {

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible deserializar el tipo '{0}'.", type.ToString()));

            obj = reader.ReadValue(name, type);
        }
    }
}
