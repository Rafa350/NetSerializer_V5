using System.Globalization;

namespace NetSerializer.V5.Formatters.Xml {

    public sealed class XmlFormatReaderSettings {

        public CultureInfo Culture;
        public bool CheckNames;
        public bool UseSchemaValidation;
        public XmlFormatReaderPreprocessor Preprocesor;

        public XmlFormatReaderSettings() {

            Culture = CultureInfo.InvariantCulture;
            CheckNames = true;
            UseSchemaValidation = true;
            Preprocesor = null;
        }
    }
}
