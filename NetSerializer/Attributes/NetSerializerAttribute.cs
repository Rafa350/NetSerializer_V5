using System;

namespace NetSerializer.V5.Attributes {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NetSerializerAttribute: Attribute {

        private string _aliasName = null;
        private Type _serializerType = null;

        public NetSerializerAttribute() {

        }

        public NetSerializerAttribute(Type serializerType) {

            _serializerType = serializerType ?? throw new ArgumentNullException(nameof(serializerType));
        }

        public string AliasName {
            get => _aliasName;
            init => _aliasName = value;
        }

        public Type SerializerType {
            get => _serializerType;
            init => _serializerType = value;
        }
    }
}
