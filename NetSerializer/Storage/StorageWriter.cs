namespace NetSerializer.V5.Storage {

    using System;

    /// <summary>
    /// Clase base pels escriptors de dades.
    /// </summary>
    /// 
    public abstract class StorageWriter {

        /// <summary>
        /// Comprova si el tipus te un conversor de valor.
        /// </summary>
        /// <param name="type">El tipus a comprovar.</param>
        /// <returns>True si en cas afirmatiu.</returns>
        /// 
        public virtual bool HasValueConverter(Type type) {

            return false;
        }

        /// <summary>
        /// Inicia l'escriptura d'un valor.
        /// </summary>
        /// <param name="name">El nom del valor</param>
        /// <param name="type">El tipus del valor</param>
        /// 
        public abstract void WriteValueStart(string name, Type type);

        /// <summary>
        /// Finalitza l'escriptura d'un valor.
        /// </summary>
        /// 
        public abstract void WriteValueEnd();

        /// <summary>
        /// Escriu un valor.
        /// </summary>
        /// <param name="value">El valor.</param>
        /// 
        public abstract void WriteValue(object value);

        /// <summary>
        /// Escriu un valor null.
        /// </summary>
        /// <param name="name">Nom del node.</param>
        /// 
        public abstract void WriteNull(string name);

        /// <summary>
        /// Inicia l'escriptura d'un array
        /// </summary>
        /// <param name="name">El nom del node.</param>
        /// <param name="array">El array.</param>
        /// 
        public abstract void WriteArrayStart(string name, Array array);

        /// <summary>
        /// Finalirtza l'escriptura d'un array
        /// </summary>
        /// 
        public abstract void WriteArrayEnd();

        /// <summary>
        /// Inicia l'escriprtura d'un struct
        /// </summary>
        /// <param name="name">El nom del node.</param>
        /// <param name="value">El struct</param>
        /// 
        public abstract void WriteStructStart(string name, object value);

        /// <summary>
        /// Finalitza l'escriptura d'un struct
        /// </summary>
        /// 
        public abstract void WriteStructEnd();

        /// <summary>
        /// Inicia l'escriptura d'un objecte.
        /// </summary>
        /// <param name="name">Nom del node</param>
        /// <param name="obj">El objecte.</param>
        /// <param name="id">Identificador del objecte.</param>
        /// 
        public abstract void WriteObjectStart(string name, object obj, int id);

        /// <summary>
        /// Finalitza l'escriptura d'un objecte.
        /// </summary>
        public abstract void WriteObjectEnd();

        /// <summary>
        /// Escriu una referencia a un objecte previament escrit.
        /// </summary>
        /// <param name="name">El nom del node.</param>
        /// <param name="id">Identificador del objecte.</param>
        /// 
        public abstract void WriteObjectReference(string name, int id);

        /// <summary>
        /// Inicialitza el proces d'escriptura.
        /// </summary>
        /// <param name="version">Numero de versio.</param>
        /// 
        public abstract void Initialize(int version);

        /// <summary>
        /// Tanca el objecte, i finalitza el proces escriptures.
        /// </summary>
        /// 
        public abstract void Close();
    }
}
