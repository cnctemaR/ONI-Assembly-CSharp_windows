using System;

namespace System.Xml.Schema
{
	internal class Datatype_positiveInteger : Datatype_nonNegativeInteger
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_positiveInteger.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.PositiveInteger;
			}
		}

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(1m, decimal.MaxValue);
	}
}
