using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class SoapElementAttribute : Attribute
	{
		public SoapElementAttribute()
		{
		}

		public SoapElementAttribute(string elementName)
		{
			this.elementName = elementName;
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

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SEA ");
			KeyHelper.AddField(sb, 1, this.elementName);
			KeyHelper.AddField(sb, 2, this.dataType);
			KeyHelper.AddField(sb, 3, this.isNullable);
			sb.Append('|');
		}

		private string dataType;

		private string elementName;

		private bool isNullable;
	}
}
