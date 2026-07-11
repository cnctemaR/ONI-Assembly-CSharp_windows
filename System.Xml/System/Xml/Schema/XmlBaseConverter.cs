using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Schema
{
	internal abstract class XmlBaseConverter : XmlValueConverter
	{
		protected XmlBaseConverter(XmlSchemaType schemaType)
		{
			XmlSchemaDatatype datatype = schemaType.Datatype;
			while (schemaType != null && !(schemaType is XmlSchemaSimpleType))
			{
				schemaType = schemaType.BaseXmlSchemaType;
			}
			if (schemaType == null)
			{
				schemaType = XmlSchemaType.GetBuiltInSimpleType(datatype.TypeCode);
			}
			this.schemaType = schemaType;
			this.typeCode = schemaType.TypeCode;
			this.clrTypeDefault = schemaType.Datatype.ValueType;
		}

		protected XmlBaseConverter(XmlTypeCode typeCode)
		{
			if (typeCode != XmlTypeCode.Item)
			{
				if (typeCode != XmlTypeCode.Node)
				{
					if (typeCode == XmlTypeCode.AnyAtomicType)
					{
						this.clrTypeDefault = XmlBaseConverter.XmlAtomicValueType;
					}
				}
				else
				{
					this.clrTypeDefault = XmlBaseConverter.XPathNavigatorType;
				}
			}
			else
			{
				this.clrTypeDefault = XmlBaseConverter.XPathItemType;
			}
			this.typeCode = typeCode;
		}

		protected XmlBaseConverter(XmlBaseConverter converterAtomic)
		{
			this.schemaType = converterAtomic.schemaType;
			this.typeCode = converterAtomic.typeCode;
			this.clrTypeDefault = Array.CreateInstance(converterAtomic.DefaultClrType, 0).GetType();
		}

		protected XmlBaseConverter(XmlBaseConverter converterAtomic, Type clrTypeDefault)
		{
			this.schemaType = converterAtomic.schemaType;
			this.typeCode = converterAtomic.typeCode;
			this.clrTypeDefault = clrTypeDefault;
		}

		public override bool ToBoolean(bool value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(DateTime value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(DateTimeOffset value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(decimal value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(double value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(int value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(long value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(float value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(string value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override bool ToBoolean(object value)
		{
			return (bool)this.ChangeType(value, XmlBaseConverter.BooleanType, null);
		}

		public override DateTime ToDateTime(bool value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(DateTime value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(DateTimeOffset value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(decimal value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(double value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(int value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(long value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(float value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(string value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTime ToDateTime(object value)
		{
			return (DateTime)this.ChangeType(value, XmlBaseConverter.DateTimeType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(bool value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(DateTime value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(DateTimeOffset value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(decimal value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(double value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(int value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(long value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(float value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(string value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override DateTimeOffset ToDateTimeOffset(object value)
		{
			return (DateTimeOffset)this.ChangeType(value, XmlBaseConverter.DateTimeOffsetType, null);
		}

		public override decimal ToDecimal(bool value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(DateTime value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(DateTimeOffset value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(decimal value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(double value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(int value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(long value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(float value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(string value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override decimal ToDecimal(object value)
		{
			return (decimal)this.ChangeType(value, XmlBaseConverter.DecimalType, null);
		}

		public override double ToDouble(bool value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(DateTime value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(DateTimeOffset value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(decimal value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(double value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(int value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(long value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(float value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(string value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override double ToDouble(object value)
		{
			return (double)this.ChangeType(value, XmlBaseConverter.DoubleType, null);
		}

		public override int ToInt32(bool value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(DateTime value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(DateTimeOffset value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(decimal value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(double value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(int value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(long value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(float value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(string value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override int ToInt32(object value)
		{
			return (int)this.ChangeType(value, XmlBaseConverter.Int32Type, null);
		}

		public override long ToInt64(bool value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(DateTime value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(DateTimeOffset value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(decimal value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(double value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(int value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(long value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(float value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(string value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override long ToInt64(object value)
		{
			return (long)this.ChangeType(value, XmlBaseConverter.Int64Type, null);
		}

		public override float ToSingle(bool value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(DateTime value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(DateTimeOffset value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(decimal value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(double value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(int value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(long value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(float value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(string value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override float ToSingle(object value)
		{
			return (float)this.ChangeType(value, XmlBaseConverter.SingleType, null);
		}

		public override string ToString(bool value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(DateTime value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(DateTimeOffset value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(decimal value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(double value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(int value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(long value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(float value)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, null);
		}

		public override string ToString(string value, IXmlNamespaceResolver nsResolver)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, nsResolver);
		}

		public override string ToString(object value, IXmlNamespaceResolver nsResolver)
		{
			return (string)this.ChangeType(value, XmlBaseConverter.StringType, nsResolver);
		}

		public override string ToString(string value)
		{
			return this.ToString(value, null);
		}

		public override string ToString(object value)
		{
			return this.ToString(value, null);
		}

		public override object ChangeType(bool value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(DateTime value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(DateTimeOffset value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(decimal value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(double value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(int value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(long value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(float value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(string value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			return this.ChangeType(value, destinationType, nsResolver);
		}

		public override object ChangeType(string value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		public override object ChangeType(object value, Type destinationType)
		{
			return this.ChangeType(value, destinationType, null);
		}

		protected XmlSchemaType SchemaType
		{
			get
			{
				return this.schemaType;
			}
		}

		protected XmlTypeCode TypeCode
		{
			get
			{
				return this.typeCode;
			}
		}

		protected string XmlTypeName
		{
			get
			{
				XmlSchemaType baseXmlSchemaType = this.schemaType;
				if (baseXmlSchemaType != null)
				{
					while (baseXmlSchemaType.QualifiedName.IsEmpty)
					{
						baseXmlSchemaType = baseXmlSchemaType.BaseXmlSchemaType;
					}
					return XmlBaseConverter.QNameToString(baseXmlSchemaType.QualifiedName);
				}
				if (this.typeCode == XmlTypeCode.Node)
				{
					return "node";
				}
				if (this.typeCode == XmlTypeCode.AnyAtomicType)
				{
					return "xdt:anyAtomicType";
				}
				return "item";
			}
		}

		protected Type DefaultClrType
		{
			get
			{
				return this.clrTypeDefault;
			}
		}

		protected static bool IsDerivedFrom(Type derivedType, Type baseType)
		{
			while (derivedType != null)
			{
				if (derivedType == baseType)
				{
					return true;
				}
				derivedType = derivedType.BaseType;
			}
			return false;
		}

		protected Exception CreateInvalidClrMappingException(Type sourceType, Type destinationType)
		{
			if (sourceType == destinationType)
			{
				return new InvalidCastException(Res.GetString("Xml type '{0}' does not support Clr type '{1}'.", new object[] { this.XmlTypeName, sourceType.Name }));
			}
			return new InvalidCastException(Res.GetString("Xml type '{0}' does not support a conversion from Clr type '{1}' to Clr type '{2}'.", new object[] { this.XmlTypeName, sourceType.Name, destinationType.Name }));
		}

		protected static string QNameToString(XmlQualifiedName name)
		{
			if (name.Namespace.Length == 0)
			{
				return name.Name;
			}
			if (name.Namespace == "http://www.w3.org/2001/XMLSchema")
			{
				return "xs:" + name.Name;
			}
			if (name.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
			{
				return "xdt:" + name.Name;
			}
			return "{" + name.Namespace + "}" + name.Name;
		}

		protected virtual object ChangeListType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
		{
			throw this.CreateInvalidClrMappingException(value.GetType(), destinationType);
		}

		protected static byte[] StringToBase64Binary(string value)
		{
			return Convert.FromBase64String(XmlConvert.TrimString(value));
		}

		protected static DateTime StringToDate(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Date);
		}

		protected static DateTime StringToDateTime(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.DateTime);
		}

		protected static TimeSpan StringToDayTimeDuration(string value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.DayTimeDuration).ToTimeSpan(XsdDuration.DurationType.DayTimeDuration);
		}

		protected static TimeSpan StringToDuration(string value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.Duration).ToTimeSpan(XsdDuration.DurationType.Duration);
		}

		protected static DateTime StringToGDay(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GDay);
		}

		protected static DateTime StringToGMonth(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonth);
		}

		protected static DateTime StringToGMonthDay(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonthDay);
		}

		protected static DateTime StringToGYear(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYear);
		}

		protected static DateTime StringToGYearMonth(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYearMonth);
		}

		protected static DateTimeOffset StringToDateOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Date);
		}

		protected static DateTimeOffset StringToDateTimeOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.DateTime);
		}

		protected static DateTimeOffset StringToGDayOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GDay);
		}

		protected static DateTimeOffset StringToGMonthOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonth);
		}

		protected static DateTimeOffset StringToGMonthDayOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonthDay);
		}

		protected static DateTimeOffset StringToGYearOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYear);
		}

		protected static DateTimeOffset StringToGYearMonthOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYearMonth);
		}

		protected static byte[] StringToHexBinary(string value)
		{
			byte[] array;
			try
			{
				array = XmlConvert.FromBinHexString(XmlConvert.TrimString(value), false);
			}
			catch (XmlException ex)
			{
				throw new FormatException(ex.Message);
			}
			return array;
		}

		protected static XmlQualifiedName StringToQName(string value, IXmlNamespaceResolver nsResolver)
		{
			value = value.Trim();
			string text;
			string text2;
			try
			{
				ValidateNames.ParseQNameThrow(value, out text, out text2);
			}
			catch (XmlException ex)
			{
				throw new FormatException(ex.Message);
			}
			if (nsResolver == null)
			{
				throw new InvalidCastException(Res.GetString("The String '{0}' cannot be represented as an XmlQualifiedName.  A namespace for prefix '{1}' cannot be found.", new object[] { value, text }));
			}
			string text3 = nsResolver.LookupNamespace(text);
			if (text3 == null)
			{
				throw new InvalidCastException(Res.GetString("The String '{0}' cannot be represented as an XmlQualifiedName.  A namespace for prefix '{1}' cannot be found.", new object[] { value, text }));
			}
			return new XmlQualifiedName(text2, text3);
		}

		protected static DateTime StringToTime(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Time);
		}

		protected static DateTimeOffset StringToTimeOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Time);
		}

		protected static TimeSpan StringToYearMonthDuration(string value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.YearMonthDuration).ToTimeSpan(XsdDuration.DurationType.YearMonthDuration);
		}

		protected static string AnyUriToString(Uri value)
		{
			return value.OriginalString;
		}

		protected static string Base64BinaryToString(byte[] value)
		{
			return Convert.ToBase64String(value);
		}

		protected static string DateToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Date).ToString();
		}

		protected static string DateTimeToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.DateTime).ToString();
		}

		protected static string DayTimeDurationToString(TimeSpan value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.DayTimeDuration).ToString(XsdDuration.DurationType.DayTimeDuration);
		}

		protected static string DurationToString(TimeSpan value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.Duration).ToString(XsdDuration.DurationType.Duration);
		}

		protected static string GDayToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GDay).ToString();
		}

		protected static string GMonthToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonth).ToString();
		}

		protected static string GMonthDayToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonthDay).ToString();
		}

		protected static string GYearToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYear).ToString();
		}

		protected static string GYearMonthToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYearMonth).ToString();
		}

		protected static string DateOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Date).ToString();
		}

		protected static string DateTimeOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.DateTime).ToString();
		}

		protected static string GDayOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GDay).ToString();
		}

		protected static string GMonthOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonth).ToString();
		}

		protected static string GMonthDayOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GMonthDay).ToString();
		}

		protected static string GYearOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYear).ToString();
		}

		protected static string GYearMonthOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.GYearMonth).ToString();
		}

		protected static string QNameToString(XmlQualifiedName qname, IXmlNamespaceResolver nsResolver)
		{
			if (nsResolver == null)
			{
				return "{" + qname.Namespace + "}" + qname.Name;
			}
			string text = nsResolver.LookupPrefix(qname.Namespace);
			if (text == null)
			{
				throw new InvalidCastException(Res.GetString("The QName '{0}' cannot be represented as a String.  A prefix for namespace '{1}' cannot be found.", new object[]
				{
					qname.ToString(),
					qname.Namespace
				}));
			}
			if (text.Length == 0)
			{
				return qname.Name;
			}
			return text + ":" + qname.Name;
		}

		protected static string TimeToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Time).ToString();
		}

		protected static string TimeOffsetToString(DateTimeOffset value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.Time).ToString();
		}

		protected static string YearMonthDurationToString(TimeSpan value)
		{
			return new XsdDuration(value, XsdDuration.DurationType.YearMonthDuration).ToString(XsdDuration.DurationType.YearMonthDuration);
		}

		internal static DateTime DateTimeOffsetToDateTime(DateTimeOffset value)
		{
			return value.LocalDateTime;
		}

		internal static int DecimalToInt32(decimal value)
		{
			if (value < -2147483648m || value > 2147483647m)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"Int32"
				}));
			}
			return (int)value;
		}

		protected static long DecimalToInt64(decimal value)
		{
			if (value < -9223372036854775808m || value > 9223372036854775807m)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"Int64"
				}));
			}
			return (long)value;
		}

		protected static ulong DecimalToUInt64(decimal value)
		{
			if (value < 0m || value > 18446744073709551615m)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"UInt64"
				}));
			}
			return (ulong)value;
		}

		protected static byte Int32ToByte(int value)
		{
			if (value < 0 || value > 255)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"Byte"
				}));
			}
			return (byte)value;
		}

		protected static short Int32ToInt16(int value)
		{
			if (value < -32768 || value > 32767)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"Int16"
				}));
			}
			return (short)value;
		}

		protected static sbyte Int32ToSByte(int value)
		{
			if (value < -128 || value > 127)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"SByte"
				}));
			}
			return (sbyte)value;
		}

		protected static ushort Int32ToUInt16(int value)
		{
			if (value < 0 || value > 65535)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"UInt16"
				}));
			}
			return (ushort)value;
		}

		protected static int Int64ToInt32(long value)
		{
			if (value < -2147483648L || value > 2147483647L)
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"Int32"
				}));
			}
			return (int)value;
		}

		protected static uint Int64ToUInt32(long value)
		{
			if (value < 0L || value > (long)((ulong)(-1)))
			{
				throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new string[]
				{
					XmlConvert.ToString(value),
					"UInt32"
				}));
			}
			return (uint)value;
		}

		protected static DateTime UntypedAtomicToDateTime(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.AllXsd);
		}

		protected static DateTimeOffset UntypedAtomicToDateTimeOffset(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.AllXsd);
		}

		private XmlSchemaType schemaType;

		private XmlTypeCode typeCode;

		private Type clrTypeDefault;

		protected static readonly Type ICollectionType = typeof(ICollection);

		protected static readonly Type IEnumerableType = typeof(IEnumerable);

		protected static readonly Type IListType = typeof(IList);

		protected static readonly Type ObjectArrayType = typeof(object[]);

		protected static readonly Type StringArrayType = typeof(string[]);

		protected static readonly Type XmlAtomicValueArrayType = typeof(XmlAtomicValue[]);

		protected static readonly Type DecimalType = typeof(decimal);

		protected static readonly Type Int32Type = typeof(int);

		protected static readonly Type Int64Type = typeof(long);

		protected static readonly Type StringType = typeof(string);

		protected static readonly Type XmlAtomicValueType = typeof(XmlAtomicValue);

		protected static readonly Type ObjectType = typeof(object);

		protected static readonly Type ByteType = typeof(byte);

		protected static readonly Type Int16Type = typeof(short);

		protected static readonly Type SByteType = typeof(sbyte);

		protected static readonly Type UInt16Type = typeof(ushort);

		protected static readonly Type UInt32Type = typeof(uint);

		protected static readonly Type UInt64Type = typeof(ulong);

		protected static readonly Type XPathItemType = typeof(XPathItem);

		protected static readonly Type DoubleType = typeof(double);

		protected static readonly Type SingleType = typeof(float);

		protected static readonly Type DateTimeType = typeof(DateTime);

		protected static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);

		protected static readonly Type BooleanType = typeof(bool);

		protected static readonly Type ByteArrayType = typeof(byte[]);

		protected static readonly Type XmlQualifiedNameType = typeof(XmlQualifiedName);

		protected static readonly Type UriType = typeof(Uri);

		protected static readonly Type TimeSpanType = typeof(TimeSpan);

		protected static readonly Type XPathNavigatorType = typeof(XPathNavigator);
	}
}
