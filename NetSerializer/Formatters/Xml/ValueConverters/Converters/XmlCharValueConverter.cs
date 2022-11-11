using System;
using System.Xml;

namespace NetSerializer.V5.Formatters.Xml.ValueConverters.Converters {

    internal class XmlCharValueConverter: IXmlValueConverter {

        public bool CanConvert(Type type) =>
            type == typeof(char);

        public object ConvertFromString(string str, Type type) =>
            Convert.ToChar(XmlConvert.ToUInt32(str));

        public string ConvertToString(object obj) =>
            XmlConvert.ToString(Convert.ToUInt32(obj));
    }
}
