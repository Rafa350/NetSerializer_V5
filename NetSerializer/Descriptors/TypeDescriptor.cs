using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetSerializer.V5.Descriptors {

    /// <summary>
    /// Desriptor d'un tipus.
    /// </summary>
    /// 
    public sealed class TypeDescriptor {

        private readonly List<PropertyDescriptor> _propertyDescriptors = new List<PropertyDescriptor>();

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// 
        public TypeDescriptor(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Obte les propietats serializables
            //
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {

                // Si es una propietat indexada la descarta
                //
                if (propertyInfo.GetIndexParameters().Length > 0)
                    continue;

                _propertyDescriptors.Add(new PropertyDescriptor(propertyInfo));
            }

            // Ordena els descriptors per orde alfabetic del nom
            //
            _propertyDescriptors.Sort((a, b) => String.Compare(a.Name, b.Name));
        }

        /// <summary>
        /// Enumera els descriptors de les propietats serialitzables
        /// </summary>
        /// 
        public IEnumerable<PropertyDescriptor> PropertyDescriptors =>
            _propertyDescriptors;
    }
}

