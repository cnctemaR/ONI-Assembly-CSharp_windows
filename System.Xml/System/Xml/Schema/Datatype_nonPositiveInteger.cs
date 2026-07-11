using System;

namespace System.Xml.Schema
{
	internal class Datatype_nonPositiveInteger : Datatype_integer
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_nonPositiveInteger.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.NonPositiveInteger;
			}
		}

		internal override bool HasValueFacets
		{
			get
			{
				return true;
			}
		}

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.MinValue, 0m);
	}
}
