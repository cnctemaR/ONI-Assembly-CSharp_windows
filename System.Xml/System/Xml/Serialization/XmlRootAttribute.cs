using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.ReturnValue)]
	public class XmlRootAttribute : Attribute
	{
		public XmlRootAttribute()
		{
		}

		public XmlRootAttribute(string elementName)
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
				this.isNullableSpecified = true;
				this.isNullable = value;
			}
		}

		public bool IsNullableSpecified
		{
			get
			{
				return this.isNullableSpecified;
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
			sb.Append("XRA ");
			KeyHelper.AddField(sb, 1, this.ns);
			KeyHelper.AddField(sb, 2, this.elementName);
			KeyHelper.AddField(sb, 3, this.dataType);
			KeyHelper.AddField(sb, 4, this.isNullable);
			sb.Append('|');
		}

		internal string Key
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.AddKeyHash(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		private string dataType;

		private string elementName;

		private bool isNullable = true;

		private bool isNullableSpecified;

		private string ns;
	}
}
