using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlAttributes
	{
		public XmlAttributes()
		{
		}

		public XmlAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj is XmlAnyAttributeAttribute)
				{
					this.xmlAnyAttribute = (XmlAnyAttributeAttribute)obj;
				}
				else if (obj is XmlAnyElementAttribute)
				{
					this.xmlAnyElements.Add((XmlAnyElementAttribute)obj);
				}
				else if (obj is XmlArrayAttribute)
				{
					this.xmlArray = (XmlArrayAttribute)obj;
				}
				else if (obj is XmlArrayItemAttribute)
				{
					this.xmlArrayItems.Add((XmlArrayItemAttribute)obj);
				}
				else if (obj is XmlAttributeAttribute)
				{
					this.xmlAttribute = (XmlAttributeAttribute)obj;
				}
				else if (obj is XmlChoiceIdentifierAttribute)
				{
					this.xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)obj;
				}
				else if (obj is DefaultValueAttribute)
				{
					this.xmlDefaultValue = ((DefaultValueAttribute)obj).Value;
				}
				else if (obj is XmlElementAttribute)
				{
					this.xmlElements.Add((XmlElementAttribute)obj);
				}
				else if (obj is XmlEnumAttribute)
				{
					this.xmlEnum = (XmlEnumAttribute)obj;
				}
				else if (obj is XmlIgnoreAttribute)
				{
					this.xmlIgnore = true;
				}
				else if (obj is XmlNamespaceDeclarationsAttribute)
				{
					this.xmlns = true;
				}
				else if (obj is XmlRootAttribute)
				{
					this.xmlRoot = (XmlRootAttribute)obj;
				}
				else if (obj is XmlTextAttribute)
				{
					this.xmlText = (XmlTextAttribute)obj;
				}
				else if (obj is XmlTypeAttribute)
				{
					this.xmlType = (XmlTypeAttribute)obj;
				}
			}
		}

		public XmlAnyAttributeAttribute XmlAnyAttribute
		{
			get
			{
				return this.xmlAnyAttribute;
			}
			set
			{
				this.xmlAnyAttribute = value;
			}
		}

		public XmlAnyElementAttributes XmlAnyElements
		{
			get
			{
				return this.xmlAnyElements;
			}
		}

		public XmlArrayAttribute XmlArray
		{
			get
			{
				return this.xmlArray;
			}
			set
			{
				this.xmlArray = value;
			}
		}

		public XmlArrayItemAttributes XmlArrayItems
		{
			get
			{
				return this.xmlArrayItems;
			}
		}

		public XmlAttributeAttribute XmlAttribute
		{
			get
			{
				return this.xmlAttribute;
			}
			set
			{
				this.xmlAttribute = value;
			}
		}

		public XmlChoiceIdentifierAttribute XmlChoiceIdentifier
		{
			get
			{
				return this.xmlChoiceIdentifier;
			}
		}

		public object XmlDefaultValue
		{
			get
			{
				return this.xmlDefaultValue;
			}
			set
			{
				this.xmlDefaultValue = value;
			}
		}

		public XmlElementAttributes XmlElements
		{
			get
			{
				return this.xmlElements;
			}
		}

		public XmlEnumAttribute XmlEnum
		{
			get
			{
				return this.xmlEnum;
			}
			set
			{
				this.xmlEnum = value;
			}
		}

		public bool XmlIgnore
		{
			get
			{
				return this.xmlIgnore;
			}
			set
			{
				this.xmlIgnore = value;
			}
		}

		public bool Xmlns
		{
			get
			{
				return this.xmlns;
			}
			set
			{
				this.xmlns = value;
			}
		}

		public XmlRootAttribute XmlRoot
		{
			get
			{
				return this.xmlRoot;
			}
			set
			{
				this.xmlRoot = value;
			}
		}

		public XmlTextAttribute XmlText
		{
			get
			{
				return this.xmlText;
			}
			set
			{
				this.xmlText = value;
			}
		}

		public XmlTypeAttribute XmlType
		{
			get
			{
				return this.xmlType;
			}
			set
			{
				this.xmlType = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XA ");
			KeyHelper.AddField(sb, 1, this.xmlIgnore);
			KeyHelper.AddField(sb, 2, this.xmlns);
			KeyHelper.AddField(sb, 3, this.xmlAnyAttribute != null);
			this.xmlAnyElements.AddKeyHash(sb);
			this.xmlArrayItems.AddKeyHash(sb);
			this.xmlElements.AddKeyHash(sb);
			if (this.xmlArray != null)
			{
				this.xmlArray.AddKeyHash(sb);
			}
			if (this.xmlAttribute != null)
			{
				this.xmlAttribute.AddKeyHash(sb);
			}
			if (this.xmlDefaultValue == null)
			{
				sb.Append("n");
			}
			else if (!(this.xmlDefaultValue is DBNull))
			{
				string text = XmlCustomFormatter.ToXmlString(TypeTranslator.GetTypeData(this.xmlDefaultValue.GetType()), this.xmlDefaultValue);
				sb.Append("v" + text);
			}
			if (this.xmlEnum != null)
			{
				this.xmlEnum.AddKeyHash(sb);
			}
			if (this.xmlRoot != null)
			{
				this.xmlRoot.AddKeyHash(sb);
			}
			if (this.xmlText != null)
			{
				this.xmlText.AddKeyHash(sb);
			}
			if (this.xmlType != null)
			{
				this.xmlType.AddKeyHash(sb);
			}
			if (this.xmlChoiceIdentifier != null)
			{
				this.xmlChoiceIdentifier.AddKeyHash(sb);
			}
			sb.Append("|");
		}

		private XmlAnyAttributeAttribute xmlAnyAttribute;

		private XmlAnyElementAttributes xmlAnyElements = new XmlAnyElementAttributes();

		private XmlArrayAttribute xmlArray;

		private XmlArrayItemAttributes xmlArrayItems = new XmlArrayItemAttributes();

		private XmlAttributeAttribute xmlAttribute;

		private XmlChoiceIdentifierAttribute xmlChoiceIdentifier;

		private object xmlDefaultValue = DBNull.Value;

		private XmlElementAttributes xmlElements = new XmlElementAttributes();

		private XmlEnumAttribute xmlEnum;

		private bool xmlIgnore;

		private bool xmlns;

		private XmlRootAttribute xmlRoot;

		private XmlTextAttribute xmlText;

		private XmlTypeAttribute xmlType;
	}
}
