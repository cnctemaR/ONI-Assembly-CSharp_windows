using System;

namespace System.Xml
{
	public class XmlNotation : XmlNode
	{
		internal XmlNotation(string name, string publicId, string systemId, XmlDocument doc)
			: base(doc)
		{
			this.name = doc.NameTable.Add(name);
			this.publicId = publicId;
			this.systemId = systemId;
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.name;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Notation;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			throw new InvalidOperationException(Res.GetString("'Entity' and 'Notation' nodes cannot be cloned."));
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public string PublicId
		{
			get
			{
				return this.publicId;
			}
		}

		public string SystemId
		{
			get
			{
				return this.systemId;
			}
		}

		public override string OuterXml
		{
			get
			{
				return string.Empty;
			}
		}

		public override string InnerXml
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException(Res.GetString("Cannot set the 'InnerXml' for the current node because it is either read-only or cannot have children."));
			}
		}

		public override void WriteTo(XmlWriter w)
		{
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		private string publicId;

		private string systemId;

		private string name;
	}
}
