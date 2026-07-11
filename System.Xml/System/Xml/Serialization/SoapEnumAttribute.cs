using System;

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
				if (this.name != null)
				{
					return this.name;
				}
				return string.Empty;
			}
			set
			{
				this.name = value;
			}
		}

		private string name;
	}
}
