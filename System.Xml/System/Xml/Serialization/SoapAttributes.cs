using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Xml.Serialization
{
	public class SoapAttributes
	{
		public SoapAttributes()
		{
		}

		public SoapAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj is SoapAttributeAttribute)
				{
					this.soapAttribute = (SoapAttributeAttribute)obj;
				}
				else if (obj is DefaultValueAttribute)
				{
					this.soapDefaultValue = ((DefaultValueAttribute)obj).Value;
				}
				else if (obj is SoapElementAttribute)
				{
					this.soapElement = (SoapElementAttribute)obj;
				}
				else if (obj is SoapEnumAttribute)
				{
					this.soapEnum = (SoapEnumAttribute)obj;
				}
				else if (obj is SoapIgnoreAttribute)
				{
					this.soapIgnore = true;
				}
				else if (obj is SoapTypeAttribute)
				{
					this.soapType = (SoapTypeAttribute)obj;
				}
			}
		}

		public SoapAttributeAttribute SoapAttribute
		{
			get
			{
				return this.soapAttribute;
			}
			set
			{
				this.soapAttribute = value;
			}
		}

		public object SoapDefaultValue
		{
			get
			{
				return this.soapDefaultValue;
			}
			set
			{
				this.soapDefaultValue = value;
			}
		}

		public SoapElementAttribute SoapElement
		{
			get
			{
				return this.soapElement;
			}
			set
			{
				this.soapElement = value;
			}
		}

		public SoapEnumAttribute SoapEnum
		{
			get
			{
				return this.soapEnum;
			}
			set
			{
				this.soapEnum = value;
			}
		}

		public bool SoapIgnore
		{
			get
			{
				return this.soapIgnore;
			}
			set
			{
				this.soapIgnore = value;
			}
		}

		public SoapTypeAttribute SoapType
		{
			get
			{
				return this.soapType;
			}
			set
			{
				this.soapType = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SA ");
			if (this.soapIgnore)
			{
				sb.Append('i');
			}
			if (this.soapAttribute != null)
			{
				this.soapAttribute.AddKeyHash(sb);
			}
			if (this.soapElement != null)
			{
				this.soapElement.AddKeyHash(sb);
			}
			if (this.soapEnum != null)
			{
				this.soapEnum.AddKeyHash(sb);
			}
			if (this.soapType != null)
			{
				this.soapType.AddKeyHash(sb);
			}
			if (this.soapDefaultValue == null)
			{
				sb.Append("n");
			}
			else if (!(this.soapDefaultValue is DBNull))
			{
				string text = XmlCustomFormatter.ToXmlString(TypeTranslator.GetTypeData(this.soapDefaultValue.GetType()), this.soapDefaultValue);
				sb.Append("v" + text);
			}
			sb.Append("|");
		}

		private SoapAttributeAttribute soapAttribute;

		private object soapDefaultValue = DBNull.Value;

		private SoapElementAttribute soapElement;

		private SoapEnumAttribute soapEnum;

		private bool soapIgnore;

		private SoapTypeAttribute soapType;
	}
}
