using System;
using System.Text;

namespace NetSerializer.V5.TypeSerializers.Serializers.Infrastructure {

    internal sealed class MultidimensionalIndex {

        private readonly int _dimensions;
        private readonly int[] _bounds;
        private readonly int[] _index;

        public MultidimensionalIndex(Array array) {

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            _dimensions = array.Rank;

            _bounds = new int[_dimensions];
            for (int i = 0; i < _dimensions; i++)
                _bounds[i] = array.GetUpperBound(i);

            _index = new int[_dimensions];
            for (int i = 0; i < _dimensions; i++)
                _index[i] = 0;
        }

        public override string ToString() {

            var sb = new StringBuilder();

            bool first = true;
            for (int i = 0; i < _dimensions; i++) {
                if (first)
                    first = false;
                else
                    sb.Append(',');
                sb.Append(_index[i]);
            }

            return sb.ToString();
        }

        public bool Next() {

            for (int i = _dimensions - 1; i >= 0; i--) {

                _index[i] += 1;
                if (_index[i] <= _bounds[i])
                    return true;
                else
                    _index[i] = 0;
            }

            return false;
        }

        public int[] Current =>
            _index;
    }
}
