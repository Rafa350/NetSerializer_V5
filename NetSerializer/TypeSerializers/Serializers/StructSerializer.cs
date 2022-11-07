using System;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Storage;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    public class StructSerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public StructSerializer(ITypeSerializerProvider typeSerializerProvider) :
            base(typeSerializerProvider) {

        }

        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        /// <summary>
        /// Comprova si es pot serialitzar la propietat.
        /// </summary>
        /// <param name="propertyDescriptor">Descriptor de la propietat.</param>
        /// <returns>True si pot serialitzar.</returns>
        /// 
        public virtual bool CanSerializeProperty(PropertyDescriptor propertyDescriptor) {

            return propertyDescriptor.CanRead && propertyDescriptor.CanWrite;
        }

        /// <inheritdoc/>
        /// 
        public override void Serialize(StorageWriter writer, string name, Type type, object obj) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));

            writer.WriteStructStart(name, obj);
            SerializeStruct(writer, obj);
            writer.WriteStructEnd();
        }

        /// <inheritdoc/>
        /// 
        public override void Deserialize(StorageReader reader, string name, Type type, out object obj) {

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!CanSerialize(type))
                throw new InvalidOperationException(
                    String.Format("No es posible deserializar el tipo '{0}'.", type.ToString()));

            reader.ReadStructStart(name, type);
            obj = Activator.CreateInstance(type);
            DeserializeStruct(reader, obj);
            reader.ReadStructEnd();
        }

        /// <summary>
        /// Serialitza l'objecte.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="obj">L'objecte a serialitzar.</param>
        /// 
        protected virtual void SerializeStruct(StorageWriter writer, object obj) {

            TypeDescriptor descriptor = TypeDescriptorProvider.Instance.GetDescriptor(obj);

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
        protected virtual void SerializeProperty(StorageWriter writer, object obj, PropertyDescriptor propertyDescriptor) {

            var serializer = GetSerializer(propertyDescriptor.PropertyType);
            serializer.Serialize(writer, propertyDescriptor.Name, propertyDescriptor.PropertyType, propertyDescriptor.GetValue(obj));
        }

        /// <summary>
        /// Deserialitza l'objecte.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="obj">L'objecte a deserialitzar.</param>
        /// 
        protected virtual void DeserializeStruct(StorageReader reader, object obj) {

            TypeDescriptor descriptor = TypeDescriptorProvider.Instance.GetDescriptor(obj);

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
        protected virtual void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var serializer = GetSerializer(propertyDescriptor.PropertyType);
            serializer.Deserialize(reader, propertyDescriptor.Name, propertyDescriptor.PropertyType, out object value);
            propertyDescriptor.SetValue(obj, value);
        }
    }
}
