using System;
using System.Collections.Generic;

namespace NetSerializer.V5.Model {

    internal sealed class TypeSerializationInfo {

        private Type _type;
        private string _alias;
        private List<PropertySerializationInfo> _properties;

        public TypeSerializationInfo(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(Type));

            Initialize(type, null, null);
        }

        public TypeSerializationInfo(Type type, string alias) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (String.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            Initialize(type, alias, null);
        }

        public TypeSerializationInfo(Type type, string alias, IEnumerable<PropertySerializationInfo> properties) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (String.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            Initialize(type, alias, properties);
        }

        private void Initialize(Type type, string alias, IEnumerable<PropertySerializationInfo> properties) {

            _type = type;
            _alias = alias;
            _properties = properties == null ? null : new List<PropertySerializationInfo>(properties);
        }

        public void AddProperty(PropertySerializationInfo property) {

            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (_properties == null)
                _properties = new List<PropertySerializationInfo>();
            _properties.Add(property);
        }

        public void AddProperties(IEnumerable<PropertySerializationInfo> properties) {

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            foreach (PropertySerializationInfo property in properties)
                AddProperty(property);
        }

        public Type Type =>
            _type;

        public string Alias =>
            _alias;

        public IEnumerable<PropertySerializationInfo> Properties =>
            _properties;
    }
}
