using System;

namespace NetSerializer.V5.Attributes {

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NetSerializerOptionsAttribute: Attribute {

        private readonly string _aliasName;
        private readonly bool _exclude = false;

        public string AliasName {
            get => _aliasName;
            init => _aliasName = value;
        }

        public bool Exclude {
            get => _exclude;
            init => _exclude = value;
        }
    }
}
