using System;
using System.Text;
using NetSerializer.V5.Formatters;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serialitzador d'arrays.
    /// </summary>
    /// 
    public sealed class ArraySerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public override bool CanProcess(Type type) =>
            type.IsArray;

        /// <inheritdoc/>
        /// 
        public override void Serialize(SerializationContext context, string name, Type type, object obj) {

            if (!CanProcess(type))
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));

            var writer = context.Writer;

            if (obj == null)
                writer.WriteNull(name);

            else {
                if (obj is not Array array)
                    throw new InvalidOperationException("'obj' no es 'Array'.");

                writer.WriteArrayHeader(name, array);

                var index = new MultidimensionalIndex(array);
                do {

                    var value = array.GetValue(index.Current);
                    var valueType = type.GetElementType();
                    var elementName = String.Format("{0}[{1}]", name, index);

                    var serializer = context.GetTypeSerializer(valueType);
                    serializer.Serialize(context, elementName, valueType, value);

                } while (index.Next());

                writer.WriteArrayTail();
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Deserialize(DeserializationContext context, string name, Type type, out object obj) {

            if (!CanProcess(type))
                throw new InvalidOperationException(
                    String.Format("No es posible deserializar el tipo '{0}'.", type.ToString()));

            var reader = context.Reader;

            ReadArrayResult result = reader.ReadArrayHeader(name);
            if (result.ResultType == ReadArrayResultType.Null)
                obj = null;

            else {
                var array = Array.CreateInstance(type.GetElementType(), result.Bounds);

                var index = new MultidimensionalIndex(array);
                for (int i = 0; i < result.Count; i++) {

                    var elementName = String.Format("{0}[{1}]", name, index);

                    var serializer = context.GetTypeSerializer(type.GetElementType());
                    serializer.Deserialize(context, elementName, type.GetElementType(), out object elementValue);

                    array.SetValue(elementValue, index.Current);

                    index.Next();
                }

                reader.ReadArrayTail();

                obj = array;
            }
        }

        /// <summary>
        /// Clase per la gestio dels index dels arrays
        /// </summary>
        private sealed class MultidimensionalIndex {

            private readonly int _dimensions;
            private readonly int[] _bounds;
            private readonly int[] _index;

            public MultidimensionalIndex(Array array) {

                _dimensions = array.Rank;

                _bounds = new int[_dimensions];
                for (int i = 0; i < _dimensions; i++)
                    _bounds[i] = array.GetUpperBound(i);

                _index = new int[_dimensions];
                for (int i = 0; i < _dimensions; i++)
                    _index[i] = 0;
            }

            public override string ToString() {

                var sb = new StringBuilder();

                bool first = true;
                for (int i = 0; i < _dimensions; i++) {
                    if (first)
                        first = false;
                    else
                        sb.Append(',');
                    sb.Append(_index[i]);
                }

                return sb.ToString();
            }

            public bool Next() {

                for (int i = _dimensions - 1; i >= 0; i--) {
                    _index[i] += 1;
                    if (_index[i] <= _bounds[i])
                        return true;
                    else
                        _index[i] = 0;
                }

                return false;
            }

            public int[] Current =>
                _index;
        }
    }
}
