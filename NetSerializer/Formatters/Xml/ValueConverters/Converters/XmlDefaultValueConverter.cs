using System;

namespace NetSerializer.V5.Formatters.Xml.ValueConverters.Converters {

    internal class XmlDefaultValueConverter: IXmlValueConverter {

        public bool CanConvert(Type type) {

            return System.ComponentModel.TypeDescriptor.GetConverter(type) != null;
        }

        public object ConvertFromString(string str, Type type) {

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
            return converter.ConvertFromString(str);
        }

        public string ConvertToString(object obj) {

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(obj);
            return converter.ConvertToString(obj);
        }
    }
}
