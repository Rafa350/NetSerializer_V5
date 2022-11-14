using NetSerializer.V5;
using NetSerializer.V5.Attributes;
using NetSerializer.V5.Formatters.Xml;
using Test.Model;

namespace Test {

    public enum Sex {
        Male,
        Female
    }

    public struct X {
        public int A { get; set; }
        public int B { get; set; }
        [NetSerializerOptions(Exclude = true)]
        public int C { get; set; }
    }

    public class TestClass {

        private LblPoint _point;
        private DateTime _date = DateTime.Now;
        private string _stringValue = "abcd";
        private char _charValue = 'G';
        private X _x;
        private Sex _sex = Sex.Female;
        private object _object = null;

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

        public DateTime Date {
            get => _date;
            set => _date = value;
        }

        public X X {
            get => _x;
            set => _x = value;
        }

        public Sex Sex {
            get => _sex;
            set => _sex = value;
        }

        public object O {
            get => _object;
            set => _object = value;
        }
    }

    class Program {

        static void Main(string[] args) {

            var x = new TestClass();

            XmlSerialize(@"c:\temp\ns_output.xml", x);
            var y = XmlDeserialize(@"c:\temp\ns_output.xml");
        }

        private static void XmlSerialize(string fileName, object obj) {

            var serializer = new Serializer();

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (var writer = new XmlFormatWriter(stream, null)) {
                    serializer.Serialize(writer, obj);
                }
            }
        }

        private static object XmlDeserialize(string fileName) {

            var serializer = new Serializer();

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                using (var reader = new XmlFormatReader(stream, null)) {
                    return serializer.Deserialize(reader, typeof(TestClass), "root");
                }
            }
        }
    }
}