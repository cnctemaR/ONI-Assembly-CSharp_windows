using System;

namespace System.Xml.Schema
{
	internal class Datatype_dayTimeDuration : Datatype_duration
	{
		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			if (s == null || s.Length == 0)
			{
				return new XmlSchemaException("The attribute value cannot be empty.", string.Empty);
			}
			Exception ex = DatatypeImplementation.durationFacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				XsdDuration xsdDuration;
				ex = XsdDuration.TryParse(s, XsdDuration.DurationType.DayTimeDuration, out xsdDuration);
				if (ex == null)
				{
					TimeSpan timeSpan;
					ex = xsdDuration.TryToTimeSpan(XsdDuration.DurationType.DayTimeDuration, out timeSpan);
					if (ex == null)
					{
						ex = DatatypeImplementation.durationFacetsChecker.CheckValueFacets(timeSpan, this);
						if (ex == null)
						{
							typedValue = timeSpan;
							return null;
						}
					}
				}
			}
			return ex;
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.DayTimeDuration;
			}
		}
	}
}
