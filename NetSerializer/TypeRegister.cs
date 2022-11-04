using System;
using System.Collections.Generic;
using NetSerializer.V5.Model;

namespace NetSerializer.V5 {

    public sealed class TypeRegister {

        private static TypeRegister _instance;
        private readonly Dictionary<Type, TypeSerializationInfo> _itemsByType = new Dictionary<Type, TypeSerializationInfo>();
        private readonly Dictionary<string, TypeSerializationInfo> _itemsByAlias = new Dictionary<string, TypeSerializationInfo>();

        private TypeRegister() {

            // Registra els tipus primitius
            //
            Register(typeof(Boolean), "boolean");
            Register(typeof(Byte), "byte");
            Register(typeof(SByte), "sbyte");
            Register(typeof(Int16), "int16");
            Register(typeof(Int32), "int32");
            Register(typeof(Int64), "int64");
            Register(typeof(UInt16), "uint16");
            Register(typeof(UInt32), "uint32");
            Register(typeof(UInt64), "uint64");
            Register(typeof(Single), "single");
            Register(typeof(Double), "double");
            Register(typeof(Decimal), "decimal");
            Register(typeof(Char), "char");
            Register(typeof(String), "string");
            Register(typeof(DateTime), "datetime");
            Register(typeof(TimeSpan), "timespan");
        }

        public TypeRegister Register(Type type) =>
            Register(type, type.Name);

        public TypeRegister Register(Type type, string alias) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (String.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            if (_itemsByType.ContainsKey(type))
                throw new InvalidOperationException(
                    String.Format("Ya se registro el tipo '{0}'.", type));

            if (_itemsByAlias.ContainsKey(alias))
                throw new InvalidOperationException(
                    String.Format("Ya se registro el alias '{0}'.", alias));

            var typeInfo = new TypeSerializationInfo(type, alias);
            _itemsByType.Add(type, typeInfo);
            _itemsByAlias.Add(alias, typeInfo);

            return this;
        }

        public string GetAlias(Type type) {

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_itemsByType.ContainsKey(type))
                return _itemsByType[type].Alias;
            else
                return null;
        }

        public Type GetType(string alias) {

            if (String.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            if (_itemsByAlias.ContainsKey(alias))
                return _itemsByAlias[alias].Type;
            else
                return null;
        }

        public static TypeRegister Instance {
            get {
                if (_instance == null)
                    _instance = new TypeRegister();
                return _instance;
            }
        }
    }
}
