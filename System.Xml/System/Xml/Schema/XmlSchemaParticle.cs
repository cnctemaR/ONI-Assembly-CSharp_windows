using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaParticle : XmlSchemaAnnotated
	{
		[XmlAttribute("minOccurs")]
		public string MinOccursString
		{
			get
			{
				if ((this.flags & XmlSchemaParticle.Occurs.Min) != XmlSchemaParticle.Occurs.None)
				{
					return XmlConvert.ToString(this.minOccurs);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.minOccurs = 1m;
					this.flags &= ~XmlSchemaParticle.Occurs.Min;
					return;
				}
				this.minOccurs = XmlConvert.ToInteger(value);
				if (this.minOccurs < 0m)
				{
					throw new XmlSchemaException("The value for the 'minOccurs' attribute must be xsd:nonNegativeInteger.", string.Empty);
				}
				this.flags |= XmlSchemaParticle.Occurs.Min;
			}
		}

		[XmlAttribute("maxOccurs")]
		public string MaxOccursString
		{
			get
			{
				if ((this.flags & XmlSchemaParticle.Occurs.Max) == XmlSchemaParticle.Occurs.None)
				{
					return null;
				}
				if (!(this.maxOccurs == 79228162514264337593543950335m))
				{
					return XmlConvert.ToString(this.maxOccurs);
				}
				return "unbounded";
			}
			set
			{
				if (value == null)
				{
					this.maxOccurs = 1m;
					this.flags &= ~XmlSchemaParticle.Occurs.Max;
					return;
				}
				if (value == "unbounded")
				{
					this.maxOccurs = decimal.MaxValue;
				}
				else
				{
					this.maxOccurs = XmlConvert.ToInteger(value);
					if (this.maxOccurs < 0m)
					{
						throw new XmlSchemaException("The value for the 'maxOccurs' attribute must be xsd:nonNegativeInteger or 'unbounded'.", string.Empty);
					}
					if (this.maxOccurs == 0m && (this.flags & XmlSchemaParticle.Occurs.Min) == XmlSchemaParticle.Occurs.None)
					{
						this.minOccurs = 0m;
					}
				}
				this.flags |= XmlSchemaParticle.Occurs.Max;
			}
		}

		[XmlIgnore]
		public decimal MinOccurs
		{
			get
			{
				return this.minOccurs;
			}
			set
			{
				if (value < 0m || value != decimal.Truncate(value))
				{
					throw new XmlSchemaException("The value for the 'minOccurs' attribute must be xsd:nonNegativeInteger.", string.Empty);
				}
				this.minOccurs = value;
				this.flags |= XmlSchemaParticle.Occurs.Min;
			}
		}

		[XmlIgnore]
		public decimal MaxOccurs
		{
			get
			{
				return this.maxOccurs;
			}
			set
			{
				if (value < 0m || value != decimal.Truncate(value))
				{
					throw new XmlSchemaException("The value for the 'maxOccurs' attribute must be xsd:nonNegativeInteger or 'unbounded'.", string.Empty);
				}
				this.maxOccurs = value;
				if (this.maxOccurs == 0m && (this.flags & XmlSchemaParticle.Occurs.Min) == XmlSchemaParticle.Occurs.None)
				{
					this.minOccurs = 0m;
				}
				this.flags |= XmlSchemaParticle.Occurs.Max;
			}
		}

		internal virtual bool IsEmpty
		{
			get
			{
				return this.maxOccurs == 0m;
			}
		}

		internal bool IsMultipleOccurrence
		{
			get
			{
				return this.maxOccurs > 1m;
			}
		}

		internal virtual string NameString
		{
			get
			{
				return string.Empty;
			}
		}

		internal XmlQualifiedName GetQualifiedName()
		{
			XmlSchemaElement xmlSchemaElement = this as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				return xmlSchemaElement.QualifiedName;
			}
			XmlSchemaAny xmlSchemaAny = this as XmlSchemaAny;
			if (xmlSchemaAny != null)
			{
				string text = xmlSchemaAny.Namespace;
				if (text != null)
				{
					text = text.Trim();
				}
				else
				{
					text = string.Empty;
				}
				return new XmlQualifiedName("*", (text.Length == 0) ? "##any" : text);
			}
			return XmlQualifiedName.Empty;
		}

		private decimal minOccurs = 1m;

		private decimal maxOccurs = 1m;

		private XmlSchemaParticle.Occurs flags;

		internal static readonly XmlSchemaParticle Empty = new XmlSchemaParticle.EmptyParticle();

		[Flags]
		private enum Occurs
		{
			None = 0,
			Min = 1,
			Max = 2
		}

		private class EmptyParticle : XmlSchemaParticle
		{
			internal override bool IsEmpty
			{
				get
				{
					return true;
				}
			}
		}
	}
}
