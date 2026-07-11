using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	public static class XmlSerializableServices
	{
		[MonoTODO]
		public static void AddDefaultSchema(XmlSchemaSet schemas, XmlQualifiedName typeQName)
		{
			throw new NotImplementedException();
		}

		public static XmlNode[] ReadNodes(XmlReader xmlReader)
		{
			if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.IsEmptyElement)
			{
				return new XmlNode[0];
			}
			int depth = xmlReader.Depth;
			xmlReader.Read();
			if (xmlReader.NodeType == XmlNodeType.EndElement)
			{
				return new XmlNode[0];
			}
			List<XmlNode> list = new List<XmlNode>();
			XmlDocument xmlDocument = new XmlDocument();
			while ((xmlReader.Depth > depth) & !xmlReader.EOF)
			{
				list.Add(xmlDocument.ReadNode(xmlReader));
			}
			return list.ToArray();
		}

		public static void WriteNodes(XmlWriter xmlWriter, XmlNode[] nodes)
		{
			foreach (XmlNode xmlNode in nodes)
			{
				xmlNode.WriteTo(xmlWriter);
			}
		}

		private static Dictionary<XmlQualifiedName, XmlSchemaSet> defaultSchemas = new Dictionary<XmlQualifiedName, XmlSchemaSet>();
	}
}
