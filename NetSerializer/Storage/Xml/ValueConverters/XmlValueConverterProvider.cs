using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetSerializer.V5.Storage.Xml.Attributes;
using NetSerializer.V5.Storage.Xml.ValueConverters.Converters;

namespace NetSerializer.V5.Storage.Xml.ValueConverters {

    public sealed class XmlValueConverterProvider: IXmlValueConverterProvider {

        private static XmlValueConverterProvider _instance;
        private readonly HashSet<IXmlValueConverter> _converterSet = new HashSet<IXmlValueConverter>();
        private readonly Dictionary<Type, IXmlValueConverter> _converterCache = new Dictionary<Type, IXmlValueConverter>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        private XmlValueConverterProvider() {

        }

        /// <inheritdoc/>
        /// 
        public IXmlValueConverter GetConverter(Type type) {

            if (!_converterCache.TryGetValue(type, out var converter)) {
                converter = _converterSet.Where(c => c.CanConvert(type)).FirstOrDefault();
                if (converter != null)
                    _converterCache.Add(type, converter);
            }

            return converter;
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
                            _converterSet.Add(converter);
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

            _converterSet.Add(new XmlCharValueConverter());
            _converterSet.Add(new XmlStringValueConverter());
            _converterSet.Add(new XmlDateTimeValueConverter());
        }

        /// <summary>
        /// Afeigeix un conversor.
        /// </summary>
        /// <param name="converter">El conversor.</param>
        /// 
        public void AddConverter(IXmlValueConverter converter) {

            _converterSet.Add(converter);
        }

        /// <summary>
        /// Obte una instancia unica del objecte.
        /// </summary>
        /// 
        public static IXmlValueConverterProvider Instance {
            get {
                if (_instance == null) {
                    _instance = new XmlValueConverterProvider();
                    _instance.AddDefaultConverters();
                    _instance.AddCustomConverters();
                }
                return _instance;
            }
        }
    }
}
