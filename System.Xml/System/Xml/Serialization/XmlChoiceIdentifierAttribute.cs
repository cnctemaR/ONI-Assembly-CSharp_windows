using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlChoiceIdentifierAttribute : Attribute
	{
		public XmlChoiceIdentifierAttribute()
		{
		}

		public XmlChoiceIdentifierAttribute(string name)
		{
			this.memberName = name;
		}

		public string MemberName
		{
			get
			{
				if (this.memberName == null)
				{
					return string.Empty;
				}
				return this.memberName;
			}
			set
			{
				this.memberName = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XCA ");
			KeyHelper.AddField(sb, 1, this.memberName);
			sb.Append('|');
		}

		private string memberName;
	}
}
