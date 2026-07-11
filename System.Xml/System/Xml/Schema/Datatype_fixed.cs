using System;

namespace System.Xml.Schema
{
	internal class Datatype_fixed : Datatype_decimal
	{
		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			Exception ex;
			try
			{
				Numeric10FacetsChecker numeric10FacetsChecker = this.FacetsChecker as Numeric10FacetsChecker;
				decimal num = XmlConvert.ToDecimal(s);
				ex = numeric10FacetsChecker.CheckTotalAndFractionDigits(num, 18, 4, true, true);
				if (ex == null)
				{
					return num;
				}
			}
			catch (XmlSchemaException ex2)
			{
				throw ex2;
			}
			catch (Exception ex3)
			{
				throw new XmlSchemaException(Res.GetString("The value '{0}' is invalid according to its data type.", new object[] { s }), ex3);
			}
			throw ex;
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			decimal num;
			Exception ex = XmlConvert.TryToDecimal(s, out num);
			if (ex == null)
			{
				ex = (this.FacetsChecker as Numeric10FacetsChecker).CheckTotalAndFractionDigits(num, 18, 4, true, true);
				if (ex == null)
				{
					typedValue = num;
					return null;
				}
			}
			return ex;
		}
	}
}
