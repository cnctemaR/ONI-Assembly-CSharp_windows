using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaNotation : XmlSchemaAnnotated
	{
		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute("public")]
		public string Public
		{
			get
			{
				return this.publicId;
			}
			set
			{
				this.publicId = value;
			}
		}

		[XmlAttribute("system")]
		public string System
		{
			get
			{
				return this.systemId;
			}
			set
			{
				this.systemId = value;
			}
		}

		[XmlIgnore]
		internal XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
			set
			{
				this.qname = value;
			}
		}

		[XmlIgnore]
		internal override string NameAttribute
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		private string name;

		private string publicId;

		private string systemId;

		private XmlQualifiedName qname = XmlQualifiedName.Empty;
	}
}
