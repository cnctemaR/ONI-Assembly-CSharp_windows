using System;

namespace System.Xml.Serialization
{
	internal class NullableMapping : TypeMapping
	{
		internal TypeMapping BaseMapping
		{
			get
			{
				return this.baseMapping;
			}
			set
			{
				this.baseMapping = value;
			}
		}

		internal override string DefaultElementName
		{
			get
			{
				return this.BaseMapping.DefaultElementName;
			}
		}

		private TypeMapping baseMapping;
	}
}
