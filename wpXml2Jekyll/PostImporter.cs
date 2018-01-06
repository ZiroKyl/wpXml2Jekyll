using System;
using System.Xml;

namespace wpXml2Jekyll
{
    class PostImporter
    {
        public XmlDocument ReadWpPosts(string fileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            return xmlDocument;
        }
    }
}