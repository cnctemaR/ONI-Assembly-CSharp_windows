using System;

namespace System.Xml.Schema
{
	internal class Datatype_date : Datatype_dateTimeBase
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Date;
			}
		}

		internal Datatype_date()
			: base(XsdDateTimeFlags.Date)
		{
		}
	}
}
