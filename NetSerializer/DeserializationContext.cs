using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Debug.Assert(formatter != null);
            Debug.Assert(typeSerializerProvider != null);

            _formatter = formatter;
            _typeSerializerProvider = typeSerializerProvider;
        }

        /// <summary>
        /// Registra un objecte.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// 
        public void Register(object obj) {

            Debug.Assert(obj != null);

            _register.Add(obj);
        }

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
        /// Deserialitza un objecte.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El objecte.</param>
        /// <param name="type">El tipus del objecte.</param>
        /// <returns>This.</returns>
        /// 
        public DeserializationContext Read(string name, out object value, Type type) {

            var typeSerializer = GetTypeSerializer(type);
            Debug.Assert(typeSerializer != null);

            typeSerializer.Deserialize(this, name, type, out value);

            return this;
        }

        /// <summary>
        /// Deserialitza un objecte.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El objecte.</param>
        /// <typeparam name="T">El tipus del objecte.</typeparam>
        /// <returns>This.</returns>
        /// 
        public DeserializationContext Read<T>(string name, out T value) {

            Read(name, out object o, typeof(T));
            value = (T)o;

            return this;
        }

        /// <summary>
        /// Deserialitza un valor 'bool'
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <returns>El valor.</returns>
        /// 
        public bool ReadBool(string name) {

            Read(name, out bool value);
            return value;
        }

        public short ReadShort(string name) {

            Read(name, out short value);
            return value;
        }

        public ushort ReadUShort(string name) {

            Read(name, out ushort value);
            return value;
        }

        public int ReadInt(string name) {

            Read(name, out int value);
            return value;
        }

        public uint ReadUInt(string name) {

            Read(name, out uint value);
            return value;
        }

        public long ReadLong(string name) {

            Read(name, out long value);
            return value;
        }

        public ulong ReadULong(string name) {

            Read(name, out ulong value);
            return value;
        }

        public float ReadFloat(string name) {

            Read(name, out float value);
            return value;
        }

        public double ReadDouble(string name) {

            Read(name, out double value);
            return value;
        }

        public decimal ReadDecimal(string name) {

            Read(name, out decimal value);
            return value;
        }

        public T ReadObject<T>(string name) {

            Read(name, out object o, typeof(T));
            return (T)o;
        }

        public T ReadEnum<T>(string name) {

            Read(name, out object o, typeof(T));
            return (T)o;
        }

        public void Skip(string name) {

            _formatter.Skip(name);
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
