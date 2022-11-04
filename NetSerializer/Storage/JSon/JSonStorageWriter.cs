using System;

namespace NetSerializer.V5.Storage.JSon {

    public sealed class JSonStorageWriter: StorageWriter {

        public override void WriteValue(string name, object value) {
            throw new NotImplementedException();
        }

        public override void WriteNull(string name) {
            throw new NotImplementedException();
        }

        public override void WriteArrayStart(string name, Array array) {
            throw new NotImplementedException();
        }

        public override void WriteArrayEnd() {
            throw new NotImplementedException();
        }

        public override void WriteStructStart(string name, object value) {
            throw new NotImplementedException();
        }

        public override void WriteStructEnd() {
            throw new NotImplementedException();
        }

        public override void WriteObjectStart(string name, object obj, int id) {
            throw new NotImplementedException();
        }

        public override void WriteObjectEnd() {
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
