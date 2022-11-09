using System;
using System.IO;
using System.Text;
using NetSerializer.V5.Storage.Bin.Infrastructure;

namespace NetSerializer.V5.Storage.Bin {

    public sealed class BinStorageWriterSettings {
    }

    /// <summary>
    /// Escriptor de dades en format binari.
    /// </summary>
    public sealed class BinStorageWriter: StorageWriter {

        private const int _serializerVersion = 400;

        private readonly BinStorageWriterSettings _settings;
        private readonly Stream _stream;

        private BinaryWriter _writer;

        public BinStorageWriter(Stream stream, BinStorageWriterSettings settings) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (settings == null)
                settings = new BinStorageWriterSettings();

            _settings = settings;
            _stream = stream;
        }

        public override void WriteValueStart(string name, Type type) {
            throw new NotImplementedException();
        }

        public override void WriteValueEnd() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteValue(object value) {

            var type = value.GetType();

            if (type.IsPrimitive) {
                if (type == typeof(Char)) {
                    _writer.Write((byte)DataPrefix.CharId);
                    _writer.Write((Char)value);
                }

                else if (type == typeof(Boolean)) {
                    _writer.Write((byte)DataPrefix.BooleanId);
                    _writer.Write((Boolean)value);
                }

                else if (type == typeof(Byte)) {
                    _writer.Write((byte)DataPrefix.ByteId);
                    _writer.Write((Byte)value);
                }

                else if (type == typeof(SByte)) {
                    _writer.Write((byte)DataPrefix.SByteId);
                    _writer.Write((SByte)value);
                }

                else if (type == typeof(Int16)) {
                    _writer.Write((byte)DataPrefix.Int16Id);
                    _writer.Write((Int16)value);
                }

                else if (type == typeof(UInt16)) {
                    _writer.Write((byte)DataPrefix.UInt16Id);
                    _writer.Write((UInt16)value);
                }

                else if (type == typeof(Int32)) {
                    _writer.Write((byte)DataPrefix.Int32Id);
                    _writer.Write((Int32)value);
                }

                else if (type == typeof(UInt32)) {
                    _writer.Write((byte)DataPrefix.UInt32Id);
                    _writer.Write((UInt32)value);
                }

                else if (type == typeof(Int64)) {
                    _writer.Write((byte)DataPrefix.Int64Id);
                    _writer.Write((Int64)value);
                }

                else if (type == typeof(UInt64)) {
                    _writer.Write((byte)DataPrefix.UInt64Id);
                    _writer.Write((UInt64)value);
                }

                else if (type == typeof(Single)) {
                    _writer.Write((byte)DataPrefix.SingleId);
                    _writer.Write((Single)value);
                }

                else if (type == typeof(Double)) {
                    _writer.Write((byte)DataPrefix.DoubleId);
                    _writer.Write((Double)value);
                }

                else if (type == typeof(Decimal)) {
                    _writer.Write((byte)DataPrefix.DecimalId);
                    _writer.Write((Decimal)value);
                }

                else
                    throw new InvalidOperationException(
                        String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));
            }

            else if (type.IsEnum) {
                _writer.Write((byte)DataPrefix.EnumId);
                _writer.Write(value.ToString());
            }

            else if (type == typeof(String)) {
                if (value == null)
                    _writer.Write((byte)DataPrefix.StringNullId);
                else {
                    _writer.Write((byte)DataPrefix.StringId);
                    _writer.Write((String)value);
                }
            }

            else if (type == typeof(DateTime)) {
                _writer.Write((byte)DataPrefix.DateTimeId);
                _writer.Write(((DateTime)value).ToBinary());
            }

            else if (type == typeof(TimeSpan)) {
                _writer.Write((byte)DataPrefix.TimeSpanId);
                _writer.Write(((TimeSpan)value).Ticks);
            }

            else
                throw new InvalidOperationException(
                    String.Format("No es posible serializar el tipo '{0}'.", type.ToString()));
        }

        /// <inheritdoc/>
        /// 
        public override void WriteNull(string name) {

            _writer.Write((byte)DataPrefix.NullId);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteObjectReference(string name, int id) {

            _writer.Write((byte)DataPrefix.ReferenceId);
            _writer.Write(id);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteArrayStart(string name, Array array) {

            int[] bound = new int[array.Rank];
            for (int i = 0; i < bound.Length; i++)
                bound[i] = array.GetUpperBound(i) + 1;
            int count = array.Length;

            _writer.Write((byte)DataPrefix.StartArrayId);
            _writer.Write(bound.Length);
            foreach (int b in bound)
                _writer.Write(b);
            _writer.Write(count);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteArrayEnd() {

            _writer.Write((byte)DataPrefix.EndArrayId);
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructStart(string name, object value) {

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// 
        public override void WriteStructEnd() {

            throw new NotImplementedException();
        }

        public override void WriteObjectStart(string name, object obj, int id) {

            _writer.Write((byte)DataPrefix.StartObjectId);
            _writer.Write(obj.GetType().ToString());
            _writer.Write(id);
        }

        public override void WriteObjectEnd() {

            _writer.Write((byte)DataPrefix.EndObjectId);
        }

        public override void Initialize(int version) {

            _writer = new BinaryWriter(_stream, Encoding.UTF8);
            _writer.Write(_serializerVersion);
            _writer.Write(version);
        }

        public override void Close() {

            _writer.Close();
        }
    }
}
