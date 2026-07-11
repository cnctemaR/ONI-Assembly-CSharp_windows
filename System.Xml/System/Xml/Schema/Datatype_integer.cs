using System;

namespace System.Xml.Schema
{
	internal class Datatype_integer : Datatype_decimal
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Integer;
			}
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Exception ex = this.FacetsChecker.CheckLexicalFacets(ref s, this);
			if (ex == null)
			{
				decimal num;
				ex = XmlConvert.TryToInteger(s, out num);
				if (ex == null)
				{
					ex = this.FacetsChecker.CheckValueFacets(num, this);
					if (ex == null)
					{
						typedValue = num;
						return null;
					}
				}
			}
			return ex;
		}
	}
}
