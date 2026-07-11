using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class XsltConvert
	{
		public static bool ToBoolean(XPathItem item)
		{
			if (item.IsNode)
			{
				return true;
			}
			Type valueType = item.ValueType;
			if (valueType == XsltConvert.StringType)
			{
				return item.Value.Length != 0;
			}
			if (valueType == XsltConvert.DoubleType)
			{
				double valueAsDouble = item.ValueAsDouble;
				return valueAsDouble < 0.0 || 0.0 < valueAsDouble;
			}
			return item.ValueAsBoolean;
		}

		public static bool ToBoolean(IList<XPathItem> listItems)
		{
			return listItems.Count != 0 && XsltConvert.ToBoolean(listItems[0]);
		}

		public static double ToDouble(string value)
		{
			return XPathConvert.StringToDouble(value);
		}

		public static double ToDouble(XPathItem item)
		{
			if (item.IsNode)
			{
				return XPathConvert.StringToDouble(item.Value);
			}
			Type valueType = item.ValueType;
			if (valueType == XsltConvert.StringType)
			{
				return XPathConvert.StringToDouble(item.Value);
			}
			if (valueType == XsltConvert.DoubleType)
			{
				return item.ValueAsDouble;
			}
			if (!item.ValueAsBoolean)
			{
				return 0.0;
			}
			return 1.0;
		}

		public static double ToDouble(IList<XPathItem> listItems)
		{
			if (listItems.Count == 0)
			{
				return double.NaN;
			}
			return XsltConvert.ToDouble(listItems[0]);
		}

		public static XPathNavigator ToNode(XPathItem item)
		{
			if (!item.IsNode)
			{
				XPathDocument xpathDocument = new XPathDocument();
				XmlRawWriter xmlRawWriter = xpathDocument.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, string.Empty);
				xmlRawWriter.WriteString(XsltConvert.ToString(item));
				xmlRawWriter.Close();
				return xpathDocument.CreateNavigator();
			}
			RtfNavigator rtfNavigator = item as RtfNavigator;
			if (rtfNavigator != null)
			{
				return rtfNavigator.ToNavigator();
			}
			return (XPathNavigator)item;
		}

		public static XPathNavigator ToNode(IList<XPathItem> listItems)
		{
			if (listItems.Count == 1)
			{
				return XsltConvert.ToNode(listItems[0]);
			}
			throw new XslTransformException("Cannot convert a node-set which contains zero nodes or more than one node to a single node.", new string[] { string.Empty });
		}

		public static IList<XPathNavigator> ToNodeSet(XPathItem item)
		{
			return new XmlQueryNodeSequence(XsltConvert.ToNode(item));
		}

		public static IList<XPathNavigator> ToNodeSet(IList<XPathItem> listItems)
		{
			if (listItems.Count == 1)
			{
				return new XmlQueryNodeSequence(XsltConvert.ToNode(listItems[0]));
			}
			return XmlILStorageConverter.ItemsToNavigators(listItems);
		}

		public static string ToString(double value)
		{
			return XPathConvert.DoubleToString(value);
		}

		public static string ToString(XPathItem item)
		{
			if (!item.IsNode && item.ValueType == XsltConvert.DoubleType)
			{
				return XPathConvert.DoubleToString(item.ValueAsDouble);
			}
			return item.Value;
		}

		public static string ToString(IList<XPathItem> listItems)
		{
			if (listItems.Count == 0)
			{
				return string.Empty;
			}
			return XsltConvert.ToString(listItems[0]);
		}

		public static string ToString(DateTime value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.DateTime).ToString();
		}

		public static double ToDouble(decimal value)
		{
			return (double)value;
		}

		public static double ToDouble(int value)
		{
			return (double)value;
		}

		public static double ToDouble(long value)
		{
			return (double)value;
		}

		public static decimal ToDecimal(double value)
		{
			return (decimal)value;
		}

		public static int ToInt(double value)
		{
			return checked((int)value);
		}

		public static long ToLong(double value)
		{
			return checked((long)value);
		}

		public static DateTime ToDateTime(string value)
		{
			return new XsdDateTime(value, XsdDateTimeFlags.AllXsd);
		}

		internal static XmlAtomicValue ConvertToType(XmlAtomicValue value, XmlQueryType destinationType)
		{
			XmlTypeCode xmlTypeCode = destinationType.TypeCode;
			switch (xmlTypeCode)
			{
			case XmlTypeCode.String:
				switch (value.XmlType.TypeCode)
				{
				case XmlTypeCode.String:
				case XmlTypeCode.Boolean:
				case XmlTypeCode.Double:
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToString(value));
				case XmlTypeCode.DateTime:
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToString(value.ValueAsDateTime));
				}
				break;
			case XmlTypeCode.Boolean:
				xmlTypeCode = value.XmlType.TypeCode;
				if (xmlTypeCode - XmlTypeCode.String <= 1 || xmlTypeCode == XmlTypeCode.Double)
				{
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToBoolean(value));
				}
				break;
			case XmlTypeCode.Decimal:
				if (value.XmlType.TypeCode == XmlTypeCode.Double)
				{
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToDecimal(value.ValueAsDouble));
				}
				break;
			case XmlTypeCode.Float:
			case XmlTypeCode.Duration:
				break;
			case XmlTypeCode.Double:
				xmlTypeCode = value.XmlType.TypeCode;
				switch (xmlTypeCode)
				{
				case XmlTypeCode.String:
				case XmlTypeCode.Boolean:
				case XmlTypeCode.Double:
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToDouble(value));
				case XmlTypeCode.Decimal:
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToDouble((decimal)value.ValueAs(XsltConvert.DecimalType, null)));
				case XmlTypeCode.Float:
					break;
				default:
					if (xmlTypeCode - XmlTypeCode.Long <= 1)
					{
						return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToDouble(value.ValueAsLong));
					}
					break;
				}
				break;
			case XmlTypeCode.DateTime:
				if (value.XmlType.TypeCode == XmlTypeCode.String)
				{
					return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToDateTime(value.Value));
				}
				break;
			default:
				if (xmlTypeCode - XmlTypeCode.Long <= 1)
				{
					if (value.XmlType.TypeCode == XmlTypeCode.Double)
					{
						return new XmlAtomicValue(destinationType.SchemaType, XsltConvert.ToLong(value.ValueAsDouble));
					}
				}
				break;
			}
			return value;
		}

		public static IList<XPathNavigator> EnsureNodeSet(IList<XPathItem> listItems)
		{
			if (listItems.Count == 1)
			{
				XPathItem xpathItem = listItems[0];
				if (!xpathItem.IsNode)
				{
					throw new XslTransformException("Expression must evaluate to a node-set.", new string[] { string.Empty });
				}
				if (xpathItem is RtfNavigator)
				{
					throw new XslTransformException("To use a result tree fragment in a path expression, first convert it to a node-set using the msxsl:node-set() function.", new string[] { string.Empty });
				}
			}
			return XmlILStorageConverter.ItemsToNavigators(listItems);
		}

		internal static XmlQueryType InferXsltType(Type clrType)
		{
			if (clrType == XsltConvert.BooleanType)
			{
				return XmlQueryTypeFactory.BooleanX;
			}
			if (clrType == XsltConvert.ByteType)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.DecimalType)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.DateTimeType)
			{
				return XmlQueryTypeFactory.StringX;
			}
			if (clrType == XsltConvert.DoubleType)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.Int16Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.Int32Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.Int64Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.IXPathNavigableType)
			{
				return XmlQueryTypeFactory.NodeNotRtf;
			}
			if (clrType == XsltConvert.SByteType)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.SingleType)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.StringType)
			{
				return XmlQueryTypeFactory.StringX;
			}
			if (clrType == XsltConvert.UInt16Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.UInt32Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.UInt64Type)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.XPathNavigatorArrayType)
			{
				return XmlQueryTypeFactory.NodeSDod;
			}
			if (clrType == XsltConvert.XPathNavigatorType)
			{
				return XmlQueryTypeFactory.NodeNotRtf;
			}
			if (clrType == XsltConvert.XPathNodeIteratorType)
			{
				return XmlQueryTypeFactory.NodeSDod;
			}
			if (clrType.IsEnum)
			{
				return XmlQueryTypeFactory.DoubleX;
			}
			if (clrType == XsltConvert.VoidType)
			{
				return XmlQueryTypeFactory.Empty;
			}
			return XmlQueryTypeFactory.ItemS;
		}

		internal static readonly Type BooleanType = typeof(bool);

		internal static readonly Type ByteArrayType = typeof(byte[]);

		internal static readonly Type ByteType = typeof(byte);

		internal static readonly Type DateTimeType = typeof(DateTime);

		internal static readonly Type DecimalType = typeof(decimal);

		internal static readonly Type DoubleType = typeof(double);

		internal static readonly Type ICollectionType = typeof(ICollection);

		internal static readonly Type IEnumerableType = typeof(IEnumerable);

		internal static readonly Type IListType = typeof(IList);

		internal static readonly Type Int16Type = typeof(short);

		internal static readonly Type Int32Type = typeof(int);

		internal static readonly Type Int64Type = typeof(long);

		internal static readonly Type IXPathNavigableType = typeof(IXPathNavigable);

		internal static readonly Type ObjectType = typeof(object);

		internal static readonly Type SByteType = typeof(sbyte);

		internal static readonly Type SingleType = typeof(float);

		internal static readonly Type StringType = typeof(string);

		internal static readonly Type TimeSpanType = typeof(TimeSpan);

		internal static readonly Type UInt16Type = typeof(ushort);

		internal static readonly Type UInt32Type = typeof(uint);

		internal static readonly Type UInt64Type = typeof(ulong);

		internal static readonly Type UriType = typeof(Uri);

		internal static readonly Type VoidType = typeof(void);

		internal static readonly Type XmlAtomicValueType = typeof(XmlAtomicValue);

		internal static readonly Type XmlQualifiedNameType = typeof(XmlQualifiedName);

		internal static readonly Type XPathItemType = typeof(XPathItem);

		internal static readonly Type XPathNavigatorArrayType = typeof(XPathNavigator[]);

		internal static readonly Type XPathNavigatorType = typeof(XPathNavigator);

		internal static readonly Type XPathNodeIteratorType = typeof(XPathNodeIterator);
	}
}
