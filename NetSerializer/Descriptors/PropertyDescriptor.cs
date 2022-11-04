using System;
using System.Reflection;

namespace NetSerializer.V5.Descriptors {

    /// <summary>
    /// Descriptor d'una propietat
    /// </summary>
    /// 
    public sealed class PropertyDescriptor {

        private readonly PropertyInfo _propertyInfo;
        private readonly bool _canSerialize;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="propertyInfo">Informacio de la propietat.</param>
        /// 
        public PropertyDescriptor(PropertyInfo propertyInfo) {

            _propertyInfo = propertyInfo;
            _canSerialize = propertyInfo.PropertyType.GetCustomAttribute<NonSerializedAttribute>() == null;
        }

        /// <summary>
        /// Obte el valor de la propietat
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        public object GetValue(object obj) =>
            _propertyInfo.GetValue(obj);

        /// <summary>
        /// Asigna el valor de la propietat.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void SetValue(object obj, object value) =>
            _propertyInfo.SetValue(obj, value);

        /// <summary>
        /// Obte el tipus de la propietat
        /// </summary>
        /// 
        public Type PropertyType =>
            _propertyInfo.PropertyType;

        /// <summary>
        /// Obte el nom de la propietat.
        /// </summary>
        /// 
        public string Name =>
            _propertyInfo.Name;

        /// <summary>
        /// Indica si es pot lleigir la propietat.
        /// </summary>
        /// 
        public bool CanRead =>
            _propertyInfo.CanRead && _canSerialize;

        /// <summary>
        /// Indica si es pot escriure la propietat
        /// </summary>
        /// 
        public bool CanWrite =>
            _propertyInfo.CanWrite && _canSerialize;
    }
}
