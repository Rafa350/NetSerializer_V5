using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="stream">Stream d'entrada.</param>
        /// <param name="settings">Parametres de configuracio. Si es null, s'utilitza la configuracio per defecte.</param>
        /// 
        public XmlFormatReader(Stream stream, XmlFormatReaderSettings settings) {

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _settings = settings ?? new XmlFormatReaderSettings();
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
                    _serializerVersion = XmlConvert.ToInt32(value);
                if (attributes.TryGetValue("useNames", out value))
                    _useNames = XmlConvert.ToBoolean(value);
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
                var attributes = _reader.GetAttributes();
                if (attributes.TryGetValue("version", out string value))
                    _dataVersion = XmlConvert.ToInt32(value);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Close() {

            _reader.Close();
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

            _reader.Read();

            if (_reader.Name == "null")
                return null;

            else {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames) {
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un valor de nombre '{0}'.", name));
                }

                return ConvertFromString(GetContent(), type);
            }
        }

        /// <inheritdoc/>
        /// 
        public override ReadObjectResult ReadObjectStart(string name) {

            _reader.Read();

            if (_reader.Name == "null")
                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Null
                };

            else if (_reader.Name == "object") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames)
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un objeto de nombre '{0}'.", name));

                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Object,
                    ObjectType = Type.GetType(GetAttribute(attributes, "type", true)),
                    ObjectId = XmlConvert.ToInt32(GetAttribute(attributes, "id", true))
                };
            }

            else if (_reader.Name == "reference") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames)
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba un objeto de nombre '{0}'.", name));

                return new ReadObjectResult() {
                    ResultType = ReadObjectResultType.Reference,
                    ObjectId = XmlConvert.ToInt32(GetAttribute(attributes, "id", true))
                };
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
                return new ReadArrayResult() {
                    ResultType = ReadArrayResultType.Null
                };

            else if (_reader.Name == "array") {

                var attributes = _reader.GetAttributes();

                if (_useNames && _settings.CheckNames) {
                    if (name != GetAttribute(attributes, "name"))
                        throw new InvalidOperationException(String.Format("Se esperaba 'name={0}',", name));
                }

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

        /// <summary>
        /// Converteix un text a un objecte del tipus expecificat.
        /// </summary>
        /// <param name="value">El text convertir.</param>
        /// <param name="type">El tipus del objecte.</param>
        /// <returns>El resultat.</returns>
        /// <exception cref="InvalidOperationException">Imposible realitzar la converssio.</exception>
        /// 
        private static object ConvertFromString(string value, Type type) {

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
