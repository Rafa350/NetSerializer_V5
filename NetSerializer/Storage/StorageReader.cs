﻿using System;

namespace NetSerializer.V5.Storage {

    /// <summary>
    /// Clase base pels lectors de dades.
    /// </summary>
    /// 
    public abstract class StorageReader {

        /// <summary>
        /// Comprova si el tipus te un conversor valor.
        /// </summary>
        /// <param name="type">El tipus a comprovar.</param>
        /// <returns>True si en cas afirmatiu.</returns>
        /// 
        public virtual bool HasValueConverter(Type type) {

            return false;
        }

        /// <summary>
        /// Llegeix un valor simple.
        /// </summary>
        /// <param name="name">Nom del node.</param>
        /// <param name="type">Tipus del valor.</param>
        /// <returns>El valor lleigit.</returns>
        /// 
        public abstract object ReadValue(string name, Type type);

        /// <summary>
        /// Llegeix l'inici d'un objecte.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <returns>El resultat.</returns>
        /// 
        public abstract ReadObjectResult ReadObjectStart(string name);

        /// <summary>
        /// Llegeix el final d'un objecte
        /// </summary>
        /// 
        public abstract void ReadObjectEnd();

        /// <summary>
        /// Llegaix l'inici d'un struct,
        /// </summary>
        /// <param name="name">El nom del node.</param>
        /// <returns>El resultat.</returns>
        /// 
        public abstract ReadArrayResult ReadArrayStart(string name);

        /// <summary>
        /// Llegeix el final d'un array
        /// </summary>
        /// 
        public abstract void ReadArrayEnd();

        /// <summary>
        /// Llegeix l'inici d'un struct.
        /// </summary>
        /// <param name="name">El nom del node.</param>
        /// <param name="type">El tipus.</param>
        /// 
        public abstract void ReadStructStart(string name, Type type);

        /// <summary>
        /// Llegeix el final d'un struct
        /// </summary>
        /// 
        public abstract void ReadStructEnd();

        /// <summary>
        /// Inicia el proces de lectura de dades.
        /// </summary>
        /// 
        public abstract void Initialize();

        /// <summary>
        /// Tanca el objecte, i finalitza el proces de lectura de dades.
        /// </summary>
        /// 
        public abstract void Close();

        /// <summary>
        /// Obte el numero de versio de dades.
        /// </summary>
        /// 
        public abstract int Version { get; }
    }

    /// <summary>
    /// Indentifica el tipus de resultat del metode 'StoregeReader.ReadObjectStart'
    /// </summary>
    /// 
    public enum ReadObjectResultType {
        Null,
        Object,
        Reference
    }

    /// <summary>
    /// El resultat del metode 'StoregeReader.ReadObjectStart'
    /// </summary>
    /// 
    public sealed class ReadObjectResult {

        private readonly ReadObjectResultType _resultType;
        private readonly Type _objectType;
        private readonly int _objectId;

        /// <summary>
        /// El tipus de resultat.
        /// </summary>
        /// 
        public ReadObjectResultType ResultType {
            get => _resultType;
            init => _resultType = value;
        }

        /// <summary>
        /// El tipus del objecte.
        /// </summary>
        /// 
        public Type ObjectType {
            get => _objectType;
            init => _objectType= value;
        }

        /// <summary>
        /// El identificador del objecte
        /// </summary>
        /// 
        public int ObjectId {
            get => _objectId;
            init => _objectId = value;
        }
    }

    /// <summary>
    /// Identifica el tipus de resultat del metode 'StorageReader.ReadArrayStart'
    /// </summary>
    /// 
    public enum ReadArrayResultType {
        Null,
        Array
    }

    /// <summary>
    /// El resultat de metode 'StorageReader.ReadArrayStart'
    /// </summary>
    /// 
    public sealed class ReadArrayResult {

        private readonly ReadArrayResultType _resultType;
        private readonly int _count;
        private readonly int[] _bounds;

        /// <summary>
        /// Obte el tipus de resultat.
        /// </summary>
        /// 
        public ReadArrayResultType ResultType {
            get => _resultType;
            init => _resultType = value;
        }

        /// <summary>
        /// Numero d'elements del array.
        /// </summary>
        /// 
        public int Count {
            get => _count;
            init => _count = value;
        }

        /// <summary>
        /// Tamany dels index del array.
        /// </summary>
        /// 
        public int[] Bounds {
            get => _bounds;
            init => _bounds = value;
        }
    }
}
