using System;

namespace NetSerializer.V5.Storage.Xml.ValueConverters {

    public interface IXmlValueConverterProvider {

        /// <summary>
        /// Obte un conversor pel tipus especificat.
        /// </summary>
        /// <param name="type">El tipus.</param>
        /// <returns>El conversor. Null si no el troba.</returns>
        /// 
        IXmlValueConverter GetConverter(Type type);
    }
}
