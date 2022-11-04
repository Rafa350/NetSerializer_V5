using System;
using System.Collections.Generic;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Storage;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serializador de clases.
    /// </summary>
    /// 
    public class ClassSerializer: TypeSerializer {

        private static readonly List<object> _objList = new List<object>();

        /// <inheritdoc/>
        /// 
        public ClassSerializer(ITypeSerializerProvider typeSerializerProvider) :
            base(typeSerializerProvider) {

        }

        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsClass;
        }

        /// <inheritdoc/>
        ///
        public override void Initialize() {

            _objList.Clear();
        }

        /// <summary>
        /// Comprova si es pot serialitzar la propietat.
        /// </summary>
        /// <param name="propertyDescriptor">Descriptor de la propietat.</param>
        /// <returns>TRue si es serializable.</returns>
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

            if (obj == null)
                writer.WriteNull(name);

            else {
                if (!type.IsAssignableFrom(obj.GetType()))
                    throw new InvalidOperationException(
                        String.Format("El objeto a serializar no hereda del tipo '{0}'.", type.ToString()));

                int id = _objList.IndexOf(obj);
                if (id == -1) {
                    _objList.Add(obj);
                    id = _objList.Count - 1;
                    writer.WriteObjectStart(name, obj, id);
                    SerializeObject(writer, obj);
                    writer.WriteObjectEnd();
                }

                else
                    writer.WriteObjectReference(name, id);
            }
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

            ReadObjectResult result = reader.ReadObjectStart(name);
            switch (result.ResultType) {

                default:
                case ReadObjectResultType.Null:
                    obj = null;
                    break;

                case ReadObjectResultType.Object: {

                        var typeNameConverter = new TypeNameConverter();
                        Type objectType = typeNameConverter.ToType(result.TypeName);

                        if (!type.IsAssignableFrom(objectType))
                            throw new InvalidOperationException(
                                String.Format("El objecto de tipo '{0}', a deserializar no hereda del tipo '{1}'.", objectType.ToString(), type.ToString()));

                        obj = CreateObject(objectType);
                        _objList.Add(obj);
                        DeserializeObject(reader, obj);
                        reader.ReadObjectEnd();
                    }
                    break;

                case ReadObjectResultType.Reference:
                    obj = _objList[result.ObjectId];
                    break;
            }
        }

        /// <summary>
        /// Crea una instancia del objecte. Necesita un constructor sense parametres.
        /// </summary>
        /// <param name="objectType">El tuipus d'objecte.</param>
        /// <returns>El objecte.</returns>
        /// 
        protected virtual object CreateObject(Type objectType) {

            return Activator.CreateInstance(objectType);
        }

        /// <summary>
        /// Serialitzacio per defecte del objecte.
        /// </summary>
        /// <param name="writer">El objecte per escriure dades.</param>
        /// <param name="obj">El objecte a serialitzar.</param>
        /// 
        protected virtual void SerializeObject(StorageWriter writer, object obj) {

            var typeDescriptor = TypeDescriptorProvider.Instance.GetDescriptor(obj);

            foreach (var propertyDescriptor in typeDescriptor.PropertyDescriptors)
                if (CanSerializeProperty(propertyDescriptor))
                    SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <summary>
        /// Serialitzacio per defecte d'una propietat. Nomes pot serialitzar les
        /// propietats amb 'getter'
        /// </summary>
        /// <param name="writer">El objecte per escriure dades.</param>
        /// <param name="obj">El objecte a serialitzar.</param>
        /// <param name="propertyDescriptor">La propietat a serialitzar</param>
        /// 
        protected virtual void SerializeProperty(StorageWriter writer, object obj, PropertyDescriptor propertyDescriptor) {

            if (propertyDescriptor.CanRead) {

                var serializer = GetSerializer(
                    propertyDescriptor.PropertyType);

                serializer.Serialize(
                    writer,
                    propertyDescriptor.Name,
                    propertyDescriptor.PropertyType,
                    propertyDescriptor.GetValue(obj));
            }
        }

        /// <summary>
        /// Deserialitzacio per defecte del objecte.
        /// </summary>
        /// <param name="reader">Objecte per la lectura de dades.</param>
        /// <param name="obj">El objecte a deserialitzar.</param>
        /// 
        protected virtual void DeserializeObject(StorageReader reader, object obj) {

            var typeDescriptor = TypeDescriptorProvider.Instance.GetDescriptor(obj);

            foreach (var propertyDescriptor in typeDescriptor.PropertyDescriptors)
                if (CanSerializeProperty(propertyDescriptor))
                    DeserializeProperty(reader, obj, propertyDescriptor);
        }

        /// <summary>
        /// Deserialitzacio per defecte d'una propietat.
        /// </summary>
        /// <param name="reader">Objecte per la lectura de dades.</param>
        /// <param name="obj">L'objecte.</param>
        /// <param name="propertyDescriptor">El descriptor de la propietat.</param>
        /// 
        protected virtual void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            if (propertyDescriptor.CanWrite) {

                var serializer = GetSerializer(
                    propertyDescriptor.PropertyType);

                serializer.Deserialize(
                    reader,
                    propertyDescriptor.Name,
                    propertyDescriptor.PropertyType,
                    out object value);

                propertyDescriptor.SetValue(obj, value);
            }
        }
    }
}
