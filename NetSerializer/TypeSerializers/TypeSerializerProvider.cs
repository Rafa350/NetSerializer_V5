using System;
using System.Collections.Generic;
using System.Reflection;
using NetSerializer.V5.Attributes;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Gestiona els serialitzadors.
    /// </summary>
    /// 
    public sealed class TypeSerializerProvider: ITypeSerializerProvider {

        private readonly List<ITypeSerializer> _serializerSet = new List<ITypeSerializer>();
        private readonly Dictionary<Type, ITypeSerializer> _serializerCache = new Dictionary<Type, ITypeSerializer>();

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        public TypeSerializerProvider() {

            AddCustomSerializers();
            AddDefaultSerializers();
        }

        /// <summary>
        /// Registra els serialitzadors per defecte..
        /// </summary>
        /// 
        private void AddDefaultSerializers() {

            // Es important l'ordre en que es registren els serialitzadors
            //
            _serializerSet.Add(new ValueSerializer());    // Sempre el primer
            _serializerSet.Add(new ArraySerializer());
            _serializerSet.Add(new StructSerializer());
            _serializerSet.Add(new ClassSerializer());    // Sempre l'ultim
        }

        /// <summary>
        /// Registra els serialitzadors del objectes marcats amb l'atribut 'NetSerializer'
        /// </summary>
        /// 
        private void AddCustomSerializers() {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    var attr = type.GetCustomAttribute<NetSerializerAttribute>();
                    if (attr != null) {
                        var serializerType = attr.SerializerType;
                        if ((serializerType != null) && typeof(ITypeSerializer).IsAssignableFrom(serializerType)) {
                            ITypeSerializer serializer = (ITypeSerializer)Activator.CreateInstance(serializerType);
                            _serializerSet.Add(serializer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registra un serialitzador de tipus.
        /// </summary>
        /// <param name="serializer">El serializador a registrar.</param>
        /// 
        public void AddSerializer(ITypeSerializer serializer) {

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (_serializerSet.Contains(serializer))
                throw new InvalidOperationException("Ya se registró este serializador.");

            // S'insereixen abans dels serialitzadors estandard
            //
            _serializerSet.Insert(0, serializer);
        }

        /// <summary>
        /// Obtiene el serializador para un tipo de objeto determinado.
        /// </summary>
        /// <param name="type">El tipo de objeto.</param>
        /// <returns>El serializador correspondiente.</returns>
        /// <exception cref="InvalidOperationException">Se produce si no hay ningun 
        /// serializaor registrado para el tipo de objeto, o si no es posible 
        /// instanciar el serializador.</exception>
        /// 
        public ITypeSerializer GetTypeSerializer(Type type) {

            if (!_serializerCache.TryGetValue(type, out ITypeSerializer serializer)) {

                serializer = _serializerSet.Find(item => item.CanProcess(type));
                if (serializer == null)
                    throw new InvalidOperationException(
                        String.Format(
                            "No se registró el serializador para el tipo '{0}'.",
                            type));

                _serializerCache.Add(type, serializer);
            }

            return serializer;
        }
    }
}
