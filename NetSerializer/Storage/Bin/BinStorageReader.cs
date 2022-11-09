using System;
using System.IO;
using System.Text;
using NetSerializer.V5.Storage.Bin.Infrastructure;

namespace NetSerializer.V5.Storage.Bin {

    public sealed class BinStorageReaderSettings {
    }

    /// <summary>
    /// Lector de dades en format binari
    /// </summary>
    public sealed class BinStorageReader: StorageReader {

        private readonly Stream _stream;
        private readonly BinStorageReaderSettings _settings;

        private BinaryReader _reader;
        private int _serializerVersion;
        private int _version;

        public BinStorageReader(Stream stream, BinStorageReaderSettings settings) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (settings == null)
                settings = new BinStorageReaderSettings();

            _stream = stream;
            _settings = settings;
        }

        /// <inheritdoc/>
        /// 
        public override object ReadValue(string name, Type type) {

            DataPrefix prefix = (DataPrefix)_reader.ReadByte();
            switch (prefix) {
                case DataPrefix.BooleanId:
                    return _reader.ReadBoolean();

                case DataPrefix.ByteId:
                    return _reader.ReadByte();

                case DataPrefix.SByteId:
                    return _reader.ReadSByte();

                case DataPrefix.CharId:
                    return _reader.ReadChar();

                case DataPrefix.Int16Id:
                    return _reader.ReadInt16();

                case DataPrefix.UInt16Id:
                    return _reader.ReadUInt16();

                case DataPrefix.Int32Id:
                    return _reader.ReadInt32();

                case DataPrefix.UInt32Id:
                    return _reader.ReadUInt32();

                case DataPrefix.Int64Id:
                    return _reader.ReadInt64();

                case DataPrefix.UInt64Id:
                    return _reader.ReadUInt64();

                case DataPrefix.SingleId:
                    return _reader.ReadSingle();

                case DataPrefix.DoubleId:
                    return _reader.ReadDouble();

                case DataPrefix.DecimalId:
                    return _reader.ReadDecimal();

                case DataPrefix.EnumId:
                    return Enum.Parse(type, _reader.ReadString());

                case DataPrefix.StringId:
                    return _reader.ReadString();

                case DataPrefix.StringNullId:
                    return null;

                case DataPrefix.DateTimeId:
                    return DateTime.FromBinary(_reader.ReadInt64());

                case DataPrefix.TimeSpanId:
                    return new TimeSpan(_reader.ReadInt64());

                case DataPrefix.NullId:
                    return null;

                default:
                    throw new InvalidDataException();
            }
        }

        /// <inheritdoc/>
        /// 
        public override ReadObjectResult ReadObjectStart(string name) {

/*            DataPrefix prefix = (DataPrefix)_reader.ReadByte();

            if (prefix == DataPrefix.StartObjectId)
                return new ReadObjectResultObject() {
                    _reader.ReadString(),
                    _reader.ReadInt32());

            else if (prefix == DataPrefix.ReferenceId)
                return ReadObjectResult.Reference(
                    _reader.ReadInt32());

            else if (prefix == DataPrefix.NullId)
                return ReadObjectResult.Null();

            else*/
                throw new InvalidDataException();
        }

        /// <inheritdoc/>
        /// 
        public override void ReadObjectEnd() {

            DataPrefix prefix = (DataPrefix)_reader.ReadByte();
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructStart(string name, Type type) {
        }

        /// <inheritdoc/>
        /// 
        public override void ReadStructEnd() {
        }

        /// <inheritdoc/>
        /// 
        public override ReadArrayResult ReadArrayStart(string name) {

            DataPrefix prefix = (DataPrefix)_reader.ReadByte();

            if (prefix == DataPrefix.StartArrayId) {

                int boundLength = _reader.ReadInt32();
                var bound = new int[boundLength];
                for (int b = 0; b < boundLength; b++)
                    bound[b] = _reader.ReadInt32();
                var count = _reader.ReadInt32();

                return new ReadArrayResult() {
                    ResultType = ReadArrayResultType.Array,
                    Count = count,
                    Bounds = bound
                };
            }

            else if (prefix == DataPrefix.NullId)
                return new ReadArrayResult() {
                    ResultType = ReadArrayResultType.Null
                };

            else
                throw new InvalidDataException();
        }

        /// <inheritdoc/>
        /// 
        public override void ReadArrayEnd() {

            DataPrefix prefix = (DataPrefix)_reader.ReadByte();
        }

        /// <inheritdoc/>
        /// 
        public override void Initialize() {

            _reader = new BinaryReader(_stream, Encoding.UTF8);
            _serializerVersion = _reader.ReadInt32();
            _version = _reader.ReadInt32();
        }

        /// <inheritdoc/>
        /// 
        public override void Close() {

            _reader.Close();
        }

        /// <inheritdoc/>
        /// 
        public override int Version =>
            _version;
    }
}
