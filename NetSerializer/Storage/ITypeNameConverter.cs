namespace NetSerializer.V5.Storage {

    using System;

    public interface ITypeNameConverter {

        string ToString(Type type);
        Type ToType(string typeName);
    }
}
