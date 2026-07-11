using System;

namespace System.Xml.Schema
{
	internal class Datatype_short : Datatype_int
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_short.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Short;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((short)value1).CompareTo(value2);
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_short.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_short.listValueType;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_short.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				short num;
				ex = XmlConvert.TryToInt16(s, out num);
				if (ex == null)
				{
					ex = Datatype_short.numeric10FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(short);

		private static readonly Type listValueType = typeof(short[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(-32768m, 32767m);
	}
}
