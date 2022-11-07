using System.Globalization;

namespace NetSerializer.V5.Storage.Xml {

    public sealed class XmlStorageReaderSettings {

        public CultureInfo Culture;
        public bool CheckNames;
        public bool CheckTypes;
        public bool UseSchemaValidation;
        public XmlReaderPreprocessor Preprocesor;

        public XmlStorageReaderSettings() {

            Culture = CultureInfo.InvariantCulture;
            CheckNames = true;
            CheckTypes = false;
            UseSchemaValidation = true;
            Preprocesor = null;
        }
    }
}
