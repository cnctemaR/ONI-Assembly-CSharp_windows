using System;

namespace System.Xml.Serialization
{
	internal class SpecialMapping : TypeMapping
	{
		internal bool NamedAny
		{
			get
			{
				return this.namedAny;
			}
			set
			{
				this.namedAny = value;
			}
		}

		private bool namedAny;
	}
}
