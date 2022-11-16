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

        public DeserializationContext Read(string name, out bool value) {

            var typeSerializer = GetTypeSerializer(typeof(bool));
            typeSerializer.Deserialize(this, name, typeof(bool), out object v); 
            value = (bool) v;

            return this;
        }

        public DeserializationContext Read(string name, out byte value) {

            var typeSerializer = GetTypeSerializer(typeof(byte));
            typeSerializer.Deserialize(this, name, typeof(byte), out object v);
            value = (byte)v;

            return this;
        }

        public DeserializationContext Read(string name, out sbyte value) {

            var typeSerializer = GetTypeSerializer(typeof(sbyte));
            typeSerializer.Deserialize(this, name, typeof(sbyte), out object v);
            value = (sbyte)v;

            return this;
        }

        public DeserializationContext Read(string name, out short value) {

            var typeSerializer = GetTypeSerializer(typeof(short));
            typeSerializer.Deserialize(this, name, typeof(short), out object v);
            value = (short)v;

            return this;
        }

        public DeserializationContext Read(string name, out ushort value) {

            var typeSerializer = GetTypeSerializer(typeof(ushort));
            typeSerializer.Deserialize(this, name, typeof(ushort), out object v);
            value = (ushort)v;

            return this;
        }

        public DeserializationContext Read(string name, out int value) {

            var typeSerializer = GetTypeSerializer(typeof(int));
            typeSerializer.Deserialize(this, name, typeof(int), out object v);
            value = (int)v;

            return this;
        }

        public DeserializationContext Read(string name, out uint value) {

            var typeSerializer = GetTypeSerializer(typeof(uint));
            typeSerializer.Deserialize(this, name, typeof(uint), out object v);
            value = (uint)v;

            return this;
        }

        public DeserializationContext Read(string name, out long value) {

            var typeSerializer = GetTypeSerializer(typeof(long));
            typeSerializer.Deserialize(this, name, typeof(long), out object v);
            value = (long)v;

            return this;
        }

        public DeserializationContext Read(string name, out ulong value) {

            var typeSerializer = GetTypeSerializer(typeof(ulong));
            typeSerializer.Deserialize(this, name, typeof(ulong), out object v);
            value = (ulong)v;

            return this;
        }

        public DeserializationContext Read(string name, out object value, Type type) {

            var typeSerializer = GetTypeSerializer(type);
            typeSerializer.Deserialize(this, name, type, out value);

            return this;
        }

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
            _formatter.Version;
    }
}
