using System;

namespace NetSerializer.V5.Formatters.JSon {

    public sealed class JSonStorageWriter: FormatWriter {

        public override void WriteValue(string name, object value) {
            throw new NotImplementedException();
        }

        public override void WriteNull(string name) {
            throw new NotImplementedException();
        }

        public override void WriteArrayHeader(string name, Array array) {
            throw new NotImplementedException();
        }

        public override void WriteArrayTail() {
            throw new NotImplementedException();
        }

        public override void WriteStructHeader(string name, object value) {
            throw new NotImplementedException();
        }

        public override void WriteStructTail() {
            throw new NotImplementedException();
        }

        public override void WriteObjectHeader(string name, object obj, int id) {
            throw new NotImplementedException();
        }

        public override void WriteObjectTail() {
            throw new NotImplementedException();
        }

        public override void WriteObjectReference(string name, int id) {
            throw new NotImplementedException();
        }

        public override void Initialize(int version) {
            throw new NotImplementedException();
        }

        public override void Close() {
            throw new NotImplementedException();
        }
    }
}
