using System;

namespace System.Xml.Schema
{
	internal class Datatype_token : Datatype_normalizedString
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Token;
			}
		}

		internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet
		{
			get
			{
				return XmlSchemaWhiteSpace.Collapse;
			}
		}
	}
}
