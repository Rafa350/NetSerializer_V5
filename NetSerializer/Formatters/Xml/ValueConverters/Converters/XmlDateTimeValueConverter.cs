using System;
using System.Xml;

namespace NetSerializer.V5.Formatters.Xml.ValueConverters.Converters {

    internal class XmlDateTimeValueConverter: IXmlValueConverter {

        public bool CanConvert(Type type) =>
            type == typeof(DateTime);

        public object ConvertFromString(string str, Type type) {

            return DateTime.Parse(str);
        }

        public string ConvertToString(object obj) {

            var dateTime = (DateTime)obj;
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
        }
    }
}