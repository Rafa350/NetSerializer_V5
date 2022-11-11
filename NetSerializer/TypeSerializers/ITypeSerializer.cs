using System;
using NetSerializer.V5.Formatters;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Interface que deben implementar todos los serializadores de tipos.
    /// </summary>
    public interface ITypeSerializer {

        /// <summary>
        /// Indica si es possible serialitzar o deserialitzar un tipus determinat.
        /// </summary>
        /// <param name="type">El tipus a serializar o deserialitzar.</param>
        /// <returns>True si es posible, false en cas contrari.</returns>
        /// 
        bool CanSerialize(Type type);

        /// <summary>
        /// Inicialitzcio del serialitzador. S'ha de cridar abans d'iniciar el process de serialitzacio o
        /// de deserialitzacio.
        /// </summary>
        /// 
        void Initialize();

        /// <summary>
        /// Serializa un objeto.
        /// </summary>
        /// <param name="writer">El objeto StorageWriter, que realizara la escritura del objeto serializado.</param>
        /// <param name="name">Nombre del nodo.</param>
        /// <param name="type">Tipo del objeto a serializar.</param>
        /// <param name="obj">El objeto a serializar.</param>
        /// 
        void Serialize(FormatWriter writer, string name, Type type, object obj);

        /// <summary>
        /// Deserializa un objeto.
        /// </summary>
        /// <param name="reader">El objeto StorageReader, que realizara la lectura del objeto serializado.</param>
        /// <param name="name">Nombre del nodo.</param>
        /// <param name="type">Tipo del objeto a deserializar.</param>
        /// <returns>L'objecte deserialitzat.</returns>
        /// 
        object Deserialize(FormatReader reader, string name, Type type);
    }
}
