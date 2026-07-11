using System;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
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
				if (this.elementName == null)
				{
					return string.Empty;
				}
				return this.elementName;
			}
			set
			{
				this.elementName = value;
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

		public bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
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

		[MonoTODO]
		public int Order
		{
			get
			{
				return this.order;
			}
			set
			{
				this.order = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAAT ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.elementName);
			KeyHelper.AddField(sb, 3, this.form.ToString(), XmlSchemaForm.None.ToString());
			KeyHelper.AddField(sb, 4, this.isNullable);
			sb.Append('|');
		}

		private string elementName;

		private XmlSchemaForm form;

		private bool isNullable;

		private string ns;

		private int order = -1;
	}
}
