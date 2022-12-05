using System;
using System.Collections.Generic;
using NetSerializer.V5.Formatters;
using NetSerializer.V5.TypeSerializers;

namespace NetSerializer.V5 {

    public sealed class SerializationContext {

        private readonly ITypeSerializerProvider _typeSerializerProvider;
        private readonly FormatWriter _formatter;
        private readonly List<object> _register = new List<object>();

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="formatter">El formatejador.</param>
        /// <param name="typeSerializerProvider">El proveidor de serialitzadors.</param>
        /// 
        public SerializationContext(FormatWriter formatter, ITypeSerializerProvider typeSerializerProvider) {
            
            _formatter = formatter;
            _typeSerializerProvider = typeSerializerProvider;
        }

        /// <summary>
        /// Registra un objecte.
        /// </summary>
        /// <param name="obj">L'objecte a registrar.</param>
        /// <returns>El identificador del objecte.</returns>
        /// 
        public int RegisterObject(object obj) {

            _register.Add(obj);
            return _register.Count - 1;
        }

        /// <summary>
        /// Obte el identificador d'un objecte registrat.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <returns>El identificador. -1 si no ha estat registrat previament.</returns>
        /// 
        public int GetObjectId(object obj) =>
            _register.IndexOf(obj);

        /// <summary>
        /// Obte el serialitzador del tipus especificat.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>El serialitzador.</returns>
        /// 
        public ITypeSerializer GetTypeSerializer(Type type) =>
            _typeSerializerProvider.GetTypeSerializer(type);

        /// <summary>
        /// Serialitza un valor de tipus 'object'
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El valor.</param>
        /// <param name="type">El tipus del valor.</param>
        /// <returns>This</returns>
        /// 
        public SerializationContext Write(string name, object value, Type type) {

            var typeSerializer = GetTypeSerializer(type);
            typeSerializer.Serialize(this, name, type, value);

            return this;
        }

        /// <summary>
        /// Serialitza un valor de tipus generic.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El valor.</param>
        /// <typeparam name="T">El tipus del valor.</typeparam>
        /// <returns>This</returns>
        /// 
        public SerializationContext Write<T>(string name, T value) {

            return Write(name, value, typeof(T));
        }

        /// <summary>
        /// El formatejador.
        /// </summary>
        /// 
        public FormatWriter Writer =>
            _formatter;
    }
}
