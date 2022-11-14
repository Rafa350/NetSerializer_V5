using System;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    /// <summary>
    /// Serializa i deserializa tipos primitivos, enumeradores, DateTime y TimeStamp.
    /// </summary>
    /// 
    public class ValueSerializer: TypeSerializer {

        /// <inheritdoc/>
        /// 
        public override bool CanProcess(Type type) {

            return
                type.IsPrimitive ||
                type.IsEnum ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(TimeSpan)) ||
                (type == typeof(Guid)) ||
                (type == typeof(decimal));
        }

        /// <inheritdoc/>
        /// 
        public override void Serialize(SerializationContext context, string name, Type type, object obj) {

            if (!CanProcess(type))
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));

            var writer = context.Writer;
            if (obj == null)
                writer.WriteNull(name);
            else 
                writer.WriteValue(name, obj);
        }

        /// <inheritdoc/>
        /// 
        public override void Deserialize(DeserializationContext context, string name, Type type, out object obj) {

            if (!CanProcess(type))
                throw new InvalidOperationException(
                    String.Format("No es posible deserializar el tipo '{0}'.", type.ToString()));

            var reader = context.Reader;
            obj = reader.ReadValue(name, type);
        }
    }
}
