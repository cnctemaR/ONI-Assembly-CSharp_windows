using System;

namespace System.Xml.Schema
{
	internal class Datatype_time : Datatype_dateTimeBase
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Time;
			}
		}

		internal Datatype_time()
			: base(XsdDateTimeFlags.Time)
		{
		}
	}
}
