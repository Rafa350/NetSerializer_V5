namespace NetSerializer.V5.Formatters {

    using System;

    /// <summary>
    /// Clase base pels escriptors de dades.
    /// </summary>
    /// 
    public abstract class FormatWriter: IDisposable {

        /// <summary>
        /// Implementa el patro 'Dispose'.
        /// </summary>
        /// 
        public void Dispose() {

            Close();
        }

        /// <summary>
        /// Comprova si el tipus es pot escriure com un valor simple.
        /// </summary>
        /// <param name="type">El tipus a comprovar.</param>
        /// <returns>True si en cas afirmatiu.</returns>
        /// 
        public virtual bool CanWriteValue(Type type) {

            return false;
        }

        /// <summary>
        /// Escriu un valor.
        /// </summary>
        /// <param name="name">El Nom.</param>
        /// <param name="value">El valor.</param>
        /// 
        public abstract void WriteValue(string name, object value);

        /// <summary>
        /// Escriu un valor null.
        /// </summary>
        /// <param name="name">El Nom.</param>
        /// 
        public abstract void WriteNull(string name);

        /// <summary>
        /// Inicia l'escriptura d'un array
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="array">El array.</param>
        /// 
        public abstract void WriteArrayHeader(string name, Array array);

        /// <summary>
        /// Finalirtza l'escriptura d'un array
        /// </summary>
        /// 
        public abstract void WriteArrayTail();

        /// <summary>
        /// Inicia l'escriprtura d'un struct
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El struct</param>
        /// 
        public abstract void WriteStructHeader(string name, object value);

        /// <summary>
        /// Finalitza l'escriptura d'un struct
        /// </summary>
        /// 
        public abstract void WriteStructTail();

        /// <summary>
        /// Inicia l'escriptura d'un objecte.
        /// </summary>
        /// <param name="name">El nom</param>
        /// <param name="obj">El objecte.</param>
        /// <param name="id">Identificador del objecte.</param>
        /// 
        public abstract void WriteObjectHeader(string name, object obj, int id);

        /// <summary>
        /// Finalitza l'escriptura d'un objecte.
        /// </summary>
        public abstract void WriteObjectTail();

        /// <summary>
        /// Escriu una referencia a un objecte previament escrit.
        /// </summary>
        /// <param name="name">El nom.</param>
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
