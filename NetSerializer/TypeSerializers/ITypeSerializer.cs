using System;

namespace NetSerializer.V5.TypeSerializers {

    /// <summary>
    /// Interficie que cal que implementin tots els serialitzadors de tipus.
    /// </summary>
    public interface ITypeSerializer {

        /// <summary>
        /// Indica si es possible serialitzar o deserialitzar el tipus especificat.
        /// </summary>
        /// <param name="type">El tipus a serializar o deserialitzar.</param>
        /// <returns>True en cas afirmatiu.</returns>
        /// 
        bool CanProcess(Type type);

        /// <summary>
        /// Serializa un objete.
        /// </summary>
        /// <param name="context">El context de serialitzacio.</param>
        /// <param name="name">El nom.</param>
        /// <param name="type">El tipus del objecte a serioalitzar.</param>
        /// <param name="obj">L'objecte a serailitzar.</param>
        /// 
        void Serialize(SerializationContext context, string name, Type type, object obj);

        /// <summary>
        /// Deserializa un objete.
        /// </summary>
        /// <param name="context">El context de deserialitzacio.</param>
        /// <param name="name">El nom.</param>
        /// <param name="type">Tipus del objecte a deserialitzar.</param>
        /// <returns>L'objecte deserialitzat.</returns>
        /// 
        void Deserialize(DeserializationContext context, string name, Type type, out object obj);
    }
}
