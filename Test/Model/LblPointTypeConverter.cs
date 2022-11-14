using System.ComponentModel;
using System.Globalization;

namespace Test.Model {

    public sealed class LblPointTypeConverter: TypeConverter {


        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {

            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {

            return destinationType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {

            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            string str = (string)value;
            string separator = culture.TextInfo.ListSeparator;

            string[] s = str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return new LblPoint(
               double.Parse(s[0], culture),
               double.Parse(s[1], culture));
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {

            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            if (value == null)
                return null;

            string separator = culture.TextInfo.ListSeparator;
            if (!separator.EndsWith(' '))
                separator += ' ';

            LblPoint point = (LblPoint)value;
            return string.Format(
                culture,
                "{1}{0}{2}",
                separator,
                point.X,
                point.Y);
        }
    }
}