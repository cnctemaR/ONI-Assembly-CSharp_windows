using System;

namespace System.Xml.Schema
{
	internal class Datatype_unsignedLong : Datatype_nonNegativeInteger
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_unsignedLong.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.UnsignedLong;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((ulong)value1).CompareTo(value2);
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_unsignedLong.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_unsignedLong.listValueType;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_unsignedLong.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				ulong num;
				ex = XmlConvert.TryToUInt64(s, out num);
				if (ex == null)
				{
					ex = Datatype_unsignedLong.numeric10FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(ulong);

		private static readonly Type listValueType = typeof(ulong[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(0m, 18446744073709551615m);
	}
}
