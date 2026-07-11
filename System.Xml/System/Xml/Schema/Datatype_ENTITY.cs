using System;

namespace System.Xml.Schema
{
	internal class Datatype_ENTITY : Datatype_NCName
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Entity;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.ENTITY;
			}
		}
	}
}
