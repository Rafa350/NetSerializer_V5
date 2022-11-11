using System;

namespace NetSerializer.V5.Formatters.Xml.ValueConverters {

    public interface IXmlValueConverter {

        /// <summary>
        /// Comprova si un tipus es pot convertir.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>True en cas afirmatiu.</returns>
        /// 
        bool CanConvert(Type type);

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <returns>El text que representa l'objecte.</returns>
        /// 
        string ConvertToString(object obj);

        /// <summary>
        /// Converteix un text a un objecte.
        /// </summary>
        /// <param name="str">El text.</param>
        /// <param name="type">El tipus d'objecte.</param>
        /// <returns>L'objecte.</returns>
        /// 
        object ConvertFromString(string str, Type type);
    }
}
