using System;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SoapEnumAttribute : Attribute
	{
		public SoapEnumAttribute()
		{
		}

		public SoapEnumAttribute(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					return string.Empty;
				}
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SENA ");
			KeyHelper.AddField(sb, 1, this.name);
			sb.Append('|');
		}

		private string name;
	}
}
