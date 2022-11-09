using System;
using System.Collections.Generic;
using System.Reflection;
using NetSerializer.V5.Storage.Xml.Attributes;

namespace NetSerializer.V5.Storage.Xml.ValueConverters {

    public sealed class XmlValueConverterProvider: IXmlValueConverterProvider {

        private static XmlValueConverterProvider _instance;
        private readonly Dictionary<Type, IXmlValueConverter> _converters = new Dictionary<Type, IXmlValueConverter>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        private XmlValueConverterProvider() {

            AddDefaultConverters();
            AddCustomConverters();
        }

        /// <inheritdoc/>
        /// 
        public IXmlValueConverter GetConverter(Type type) {

            if (_converters.TryGetValue(type, out var converter))
                return converter;
            else
                return null;
        }

        /// <summary>
        /// Afegeix els conversors definits pel atribut 'XmlValueConverter'
        /// </summary>
        private void AddCustomConverters() {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    var attr = type.GetCustomAttribute<XmlValueConverterAttribute>();
                    if (attr != null) {
                        var converterType = attr.ConverterType;
                        if ((converterType != null) && typeof(IXmlValueConverter).IsAssignableFrom(converterType)) {
                            var converter = (IXmlValueConverter)Activator.CreateInstance(converterType);
                            _converters.Add(type, converter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Afegeix els conversors per defecte.
        /// </summary>
        /// 
        private void AddDefaultConverters() {

            _converters.Add(typeof(char), new XmlCharValueConverter());
            _converters.Add(typeof(string), new XmlStringValueConverter());
        }

        /// <summary>
        /// Afeigeix un conversor.
        /// </summary>
        /// <param name="type">El tipus a convertir.</param>
        /// <param name="converter">El conversor.</param>
        /// 
        public void AddConverter(Type type, IXmlValueConverter converter) {

            _converters.Add(type, converter);
        }

        /// <summary>
        /// Obte una instancia unica del objecte.
        /// </summary>
        /// 
        public static IXmlValueConverterProvider Instance {
            get {
                if (_instance == null)
                    _instance = new XmlValueConverterProvider();
                return _instance;
            }
        }
    }
}
