using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaGroupRef : XmlSchemaParticle
	{
		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return this.refName;
			}
			set
			{
				this.refName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		[XmlIgnore]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				return this.particle;
			}
		}

		internal void SetParticle(XmlSchemaGroupBase value)
		{
			this.particle = value;
		}

		[XmlIgnore]
		internal XmlSchemaGroup Redefined
		{
			get
			{
				return this.refined;
			}
			set
			{
				this.refined = value;
			}
		}

		private XmlQualifiedName refName = XmlQualifiedName.Empty;

		private XmlSchemaGroupBase particle;

		private XmlSchemaGroup refined;
	}
}
