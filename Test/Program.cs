using NetSerializer.V5;
using NetSerializer.V5.Storage;
using NetSerializer.V5.Storage.Xml;

namespace LabelTest {

    class Program {

        private struct TestStruct {

            public int a { get; set; }
            public int b { get; set; }

            public int bb => b;

            public TestStruct() {
                a = 1;
                b = 2;
            }
        }

        static void Main(string[] args) {

            var obj1 = new TestStruct() { a = 10, b = 20 };

            var obj2 = new List<TestStruct>() {
                new TestStruct() { a = 10, b = 20 },
                new TestStruct() { a = 11, b = 21 },
                new TestStruct() { a = 12, b = 22 }
            };
            XmlSerialize(@"c:\temp\ns_output.xml", obj1);
        }

        private static void XmlSerialize(string fileName, object obj) {

            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            StorageWriter writer = new XmlStorageWriter(stream, null);
            Serializer s = new Serializer(writer, 100);
            try {
                s.Serialize(obj, "root");
            }
            finally {
                s.Close();
            }
        }

        private static object XmlDeserialize(string fileName) {

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StorageReader reader = new XmlStorageReader(stream, null);
            Deserializer d = new Deserializer(reader);
            try {
                return d.Deserialize(typeof(TestStruct), "root");
            }
            finally {
                d.Close();
            }
        }
    }
}