using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using NetSerializer.V5.Storage.Xml.Infrastructure;
using NetSerializer.V5.Storage.Xml.ValueConverters;

namespace NetSerializer.V5.Storage.Xml {

    /// <summary>
    /// Escriptor de dades en format XML.
    /// </summary>
    /// 
    public sealed class XmlStorageWriter: StorageWriter {

        private const int _serializerVersion = 500;
        private readonly XmlStorageWriterSettings _settings;
        private readonly Stream _stream;
        private XmlWriter _writer;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">El stream d'escriptura.</param>
        /// <param name="settings">Parametres de configuracio.</param>
        /// 
        public XmlStorageWriter(Stream stream, XmlStorageWriterSettings settings) {

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _settings = settings ?? new XmlStorageWriterSettings();
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
                CloseOutput = true
            };
            _writer = XmlWriter.Create(_stream, writerSettings);

            _writer.WriteStartDocument();
            _writer.WriteStartElement("document");
            _writer.WriteAttribute("version", _serializerVersion);
            _writer.WriteAttribute("encodeStrings", _settings.EncodedStrings);
            _writer.WriteAttribute("useNames", _settings.UseNames);
            _writer.WriteAttribute("useMeta", _settings.UseMeta);
            _writer.WriteStartElement("data");
            _writer.WriteAttribute("version", version);
        }

        /// <summary>
        /// Finalitza l'operacio d'escriptura.
        /// </summary>
        /// 
        public override void Close() {

            _writer.WriteEndElement();
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Close();
        }

        /// <inheritdoc/>
        /// 
        public override bool HasValueConverter(Type type) {

            var provider = XmlValueConverterProvider.Instance;
            return provider.GetConverter(type) != null;
        }

        /// <inheritdoc/>
        /// 
        public override void WriteValueStart(string name, Type type) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement("value");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteValueEnd() {

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteValue(object value) {

            Type type = value.GetType();
            var provider = XmlValueConverterProvider.Instance;
            var converter = provider.GetConverter(type);
            if (converter != null)
                _writer.WriteValue(converter.ConvertToString(value));
            else
                _writer.WriteValue(ValueToString(value));
        }

        /// <inheritdoc/>
        /// 
        public override void WriteNull(string name) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement("null");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectReference(string name, int id) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement("reference");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
            _writer.WriteAttribute("id", id);
            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectStart(string name, object obj, int id) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            _writer.WriteStartElement("object");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
            _writer.WriteAttribute("type", obj.GetType().AssemblyQualifiedName);
            _writer.WriteAttribute("id", id);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectEnd() {

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructStart(string name, object value) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type type = value.GetType();

            if (!type.IsValueType || type.IsPrimitive || type.IsEnum)
                throw new InvalidOperationException(
                    String.Format("No se puede escribir el tipo '{0}'.", type));

            _writer.WriteStartElement("struct");
            if (_settings.UseNames)
                _writer.WriteAttribute("name", name);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructEnd() {

            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteArrayStart(string name, Array array) {

            if (_settings.UseNames && String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _writer.WriteStartElement("array");
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
        public override void WriteArrayEnd() {

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Converteix un valor a string.
        /// </summary>
        /// <param name="value">El valor a convertir.</param>
        /// <param name="culture">Informacio cultural.</param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        private string ValueToString(object value) {

            if (value == null)
                return null;

            else {
                Type type = value.GetType();

                // Es tipus 'char'
                //
                if (type == typeof(char))
                    return Convert.ToUInt16(value).ToString();

                // Es tipus 'string'
                //
                else if (type == typeof(string)) {
                    string s = (string)value;
                    if (s.Length == 0)
                        return null;
                    else {
                        if (_settings.EncodedStrings) {
                            byte[] bytes = Encoding.UTF8.GetBytes(s);
                            return Convert.ToBase64String(bytes);
                        }
                        else
                            return s;
                    }
                }

                // Altres tipus
                //
                else {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if ((converter != null) && converter.CanConvertTo(typeof(string)))
                        return converter.ConvertToString(null, CultureInfo.InvariantCulture, value);
                    else
                        return value.ToString();
                }
            }
        }
    }
}
