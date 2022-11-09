using System.Globalization;
using NetSerializer.V5.Storage.Xml.ValueConverters;

namespace Test.Types {

    public sealed class LblPointConverter: IXmlValueConverter {

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public LblPointConverter() {
        }

        public bool CanConvert(object obj) {

            return obj is LblPoint;
        }

        public object ConvertFromString(string str) {

            string separator = _culture.TextInfo.ListSeparator;

            string[] s = str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return new LblPoint(
               double.Parse(s[0], _culture),
               double.Parse(s[1], _culture));
        }

        public string ConvertToString(object obj) {

            string separator = _culture.TextInfo.ListSeparator;
            if (!separator.EndsWith(' '))
                separator += ' ';

            LblPoint point = (LblPoint)obj;
            return string.Format(
                _culture,
                "{1}{0}{2}",
                separator,
                point.X,
                point.Y);
        }
    }
}