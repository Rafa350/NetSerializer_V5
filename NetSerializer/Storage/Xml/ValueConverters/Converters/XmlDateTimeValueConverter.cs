using System;
using System.Xml;

namespace NetSerializer.V5.Storage.Xml.ValueConverters.Converters {

    internal class XmlDateTimeValueConverter: IXmlValueConverter {

        public bool CanConvert(Type type) =>
            type == typeof(DateTime);

        public object ConvertFromString(string str) {

            return DateTime.Parse(str);
        }

        public string ConvertToString(object obj) {

            var dateTime = (DateTime)obj;
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
        }
    }
}