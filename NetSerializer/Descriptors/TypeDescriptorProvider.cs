using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetSerializer.V5.Descriptors {

    public sealed class TypeDescriptorProvider {

        private readonly Dictionary<Type, TypeDescriptor> _cache = new Dictionary<Type, TypeDescriptor>();
        private static TypeDescriptorProvider _instance;

        /// <summary>
        /// Constructor de la clase. Es privat per gestionar la creacio
        /// en forma singleton.
        /// </summary>
        /// 
        private TypeDescriptorProvider() {

        }

        /// <summary>
        /// Obte el descriptor d'una clase.
        /// </summary>
        /// <param name="type">El tipus de la clase.</param>
        /// <returns></returns>
        /// 
        public TypeDescriptor GetDescriptor(Type type) {

            Debug.Assert(type != null);

            if (!_cache.TryGetValue(type, out TypeDescriptor typeDescriptor)) {
                typeDescriptor = new TypeDescriptor(type);
                _cache.Add(type, typeDescriptor);
            }

            return typeDescriptor;
        }

        /// <summary>
        /// Obte una instancia unica a la clase.
        /// </summary>
        /// 
        public static TypeDescriptorProvider Instance {
            get {
                if (_instance == null)
                    _instance = new TypeDescriptorProvider();
                return _instance;
            }
        }
    }
}

