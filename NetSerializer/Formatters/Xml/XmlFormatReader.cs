using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Formatters.Xml.Infrastructure;
using NetSerializer.V5.Formatters.Xml.ValueConverters;

namespace NetSerializer.V5.Formatters.Xml {

    /// <summary>
    /// Lector de dades en format XML.
    /// </summary>
    /// 
    public sealed class XmlFormatReader: FormatReader {

        private const string _schemaResourceName = "NetSerializer.V5.Formatters.Xml.Schemas.DataSchema.xsd";

        private readonly Stream _stream;
        private readonly XmlFormatReaderSettings _settings;
        private XmlReader _reader;
        private int _serializerVersion = 0;
        private int _dataVersion = 0;
        private bool _useNames = false;
        private bool _useMeta = false;
        private bool _encodedStrings = false;
        private bool _compactMode = false;
        private bool _closed = false;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="stream">Stream d'entrada.</param>
        /// <param name="settings">Parametres de configuracio. Si es null, s'utilitza la configuracio per defecte.</param>
        /// 
        public XmlFormatReader(Stream stream, XmlFormatReaderSettings settings = null) {

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _settings = settings ?? new XmlFormatReaderSettings();

            if (!stream.CanRead)
                throw new InvalidOperationException("Es stream especificado no es de lectura.");
        }

        /// <inheritdoc/>
        /// 
        public override void Initialize() {

            var readerSettings = new XmlReaderSettings {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true,
                CheckCharacters = true,
                CloseInput = true
            };

            Stream inputStream;
            if (_settings.Preprocesor != null) {
                inputStream = new MemoryStream();
                _settings.Preprocesor.Process(_stream, inputStream, false);
                inputStream.Flush();
                inputStream.Seek(0, SeekOrigin.Begin);
            }
            else
                inputStream = _stream;

            if (_settings.UseSchemaValidation) {
                var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_schemaResourceName);
                if (resourceStream == null)
                    throw new Exception(String.Format("No se encontro el recurso '{0}'", _schemaResourceName));
                var schema = XmlSchema.Read(resourceStream, null);
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Schemas.Add(schema);
            }
            else
                readerSettings.ValidationType = ValidationType.None;

            readerSettings.MaxCharactersInDocument = 1000000;
            readerSettings.ConformanceLevel = ConformanceLevel.Document;

            _reader = XmlReader.Create(inputStream, readerSettings);

            _reader.Read();

            // Processa la seccio <document>
            //
            _reader.Read();
            if (_reader.HasAttributes) {
                var attributes = _reader.ReadAttributes();
                string value;
                if (attributes.TryGetValue("version", out value))
                    _serializerVersion = XmlConvert.ToInt32(value);
                if (attributes.TryGetValue("useNames", out value))
                    _useNames = XmlConvert.ToBoolean(value);
                if (attributes.TryGetValue("compactMode", out value))
                    _compactMode = XmlConvert.ToBoolean(value);
                if (attributes.TryGetValue("useMeta", out value))
                    _useMeta = XmlConvert.ToBoolean(value);
                if (attributes.TryGetValue("encodeStrings", out value))
                    _encodedStrings = XmlConvert.ToBoolean(value);
            }

            if (_settings.CheckNames && !_useNames)
                throw new InvalidOperationException("No es posible verificar los nombres de los elementos. La aplicacion requiere 'useNames=true'.");

            // Processa la seccio <data>
            //
            _reader.Read();
            if (_reader.HasAttributes) {
                var attributes = _reader.ReadAttributes();
                if (attributes.TryGetValue("version", out string value))
                    _dataVersion = XmlConvert.ToInt32(value);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Close() {

            if (!_closed) {
                _reader.Close();
                _closed = true;
            }
        }

        /// <inheritdoc/>
        /// 
        public override bool CanReadValue(Type type) {

            if (XmlValueConverterProvider.Instance.GetConverter(type) != null)
                return true;

            if (type.GetCustomAttribute<TypeConverterAttribute>() != null)
                return true;

            return false;
        }

        /// <inheritdoc/>
        /// 
        public override object ReadValue(string name, Type type) {

            Debug.Assert(type != null);

            _reader.Read();
            if (_reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("Se esperaba inicio de nodo.");

            var attributes = _reader.ReadAttributes();

            // Comprova el nom.
            //
            if (_useNames && _settings.CheckNames)
                if (name != GetAttribute(attributes, "name"))
                    throw new InvalidOperationException($"Se esperaba el nombre '{name}', para este nodo.");

            if (_reader.Name == (_compactMode ? "n" : "null"))
                return null;

            else if (_reader.Name == (_compactMode ? "v" : "value"))
                return ConvertFromString(_reader.ReadContent(), type);

            else
                throw new InvalidOperationException("Se esperaba un nodo 'null' o 'value'.");
        }

        /// <inheritdoc/>
        /// 
        public override ReadObjectResult ReadObjectHeader(string name) {

            _reader.Read();
            if (_reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("Se esperaba inicio de nodo.");

            var attributes = _reader.ReadAttributes();

            // Comprova el nom.
            //
            if (_useNames && _settings.CheckNames)
                if (name != GetAttribute(attributes, "name"))
                    throw new InvalidOperationException($"Se esperaba el nombre '{name}', para este nodo.");

            if (_reader.Name == (_compactMode ? "n" : "null"))
                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Null
                };

            else if (_reader.Name == (_compactMode ? "o" : "object"))
                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Object,
                    ObjectType = Type.GetType(GetAttribute(attributes, "type", true)),
                    ObjectId = XmlConvert.ToInt32(GetAttribute(attributes, "id", true))
                };

            else if (_reader.Name == (_compactMode ? "r" : "reference"))
                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Reference,
                    ObjectId = XmlConvert.ToInt32(GetAttribute(attributes, "id", true))
                };

            else
                throw new InvalidDataException("Se esperaba un nodo 'null', 'object' o 'reference'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadObjectTail() {

            _reader.Read();
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructHeader(string name, Type type) {

            Debug.Assert(type != null);

            _reader.Read();
            if (_reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("Se esperaba inicio de nodo.");

            if (_reader.Name != (_compactMode ? "s" : "struct"))
                throw new InvalidDataException("Se esperaba 'struct'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructTail() {

            _reader.Read();
        }

        /// <inheritdoc/>
        /// 
        public override ReadArrayResult ReadArrayHeader(string name) {

            _reader.Read();
            if (_reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("Se esperaba inicio de nodo.");

            var attributes = _reader.ReadAttributes();

            // Comprova el nom.
            //
            if (_useNames && _settings.CheckNames)
                if (name != GetAttribute(attributes, "name"))
                    throw new InvalidOperationException($"Se esperaba el nombre '{name}', para este nodo.");

            if (_reader.Name == (_compactMode ? "n" : "null"))
                return new ReadArrayResult() {
                    ResultType = ReadArrayResultType.Null
                };

            else if (_reader.Name == (_compactMode ? "a" : "array")) {
                string[] boundStr = attributes["bound"].Split(new char[] { ',' });
                var bound = new int[boundStr.Length];
                for (int i = 0; i < boundStr.Length; i++)
                    bound[i] = XmlConvert.ToInt32(boundStr[i]);

                var count = XmlConvert.ToInt32(GetAttribute(attributes, "count"));

                return new ReadArrayResult() {
                    ResultType = ReadArrayResultType.Array,
                    Count = count,
                    Bounds = bound
                };
            }

            else
                throw new InvalidDataException("Se esperaba 'null' o 'array'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadArrayTail() {

            _reader.Read();
        }

        /// <inheritdoc/>
        /// 
        public override void Skip(string name) {

            _reader.Read();
            if (_reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("Se esperaba inicio de nodo.");

            var attributes = _reader.ReadAttributes();

            // Comprova el nom.
            //
            if (_useNames && _settings.CheckNames)
                if (name != GetAttribute(attributes, "name"))
                    throw new InvalidOperationException(String.Format("Se esperaba un objeto de nombre '{0}'.", name));

            if (_reader.Name == (_compactMode ? "n" : "null"))
                return;

            else {
                while (_reader.NodeType != XmlNodeType.EndElement)
                    _reader.Read();
            }
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="attributes">Diccionari d'atributs.</param>
        /// <param name="name">Nom del atribut.</param>
        /// <param name="required">Indica si es obligatori i no pot ser nul.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        private static string GetAttribute(IDictionary<string, string> attributes, string name, bool required = false) {

            Debug.Assert(attributes != null);
            Debug.Assert(!String.IsNullOrEmpty(name));

            if (attributes.ContainsKey(name))
                return attributes[name];
            else if (!required)
                return null;
            else
                throw new Exception(String.Format("El atributo obligatorio '{0}' no existe.", name));
        }

        /// <summary>
        /// Converteix un text a un objecte del tipus expecificat.
        /// </summary>
        /// <param name="value">El text convertir.</param>
        /// <param name="type">El tipus del objecte.</param>
        /// <returns>El resultat.</returns>
        /// <exception cref="InvalidOperationException">Imposible realitzar la converssio.</exception>
        /// 
        private static object ConvertFromString(string value, Type type) {

            Debug.Assert(!String.IsNullOrEmpty(value));
            Debug.Assert(type != null);

            // Primer ho intenta amb els conversors propis.
            //
            var valueConverter = XmlValueConverterProvider.Instance.GetConverter(type);
            if (valueConverter != null)
                return valueConverter.ConvertFromString(value, type);

            // Si no pot, ho intenta amb els conversors del sistema.
            //
            else {
                var typeDescriptor = TypeDescriptorProvider.Instance.GetDescriptor(type);
                var typeConverter = typeDescriptor.CustomConverter;
                if (typeConverter == null)
                    typeConverter = typeDescriptor.DefaultConverter;

                if ((typeConverter != null) && typeConverter.CanConvertFrom(typeof(string)))
                    return typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, value);

                // Si tampoc pot, genera una excepcio.
                //
                else
                    throw new InvalidOperationException(
                        String.Format("No se puede convertir '{0}', al tipo '{1}'.", value, type.FullName));
            }
        }

        /// <inheritdoc/>
        /// 
        public override int Version =>
            _dataVersion;
    }
}
