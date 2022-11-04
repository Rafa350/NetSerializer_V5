using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using NetSerializer.V5.Storage.Xml.Infrastructure;

namespace NetSerializer.V5.Storage.Xml {

    /// <summary>
    /// Lector de dades en format XML.
    /// </summary>
    /// 
    public sealed class XmlStorageReader: StorageReader {

        private const string _schemaResourceName = "NetSerializer.V5.Storage.Xml.Schemas.DataSchema.xsd";

        private readonly ITypeNameConverter _typeNameConverter = new TypeNameConverter();
        private readonly Stream _stream;
        private readonly XmlStorageReaderSettings _settings;
        private XmlReader _reader;
        private int _serializerVersion = 0;
        private int _dataVersion = 0;
        private bool _useNames = false;
        private bool _useTypes = false;
        private bool _encodedStrings = false;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="stream">Stream d'entrada. Si es null es dispara una excepcio.</param>
        /// <param name="settings">Parametres de configuracio. Si es null, s'utilitza la configuracio per defecte.</param>
        /// 
        public XmlStorageReader(Stream stream, XmlStorageReaderSettings settings) {

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _settings = settings ?? new XmlStorageReaderSettings();
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
                var attributes = _reader.GetAttributes();
                string value;
                if (attributes.TryGetValue("version", out value))
                    _serializerVersion = Convert.ToInt32(value);
                if (attributes.TryGetValue("useNames", out value))
                    _useNames = Convert.ToBoolean(value);
                if (attributes.TryGetValue("useTypes", out value))
                    _useTypes = Convert.ToBoolean(value);
                if (attributes.TryGetValue("encodeStrings", out value))
                    _encodedStrings = Convert.ToBoolean(value);
            }

            if (_settings.CheckNames && !_useNames)
                throw new InvalidOperationException("No es posible verificar los nombres de los elementos. La aplicacion requiere 'useNames=true'.");

            // Processa la seccio <data>
            //
            _reader.Read();
            if (_reader.HasAttributes) {
                var attributes = _reader.GetAttributes();
                if (attributes.TryGetValue("version", out string value))
                    _dataVersion = Convert.ToInt32(value);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Close() {

            _reader.Close();
        }

        /// <inheritdoc/>
        /// 
        public override object ReadValue(string name, Type type) {

            _reader.Read();

            if (_reader.Name == "null")
                return null;

            else {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames) {
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un valor de nombre '{0}'.", name));
                }

                if (_useTypes && _settings.CheckTypes) {
                    string typeName = GetAttribute(attributes, "type");
                    if (type != (typeName == null ? null : TypeFromString(typeName)))
                        throw new InvalidOperationException(String.Format("Se esperaba un valor de tipo '{0}'.", type));
                }

                string content = GetContent();
                return ValueFromString(type, content);
            }
        }

        /// <inheritdoc/>
        /// 
        public override ReadObjectResult ReadObjectStart(string name) {

            _reader.Read();

            if (_reader.Name == "null")
                return ReadObjectResult.Null();

            else if (_reader.Name == "object") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames)
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un objeto de nombre '{0}'.", name));

                return ReadObjectResult.Object(
                    GetAttribute(attributes, "type", true),
                    Convert.ToInt32(GetAttribute(attributes, "id", true)));
            }

            else if (_reader.Name == "reference") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames)
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un objeto de nombre '{0}'.", name));

                return ReadObjectResult.Reference(
                    Convert.ToInt32(GetAttribute(attributes, "id", true)));
            }

            else
                throw new InvalidDataException("Se esperaba un nodo 'null', 'object' o 'reference'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadObjectEnd() {

            _reader.Read();
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructStart(string name, Type type) {

            _reader.Read();

            if (_reader.Name != "struct")
                throw new InvalidDataException("Se esperaba 'struct'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructEnd() {

            _reader.Read();
        }

        /// <inheritdoc/>
        /// 
        public override ReadArrayResult ReadArrayStart(string name) {

            _reader.Read();

            if (_reader.Name == "null")
                return ReadArrayResult.Null();

            else if (_reader.Name == "array") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames) {
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba 'name={0}',", name));
                }

                string[] boundStr = attributes["bound"].Split(new char[] { ',' });
                var bound = new int[boundStr.Length];
                for (int i = 0; i < boundStr.Length; i++)
                    bound[i] = Convert.ToInt32(boundStr[i]);

                var count = Convert.ToInt32(GetAttribute(attributes, "count"));

                return ReadArrayResult.Array(count, bound);
            }

            else
                throw new InvalidDataException("Se esperaba 'null' o 'array'.");
        }

        /// <inheritdoc/>
        /// 
        public override void ReadArrayEnd() {

            _reader.Read();
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

            if (attributes.ContainsKey(name))
                return attributes[name];
            else if (!required)
                return null;
            else
                throw new Exception(String.Format("El atributo obligatorio '{0}' no existe.", name));
        }

        /// <summary>
        /// Obte el contingut d'un node.
        /// </summary>
        /// <returns></returns>
        /// 
        private string GetContent() {

            string value = String.Empty;

            if (!_reader.IsEmptyElement) {
                _reader.Read();
                if (_reader.NodeType != XmlNodeType.EndElement) {
                    value = _reader.Value;
                    _reader.Read();
                }
            }

            return value;
        }

        private Type TypeFromString(string typeName) {

            return _typeNameConverter.ToType(typeName);
        }

        /// <summary>
        /// Converteix una cadena a objecte.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        private object ValueFromString(Type type, string value) {

            if (value == null)
                return null;

            else {
                // Tipus 'char'
                //
                if (type == typeof(char))
                    return Convert.ToChar(UInt16.Parse(value));

                // Tipus 'string'
                //
                else if (type == typeof(string)) {
                    if (_settings.EncodedStrings) {
                        byte[] bytes = Convert.FromBase64String(value);
                        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    }
                    else
                        return value;
                }

                // Altres tipus
                //
                else {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if ((converter != null) && converter.CanConvertFrom(typeof(string)))
                        return converter.ConvertFromString(null, _settings.Culture, value);
                    else
                        return Convert.ChangeType(value, type);
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override int Version =>
            _dataVersion;
    }
}
