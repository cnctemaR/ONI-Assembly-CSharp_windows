using System;

namespace System.Xml.Schema
{
	internal class Datatype_byte : Datatype_short
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_byte.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Byte;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((sbyte)value1).CompareTo(value2);
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_byte.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_byte.listValueType;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_byte.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				sbyte b;
				ex = XmlConvert.TryToSByte(s, out b);
				if (ex == null)
				{
					ex = Datatype_byte.numeric10FacetsChecker.CheckValueFacets((short)b, this);
					if (ex == null)
					{
						typedValue = b;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(sbyte);

		private static readonly Type listValueType = typeof(sbyte[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(-128m, 127m);
	}
}
