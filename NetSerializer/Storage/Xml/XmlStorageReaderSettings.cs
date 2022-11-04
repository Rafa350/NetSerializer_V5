using System.Globalization;

namespace NetSerializer.V5.Storage.Xml {

    public sealed class XmlStorageReaderSettings {

        public CultureInfo Culture;
        public bool CheckNames;
        public bool CheckTypes;
        public bool UseSchemaValidation;
        public bool EncodedStrings;
        public XmlReaderPreprocessor Preprocesor;

        public XmlStorageReaderSettings() {

            Culture = CultureInfo.InvariantCulture;
            CheckNames = true;
            CheckTypes = false;
            EncodedStrings = true;
            UseSchemaValidation = true;
            Preprocesor = null;
        }
    }
}
