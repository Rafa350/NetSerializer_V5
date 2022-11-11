using NetSerializer.V5;
using NetSerializer.V5.Attributes;
using NetSerializer.V5.Formatters.Xml;
using Test.Types;

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
    }

    class Program {

        static void Main(string[] args) {

            var x = new TestClass();

            XmlSerialize(@"c:\temp\ns_output.xml", x);
            var y = XmlDeserialize(@"c:\temp\ns_output.xml");
        }

        private static void XmlSerialize(string fileName, object obj) {

            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new XmlFormatWriter(stream, null);
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
            var reader = new XmlFormatReader(stream, null);

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