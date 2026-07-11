using System;

namespace System.Xml.Schema
{
	internal class Datatype_int : Datatype_long
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_int.numeric10FacetsChecker;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Int;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((int)value1).CompareTo(value2);
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_int.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_int.listValueType;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_int.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				int num;
				ex = XmlConvert.TryToInt32(s, out num);
				if (ex == null)
				{
					ex = Datatype_int.numeric10FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(int);

		private static readonly Type listValueType = typeof(int[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(-2147483648m, 2147483647m);
	}
}
