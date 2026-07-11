using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using Unity;

namespace System.Xml.Schema
{
	public sealed class XmlAtomicValue : XPathItem, ICloneable
	{
		internal XmlAtomicValue(XmlSchemaType xmlType, bool value)
		{
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.clrType = TypeCode.Boolean;
			this.unionVal.boolVal = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, DateTime value)
		{
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.clrType = TypeCode.DateTime;
			this.unionVal.dtVal = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, double value)
		{
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.clrType = TypeCode.Double;
			this.unionVal.dblVal = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, int value)
		{
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.clrType = TypeCode.Int32;
			this.unionVal.i32Val = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, long value)
		{
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.clrType = TypeCode.Int64;
			this.unionVal.i64Val = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.objVal = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, string value, IXmlNamespaceResolver nsResolver)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.objVal = value;
			if (nsResolver != null && (this.xmlType.TypeCode == XmlTypeCode.QName || this.xmlType.TypeCode == XmlTypeCode.Notation))
			{
				string prefixFromQName = this.GetPrefixFromQName(value);
				this.nsPrefix = new XmlAtomicValue.NamespacePrefixForQName(prefixFromQName, nsResolver.LookupNamespace(prefixFromQName));
			}
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.objVal = value;
		}

		internal XmlAtomicValue(XmlSchemaType xmlType, object value, IXmlNamespaceResolver nsResolver)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (xmlType == null)
			{
				throw new ArgumentNullException("xmlType");
			}
			this.xmlType = xmlType;
			this.objVal = value;
			if (nsResolver != null && (this.xmlType.TypeCode == XmlTypeCode.QName || this.xmlType.TypeCode == XmlTypeCode.Notation))
			{
				string @namespace = (this.objVal as XmlQualifiedName).Namespace;
				this.nsPrefix = new XmlAtomicValue.NamespacePrefixForQName(nsResolver.LookupPrefix(@namespace), @namespace);
			}
		}

		public XmlAtomicValue Clone()
		{
			return this;
		}

		object ICloneable.Clone()
		{
			return this;
		}

		public override bool IsNode
		{
			get
			{
				return false;
			}
		}

		public override XmlSchemaType XmlType
		{
			get
			{
				return this.xmlType;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this.xmlType.Datatype.ValueType;
			}
		}

		public override object TypedValue
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ChangeType(this.unionVal.boolVal, this.ValueType);
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ChangeType(this.unionVal.i32Val, this.ValueType);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ChangeType(this.unionVal.i64Val, this.ValueType);
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ChangeType(this.unionVal.dblVal, this.ValueType);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ChangeType(this.unionVal.dtVal, this.ValueType);
						}
					}
				}
				return valueConverter.ChangeType(this.objVal, this.ValueType, this.nsPrefix);
			}
		}

		public override bool ValueAsBoolean
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return this.unionVal.boolVal;
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ToBoolean(this.unionVal.i32Val);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ToBoolean(this.unionVal.i64Val);
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ToBoolean(this.unionVal.dblVal);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ToBoolean(this.unionVal.dtVal);
						}
					}
				}
				return valueConverter.ToBoolean(this.objVal);
			}
		}

		public override DateTime ValueAsDateTime
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ToDateTime(this.unionVal.boolVal);
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ToDateTime(this.unionVal.i32Val);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ToDateTime(this.unionVal.i64Val);
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ToDateTime(this.unionVal.dblVal);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return this.unionVal.dtVal;
						}
					}
				}
				return valueConverter.ToDateTime(this.objVal);
			}
		}

		public override double ValueAsDouble
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ToDouble(this.unionVal.boolVal);
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ToDouble(this.unionVal.i32Val);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ToDouble(this.unionVal.i64Val);
						}
						if (typeCode == TypeCode.Double)
						{
							return this.unionVal.dblVal;
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ToDouble(this.unionVal.dtVal);
						}
					}
				}
				return valueConverter.ToDouble(this.objVal);
			}
		}

		public override int ValueAsInt
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ToInt32(this.unionVal.boolVal);
						}
						if (typeCode == TypeCode.Int32)
						{
							return this.unionVal.i32Val;
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ToInt32(this.unionVal.i64Val);
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ToInt32(this.unionVal.dblVal);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ToInt32(this.unionVal.dtVal);
						}
					}
				}
				return valueConverter.ToInt32(this.objVal);
			}
		}

		public override long ValueAsLong
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ToInt64(this.unionVal.boolVal);
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ToInt64(this.unionVal.i32Val);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return this.unionVal.i64Val;
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ToInt64(this.unionVal.dblVal);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ToInt64(this.unionVal.dtVal);
						}
					}
				}
				return valueConverter.ToInt64(this.objVal);
			}
		}

		public override object ValueAs(Type type, IXmlNamespaceResolver nsResolver)
		{
			XmlValueConverter valueConverter = this.xmlType.ValueConverter;
			if (type == typeof(XPathItem) || type == typeof(XmlAtomicValue))
			{
				return this;
			}
			if (this.objVal == null)
			{
				TypeCode typeCode = this.clrType;
				if (typeCode <= TypeCode.Int32)
				{
					if (typeCode == TypeCode.Boolean)
					{
						return valueConverter.ChangeType(this.unionVal.boolVal, type);
					}
					if (typeCode == TypeCode.Int32)
					{
						return valueConverter.ChangeType(this.unionVal.i32Val, type);
					}
				}
				else
				{
					if (typeCode == TypeCode.Int64)
					{
						return valueConverter.ChangeType(this.unionVal.i64Val, type);
					}
					if (typeCode == TypeCode.Double)
					{
						return valueConverter.ChangeType(this.unionVal.dblVal, type);
					}
					if (typeCode == TypeCode.DateTime)
					{
						return valueConverter.ChangeType(this.unionVal.dtVal, type);
					}
				}
			}
			return valueConverter.ChangeType(this.objVal, type, nsResolver);
		}

		public override string Value
		{
			get
			{
				XmlValueConverter valueConverter = this.xmlType.ValueConverter;
				if (this.objVal == null)
				{
					TypeCode typeCode = this.clrType;
					if (typeCode <= TypeCode.Int32)
					{
						if (typeCode == TypeCode.Boolean)
						{
							return valueConverter.ToString(this.unionVal.boolVal);
						}
						if (typeCode == TypeCode.Int32)
						{
							return valueConverter.ToString(this.unionVal.i32Val);
						}
					}
					else
					{
						if (typeCode == TypeCode.Int64)
						{
							return valueConverter.ToString(this.unionVal.i64Val);
						}
						if (typeCode == TypeCode.Double)
						{
							return valueConverter.ToString(this.unionVal.dblVal);
						}
						if (typeCode == TypeCode.DateTime)
						{
							return valueConverter.ToString(this.unionVal.dtVal);
						}
					}
				}
				return valueConverter.ToString(this.objVal, this.nsPrefix);
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		private string GetPrefixFromQName(string value)
		{
			int num2;
			int num = ValidateNames.ParseQName(value, 0, out num2);
			if (num == 0 || num != value.Length)
			{
				return null;
			}
			if (num2 != 0)
			{
				return value.Substring(0, num2);
			}
			return string.Empty;
		}

		internal XmlAtomicValue()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private XmlSchemaType xmlType;

		private object objVal;

		private TypeCode clrType;

		private XmlAtomicValue.Union unionVal;

		private XmlAtomicValue.NamespacePrefixForQName nsPrefix;

		[StructLayout(LayoutKind.Explicit, Size = 8)]
		private struct Union
		{
			[FieldOffset(0)]
			public bool boolVal;

			[FieldOffset(0)]
			public double dblVal;

			[FieldOffset(0)]
			public long i64Val;

			[FieldOffset(0)]
			public int i32Val;

			[FieldOffset(0)]
			public DateTime dtVal;
		}

		private class NamespacePrefixForQName : IXmlNamespaceResolver
		{
			public NamespacePrefixForQName(string prefix, string ns)
			{
				this.ns = ns;
				this.prefix = prefix;
			}

			public string LookupNamespace(string prefix)
			{
				if (prefix == this.prefix)
				{
					return this.ns;
				}
				return null;
			}

			public string LookupPrefix(string namespaceName)
			{
				if (this.ns == namespaceName)
				{
					return this.prefix;
				}
				return null;
			}

			public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(1);
				dictionary[this.prefix] = this.ns;
				return dictionary;
			}

			public string prefix;

			public string ns;
		}
	}
}
