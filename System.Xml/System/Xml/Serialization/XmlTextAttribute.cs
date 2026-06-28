using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlTextAttribute : Attribute
	{
		public XmlTextAttribute()
		{
		}

		public XmlTextAttribute(Type type)
		{
			this.type = type;
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
			sb.Append("XTXA ");
			KeyHelper.AddField(sb, 1, this.type);
			KeyHelper.AddField(sb, 2, this.dataType);
			sb.Append('|');
		}

		private string dataType;

		private Type type;
	}
}
