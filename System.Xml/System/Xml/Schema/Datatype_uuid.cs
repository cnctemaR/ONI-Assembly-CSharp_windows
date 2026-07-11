using System;

namespace System.Xml.Schema
{
	internal class Datatype_uuid : Datatype_anySimpleType
	{
		public override Type ValueType
		{
			get
			{
				return Datatype_uuid.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_uuid.listValueType;
			}
		}

		internal override RestrictionFlags ValidRestrictionFlags
		{
			get
			{
				return (RestrictionFlags)0;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			if (!((Guid)value1).Equals(value2))
			{
				return -1;
			}
			return 0;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			object obj;
			try
			{
				obj = XmlConvert.ToGuid(s);
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

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			Guid guid;
			Exception ex = XmlConvert.TryToGuid(s, out guid);
			if (ex == null)
			{
				typedValue = guid;
				return null;
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(Guid);

		private static readonly Type listValueType = typeof(Guid[]);
	}
}
