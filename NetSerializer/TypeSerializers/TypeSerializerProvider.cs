using System;
using System.Collections.Generic;
using System.Linq;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Gestiona els serialitzadors.
    /// </summary>
    /// 
    public sealed class TypeSerializerProvider: ITypeSerializerProvider {

        private readonly List<ITypeSerializer> _serializerInstances = new List<ITypeSerializer>();
        private readonly Dictionary<Type, ITypeSerializer> _serializerCache = new Dictionary<Type, ITypeSerializer>();

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        public TypeSerializerProvider() {

            AddSerializers();
        }

        /// <summary>
        /// Afegeix els serialitzadors que es trobin el domini de l'aplicacio.
        /// </summary>
        /// 
        private void AddSerializers() {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System.") && !a.FullName.StartsWith("Microsoft."));
            foreach (var assembly in assemblies) {

                var types = assembly.GetTypes();
                foreach (var type in types) {

                    // Afegeix si es una clase derivada de 'CustomTypeSerializer'.
                    //
                    if (type.IsClass && !type.IsAbstract && typeof(CustomClassSerializer).IsAssignableFrom(type)) { 
                        ITypeSerializer serializer = (ITypeSerializer)Activator.CreateInstance(type);
                        _serializerInstances.Add(serializer);
                    }
                }
            }

            _serializerInstances.Add(new ValueSerializer());
            _serializerInstances.Add(new ArraySerializer());
            _serializerInstances.Add(new StructSerializer());
            _serializerInstances.Add(new ListSerializer());
            _serializerInstances.Add(new ClassSerializer());  // La ultima de la llista
        }

        /// <summary>
        /// Obte el serialitzador per un tipus especificat.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <param name="throwOnError">True si llança una excepcio en cas d'error.</param>
        /// <returns>El serialitzador.</returns>
        /// <exception cref="InvalidOperationException">No s'ha trobat cap serialitzador.</exception>
        /// 
        public ITypeSerializer GetTypeSerializer(Type type, bool throwOnError = true) {

            if (!_serializerCache.TryGetValue(type, out ITypeSerializer serializer)) {

                serializer = _serializerInstances.Find(item => item.CanProcess(type));
                if (serializer != null)
                    _serializerCache.Add(type, serializer);

                else if (throwOnError)
                    throw new InvalidOperationException($"No se registró el serializador para el tipo '{type}'.");
            }

            return serializer;
        }
    }
}
