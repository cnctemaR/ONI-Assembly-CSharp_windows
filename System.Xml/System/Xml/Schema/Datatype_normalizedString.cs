using System;

namespace System.Xml.Schema
{
	internal class Datatype_normalizedString : Datatype_string
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.NormalizedString;
			}
		}

		internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet
		{
			get
			{
				return XmlSchemaWhiteSpace.Replace;
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
