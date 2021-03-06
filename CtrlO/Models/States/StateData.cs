﻿using System.Xml.Serialization;

namespace CtrlO.Models.States
{
    [XmlRoot("state")]
    public class StateData
    {
        [XmlElement("file")]
        public string File { get; set; }

        [XmlArray("urls")]
        [XmlArrayItem("url")]
        public UrlSateData[] Urls { get; set; }
    }
}