using System;

namespace System.Xml.Schema
{
	internal class Datatype_long : Datatype_integer
	{
		internal override FacetsChecker FacetsChecker
		{
			get
			{
				return Datatype_long.numeric10FacetsChecker;
			}
		}

		internal override bool HasValueFacets
		{
			get
			{
				return true;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Long;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((long)value1).CompareTo(value2);
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_long.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_long.listValueType;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = Datatype_long.numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				long num;
				ex = XmlConvert.TryToInt64(s, out num);
				if (ex == null)
				{
					ex = Datatype_long.numeric10FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(long);

		private static readonly Type listValueType = typeof(long[]);

		private static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(-9223372036854775808m, 9223372036854775807m);
	}
}
