using System;

namespace System.Xml.Schema
{
	internal class Datatype_ID : Datatype_NCName
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Id;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.ID;
			}
		}
	}
}
