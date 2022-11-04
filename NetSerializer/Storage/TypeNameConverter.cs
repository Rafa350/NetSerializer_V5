namespace NetSerializer.V5.Storage {

    using System;

    public sealed class TypeNameConverter: ITypeNameConverter {

        public string ToString(Type type) {

            string typeName = TypeRegister.Instance.GetAlias(type);
            if (typeName == null) {

                // Obte el nom directament del sistema
                //
                typeName = type.AssemblyQualifiedName;
                if (typeName.Contains("mscorlib"))
                    typeName = type.FullName;
            }

            return typeName;
        }

        public Type ToType(string typeName) {

            Type type = TypeRegister.Instance.GetType(typeName);
            if (type == null) {
                type = Type.GetType(typeName, false);
                if (type == null)
                    throw new Exception(String.Format("No se encontro el tipo '{0}'", typeName));
            }

            return type;
        }
    }
}
