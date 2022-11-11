using System;
using NetSerializer.V5.Formatters;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serializa i deserializa tipos primitivos, enumeradores, DateTime y TimeStamp.
    /// </summary>
    /// 
    public class ValueSerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return
                type.IsPrimitive ||
                type.IsEnum ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal));
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
                writer.WriteValueStart(name, type);
                writer.WriteValue(obj);
                writer.WriteValueEnd();
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

            return reader.ReadValue(name, type);
        }
    }
}
