using System;

namespace System.Xml.Schema
{
	internal class Datatype_nonNegativeInteger : Datatype_integer
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_nonNegativeInteger.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.NonNegativeInteger;
			}
		}

		internal override bool HasValueFacets
		{
			get
			{
				return true;
			}
		}

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(0m, decimal.MaxValue);
	}
}
