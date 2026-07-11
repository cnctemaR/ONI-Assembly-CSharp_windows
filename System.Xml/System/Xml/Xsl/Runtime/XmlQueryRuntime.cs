using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.XPath;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlQueryRuntime
	{
		internal XmlQueryRuntime(XmlQueryStaticData data, object defaultDataSource, XmlResolver dataSources, XsltArgumentList argList, XmlSequenceWriter seqWrt)
		{
			string[] names = data.Names;
			Int32Pair[] array = data.Filters;
			WhitespaceRuleLookup whitespaceRuleLookup = ((data.WhitespaceRules != null && data.WhitespaceRules.Count != 0) ? new WhitespaceRuleLookup(data.WhitespaceRules) : null);
			this.ctxt = new XmlQueryContext(this, defaultDataSource, dataSources, argList, whitespaceRuleLookup);
			this.xsltLib = null;
			this.earlyInfo = data.EarlyBound;
			this.earlyObjects = ((this.earlyInfo != null) ? new object[this.earlyInfo.Length] : null);
			this.globalNames = data.GlobalNames;
			this.globalValues = ((this.globalNames != null) ? new object[this.globalNames.Length] : null);
			this.nameTableQuery = this.ctxt.QueryNameTable;
			this.atomizedNames = null;
			if (names != null)
			{
				XmlNameTable defaultNameTable = this.ctxt.DefaultNameTable;
				this.atomizedNames = new string[names.Length];
				if (defaultNameTable != this.nameTableQuery && defaultNameTable != null)
				{
					for (int i = 0; i < names.Length; i++)
					{
						string text = defaultNameTable.Get(names[i]);
						this.atomizedNames[i] = this.nameTableQuery.Add(text ?? names[i]);
					}
				}
				else
				{
					for (int i = 0; i < names.Length; i++)
					{
						this.atomizedNames[i] = this.nameTableQuery.Add(names[i]);
					}
				}
			}
			this.filters = null;
			if (array != null)
			{
				this.filters = new XmlNavigatorFilter[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.filters[i] = XmlNavNameFilter.Create(this.atomizedNames[array[i].Left], this.atomizedNames[array[i].Right]);
				}
			}
			this.prefixMappingsList = data.PrefixMappingsList;
			this.types = data.Types;
			this.collations = data.Collations;
			this.docOrderCmp = new DocumentOrderComparer();
			this.indexes = null;
			this.stkOutput = new Stack<XmlQueryOutput>(16);
			this.output = new XmlQueryOutput(this, seqWrt);
		}

		public string[] DebugGetGlobalNames()
		{
			return this.globalNames;
		}

		public IList DebugGetGlobalValue(string name)
		{
			for (int i = 0; i < this.globalNames.Length; i++)
			{
				if (this.globalNames[i] == name)
				{
					return (IList)this.globalValues[i];
				}
			}
			return null;
		}

		public void DebugSetGlobalValue(string name, object value)
		{
			for (int i = 0; i < this.globalNames.Length; i++)
			{
				if (this.globalNames[i] == name)
				{
					this.globalValues[i] = (IList<XPathItem>)XmlAnyListConverter.ItemList.ChangeType(value, typeof(XPathItem[]), null);
					return;
				}
			}
		}

		public object DebugGetXsltValue(IList seq)
		{
			if (seq != null && seq.Count == 1)
			{
				XPathItem xpathItem = seq[0] as XPathItem;
				if (xpathItem != null && !xpathItem.IsNode)
				{
					return xpathItem.TypedValue;
				}
				if (xpathItem is RtfNavigator)
				{
					return ((RtfNavigator)xpathItem).ToNavigator();
				}
			}
			return seq;
		}

		public XmlQueryContext ExternalContext
		{
			get
			{
				return this.ctxt;
			}
		}

		public XsltLibrary XsltFunctions
		{
			get
			{
				if (this.xsltLib == null)
				{
					this.xsltLib = new XsltLibrary(this);
				}
				return this.xsltLib;
			}
		}

		public object GetEarlyBoundObject(int index)
		{
			object obj = this.earlyObjects[index];
			if (obj == null)
			{
				obj = this.earlyInfo[index].CreateObject();
				this.earlyObjects[index] = obj;
			}
			return obj;
		}

		public bool EarlyBoundFunctionExists(string name, string namespaceUri)
		{
			if (this.earlyInfo == null)
			{
				return false;
			}
			for (int i = 0; i < this.earlyInfo.Length; i++)
			{
				if (namespaceUri == this.earlyInfo[i].NamespaceUri)
				{
					return new XmlExtensionFunction(name, namespaceUri, -1, this.earlyInfo[i].EarlyBoundType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).CanBind();
				}
			}
			return false;
		}

		public bool IsGlobalComputed(int index)
		{
			return this.globalValues[index] != null;
		}

		public object GetGlobalValue(int index)
		{
			return this.globalValues[index];
		}

		public void SetGlobalValue(int index, object value)
		{
			this.globalValues[index] = value;
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTableQuery;
			}
		}

		public string GetAtomizedName(int index)
		{
			return this.atomizedNames[index];
		}

		public XmlNavigatorFilter GetNameFilter(int index)
		{
			return this.filters[index];
		}

		public XmlNavigatorFilter GetTypeFilter(XPathNodeType nodeType)
		{
			if (nodeType == XPathNodeType.All)
			{
				return XmlNavNeverFilter.Create();
			}
			if (nodeType == XPathNodeType.Attribute)
			{
				return XmlNavAttrFilter.Create();
			}
			return XmlNavTypeFilter.Create(nodeType);
		}

		public XmlQualifiedName ParseTagName(string tagName, int indexPrefixMappings)
		{
			string text;
			string text2;
			string text3;
			this.ParseTagName(tagName, indexPrefixMappings, out text, out text2, out text3);
			return new XmlQualifiedName(text2, text3);
		}

		public XmlQualifiedName ParseTagName(string tagName, string ns)
		{
			string text;
			string text2;
			ValidateNames.ParseQNameThrow(tagName, out text, out text2);
			return new XmlQualifiedName(text2, ns);
		}

		internal void ParseTagName(string tagName, int idxPrefixMappings, out string prefix, out string localName, out string ns)
		{
			ValidateNames.ParseQNameThrow(tagName, out prefix, out localName);
			ns = null;
			foreach (StringPair stringPair in this.prefixMappingsList[idxPrefixMappings])
			{
				if (prefix == stringPair.Left)
				{
					ns = stringPair.Right;
					break;
				}
			}
			if (ns != null)
			{
				return;
			}
			if (prefix.Length == 0)
			{
				ns = "";
				return;
			}
			if (prefix.Equals("xml"))
			{
				ns = "http://www.w3.org/XML/1998/namespace";
				return;
			}
			if (prefix.Equals("xmlns"))
			{
				ns = "http://www.w3.org/2000/xmlns/";
				return;
			}
			throw new XslTransformException("Prefix '{0}' is not defined.", new string[] { prefix });
		}

		public bool IsQNameEqual(XPathNavigator n1, XPathNavigator n2)
		{
			if (n1.NameTable == n2.NameTable)
			{
				return n1.LocalName == n2.LocalName && n1.NamespaceURI == n2.NamespaceURI;
			}
			return n1.LocalName == n2.LocalName && n1.NamespaceURI == n2.NamespaceURI;
		}

		public bool IsQNameEqual(XPathNavigator navigator, int indexLocalName, int indexNamespaceUri)
		{
			if (navigator.NameTable == this.nameTableQuery)
			{
				return this.GetAtomizedName(indexLocalName) == navigator.LocalName && this.GetAtomizedName(indexNamespaceUri) == navigator.NamespaceURI;
			}
			return this.GetAtomizedName(indexLocalName) == navigator.LocalName && this.GetAtomizedName(indexNamespaceUri) == navigator.NamespaceURI;
		}

		internal XmlQueryType[] XmlTypes
		{
			get
			{
				return this.types;
			}
		}

		internal XmlQueryType GetXmlType(int idxType)
		{
			return this.types[idxType];
		}

		public object ChangeTypeXsltArgument(int indexType, object value, Type destinationType)
		{
			return this.ChangeTypeXsltArgument(this.GetXmlType(indexType), value, destinationType);
		}

		internal object ChangeTypeXsltArgument(XmlQueryType xmlType, object value, Type destinationType)
		{
			XmlTypeCode typeCode = xmlType.TypeCode;
			if (typeCode <= XmlTypeCode.Node)
			{
				if (typeCode != XmlTypeCode.Item)
				{
					if (typeCode == XmlTypeCode.Node)
					{
						if (destinationType == XsltConvert.XPathNodeIteratorType)
						{
							value = new XPathArrayIterator((IList)value);
						}
						else if (destinationType == XsltConvert.XPathNavigatorArrayType)
						{
							IList<XPathNavigator> list = (IList<XPathNavigator>)value;
							XPathNavigator[] array = new XPathNavigator[list.Count];
							for (int i = 0; i < list.Count; i++)
							{
								array[i] = list[i];
							}
							value = array;
						}
					}
				}
				else
				{
					if (destinationType != XsltConvert.ObjectType)
					{
						throw new XslTransformException("Extension function parameters or return values which have Clr type '{0}' are not supported.", new string[] { destinationType.Name });
					}
					IList<XPathItem> list2 = (IList<XPathItem>)value;
					if (list2.Count == 1)
					{
						XPathItem xpathItem = list2[0];
						if (xpathItem.IsNode)
						{
							RtfNavigator rtfNavigator = xpathItem as RtfNavigator;
							if (rtfNavigator != null)
							{
								value = rtfNavigator.ToNavigator();
							}
							else
							{
								value = new XPathArrayIterator((IList)value);
							}
						}
						else
						{
							value = xpathItem.TypedValue;
						}
					}
					else
					{
						value = new XPathArrayIterator((IList)value);
					}
				}
			}
			else if (typeCode != XmlTypeCode.String)
			{
				if (typeCode == XmlTypeCode.Double)
				{
					if (destinationType != XsltConvert.DoubleType)
					{
						value = Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
					}
				}
			}
			else if (destinationType == XsltConvert.DateTimeType)
			{
				value = XsltConvert.ToDateTime((string)value);
			}
			return value;
		}

		public object ChangeTypeXsltResult(int indexType, object value)
		{
			return this.ChangeTypeXsltResult(this.GetXmlType(indexType), value);
		}

		internal object ChangeTypeXsltResult(XmlQueryType xmlType, object value)
		{
			if (value == null)
			{
				throw new XslTransformException("Extension functions cannot return null values.", new string[] { string.Empty });
			}
			XmlTypeCode xmlTypeCode = xmlType.TypeCode;
			if (xmlTypeCode <= XmlTypeCode.Node)
			{
				if (xmlTypeCode != XmlTypeCode.Item)
				{
					if (xmlTypeCode == XmlTypeCode.Node)
					{
						if (!xmlType.IsSingleton)
						{
							XPathArrayIterator xpathArrayIterator = value as XPathArrayIterator;
							if (xpathArrayIterator != null && xpathArrayIterator.AsList is XmlQueryNodeSequence)
							{
								value = xpathArrayIterator.AsList as XmlQueryNodeSequence;
							}
							else
							{
								XmlQueryNodeSequence xmlQueryNodeSequence = new XmlQueryNodeSequence();
								IList list = value as IList;
								if (list != null)
								{
									for (int i = 0; i < list.Count; i++)
									{
										xmlQueryNodeSequence.Add(XmlQueryRuntime.EnsureNavigator(list[i]));
									}
								}
								else
								{
									foreach (object obj in ((IEnumerable)value))
									{
										xmlQueryNodeSequence.Add(XmlQueryRuntime.EnsureNavigator(obj));
									}
								}
								value = xmlQueryNodeSequence;
							}
							value = ((XmlQueryNodeSequence)value).DocOrderDistinct(this.docOrderCmp);
						}
					}
				}
				else
				{
					Type type = value.GetType();
					xmlTypeCode = XsltConvert.InferXsltType(type).TypeCode;
					if (xmlTypeCode != XmlTypeCode.Item)
					{
						if (xmlTypeCode != XmlTypeCode.Node)
						{
							switch (xmlTypeCode)
							{
							case XmlTypeCode.String:
								if (type == XsltConvert.DateTimeType)
								{
									value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), XsltConvert.ToString((DateTime)value)));
								}
								else
								{
									value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), value));
								}
								break;
							case XmlTypeCode.Boolean:
								value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), value));
								break;
							case XmlTypeCode.Double:
								value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), ((IConvertible)value).ToDouble(null)));
								break;
							}
						}
						else
						{
							value = this.ChangeTypeXsltResult(XmlQueryTypeFactory.NodeS, value);
						}
					}
					else if (value is XPathNodeIterator)
					{
						value = this.ChangeTypeXsltResult(XmlQueryTypeFactory.NodeS, value);
					}
					else
					{
						IXPathNavigable ixpathNavigable = value as IXPathNavigable;
						if (ixpathNavigable == null)
						{
							throw new XslTransformException("Extension function parameters or return values which have Clr type '{0}' are not supported.", new string[] { type.Name });
						}
						if (value is XPathNavigator)
						{
							value = new XmlQueryNodeSequence((XPathNavigator)value);
						}
						else
						{
							value = new XmlQueryNodeSequence(ixpathNavigable.CreateNavigator());
						}
					}
				}
			}
			else if (xmlTypeCode != XmlTypeCode.String)
			{
				if (xmlTypeCode == XmlTypeCode.Double)
				{
					if (value.GetType() != XsltConvert.DoubleType)
					{
						value = ((IConvertible)value).ToDouble(null);
					}
				}
			}
			else if (value.GetType() == XsltConvert.DateTimeType)
			{
				value = XsltConvert.ToString((DateTime)value);
			}
			return value;
		}

		private static XPathNavigator EnsureNavigator(object value)
		{
			XPathNavigator xpathNavigator = value as XPathNavigator;
			if (xpathNavigator == null)
			{
				throw new XslTransformException("Extension functions cannot return null values.", new string[] { string.Empty });
			}
			return xpathNavigator;
		}

		public bool MatchesXmlType(IList<XPathItem> seq, int indexType)
		{
			XmlQueryType xmlQueryType = this.GetXmlType(indexType);
			int count = seq.Count;
			XmlQueryCardinality xmlQueryCardinality;
			if (count != 0)
			{
				if (count != 1)
				{
					xmlQueryCardinality = XmlQueryCardinality.More;
				}
				else
				{
					xmlQueryCardinality = XmlQueryCardinality.One;
				}
			}
			else
			{
				xmlQueryCardinality = XmlQueryCardinality.Zero;
			}
			if (!(xmlQueryCardinality <= xmlQueryType.Cardinality))
			{
				return false;
			}
			xmlQueryType = xmlQueryType.Prime;
			for (int i = 0; i < seq.Count; i++)
			{
				if (!this.CreateXmlType(seq[0]).IsSubtypeOf(xmlQueryType))
				{
					return false;
				}
			}
			return true;
		}

		public bool MatchesXmlType(XPathItem item, int indexType)
		{
			return this.CreateXmlType(item).IsSubtypeOf(this.GetXmlType(indexType));
		}

		public bool MatchesXmlType(IList<XPathItem> seq, XmlTypeCode code)
		{
			return seq.Count == 1 && this.MatchesXmlType(seq[0], code);
		}

		public bool MatchesXmlType(XPathItem item, XmlTypeCode code)
		{
			if (code > XmlTypeCode.AnyAtomicType)
			{
				return !item.IsNode && item.XmlType.TypeCode == code;
			}
			if (code == XmlTypeCode.Item)
			{
				return true;
			}
			if (code == XmlTypeCode.Node)
			{
				return item.IsNode;
			}
			if (code == XmlTypeCode.AnyAtomicType)
			{
				return !item.IsNode;
			}
			if (!item.IsNode)
			{
				return false;
			}
			switch (((XPathNavigator)item).NodeType)
			{
			case XPathNodeType.Root:
				return code == XmlTypeCode.Document;
			case XPathNodeType.Element:
				return code == XmlTypeCode.Element;
			case XPathNodeType.Attribute:
				return code == XmlTypeCode.Attribute;
			case XPathNodeType.Namespace:
				return code == XmlTypeCode.Namespace;
			case XPathNodeType.Text:
				return code == XmlTypeCode.Text;
			case XPathNodeType.SignificantWhitespace:
				return code == XmlTypeCode.Text;
			case XPathNodeType.Whitespace:
				return code == XmlTypeCode.Text;
			case XPathNodeType.ProcessingInstruction:
				return code == XmlTypeCode.ProcessingInstruction;
			case XPathNodeType.Comment:
				return code == XmlTypeCode.Comment;
			default:
				return false;
			}
		}

		private XmlQueryType CreateXmlType(XPathItem item)
		{
			if (!item.IsNode)
			{
				return XmlQueryTypeFactory.Type((XmlSchemaSimpleType)item.XmlType, true);
			}
			if (item is RtfNavigator)
			{
				return XmlQueryTypeFactory.Node;
			}
			XPathNavigator xpathNavigator = (XPathNavigator)item;
			XPathNodeType nodeType = xpathNavigator.NodeType;
			if (nodeType > XPathNodeType.Element)
			{
				if (nodeType != XPathNodeType.Attribute)
				{
					return XmlQueryTypeFactory.Type(xpathNavigator.NodeType, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false);
				}
				if (xpathNavigator.XmlType == null)
				{
					return XmlQueryTypeFactory.Type(xpathNavigator.NodeType, XmlQualifiedNameTest.New(xpathNavigator.LocalName, xpathNavigator.NamespaceURI), DatatypeImplementation.UntypedAtomicType, false);
				}
				return XmlQueryTypeFactory.Type(xpathNavigator.NodeType, XmlQualifiedNameTest.New(xpathNavigator.LocalName, xpathNavigator.NamespaceURI), xpathNavigator.XmlType, false);
			}
			else
			{
				if (xpathNavigator.XmlType == null)
				{
					return XmlQueryTypeFactory.Type(xpathNavigator.NodeType, XmlQualifiedNameTest.New(xpathNavigator.LocalName, xpathNavigator.NamespaceURI), XmlSchemaComplexType.UntypedAnyType, false);
				}
				return XmlQueryTypeFactory.Type(xpathNavigator.NodeType, XmlQualifiedNameTest.New(xpathNavigator.LocalName, xpathNavigator.NamespaceURI), xpathNavigator.XmlType, xpathNavigator.SchemaInfo.SchemaElement.IsNillable);
			}
		}

		public XmlCollation GetCollation(int index)
		{
			return this.collations[index];
		}

		public XmlCollation CreateCollation(string collation)
		{
			return XmlCollation.Create(collation);
		}

		public int ComparePosition(XPathNavigator navigatorThis, XPathNavigator navigatorThat)
		{
			return this.docOrderCmp.Compare(navigatorThis, navigatorThat);
		}

		public IList<XPathNavigator> DocOrderDistinct(IList<XPathNavigator> seq)
		{
			if (seq.Count <= 1)
			{
				return seq;
			}
			XmlQueryNodeSequence xmlQueryNodeSequence = (XmlQueryNodeSequence)seq;
			if (xmlQueryNodeSequence == null)
			{
				xmlQueryNodeSequence = new XmlQueryNodeSequence(seq);
			}
			return xmlQueryNodeSequence.DocOrderDistinct(this.docOrderCmp);
		}

		public string GenerateId(XPathNavigator navigator)
		{
			return "ID" + this.docOrderCmp.GetDocumentIndex(navigator).ToString(CultureInfo.InvariantCulture) + navigator.UniqueId;
		}

		public bool FindIndex(XPathNavigator context, int indexId, out XmlILIndex index)
		{
			XPathNavigator xpathNavigator = context.Clone();
			xpathNavigator.MoveToRoot();
			if (this.indexes != null && indexId < this.indexes.Length)
			{
				ArrayList arrayList = this.indexes[indexId];
				if (arrayList != null)
				{
					for (int i = 0; i < arrayList.Count; i += 2)
					{
						if (((XPathNavigator)arrayList[i]).IsSamePosition(xpathNavigator))
						{
							index = (XmlILIndex)arrayList[i + 1];
							return true;
						}
					}
				}
			}
			index = new XmlILIndex();
			return false;
		}

		public void AddNewIndex(XPathNavigator context, int indexId, XmlILIndex index)
		{
			XPathNavigator xpathNavigator = context.Clone();
			xpathNavigator.MoveToRoot();
			if (this.indexes == null)
			{
				this.indexes = new ArrayList[indexId + 4];
			}
			else if (indexId >= this.indexes.Length)
			{
				ArrayList[] array = new ArrayList[indexId + 4];
				Array.Copy(this.indexes, 0, array, 0, this.indexes.Length);
				this.indexes = array;
			}
			ArrayList arrayList = this.indexes[indexId];
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				this.indexes[indexId] = arrayList;
			}
			arrayList.Add(xpathNavigator);
			arrayList.Add(index);
		}

		public XmlQueryOutput Output
		{
			get
			{
				return this.output;
			}
		}

		public void StartSequenceConstruction(out XmlQueryOutput output)
		{
			this.stkOutput.Push(this.output);
			output = (this.output = new XmlQueryOutput(this, new XmlCachedSequenceWriter()));
		}

		public IList<XPathItem> EndSequenceConstruction(out XmlQueryOutput output)
		{
			IList<XPathItem> resultSequence = ((XmlCachedSequenceWriter)this.output.SequenceWriter).ResultSequence;
			output = (this.output = this.stkOutput.Pop());
			return resultSequence;
		}

		public void StartRtfConstruction(string baseUri, out XmlQueryOutput output)
		{
			this.stkOutput.Push(this.output);
			output = (this.output = new XmlQueryOutput(this, new XmlEventCache(baseUri, true)));
		}

		public XPathNavigator EndRtfConstruction(out XmlQueryOutput output)
		{
			XmlEventCache xmlEventCache = (XmlEventCache)this.output.Writer;
			output = (this.output = this.stkOutput.Pop());
			xmlEventCache.EndEvents();
			return new RtfTreeNavigator(xmlEventCache, this.nameTableQuery);
		}

		public XPathNavigator TextRtfConstruction(string text, string baseUri)
		{
			return new RtfTextNavigator(text, baseUri);
		}

		public void SendMessage(string message)
		{
			this.ctxt.OnXsltMessageEncountered(message);
		}

		public void ThrowException(string text)
		{
			throw new XslTransformException(text);
		}

		internal static XPathNavigator SyncToNavigator(XPathNavigator navigatorThis, XPathNavigator navigatorThat)
		{
			if (navigatorThis == null || !navigatorThis.MoveTo(navigatorThat))
			{
				return navigatorThat.Clone();
			}
			return navigatorThis;
		}

		public static int OnCurrentNodeChanged(XPathNavigator currentNode)
		{
			IXmlLineInfo xmlLineInfo = currentNode as IXmlLineInfo;
			if (xmlLineInfo != null && (currentNode.NodeType != XPathNodeType.Namespace || !XmlQueryRuntime.IsInheritedNamespace(currentNode)))
			{
				XmlQueryRuntime.OnCurrentNodeChanged2(currentNode.BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
			}
			return 0;
		}

		private static bool IsInheritedNamespace(XPathNavigator node)
		{
			XPathNavigator xpathNavigator = node.Clone();
			if (xpathNavigator.MoveToParent() && xpathNavigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
			{
				while (xpathNavigator.LocalName != node.LocalName)
				{
					if (!xpathNavigator.MoveToNextNamespace(XPathNamespaceScope.Local))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private static void OnCurrentNodeChanged2(string baseUri, int lineNumber, int linePosition)
		{
		}

		private XmlQueryContext ctxt;

		private XsltLibrary xsltLib;

		private EarlyBoundInfo[] earlyInfo;

		private object[] earlyObjects;

		private string[] globalNames;

		private object[] globalValues;

		private XmlNameTable nameTableQuery;

		private string[] atomizedNames;

		private XmlNavigatorFilter[] filters;

		private StringPair[][] prefixMappingsList;

		private XmlQueryType[] types;

		private XmlCollation[] collations;

		private DocumentOrderComparer docOrderCmp;

		private ArrayList[] indexes;

		private XmlQueryOutput output;

		private Stack<XmlQueryOutput> stkOutput;

		internal const BindingFlags EarlyBoundFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		internal const BindingFlags LateBoundFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
	}
}
