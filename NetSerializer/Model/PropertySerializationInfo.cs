using System;

namespace NetSerializer.V5.Model {

    internal sealed class PropertySerializationInfo {

        private readonly string _name;

        public PropertySerializationInfo(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }

        public string Name =>
            _name;
    }
}
