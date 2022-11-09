using System;
using System.Xml;

namespace NetSerializer.V5.Storage.Xml.ValueConverters {

    internal class XmlCharValueConverter: IXmlValueConverter {

        public bool CanConvert(object obj) {

            return obj is char;
        }

        public object ConvertFromString(string str) {

            return Convert.ToChar(XmlConvert.ToUInt32(str));
        }

        public string ConvertToString(object obj) {

            return XmlConvert.ToString(Convert.ToUInt32(obj));
        }
    }
}
