namespace NetSerializer.V5.Storage.Xml.ValueConverters {

    public interface IXmlValueConverter {

        /// <summary>
        /// Comprova si el objecte es pot convertir.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <returns>True en cas afirmatiu.</returns>
        /// 
        bool CanConvert(object obj);

        /// <summary>
        /// Converteix l'objecte a string.
        /// </summary>
        /// <param name="obj">L'objecte.</param>
        /// <returns>La string que representa l'objecte.</returns>
        /// 
        string ConvertToString(object obj);

        /// <summary>
        /// Converteix una string en el objecte.
        /// </summary>
        /// <param name="str">La estring.</param>
        /// <returns>L'objecte.</returns>
        /// 
        object ConvertFromString(string str);
    }
}
