using System;
using System.IO;
using System.Xml.Serialization;

namespace SkladMe.Infrastructure
{
    public static class Serialization
    {
        public static void SerializeToXml(string path, object saveObject)
        {
            var formatter = new XmlSerializer(saveObject.GetType());
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, saveObject);
            }
        }

        public static object DeserializeFromXml(string path, Type type)
        {
            var formatter = new XmlSerializer(type);
            object newObject;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                newObject = formatter.Deserialize(fs);
            }
            return newObject;
        }
    }
}
