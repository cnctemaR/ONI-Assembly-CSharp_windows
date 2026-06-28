using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class SoapAttributeAttribute : Attribute
	{
		public SoapAttributeAttribute()
		{
		}

		public SoapAttributeAttribute(string attrName)
		{
			this.attrName = attrName;
		}

		public string AttributeName
		{
			get
			{
				if (this.attrName == null)
				{
					return string.Empty;
				}
				return this.attrName;
			}
			set
			{
				this.attrName = value;
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

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SAA ");
			KeyHelper.AddField(sb, 1, this.attrName);
			KeyHelper.AddField(sb, 2, this.dataType);
			KeyHelper.AddField(sb, 3, this.ns);
			sb.Append("|");
		}

		private string attrName;

		private string dataType;

		private string ns;
	}
}
