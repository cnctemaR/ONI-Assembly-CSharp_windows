using System;
using System.Xml.XPath;

namespace System.Xml.Schema
{
	internal class XmlAnyConverter : XmlBaseConverter
	{
		protected XmlAnyConverter(XmlTypeCode typeCode)
			: base(typeCode)
		{
		}

		public override bool ToBoolean(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsBoolean;
			}
			return (bool)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.BooleanType, null);
		}

		public override DateTime ToDateTime(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsDateTime;
			}
			return (DateTime)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return (DateTimeOffset)((XmlAtomicValue)value).ValueAs(XmlBaseConverter.DateTimeOffsetType);
			}
			return (DateTimeOffset)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override decimal ToDecimal(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return (decimal)((XmlAtomicValue)value).ValueAs(XmlBaseConverter.DecimalType);
			}
			return (decimal)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DecimalType, null);
		}

		public override double ToDouble(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsDouble;
			}
			return (double)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.DoubleType, null);
		}

		public override int ToInt32(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsInt;
			}
			return (int)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.Int32Type, null);
		}

		public override long ToInt64(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsLong;
			}
			return (long)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.Int64Type, null);
		}

		public override float ToSingle(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return (float)((XmlAtomicValue)value).ValueAs(XmlBaseConverter.SingleType);
			}
			return (float)this.ChangeTypeWildcardDestination(value, XmlBaseConverter.SingleType, null);
		}

		public override object ChangeType(bool value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(DateTime value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(DateTimeOffset value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(decimal value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Decimal), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(double value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(int value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(long value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(float value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Float), (double)value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, null);
		}

		public override object ChangeType(string value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), value);
			}
			return this.ChangeTypeWildcardSource(value, destinationType, nsResolver);
		}

		public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			Type type = value.GetType();
			if (destinationType == XmlBaseConverter.ObjectType)
			{
				destinationType = base.DefaultClrType;
			}
			if (destinationType == XmlBaseConverter.BooleanType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsBoolean;
			}
			if (destinationType == XmlBaseConverter.DateTimeType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsDateTime;
			}
			if (destinationType == XmlBaseConverter.DateTimeOffsetType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAs(XmlBaseConverter.DateTimeOffsetType);
			}
			if (destinationType == XmlBaseConverter.DecimalType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return (decimal)((XmlAtomicValue)value).ValueAs(XmlBaseConverter.DecimalType);
			}
			if (destinationType == XmlBaseConverter.DoubleType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsDouble;
			}
			if (destinationType == XmlBaseConverter.Int32Type && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsInt;
			}
			if (destinationType == XmlBaseConverter.Int64Type && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAsLong;
			}
			if (destinationType == XmlBaseConverter.SingleType && type == XmlBaseConverter.XmlAtomicValueType)
			{
				return (float)((XmlAtomicValue)value).ValueAs(XmlBaseConverter.SingleType);
			}
			if (destinationType == XmlBaseConverter.XmlAtomicValueType)
			{
				if (type == XmlBaseConverter.XmlAtomicValueType)
				{
					return (XmlAtomicValue)value;
				}
				if (type == XmlBaseConverter.BooleanType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), (bool)value);
				}
				if (type == XmlBaseConverter.ByteType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedByte), value);
				}
				if (type == XmlBaseConverter.ByteArrayType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Base64Binary), value);
				}
				if (type == XmlBaseConverter.DateTimeType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), (DateTime)value);
				}
				if (type == XmlBaseConverter.DateTimeOffsetType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime), (DateTimeOffset)value);
				}
				if (type == XmlBaseConverter.DecimalType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Decimal), value);
				}
				if (type == XmlBaseConverter.DoubleType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), (double)value);
				}
				if (type == XmlBaseConverter.Int16Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Short), value);
				}
				if (type == XmlBaseConverter.Int32Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int), (int)value);
				}
				if (type == XmlBaseConverter.Int64Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long), (long)value);
				}
				if (type == XmlBaseConverter.SByteType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Byte), value);
				}
				if (type == XmlBaseConverter.SingleType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Float), value);
				}
				if (type == XmlBaseConverter.StringType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), (string)value);
				}
				if (type == XmlBaseConverter.TimeSpanType)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Duration), value);
				}
				if (type == XmlBaseConverter.UInt16Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedShort), value);
				}
				if (type == XmlBaseConverter.UInt32Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt), value);
				}
				if (type == XmlBaseConverter.UInt64Type)
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedLong), value);
				}
				if (XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.UriType))
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyUri), value);
				}
				if (XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.XmlQualifiedNameType))
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName), value, nsResolver);
				}
			}
			if (destinationType == XmlBaseConverter.XPathItemType)
			{
				if (type == XmlBaseConverter.XmlAtomicValueType)
				{
					return (XmlAtomicValue)value;
				}
				if (XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.XPathNavigatorType))
				{
					return (XPathNavigator)value;
				}
			}
			if (destinationType == XmlBaseConverter.XPathNavigatorType && XmlBaseConverter.IsDerivedFrom(type, XmlBaseConverter.XPathNavigatorType))
			{
				return this.ToNavigator((XPathNavigator)value);
			}
			if (destinationType == XmlBaseConverter.XPathItemType)
			{
				return (XPathItem)this.ChangeType(value, XmlBaseConverter.XmlAtomicValueType, nsResolver);
			}
			if (type == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAs(destinationType, nsResolver);
			}
			return this.ChangeListType(value, destinationType, nsResolver);
		}

		private object ChangeTypeWildcardDestination(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			if (value.GetType() == XmlBaseConverter.XmlAtomicValueType)
			{
				return ((XmlAtomicValue)value).ValueAs(destinationType, nsResolver);
			}
			return this.ChangeListType(value, destinationType, nsResolver);
		}

		private object ChangeTypeWildcardSource(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			if (destinationType == XmlBaseConverter.XPathItemType)
			{
				return (XPathItem)this.ChangeType(value, XmlBaseConverter.XmlAtomicValueType, nsResolver);
			}
			return this.ChangeListType(value, destinationType, nsResolver);
		}

		private XPathNavigator ToNavigator(XPathNavigator nav)
		{
			if (base.TypeCode != XmlTypeCode.Item)
			{
				throw base.CreateInvalidClrMappingException(XmlBaseConverter.XPathNavigatorType, XmlBaseConverter.XPathNavigatorType);
			}
			return nav;
		}

		public static readonly XmlValueConverter Item = new XmlAnyConverter(XmlTypeCode.Item);

		public static readonly XmlValueConverter AnyAtomic = new XmlAnyConverter(XmlTypeCode.AnyAtomicType);
	}
}
