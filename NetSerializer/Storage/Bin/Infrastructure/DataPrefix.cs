namespace NetSerializer.V5.Storage.Bin.Infrastructure {

    enum DataPrefix: byte {
        BooleanId,
        CharId,
        ByteId,
        SByteId,
        Int16Id,
        UInt16Id,
        Int32Id,
        UInt32Id,
        Int64Id,
        UInt64Id,
        SingleId,
        DoubleId,
        DecimalId,
        StringId,
        StringNullId,
        DateTimeId,
        TimeSpanId,
        EnumId,

        NullId = 50,
        ReferenceId = 51,
        StartObjectId = 52,
        EndObjectId = 53,
        StartArrayId = 54,
        EndArrayId = 55
    }
}
