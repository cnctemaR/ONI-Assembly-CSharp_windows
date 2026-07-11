using System;

namespace System.Xml.Schema
{
	internal class Datatype_normalizedStringV1Compat : Datatype_string
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.NormalizedString;
			}
		}

		internal override bool HasValueFacets
		{
			get
			{
				return true;
			}
		}
	}
}
