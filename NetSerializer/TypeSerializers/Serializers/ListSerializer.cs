using System;
using System.Collections;

namespace NetSerializer.V5.TypeSerializers.Serializers {

    public sealed class ListSerializer: ClassSerializer {

        public override bool CanProcess(Type type) =>
            base.CanProcess(type) && typeof(IList).IsAssignableFrom(type);

        protected override void SerializeObject(SerializationContext context, object obj) {

            var list = obj as IList;
            var itemType = list.GetType().GetGenericArguments()[0];

            context.Writer.WriteValue("$C", list.Count);

            var typeSerializer = context.GetTypeSerializer(itemType);
            for (int i = 0; i < list.Count; i++)
                typeSerializer.Serialize(context, String.Format("${0}", i), itemType, list[i]);

            base.SerializeObject(context, obj);
        }

        protected override void DeserializeObject(DeserializationContext context, object obj) {

            IList list = obj as IList;
            Type itemType = list.GetType().GetGenericArguments()[0];

            int count = (int)context.Reader.ReadValue("$C", typeof(int));

            var typeSerializer = context.GetTypeSerializer(itemType);
            for (int i = 0; i < count; i++) {
                typeSerializer.Deserialize(context, String.Format("${0}", i), itemType, out object item);
                list.Add(item);
            }

            base.DeserializeObject(context, obj);
        }
    }
}
