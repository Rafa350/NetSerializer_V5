using NetSerializer.V5;
using NetSerializer.V5.Attributes;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Formatters.Xml;
using NetSerializer.V5.TypeSerializers.Serializers;
using Test.Model;

namespace Test {

    public enum Sex {
        Male,
        Female
    }

    public class Base {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Derived: Base {
        public DateTime Date { get; set; }
    }

    public struct X {
        public int A { get; set; }
        public int B { get; set; }
        [NetSerializerOptions(Exclude = true)]
        public int C { get; set; }
    }

    public class TestClass {

        private LblPoint _point;
        private Base _base = new Derived() { Date = DateTime.Now, Name = "hola", Description = "Saludo" };
        private DateTime _date = DateTime.Now;
        private string _stringValue = "abcd";
        private char _charValue = 'G';
        private X _x;
        private int[] _z = new int[] { 100, 200, 300 };
        private Sex _sex = Sex.Female;
        private object _object = null;
        private int[] _data = new int[10];
        private List<int> _ints = new List<int>() { 1, 2, 3, 4, 5, 6 };

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

        public int this[int index] {
            get => _data[index];
            set => _data[index] = value;
        }

        public DateTime Date {
            get => _date;
            set => _date = value;
        }

        public List<int> Ints {
            get => _ints;
            set => _ints = value;
        }

        public X X {
            get => _x;
            set => _x = value;
        }

        public int[] Z {
            get => _z;
            set => _z = value;
        }

        public Sex Sex {
            get => _sex;
            set => _sex = value;
        }

        public object O {
            get => _object;
            set => _object = value;
        }

        public Base Base {
            get => _base;
            set => _base = value;
        }
    }

    public class DerivedSerializer: CustomClassSerializer {

        public override bool CanProcess(Type type) =>
            type == typeof(Derived);

        protected override void SerializeObject(SerializationContext context, object obj) {

            base.SerializeObject(context, obj);
        }

        protected override void DeserializeObject(DeserializationContext context, object obj) {

            base.DeserializeObject(context, obj);
        }

        protected override void DeserializeProperty(DeserializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            base.DeserializeProperty(context, obj, propertyDescriptor);

            // Es salta la seguent propietat despres de 'Date'
            //
            if (propertyDescriptor.Name == "Date")
                context.Reader.Skip("ToSkip");
        }
    }

    class Program {

        static void Main(string[] args) {

            var x = new TestClass();

            //XmlSerialize(@"c:\temp\ns_output.xml", x);
            var y = XmlDeserialize(@"c:\temp\ns_output.xml");
        }

        private static void XmlSerialize(string fileName, object obj) {

            var serializer = new Serializer();

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (var writer = new XmlFormatWriter(stream)) {
                    serializer.Serialize(writer, obj);
                }
            }
        }

        private static object XmlDeserialize(string fileName) {

            var serializer = new Serializer();

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                using (var reader = new XmlFormatReader(stream)) {
                    return serializer.Deserialize(reader, typeof(TestClass), "root");
                }
            }
        }
    }
}