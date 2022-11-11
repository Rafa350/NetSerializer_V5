using System;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Formatters;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    public class StructSerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        /// <inheritdoc/>
        /// 
        public override void Serialize(FormatWriter writer, string name, Type type, object obj) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type));

            if (writer.CanWriteValue(type)) {
                writer.WriteValueStart(name, type);
                writer.WriteValue(obj);
                writer.WriteValueEnd();
            }

            else {
                writer.WriteStructStart(name, obj);
                SerializeStruct(writer, obj);
                writer.WriteStructEnd();
            }
        }

        /// <inheritdoc/>
        /// 
        public override object Deserialize(FormatReader reader, string name, Type type) {

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible deserializar el tipo '{0}'.", type));

            object obj;
            if (reader.CanReadValue(type))
                obj = reader.ReadValue(name, type);

            else {
                reader.ReadStructStart(name, type);
                obj = Activator.CreateInstance(type);
                DeserializeStruct(reader, obj);
                reader.ReadStructEnd();
            }
            return obj;
        }

        /// <summary>
        /// Comprova si es pot serialitzar la propietat.
        /// </summary>
        /// <param name="propertyDescriptor">Descriptor de la propietat.</param>
        /// <returns>True si pot serialitzar.</returns>
        /// 
        protected virtual bool CanSerializeProperty(PropertyDescriptor propertyDescriptor) {

            return propertyDescriptor.CanRead && propertyDescriptor.CanWrite;
        }

        /// <summary>
        /// Serialitza l'objecte.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="obj">L'objecte a serialitzar.</param>
        /// 
        protected virtual void SerializeStruct(FormatWriter writer, object obj) {

            var type = obj.GetType();
            var descriptor = TypeDescriptorProvider.Instance.GetDescriptor(type);

            foreach (var propertyDescriptor in descriptor.PropertyDescriptors)
                if (CanSerializeProperty(propertyDescriptor))
                    SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <summary>
        /// Serialitza una propietat.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="obj">L'objecte.</param>
        /// <param name="propertyDescriptor">El descriptor de la propietat.</param>
        /// 
        protected virtual void SerializeProperty(FormatWriter writer, object obj, PropertyDescriptor propertyDescriptor) {

            var serializer = TypeSerializerProvider.Instance.GetSerializer(propertyDescriptor.Type);
            serializer.Serialize(writer, propertyDescriptor.Name, propertyDescriptor.Type, propertyDescriptor.GetValue(obj));
        }

        /// <summary>
        /// Deserialitza l'objecte.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="obj">L'objecte a deserialitzar.</param>
        /// 
        protected virtual void DeserializeStruct(FormatReader reader, object obj) {

            var type = obj.GetType();
            var descriptor = TypeDescriptorProvider.Instance.GetDescriptor(type);

            foreach (var propertyDescriptor in descriptor.PropertyDescriptors)
                if (CanSerializeProperty(propertyDescriptor))
                    DeserializeProperty(reader, obj, propertyDescriptor);
        }

        /// <summary>
        /// Deserialitza una propietat.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="obj">L'objecte.</param>
        /// <param name="propertyDescriptor">El descriptor de la propietat.</param>
        /// 
        protected virtual void DeserializeProperty(FormatReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var typeSerializer = TypeSerializerProvider.Instance.GetSerializer(propertyDescriptor.Type);
            var value = typeSerializer.Deserialize(reader, propertyDescriptor.Name, propertyDescriptor.Type);
            propertyDescriptor.SetValue(obj, value);
        }
    }
}
