using System;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlAttributeAttribute : Attribute
	{
		public XmlAttributeAttribute()
		{
		}

		public XmlAttributeAttribute(string attributeName)
		{
			this.attributeName = attributeName;
		}

		public XmlAttributeAttribute(Type type)
		{
			this.type = type;
		}

		public XmlAttributeAttribute(string attributeName, Type type)
		{
			this.attributeName = attributeName;
			this.type = type;
		}

		public string AttributeName
		{
			get
			{
				if (this.attributeName == null)
				{
					return string.Empty;
				}
				return this.attributeName;
			}
			set
			{
				this.attributeName = value;
			}
		}

		public string DataType
		{
			get
			{
				if (this.dataType == null)
				{
					return string.Empty;
				}
				return this.dataType;
			}
			set
			{
				this.dataType = value;
			}
		}

		public XmlSchemaForm Form
		{
			get
			{
				return this.form;
			}
			set
			{
				this.form = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAA ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.attributeName);
			KeyHelper.AddField(sb, 3, this.form.ToString(), XmlSchemaForm.None.ToString());
			KeyHelper.AddField(sb, 4, this.dataType);
			KeyHelper.AddField(sb, 5, this.type);
			sb.Append('|');
		}

		private string attributeName;

		private string dataType;

		private Type type;

		private XmlSchemaForm form;

		private string ns;
	}
}
