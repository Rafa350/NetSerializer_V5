using NetSerializer.V5.Formatters.Xml.Attributes;

namespace Test.Model {

    /// <summary>
    /// Clase que representa un punt.
    /// </summary>
    /// 
    [XmlValueConverter(typeof(LblPointConverter))]
    public readonly struct LblPoint: IEquatable<LblPoint> {

        private readonly double _x;
        private readonly double _y;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="x">Colordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public LblPoint(double x, double y) {

            _x = x;
            _y = y;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="point">El punt a copiar.</param>
        /// 
        public LblPoint(LblPoint point) {

            _x = point._x;
            _y = point._y;
        }

        /// <summary>
        /// Obte els components del punt
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public void Deconstruct(out double x, out double y) {

            x = _x;
            y = _y;
        }

        /// <summary>
        /// Obte un punt desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X.</param>
        /// <param name="dy">Desplaçament Y.</param>
        /// <returns>El punt resultant.</returns>
        /// 
        public LblPoint Offset(double dx, double dy) =>
            new LblPoint(_x + dx, _y + dy);

        public LblPoint Offset(LblPoint point) =>
            new LblPoint(_x + point._x, _y + point._y);

        public static LblPoint operator +(LblPoint p1, LblPoint p2) =>
            new LblPoint(p1._x + p2._x, p1._y + p2._y);

        public static LblPoint operator +(LblPoint p, double delta) =>
            new LblPoint(p._x + delta, p._y + delta);

        public static bool operator ==(LblPoint p1, LblPoint p2) =>
            p1.Equals(p2);

        public static bool operator !=(LblPoint p1, LblPoint p2) =>
            !p1.Equals(p2);

        /// <summary>
        /// Comprova si dos punt son iguals.
        /// </summary>
        /// <param name="other">L'altre punt a comparar.</param>
        /// <returns>TRrue si son iguals.</returns>
        /// 
        public bool Equals(LblPoint other) =>
            (_x, _y) == (other._x, other._y);

        /// <summary>
        /// Comprova si dos objectes son iguals.
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>TRrue si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is LblPoint other) && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(_x, _y);

        public override string ToString() =>
            String.Format("X: {0}, Y: {1}", _x, _y);

        public bool IsZero =>
            (_x == 0) && (_y == 0);

        /// <summary>
        /// Obte el valor de la coordinada X.
        /// </summary>
        /// 
        public double X =>
            _x;

        /// <summary>
        /// Obte el valor de la coordinada Y.
        /// </summary>
        /// 
        public double Y =>
            _y;
    }
}
