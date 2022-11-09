using System.Globalization;
using System.Text;

namespace NetSerializer.V5.Storage.Xml {

    public sealed class XmlStorageWriterSettings {

        public Encoding Encoding { get; set; }
        public bool UseNames { get; set; }
        public bool UseTypes { get; set; }
        public bool UseMeta { get; set; }
        public int Indentation { get; set; }
        public bool EncodedStrings { get; set; }

        public XmlStorageWriterSettings() {

            UseNames = true;
            UseTypes = false;
            UseMeta = false;
            Encoding = Encoding.UTF8;
            Indentation = 4;
            EncodedStrings = true;
        }
    }
}
