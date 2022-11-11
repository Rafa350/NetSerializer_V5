using System;
using NetSerializer.V5.Formatters;
using NetSerializer.V5.TypeSerializers.Serializers.Infrastructure;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serialitzador d'arrays.
    /// </summary>
    /// 
    public sealed class ArraySerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsArray;
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
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));

            if (obj == null)
                writer.WriteNull(name);

            else {
                if (obj is not Array array)
                    throw new InvalidOperationException("'obj' no es 'Array'.");

                writer.WriteArrayStart(name, array);

                MultidimensionalIndex index = new MultidimensionalIndex(array);
                do {

                    var value = array.GetValue(index.Current);
                    var valueType = type.GetElementType();
                    var serializer = TypeSerializerProvider.Instance.GetSerializer(valueType);
                    var elementName = String.Format("{0}[{1}]", name, index);
                    serializer.Serialize(writer, elementName, valueType, value);

                } while (index.Next());

                writer.WriteArrayEnd();
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
                    String.Format("No es posible deserializar el tipo '{0}'.", type.ToString()));

            ReadArrayResult result = reader.ReadArrayStart(name);
            if (result.ResultType == ReadArrayResultType.Null)
                return null;

            else {
                Array array = Array.CreateInstance(type.GetElementType(), result.Bounds);

                var index = new MultidimensionalIndex(array);
                for (int i = 0; i < result.Count; i++) {

                    var serializer = TypeSerializerProvider.Instance.GetSerializer(type.GetElementType());
                    var elementName = String.Format("{0}[{1}]", name, index);
                    var elementValue = serializer.Deserialize(reader, elementName, type.GetElementType());
                    array.SetValue(elementValue, index.Current);

                    index.Next();
                }

                reader.ReadArrayEnd();

                return array;
            }
        }
    }
}
