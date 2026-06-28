using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Field)]
	public class XmlEnumAttribute : Attribute
	{
		public XmlEnumAttribute()
		{
		}

		public XmlEnumAttribute(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XENA ");
			KeyHelper.AddField(sb, 1, this.name);
			sb.Append('|');
		}

		private string name;
	}
}
