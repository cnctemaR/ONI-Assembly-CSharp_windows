using System;

namespace System.Xml.Schema
{
	internal class Datatype_QNameXdr : Datatype_anySimpleType
	{
		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.QName;
			}
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			if (s == null || s.Length == 0)
			{
				throw new XmlSchemaException("The attribute value cannot be empty.", string.Empty);
			}
			if (nsmgr == null)
			{
				throw new ArgumentNullException("nsmgr");
			}
			object obj;
			try
			{
				string text;
				obj = XmlQualifiedName.Parse(s.Trim(), nsmgr, out text);
			}
			catch (XmlSchemaException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new XmlSchemaException(Res.GetString("The value '{0}' is invalid according to its data type.", new object[] { s }), ex2);
			}
			return obj;
		}

		public override Type ValueType
		{
			get
			{
				return Datatype_QNameXdr.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_QNameXdr.listValueType;
			}
		}

		private static readonly Type atomicValueType = typeof(XmlQualifiedName);

		private static readonly Type listValueType = typeof(XmlQualifiedName[]);
	}
}
