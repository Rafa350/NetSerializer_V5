﻿using System;

namespace NetSerializer.V5.Attributes {

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NetSerializerOptionsAttribute: Attribute {

        private string _aliasName;
        private bool _exclude = false;

        public string AliasName {
            get => _aliasName;
            init => _aliasName = value;
        }

        public bool Exclude {
            get => _exclude;
            init => _exclude = value;
        }
    }
}
