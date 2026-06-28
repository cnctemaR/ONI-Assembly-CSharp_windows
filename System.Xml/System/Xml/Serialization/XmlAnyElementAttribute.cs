using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class XmlAnyElementAttribute : Attribute
	{
		public XmlAnyElementAttribute()
		{
		}

		public XmlAnyElementAttribute(string name)
		{
			this.elementName = name;
		}

		public XmlAnyElementAttribute(string name, string ns)
		{
			this.elementName = name;
			this.ns = ns;
		}

		public string Name
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

		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.isNamespaceSpecified = true;
				this.ns = value;
			}
		}

		internal bool NamespaceSpecified
		{
			get
			{
				return this.isNamespaceSpecified;
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
			sb.Append("XAEA ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.elementName);
			sb.Append('|');
		}

		private string elementName;

		private string ns;

		private bool isNamespaceSpecified;

		private int order = -1;
	}
}
