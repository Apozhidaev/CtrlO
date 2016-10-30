using System.Xml.Serialization;

namespace CtrlO.States
{
    public class UrlSateData
    {
        [XmlAttribute("file")]
        public string File { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }
    }
}