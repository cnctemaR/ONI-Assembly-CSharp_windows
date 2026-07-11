using System;

namespace System.Xml.Serialization
{
	internal class PrimitiveMapping : TypeMapping
	{
		internal override bool IsList
		{
			get
			{
				return this.isList;
			}
			set
			{
				this.isList = value;
			}
		}

		private bool isList;
	}
}
