using System;

namespace NetSerializer.V5.Formatters.JSon {

    public sealed class JSonStorageReader: FormatReader {

        public override object ReadValue(string name, Type type) {
            throw new System.NotImplementedException();
        }

        public override ReadObjectResult ReadObjectHeader(string name) {
            throw new System.NotImplementedException();
        }

        public override void ReadObjectTail() {
            throw new System.NotImplementedException();
        }

        public override void ReadStructHeader(string name, Type type) {
        }

        public override void ReadStructTail() {
        }

        public override ReadArrayResult ReadArrayHeader(string name) {
            throw new System.NotImplementedException();
        }

        public override void ReadArrayTail() {
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
