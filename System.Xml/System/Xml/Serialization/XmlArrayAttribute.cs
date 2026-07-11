using System;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
	public class XmlArrayAttribute : Attribute
	{
		public XmlArrayAttribute()
		{
		}

		public XmlArrayAttribute(string elementName)
		{
			this.elementName = elementName;
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

		public bool IsNullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
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

		private string ns;

		private bool nullable;

		private XmlSchemaForm form;

		private int order = -1;
	}
}
