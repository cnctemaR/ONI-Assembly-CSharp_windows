using System;

namespace System.Xml.Serialization
{
	internal class ConstantMapping : Mapping
	{
		internal string XmlName
		{
			get
			{
				if (this.xmlName != null)
				{
					return this.xmlName;
				}
				return string.Empty;
			}
			set
			{
				this.xmlName = value;
			}
		}

		internal string Name
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

		internal long Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string xmlName;

		private string name;

		private long value;
	}
}
