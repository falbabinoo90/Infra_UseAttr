using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Utilities
{
    public static class UtilSerialize
    {
        // Utilise XmlSerializer :
        // comportement modifiable par les attributs : [XmlIgnoreAttribute], [XmlAttribute], [XmlElement("SomeStringElementName")]
        public static string XmlSerializeToString(this object objectInstance)
        {
            XmlSerializer serializer = new XmlSerializer(objectInstance.GetType());
            StringBuilder sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(string SerializedObject, Type type)
        {
            object ResultObject;

            XmlSerializer xmlSerializer = new XmlSerializer(type);

            using (TextReader reader = new StringReader(SerializedObject))
            {
                ResultObject = xmlSerializer.Deserialize(reader);
            }

            return ResultObject;
        }
    }
}
