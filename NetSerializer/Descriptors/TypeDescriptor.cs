using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NetSerializer.V5.Attributes;

namespace NetSerializer.V5.Descriptors {

    /// <summary>
    /// Desriptor d'un tipus.
    /// </summary>
    /// 
    public sealed class TypeDescriptor {

        private readonly Type _type;
        private readonly TypeConverter _customConverter = null;
        private readonly TypeConverter _defaultConverter = null;
        private List<PropertyDescriptor> _propertyDescriptors = null;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// 
        public TypeDescriptor(Type type) {

            _type = type ?? throw new ArgumentNullException(nameof(type));

            // Obte el conversor de tipus 
            //
            _defaultConverter = System.ComponentModel.TypeDescriptor.GetConverter(type);

            // Obte el conversor de tipus customitzat
            //
            var typeConverterAttribute = type.GetCustomAttribute<TypeConverterAttribute>();
            if (typeConverterAttribute != null)
                _customConverter = (TypeConverter)Activator.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName));

            // Obte les propietats, i si son serialitzables, les afegeix a la llista.
            //
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {

                // Si es una propietat indexada la descarta
                //
                if (propertyInfo.GetIndexParameters().Length > 0)
                    continue;

                // Comprova les opcions
                //
                var netSerializerOptionsAttribute = propertyInfo.GetCustomAttribute<NetSerializerOptionsAttribute>();
                if (netSerializerOptionsAttribute != null) {

                    // Si es una propietat excluida, la descarta
                    //
                    if (netSerializerOptionsAttribute.Exclude)
                        continue;
                }

                // Crea la llista si cal.
                //
                if (_propertyDescriptors == null)
                    _propertyDescriptors = new List<PropertyDescriptor>();

                // Afegeix el descriptor de la propietat.
                //
                _propertyDescriptors.Add(new PropertyDescriptor(propertyInfo));
            }

            // Ordena els descriptors per orde alfabetic del nom
            //
            if (_propertyDescriptors != null)
                _propertyDescriptors.Sort((a, b) => String.Compare(a.Name, b.Name));
        }

        /// <summary>
        /// Obte el tipus que representa aquest descriptor.
        /// </summary>
        /// 
        public Type Type =>
            _type;

        /// <summary>
        /// Obte el conversor de tipus customitzat
        /// </summary>
        /// 
        public TypeConverter CustomConverter =>
            _customConverter;

        /// <summary>
        /// Obte el conversor de tipus predefinit
        /// </summary>
        /// 
        public TypeConverter DefaultConverter =>
            _defaultConverter;

        /// <summary>
        /// Indica si conte descriptors de propietats.
        /// </summary>
        /// 
        public bool HasPropertyDescriptors =>
            _propertyDescriptors != null;

        /// <summary>
        /// Enumera els descriptors de les propietats serialitzables
        /// </summary>
        /// 
        public IEnumerable<PropertyDescriptor> PropertyDescriptors =>
            _propertyDescriptors == null ? Enumerable.Empty<PropertyDescriptor>() : _propertyDescriptors;
    }
}

