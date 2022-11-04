using System;
using System.Xml;

namespace NetSerializer.V5.Storage.Xml.Infrastructure {

    internal static class XmlWriterHelper {

        public static void WriteAttribute(this XmlWriter writer, string localName, string value) {

            writer.WriteAttribute(localName, null, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, string ns, string value) {

            if (String.IsNullOrEmpty(localName))
                throw new ArgumentNullException("localName");

            if (!String.IsNullOrEmpty(value))
                writer.WriteAttributeString(localName, ns, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, int value) {

            writer.WriteAttribute(localName, null, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, string ns, int value) {

            if (String.IsNullOrEmpty(localName))
                throw new ArgumentNullException("localName");

            writer.WriteAttributeString(localName, ns, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, bool value) {

            writer.WriteAttribute(localName, null, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, string ns, bool value) {

            if (String.IsNullOrEmpty(localName))
                throw new ArgumentNullException("localName");

            writer.WriteAttributeString(localName, ns, XmlConvert.ToString(value));
        }
    }
}
