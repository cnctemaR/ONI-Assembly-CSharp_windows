using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaGroup : XmlSchemaAnnotated
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

		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("all", typeof(XmlSchemaAll))]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				return this.particle;
			}
			set
			{
				this.particle = value;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
		}

		[XmlIgnore]
		internal XmlSchemaParticle CanonicalParticle
		{
			get
			{
				return this.canonicalParticle;
			}
			set
			{
				this.canonicalParticle = value;
			}
		}

		[XmlIgnore]
		internal XmlSchemaGroup Redefined
		{
			get
			{
				return this.redefined;
			}
			set
			{
				this.redefined = value;
			}
		}

		[XmlIgnore]
		internal int SelfReferenceCount
		{
			get
			{
				return this.selfReferenceCount;
			}
			set
			{
				this.selfReferenceCount = value;
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

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qname = value;
		}

		internal override XmlSchemaObject Clone()
		{
			return this.Clone(null);
		}

		internal XmlSchemaObject Clone(XmlSchema parentSchema)
		{
			XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)base.MemberwiseClone();
			if (XmlSchemaComplexType.HasParticleRef(this.particle, parentSchema))
			{
				xmlSchemaGroup.particle = XmlSchemaComplexType.CloneParticle(this.particle, parentSchema) as XmlSchemaGroupBase;
			}
			xmlSchemaGroup.canonicalParticle = XmlSchemaParticle.Empty;
			return xmlSchemaGroup;
		}

		private string name;

		private XmlSchemaGroupBase particle;

		private XmlSchemaParticle canonicalParticle;

		private XmlQualifiedName qname = XmlQualifiedName.Empty;

		private XmlSchemaGroup redefined;

		private int selfReferenceCount;
	}
}
