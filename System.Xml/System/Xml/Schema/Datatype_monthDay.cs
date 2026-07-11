using System;

namespace System.Xml.Schema
{
	internal class Datatype_monthDay : Datatype_dateTimeBase
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.GMonthDay;
			}
		}

		internal Datatype_monthDay()
			: base(XsdDateTimeFlags.GMonthDay)
		{
		}
	}
}
