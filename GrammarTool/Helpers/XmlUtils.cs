using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GrammarTool.Helpers
{
    public class XmlUtils
    {
        public static string Serialize(Object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            try
            {
                using (var writer = new StringWriter())
                {
                    new XmlSerializer(o.GetType()).Serialize(writer, o);
                    return writer.ToString().Replace(" encoding=\"utf-16\"", "");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        //TODO: verification
        public static Example DeserializeExample(string path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Example));
            Example example;
            using (XmlReader reader = XmlReader.Create(path))
            {
                example = (Example)ser.Deserialize(reader);
            }

            return example;
        }

        //TODO: verification
        public static Scanner DeserializeScanner(string path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Scanner));
            Scanner scanner;
            using (XmlReader reader = XmlReader.Create(path))
            {
                scanner = (Scanner)ser.Deserialize(reader);
            }

            return scanner;
        }



        //Source: https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
