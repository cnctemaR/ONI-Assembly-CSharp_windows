using System;

namespace System.Xml.Schema
{
	internal class Datatype_boolean : Datatype_anySimpleType
	{
		internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
		{
			return XmlBooleanConverter.Create(schemaType);
		}

		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return DatatypeImplementation.miscFacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Boolean;
			}
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_boolean.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_boolean.listValueType;
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
				return RestrictionFlags.Pattern | RestrictionFlags.WhiteSpace;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((bool)value1).CompareTo(value2);
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = DatatypeImplementation.miscFacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				bool flag;
				ex = XmlConvert.TryToBoolean(s, out flag);
				if (ex == null)
				{
					typedValue = flag;
					return null;
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(bool);

		private static readonly Type listValueType = typeof(bool[]);
	}
}
