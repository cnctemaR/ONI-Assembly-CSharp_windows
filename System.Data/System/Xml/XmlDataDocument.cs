using System;
using System.Data;
using System.IO;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDataDocument : XmlDocument
	{
		public XmlDataDocument()
		{
		}

		public XmlDataDocument(DataSet dataset)
		{
		}

		public DataSet DataSet
		{
			get
			{
				throw null;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			throw null;
		}

		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			throw null;
		}

		public override XmlEntityReference CreateEntityReference(string name)
		{
			throw null;
		}

		[MonoTODO("Create optimized XPathNavigator")]
		protected override XPathNavigator CreateNavigator(XmlNode node)
		{
			throw null;
		}

		public override XmlElement GetElementById(string elemId)
		{
			throw null;
		}

		public XmlElement GetElementFromRow(DataRow r)
		{
			throw null;
		}

		public DataRow GetRowFromElement(XmlElement e)
		{
			throw null;
		}

		public override void Load(Stream inStream)
		{
		}

		public override void Load(TextReader txtReader)
		{
		}

		public override void Load(string filename)
		{
		}

		public override void Load(XmlReader reader)
		{
		}
	}
}
