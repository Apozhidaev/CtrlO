using System.Xml.Serialization;

namespace CtrlO.States
{
    [XmlRoot("state")]
    public class StateData
    {
        [XmlElement("auto")]
        public bool Auto { get; set; }

        [XmlElement("file")]
        public string File { get; set; }

        [XmlArray("urls")]
        [XmlArrayItem("url")]
        public UrlSateData[] Urls { get; set; }
    }
}