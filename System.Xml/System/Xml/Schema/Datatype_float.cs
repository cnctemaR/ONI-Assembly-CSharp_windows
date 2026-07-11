using System;

namespace System.Xml.Schema
{
	internal class Datatype_float : Datatype_anySimpleType
	{
		internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
		{
			return XmlNumeric2Converter.Create(schemaType);
		}

		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return DatatypeImplementation.numeric2FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Float;
			}
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_float.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_float.listValueType;
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
				return RestrictionFlags.Pattern | RestrictionFlags.Enumeration | RestrictionFlags.WhiteSpace | RestrictionFlags.MaxInclusive | RestrictionFlags.MaxExclusive | RestrictionFlags.MinInclusive | RestrictionFlags.MinExclusive;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((float)value1).CompareTo(value2);
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = DatatypeImplementation.numeric2FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				float num;
				ex = XmlConvert.TryToSingle(s, out num);
				if (ex == null)
				{
					ex = DatatypeImplementation.numeric2FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(float);

		private static readonly Type listValueType = typeof(float[]);
	}
}
