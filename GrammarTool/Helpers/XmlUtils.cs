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
                    return writer.ToString();
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
    }
}
