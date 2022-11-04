using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace NetSerializer.V5.Storage.Xml {

    public sealed class XmlReaderPreprocessor {

        private readonly XslCompiledTransform _transform;

        public XmlReaderPreprocessor(Stream template) {

            if (template == null)
                throw new ArgumentNullException(nameof(template));

            // Carrega la transformacio
            //
            _transform = new XslCompiledTransform();
            _transform.Load(XmlReader.Create(template));
        }

        public void Process(Stream input, Stream output, bool closeOutput) {

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var rdSettings = new XmlReaderSettings();
            rdSettings.IgnoreWhitespace = true;
            rdSettings.IgnoreComments = true;
            rdSettings.IgnoreProcessingInstructions = true;

            using (var reader = XmlReader.Create(input, rdSettings)) {

                var wrSettings = new XmlWriterSettings();
                wrSettings.Encoding = Encoding.UTF8;
                wrSettings.IndentChars = "    ";
                wrSettings.Indent = true;
                wrSettings.ConformanceLevel = ConformanceLevel.Document;
                wrSettings.CloseOutput = closeOutput;

                using (var writer = XmlWriter.Create(output, wrSettings)) {

                    // Realitza la transformacio
                    //
                    _transform.Transform(reader, null, writer);
                }
            }
        }
    }
}
