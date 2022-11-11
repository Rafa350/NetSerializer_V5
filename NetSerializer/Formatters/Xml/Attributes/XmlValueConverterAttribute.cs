using System;

namespace NetSerializer.V5.Formatters.Xml.Attributes {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class XmlValueConverterAttribute: Attribute {

        private Type _converterType;

        public XmlValueConverterAttribute() {
        }

        public XmlValueConverterAttribute(Type converterType) {

            _converterType = converterType;
        }

        public Type ConverterType {
            get => _converterType;
            init => _converterType = value;
        }
    }
}
