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
        /// Serialitza un valor de tipus 'bool'
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El valor.</param>
        /// <returns>This</returns>
        /// 
        public SerializationContext Write(string name, bool value) {

            var typeSerializer = GetTypeSerializer(typeof(bool));
            typeSerializer.Serialize(this, name, typeof(bool), value);

            return this;
        }

        public SerializationContext Write(string name, sbyte value) {

            var typeSerializer = GetTypeSerializer(typeof(sbyte));
            typeSerializer.Serialize(this, name, typeof(sbyte), value);

            return this;
        }

        public SerializationContext Write(string name, byte value) {

            var typeSerializer = GetTypeSerializer(typeof(byte));
            typeSerializer.Serialize(this, name, typeof(byte), value);

            return this;
        }

        public SerializationContext Write(string name, short value) {

            var typeSerializer = GetTypeSerializer(typeof(short));
            typeSerializer.Serialize(this, name, typeof(short), value);

            return this;
        }

        public SerializationContext Write(string name, ushort value) {

            var typeSerializer = GetTypeSerializer(typeof(ushort));
            typeSerializer.Serialize(this, name, typeof(ushort), value);

            return this;
        }

        public SerializationContext Write(string name, int value) {

            var typeSerializer = GetTypeSerializer(typeof(int));
            typeSerializer.Serialize(this, name, typeof(int), value);

            return this;
        }

        public SerializationContext Write(string name, uint value) {

            var typeSerializer = GetTypeSerializer(typeof(uint));
            typeSerializer.Serialize(this, name, typeof(uint), value);

            return this;
        }

        public SerializationContext Write(string name, long value) {

            var typeSerializer = GetTypeSerializer(typeof(long));
            typeSerializer.Serialize(this, name, typeof(long), value);

            return this;
        }

        public SerializationContext Write(string name, ulong value) {

            var typeSerializer = GetTypeSerializer(typeof(ulong));
            typeSerializer.Serialize(this, name, typeof(ulong), value);

            return this;
        }

        public SerializationContext Write(string name, float value) {

            var typeSerializer = GetTypeSerializer(typeof(float));
            typeSerializer.Serialize(this, name, typeof(float), value);

            return this;
        }

        public SerializationContext Write(string name, double value) {

            var typeSerializer = GetTypeSerializer(typeof(double));
            typeSerializer.Serialize(this, name, typeof(double), value);

            return this;
        }

        public SerializationContext Write(string name, decimal value) {

            var typeSerializer = GetTypeSerializer(typeof(decimal));
            typeSerializer.Serialize(this, name, typeof(decimal), value);

            return this;
        }

        public SerializationContext Write(string name, char value) {

            var typeSerializer = GetTypeSerializer(typeof(char));
            typeSerializer.Serialize(this, name, typeof(char), value);

            return this;
        }

        public SerializationContext Write(string name, string value) {

            var typeSerializer = GetTypeSerializer(typeof(string));
            typeSerializer.Serialize(this, name, typeof(string), value);

            return this;
        }

        public SerializationContext Write(string name, DateTime value) {

            var typeSerializer = GetTypeSerializer(typeof(DateTime));
            typeSerializer.Serialize(this, name, typeof(DateTime), value);

            return this;
        }

        public SerializationContext Write<T>(string name, T value) {

            var typeSerializer = GetTypeSerializer(typeof(T));
            typeSerializer.Serialize(this, name, typeof(T), value);

            return this;
        }

        public FormatWriter Writer =>
            _formatter;
    }
}
