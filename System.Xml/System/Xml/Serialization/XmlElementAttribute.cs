using System;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class XmlElementAttribute : Attribute
	{
		public XmlElementAttribute()
		{
		}

		public XmlElementAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		public XmlElementAttribute(Type type)
		{
			this.type = type;
		}

		public XmlElementAttribute(string elementName, Type type)
		{
			this.elementName = elementName;
			this.type = type;
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

		public string ElementName
		{
			get
			{
				if (this.elementName != null)
				{
					return this.elementName;
				}
				return string.Empty;
			}
			set
			{
				this.elementName = value;
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

		public string DataType
		{
			get
			{
				if (this.dataType != null)
				{
					return this.dataType;
				}
				return string.Empty;
			}
			set
			{
				this.dataType = value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
				this.nullableSpecified = true;
			}
		}

		internal bool IsNullableSpecified
		{
			get
			{
				return this.nullableSpecified;
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

		public int Order
		{
			get
			{
				return this.order;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(Res.GetString("Negative values are prohibited."), "Order");
				}
				this.order = value;
			}
		}

		private string elementName;

		private Type type;

		private string ns;

		private string dataType;

		private bool nullable;

		private bool nullableSpecified;

		private XmlSchemaForm form;

		private int order = -1;
	}
}
