namespace NetSerializer.V5.Formatters.Xml {

    public sealed class XmlFormatReaderSettings {

        public bool CheckNames { get; set; }
        public bool UseSchemaValidation { get; set; }
        public bool CompactMode { get; set; }
        public XmlFormatReaderPreprocessor Preprocesor { get; set; }

        public XmlFormatReaderSettings() {

            CheckNames = true;
            CompactMode = false;
            UseSchemaValidation = true;
            Preprocesor = null;
        }
    }
}
