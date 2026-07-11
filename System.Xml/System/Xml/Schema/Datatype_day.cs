using System;

namespace System.Xml.Schema
{
	internal class Datatype_day : Datatype_dateTimeBase
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.GDay;
			}
		}

		internal Datatype_day()
			: base(XsdDateTimeFlags.GDay)
		{
		}
	}
}
