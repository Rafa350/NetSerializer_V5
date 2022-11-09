using System;
using System.Text;

namespace NetSerializer.V5.Storage.Xml.ValueConverters.Converters {

    internal class XmlStringValueConverter: IXmlValueConverter {

        public bool CanConvert(Type type) =>
            type == typeof(string);

        public object ConvertFromString(string str) {

            var bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public string ConvertToString(object obj) {

            var bytes = Encoding.UTF8.GetBytes((string)obj);
            return Convert.ToBase64String(bytes);
        }
    }
}