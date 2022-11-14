using System;
using System.Collections.Generic;
using NetSerializer.V5.Formatters;
using NetSerializer.V5.TypeSerializers;

namespace NetSerializer.V5 {

    public sealed class DeserializationContext {

        private readonly ITypeSerializerProvider _typeSerializerProvider;
        private readonly FormatReader _formatter;
        private readonly List<object> _register = new List<object>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formatter">El formatejador de lecrtura.</param>
        /// <param name="typeSerializerProvider">El proveidor de serialitzadors.</param>
        /// 
        public DeserializationContext(FormatReader formatter, ITypeSerializerProvider typeSerializerProvider) { 
            
            _formatter = formatter;
            _typeSerializerProvider = typeSerializerProvider;
        }

        /// <summary>
        /// Registra un objecte.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// 
        public void Register(object obj) =>
            _register.Add(obj);

        /// <summary>
        /// Obte l'objecte amb el identificador especificat.
        /// </summary>
        /// <param name="id">El identificador.</param>
        /// <returns>L'objecte.</returns>
        /// 
        public object GetObject(int id) =>
            (id < 0) || (id >= _register.Count) ? null : _register[id];

        /// <summary>
        /// Obte un serialitzador pel tipus especificat.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>El serialitzador. Null si no el troba.</returns>
        /// 
        public ITypeSerializer GetTypeSerializer(Type type) =>
            _typeSerializerProvider.GetTypeSerializer(type);

        /// <summary>
        /// Obte el formatejador.
        /// </summary>
        /// 
        public FormatReader Reader =>
            _formatter;

        /// <summary>
        /// Obte el numero de versio.
        /// </summary>
        /// 
        public int Version => 
            Reader.Version;
    }
}
