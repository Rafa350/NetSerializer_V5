using System;

namespace NetSerializer.V5.Formatters.JSon {

    public sealed class JSonStorageReader: FormatReader {

        public override object ReadValue(string name, Type type) {
            throw new System.NotImplementedException();
        }

        public override ReadObjectResult ReadObjectStart(string name) {
            throw new System.NotImplementedException();
        }

        public override void ReadObjectEnd() {
            throw new System.NotImplementedException();
        }

        public override void ReadStructStart(string name, Type type) {
        }

        public override void ReadStructEnd() {
        }

        public override ReadArrayResult ReadArrayStart(string name) {
            throw new System.NotImplementedException();
        }

        public override void ReadArrayEnd() {
            throw new System.NotImplementedException();
        }

        public override void Initialize() {
            throw new System.NotImplementedException();
        }

        public override void Close() {
            throw new System.NotImplementedException();
        }

        public override int Version {
            get { throw new System.NotImplementedException(); }
        }

    }
}
