using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILStorageMethods
	{
		public XmlILStorageMethods(Type storageType)
		{
			if (storageType == typeof(int) || storageType == typeof(long) || storageType == typeof(decimal) || storageType == typeof(double))
			{
				Type type = Type.GetType("System.Xml.Xsl.Runtime." + storageType.Name + "Aggregator");
				this.AggAvg = XmlILMethods.GetMethod(type, "Average");
				this.AggAvgResult = XmlILMethods.GetMethod(type, "get_AverageResult");
				this.AggCreate = XmlILMethods.GetMethod(type, "Create");
				this.AggIsEmpty = XmlILMethods.GetMethod(type, "get_IsEmpty");
				this.AggMax = XmlILMethods.GetMethod(type, "Maximum");
				this.AggMaxResult = XmlILMethods.GetMethod(type, "get_MaximumResult");
				this.AggMin = XmlILMethods.GetMethod(type, "Minimum");
				this.AggMinResult = XmlILMethods.GetMethod(type, "get_MinimumResult");
				this.AggSum = XmlILMethods.GetMethod(type, "Sum");
				this.AggSumResult = XmlILMethods.GetMethod(type, "get_SumResult");
			}
			if (storageType == typeof(XPathNavigator))
			{
				this.SeqType = typeof(XmlQueryNodeSequence);
				this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "AddClone");
			}
			else if (storageType == typeof(XPathItem))
			{
				this.SeqType = typeof(XmlQueryItemSequence);
				this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "AddClone");
			}
			else
			{
				this.SeqType = typeof(XmlQuerySequence<>).MakeGenericType(new Type[] { storageType });
				this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "Add");
			}
			this.SeqEmpty = this.SeqType.GetField("Empty");
			this.SeqReuse = XmlILMethods.GetMethod(this.SeqType, "CreateOrReuse", new Type[] { this.SeqType });
			this.SeqReuseSgl = XmlILMethods.GetMethod(this.SeqType, "CreateOrReuse", new Type[] { this.SeqType, storageType });
			this.SeqSortByKeys = XmlILMethods.GetMethod(this.SeqType, "SortByKeys");
			this.IListType = typeof(IList<>).MakeGenericType(new Type[] { storageType });
			this.IListItem = XmlILMethods.GetMethod(this.IListType, "get_Item");
			this.IListCount = XmlILMethods.GetMethod(typeof(ICollection<>).MakeGenericType(new Type[] { storageType }), "get_Count");
			if (storageType == typeof(string))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_Value");
			}
			else if (storageType == typeof(int))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsInt");
			}
			else if (storageType == typeof(long))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsLong");
			}
			else if (storageType == typeof(DateTime))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsDateTime");
			}
			else if (storageType == typeof(double))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsDouble");
			}
			else if (storageType == typeof(bool))
			{
				this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsBoolean");
			}
			if (storageType == typeof(byte[]))
			{
				this.ToAtomicValue = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), "BytesToAtomicValue");
				return;
			}
			if (storageType != typeof(XPathItem) && storageType != typeof(XPathNavigator))
			{
				this.ToAtomicValue = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), storageType.Name + "ToAtomicValue");
			}
		}

		public MethodInfo AggAvg;

		public MethodInfo AggAvgResult;

		public MethodInfo AggCreate;

		public MethodInfo AggIsEmpty;

		public MethodInfo AggMax;

		public MethodInfo AggMaxResult;

		public MethodInfo AggMin;

		public MethodInfo AggMinResult;

		public MethodInfo AggSum;

		public MethodInfo AggSumResult;

		public Type SeqType;

		public FieldInfo SeqEmpty;

		public MethodInfo SeqReuse;

		public MethodInfo SeqReuseSgl;

		public MethodInfo SeqAdd;

		public MethodInfo SeqSortByKeys;

		public Type IListType;

		public MethodInfo IListCount;

		public MethodInfo IListItem;

		public MethodInfo ValueAs;

		public MethodInfo ToAtomicValue;
	}
}
