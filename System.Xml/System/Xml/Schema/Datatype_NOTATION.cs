using System;

namespace System.Xml.Schema
{
	internal class Datatype_NOTATION : Datatype_anySimpleType
	{
		internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
		{
			return XmlMiscConverter.Create(schemaType);
		}

		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return DatatypeImplementation.qnameFacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Notation;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.NOTATION;
			}
		}

		internal override RestrictionFlags ValidRestrictionFlags
		{
			get
			{
				return RestrictionFlags.Length | RestrictionFlags.MinLength | RestrictionFlags.MaxLength | RestrictionFlags.Pattern | RestrictionFlags.Enumeration | RestrictionFlags.WhiteSpace;
			}
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_NOTATION.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_NOTATION.listValueType;
			}
		}

		internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet
		{
			get
			{
				return XmlSchemaWhiteSpace.Collapse;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			if (s == null || s.Length == 0)
			{
				return new XmlSchemaException("The attribute value cannot be empty.", string.Empty);
			}
			Exception ex = DatatypeImplementation.qnameFacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				XmlQualifiedName xmlQualifiedName = null;
				try
				{
					string text;
					xmlQualifiedName = XmlQualifiedName.Parse(s, nsmgr, out text);
				}
				catch (ArgumentException ex)
				{
					return ex;
				}
				catch (XmlException ex)
				{
					return ex;
				}
				ex = DatatypeImplementation.qnameFacetsChecker.CheckValueFacets(xmlQualifiedName, this);
				if (ex == null)
				{
					typedValue = xmlQualifiedName;
					return null;
				}
			}
			return ex;
		}

		internal override void VerifySchemaValid(XmlSchemaObjectTable notations, XmlSchemaObject caller)
		{
			for (Datatype_NOTATION datatype_NOTATION = this; datatype_NOTATION != null; datatype_NOTATION = (Datatype_NOTATION)datatype_NOTATION.Base)
			{
				if (datatype_NOTATION.Restriction != null && (datatype_NOTATION.Restriction.Flags & RestrictionFlags.Enumeration) != (RestrictionFlags)0)
				{
					for (int i = 0; i < datatype_NOTATION.Restriction.Enumeration.Count; i++)
					{
						XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)datatype_NOTATION.Restriction.Enumeration[i];
						if (!notations.Contains(xmlQualifiedName))
						{
							throw new XmlSchemaException("NOTATION cannot be used directly in a schema; only data types derived from NOTATION by specifying an enumeration value can be used in a schema. All enumeration facet values must match the name of a notation declared in the current schema.", caller);
						}
					}
					return;
				}
			}
			throw new XmlSchemaException("NOTATION cannot be used directly in a schema; only data types derived from NOTATION by specifying an enumeration value can be used in a schema. All enumeration facet values must match the name of a notation declared in the current schema.", caller);
		}

		private static readonly Type atomicValueType = typeof(XmlQualifiedName);

		private static readonly Type listValueType = typeof(XmlQualifiedName[]);
	}
}
