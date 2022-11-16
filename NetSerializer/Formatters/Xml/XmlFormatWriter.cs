using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Formatters.Xml.Infrastructure;
using NetSerializer.V5.Formatters.Xml.ValueConverters;

namespace NetSerializer.V5.Formatters.Xml {

    /// <summary>
    /// Escriptor de dades en format XML.
    /// </summary>
    /// 
    public sealed class XmlFormatWriter: FormatWriter {

        private const int _serializerVersion = 500;
        private readonly XmlFormatWriterSettings _settings;
        private readonly Stream _stream;
        private XmlWriter _writer;
        private bool _closed = false;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">El stream d'escriptura.</param>
        /// <param name="settings">Parametres de configuracio.</param>
        /// 
        public XmlFormatWriter(Stream stream, XmlFormatWriterSettings settings = null) {

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _settings = settings ?? new XmlFormatWriterSettings();

            if (!stream.CanWrite)
                throw new InvalidOperationException("Es stream especificado no es de escritura.");
        }

        /// <summary>
        /// Inicialitza l'operacio d'escriptura.
        /// </summary>
        /// <param name="version">Numero de versio de les dades.</param>
        /// 
        public override void Initialize(int version) {

            if (version < 0)
                throw new ArgumentOutOfRangeException(nameof(version));

            var writerSettings = new XmlWriterSettings {
                Encoding = _settings.Encoding,
                Indent = _settings.Indentation > 0,
                IndentChars = new String(' ', _settings.Indentation),
                CheckCharacters = true,
                CloseOutput = false
            };
            _writer = XmlWriter.Create(_stream, writerSettings);

            _writer.WriteStartDocument();
            _writer.WriteStartElement("document");
            _writer.WriteAttribute("version", _serializerVersion);
            _writer.WriteAttribute("encodeStrings", _settings.EncodedStrings);
            _writer.WriteAttribute("useNames", _settings.UseNames);
            _writer.WriteAttribute("compactMode", _settings.CompactMode);
            _writer.WriteAttribute("useMeta", _settings.UseMeta);
            _writer.WriteStartElement("data");
            _writer.WriteAttribute("version", version);
        }

        /// <summary>
        /// Finalitza l'operacio d'escriptura.
        /// </summary>
        /// 
        public override void Close() {

            if (!_closed) {
                _writer.WriteEndElement();
                _writer.WriteEndElement();
                _writer.WriteEndDocument();
                _writer.Close();
                _closed = true;
            }
        }

        /// <inheritdoc/>
        /// 
        public override bool CanWriteValue(Type type) {

            if (XmlValueConverterProvider.Instance.GetConverter(type) != null)
                return true;

            var typeDescriptor = TypeDescriptorProvider.Instance.GetDescriptor(type);
            if (typeDescriptor.CustomConverter != null)
                return true;

            return false;
        }

        /// <inheritdoc/>
        /// 
        public override void WriteValue(string name, object value) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement(_settings.CompactMode ? "v" : "value");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);

            _writer.WriteValue(ConvertToString(value));

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteNull(string name) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement(_settings.CompactMode ? "n" : "null");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectReference(string name, int id) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement(_settings.CompactMode ? "r" : "reference");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
            _writer.WriteAttribute("id", id);
            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectHeader(string name, object obj, int id) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            _writer.WriteStartElement(_settings.CompactMode ? "o" : "object");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);

            var type = obj.GetType();
            var typeName = GetTypeName(type);

            _writer.WriteAttribute("type", typeName);
            _writer.WriteAttribute("id", id);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectTail() {

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructHeader(string name, object value) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type type = value.GetType();

            if (!type.IsValueType || type.IsPrimitive || type.IsEnum)
                throw new InvalidOperationException(
                    String.Format("No se puede escribir el tipo '{0}'.", type));

            _writer.WriteStartElement(_settings.CompactMode ? "s" : "struct");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructTail() {

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteArrayHeader(string name, Array array) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement(_settings.CompactMode ? "a" : "array");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);

            int[] bound = new int[array.Rank];
            for (int i = 0; i < bound.Length; i++)
                bound[i] = array.GetUpperBound(i) + 1;
            int count = array.Length;

            var sb = new StringBuilder();
            bool first = true;
            foreach (int x in bound) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(x);
            }

            _writer.WriteAttribute("bound", sb.ToString());
            _writer.WriteAttribute("count", count);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteArrayTail() {

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Obte el nom del tipus.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>El nom.</returns>
        /// 
        private static string GetTypeName(Type type) {

            return String.Format("{0}, {1}", type, type.Assembly.GetName().Name);
        }

        /// <summary>
        /// Converteix objecte a text.
        /// </summary>
        /// <param name="obj">L'objecte a convertir.</param>
        /// <returns>El resultat.</returns>
        /// <exception cref="InvalidOperationException">Es imposible realñitzar la converssio.</exception>
        /// 
        private static string ConvertToString(object obj) {

            Type type = obj.GetType();

            // Intenta primer amb els conversors propis.
            //
            var valueConverter = XmlValueConverterProvider.Instance.GetConverter(type);
            if (valueConverter != null)
                return valueConverter.ConvertToString(obj);

            // Si no pot, ho intenta amb els conversors del sistema
            //
            else {
                var typeDescriptor = TypeDescriptorProvider.Instance.GetDescriptor(type);
                var typeConverter = typeDescriptor.CustomConverter;
                if (typeConverter == null)
                    typeConverter = typeDescriptor.DefaultConverter;

                if ((typeConverter != null) && typeConverter.CanConvertTo(typeof(string)))
                    return typeConverter.ConvertToString(null, CultureInfo.InvariantCulture, obj);

                // Si tampoc pot, genera una excepcio.
                //
                else
                    throw new InvalidOperationException(
                        String.Format("No se puede convertir el valor '{0} de tipo '{1}' a string.", obj, type.FullName));
            }
        }
    }
}
