using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml;

namespace KwwikaTweetStreamerPublisher
{
    public class XmlConfigurator : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Object settings = null;

            if (section == null) { return settings; }

            XPathNavigator navigator = section.CreateNavigator();

            String typeName = (string)navigator.Evaluate("string(@configuratorType)");

            Type sectionType = Type.GetType(typeName);

            XmlSerializer xs = new XmlSerializer(sectionType);
            XmlNodeReader reader = new XmlNodeReader(section);

            settings = xs.Deserialize(reader);

            return settings;
        }

        #endregion
    }
}
