using NetSerializer.V5;
using NetSerializer.V5.Storage.Xml;
using Test.Types;

namespace Test {


    public class TestClass {

        private LblPoint _point;
        private string _stringValue = "abcd";
        private char _charValue = 'G';

        public TestClass() {
        }

        public string StringValue {
            get => _stringValue;
            set => _stringValue = value;
        }

        public char CharValue {
            get => _charValue;
            set => _charValue = value;
        }

        public LblPoint Point {
            get => _point;
            set => _point = value;
        }
    }

    class Program {

        static void Main(string[] args) {

            var x = new TestClass();

            XmlSerialize(@"c:\temp\ns_output.xml", x);
            var y = XmlDeserialize(@"c:\temp\ns_output.xml");
        }

        private static void XmlSerialize(string fileName, object obj) {

            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new XmlStorageWriter(stream, null);
            var s = new Serializer(writer, 100);
            try {
                s.Serialize(obj, "root");
            }
            finally {
                s.Close();
            }
        }

        private static object XmlDeserialize(string fileName) {

            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var reader = new XmlStorageReader(stream, null);

            var d = new Deserializer(reader);
            try {
                return d.Deserialize(typeof(TestClass), "root");
            }
            finally {
                d.Close();
            }
        }
    }
}