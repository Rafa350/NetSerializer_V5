using System;
using System.Collections.Generic;
using System.Xml;

namespace NetSerializer.V5.Formatters.Xml.Infrastructure {

    internal static class XmlReaderHelper {

        public static IDictionary<string, string> ReadAttributes(this XmlReader reader) {

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            if (reader.HasAttributes) {
                while (reader.MoveToNextAttribute())
                    attributes.Add(reader.Name, reader.Value);
                reader.MoveToElement();
            }

            return attributes;
        }

        public static string ReadContent(this XmlReader reader) {

            string value = String.Empty;

            if (!reader.IsEmptyElement) {
                reader.Read();
                if (reader.NodeType != XmlNodeType.EndElement) {
                    value = reader.Value;
                    reader.Read();
                }
            }

            return value;
        }
    }
}
