using System;

namespace System.Xml.Schema
{
	internal class Datatype_decimal : Datatype_anySimpleType
	{
		internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
		{
			return XmlNumeric10Converter.Create(schemaType);
		}

		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_decimal.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Decimal;
			}
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_decimal.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_decimal.listValueType;
			}
		}

		internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet
		{
			get
			{
				return XmlSchemaWhiteSpace.Collapse;
			}
		}

		internal override RestrictionFlags ValidRestrictionFlags
		{
			get
			{
				return RestrictionFlags.Pattern | RestrictionFlags.Enumeration | RestrictionFlags.WhiteSpace | RestrictionFlags.MaxInclusive | RestrictionFlags.MaxExclusive | RestrictionFlags.MinInclusive | RestrictionFlags.MinExclusive | RestrictionFlags.TotalDigits | RestrictionFlags.FractionDigits;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((decimal)value1).CompareTo(value2);
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_decimal.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				decimal num;
				ex = XmlConvert.TryToDecimal(s, out num);
				if (ex == null)
				{
					ex = Datatype_decimal.numeric10FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(decimal);

		private static readonly Type listValueType = typeof(decimal[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.MinValue, decimal.MaxValue);
	}
}
