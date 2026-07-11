using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.IlGen
{
	internal static class XmlILMethods
	{
		static XmlILMethods()
		{
			XmlILMethods.StorageMethods[typeof(string)] = new XmlILStorageMethods(typeof(string));
			XmlILMethods.StorageMethods[typeof(bool)] = new XmlILStorageMethods(typeof(bool));
			XmlILMethods.StorageMethods[typeof(int)] = new XmlILStorageMethods(typeof(int));
			XmlILMethods.StorageMethods[typeof(long)] = new XmlILStorageMethods(typeof(long));
			XmlILMethods.StorageMethods[typeof(decimal)] = new XmlILStorageMethods(typeof(decimal));
			XmlILMethods.StorageMethods[typeof(double)] = new XmlILStorageMethods(typeof(double));
			XmlILMethods.StorageMethods[typeof(float)] = new XmlILStorageMethods(typeof(float));
			XmlILMethods.StorageMethods[typeof(DateTime)] = new XmlILStorageMethods(typeof(DateTime));
			XmlILMethods.StorageMethods[typeof(byte[])] = new XmlILStorageMethods(typeof(byte[]));
			XmlILMethods.StorageMethods[typeof(XmlQualifiedName)] = new XmlILStorageMethods(typeof(XmlQualifiedName));
			XmlILMethods.StorageMethods[typeof(TimeSpan)] = new XmlILStorageMethods(typeof(TimeSpan));
			XmlILMethods.StorageMethods[typeof(XPathItem)] = new XmlILStorageMethods(typeof(XPathItem));
			XmlILMethods.StorageMethods[typeof(XPathNavigator)] = new XmlILStorageMethods(typeof(XPathNavigator));
		}

		public static MethodInfo GetMethod(Type className, string methName)
		{
			return className.GetMethod(methName);
		}

		public static MethodInfo GetMethod(Type className, string methName, params Type[] args)
		{
			return className.GetMethod(methName, args);
		}

		public static readonly MethodInfo AncCreate = XmlILMethods.GetMethod(typeof(AncestorIterator), "Create");

		public static readonly MethodInfo AncNext = XmlILMethods.GetMethod(typeof(AncestorIterator), "MoveNext");

		public static readonly MethodInfo AncDOCreate = XmlILMethods.GetMethod(typeof(AncestorDocOrderIterator), "Create");

		public static readonly MethodInfo AncDONext = XmlILMethods.GetMethod(typeof(AncestorDocOrderIterator), "MoveNext");

		public static readonly MethodInfo AttrContentCreate = XmlILMethods.GetMethod(typeof(AttributeContentIterator), "Create");

		public static readonly MethodInfo AttrContentNext = XmlILMethods.GetMethod(typeof(AttributeContentIterator), "MoveNext");

		public static readonly MethodInfo AttrCreate = XmlILMethods.GetMethod(typeof(AttributeIterator), "Create");

		public static readonly MethodInfo AttrNext = XmlILMethods.GetMethod(typeof(AttributeIterator), "MoveNext");

		public static readonly MethodInfo ContentCreate = XmlILMethods.GetMethod(typeof(ContentIterator), "Create");

		public static readonly MethodInfo ContentNext = XmlILMethods.GetMethod(typeof(ContentIterator), "MoveNext");

		public static readonly MethodInfo ContentMergeCreate = XmlILMethods.GetMethod(typeof(ContentMergeIterator), "Create");

		public static readonly MethodInfo ContentMergeNext = XmlILMethods.GetMethod(typeof(ContentMergeIterator), "MoveNext");

		public static readonly MethodInfo DescCreate = XmlILMethods.GetMethod(typeof(DescendantIterator), "Create");

		public static readonly MethodInfo DescNext = XmlILMethods.GetMethod(typeof(DescendantIterator), "MoveNext");

		public static readonly MethodInfo DescMergeCreate = XmlILMethods.GetMethod(typeof(DescendantMergeIterator), "Create");

		public static readonly MethodInfo DescMergeNext = XmlILMethods.GetMethod(typeof(DescendantMergeIterator), "MoveNext");

		public static readonly MethodInfo DiffCreate = XmlILMethods.GetMethod(typeof(DifferenceIterator), "Create");

		public static readonly MethodInfo DiffNext = XmlILMethods.GetMethod(typeof(DifferenceIterator), "MoveNext");

		public static readonly MethodInfo DodMergeCreate = XmlILMethods.GetMethod(typeof(DodSequenceMerge), "Create");

		public static readonly MethodInfo DodMergeAdd = XmlILMethods.GetMethod(typeof(DodSequenceMerge), "AddSequence");

		public static readonly MethodInfo DodMergeSeq = XmlILMethods.GetMethod(typeof(DodSequenceMerge), "MergeSequences");

		public static readonly MethodInfo ElemContentCreate = XmlILMethods.GetMethod(typeof(ElementContentIterator), "Create");

		public static readonly MethodInfo ElemContentNext = XmlILMethods.GetMethod(typeof(ElementContentIterator), "MoveNext");

		public static readonly MethodInfo FollSibCreate = XmlILMethods.GetMethod(typeof(FollowingSiblingIterator), "Create");

		public static readonly MethodInfo FollSibNext = XmlILMethods.GetMethod(typeof(FollowingSiblingIterator), "MoveNext");

		public static readonly MethodInfo FollSibMergeCreate = XmlILMethods.GetMethod(typeof(FollowingSiblingMergeIterator), "Create");

		public static readonly MethodInfo FollSibMergeNext = XmlILMethods.GetMethod(typeof(FollowingSiblingMergeIterator), "MoveNext");

		public static readonly MethodInfo IdCreate = XmlILMethods.GetMethod(typeof(IdIterator), "Create");

		public static readonly MethodInfo IdNext = XmlILMethods.GetMethod(typeof(IdIterator), "MoveNext");

		public static readonly MethodInfo InterCreate = XmlILMethods.GetMethod(typeof(IntersectIterator), "Create");

		public static readonly MethodInfo InterNext = XmlILMethods.GetMethod(typeof(IntersectIterator), "MoveNext");

		public static readonly MethodInfo KindContentCreate = XmlILMethods.GetMethod(typeof(NodeKindContentIterator), "Create");

		public static readonly MethodInfo KindContentNext = XmlILMethods.GetMethod(typeof(NodeKindContentIterator), "MoveNext");

		public static readonly MethodInfo NmspCreate = XmlILMethods.GetMethod(typeof(NamespaceIterator), "Create");

		public static readonly MethodInfo NmspNext = XmlILMethods.GetMethod(typeof(NamespaceIterator), "MoveNext");

		public static readonly MethodInfo NodeRangeCreate = XmlILMethods.GetMethod(typeof(NodeRangeIterator), "Create");

		public static readonly MethodInfo NodeRangeNext = XmlILMethods.GetMethod(typeof(NodeRangeIterator), "MoveNext");

		public static readonly MethodInfo ParentCreate = XmlILMethods.GetMethod(typeof(ParentIterator), "Create");

		public static readonly MethodInfo ParentNext = XmlILMethods.GetMethod(typeof(ParentIterator), "MoveNext");

		public static readonly MethodInfo PrecCreate = XmlILMethods.GetMethod(typeof(PrecedingIterator), "Create");

		public static readonly MethodInfo PrecNext = XmlILMethods.GetMethod(typeof(PrecedingIterator), "MoveNext");

		public static readonly MethodInfo PreSibCreate = XmlILMethods.GetMethod(typeof(PrecedingSiblingIterator), "Create");

		public static readonly MethodInfo PreSibNext = XmlILMethods.GetMethod(typeof(PrecedingSiblingIterator), "MoveNext");

		public static readonly MethodInfo PreSibDOCreate = XmlILMethods.GetMethod(typeof(PrecedingSiblingDocOrderIterator), "Create");

		public static readonly MethodInfo PreSibDONext = XmlILMethods.GetMethod(typeof(PrecedingSiblingDocOrderIterator), "MoveNext");

		public static readonly MethodInfo SortKeyCreate = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "Create");

		public static readonly MethodInfo SortKeyDateTime = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddDateTimeSortKey");

		public static readonly MethodInfo SortKeyDecimal = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddDecimalSortKey");

		public static readonly MethodInfo SortKeyDouble = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddDoubleSortKey");

		public static readonly MethodInfo SortKeyEmpty = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddEmptySortKey");

		public static readonly MethodInfo SortKeyFinish = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "FinishSortKeys");

		public static readonly MethodInfo SortKeyInt = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddIntSortKey");

		public static readonly MethodInfo SortKeyInteger = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddIntegerSortKey");

		public static readonly MethodInfo SortKeyKeys = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "get_Keys");

		public static readonly MethodInfo SortKeyString = XmlILMethods.GetMethod(typeof(XmlSortKeyAccumulator), "AddStringSortKey");

		public static readonly MethodInfo UnionCreate = XmlILMethods.GetMethod(typeof(UnionIterator), "Create");

		public static readonly MethodInfo UnionNext = XmlILMethods.GetMethod(typeof(UnionIterator), "MoveNext");

		public static readonly MethodInfo XPFollCreate = XmlILMethods.GetMethod(typeof(XPathFollowingIterator), "Create");

		public static readonly MethodInfo XPFollNext = XmlILMethods.GetMethod(typeof(XPathFollowingIterator), "MoveNext");

		public static readonly MethodInfo XPFollMergeCreate = XmlILMethods.GetMethod(typeof(XPathFollowingMergeIterator), "Create");

		public static readonly MethodInfo XPFollMergeNext = XmlILMethods.GetMethod(typeof(XPathFollowingMergeIterator), "MoveNext");

		public static readonly MethodInfo XPPrecCreate = XmlILMethods.GetMethod(typeof(XPathPrecedingIterator), "Create");

		public static readonly MethodInfo XPPrecNext = XmlILMethods.GetMethod(typeof(XPathPrecedingIterator), "MoveNext");

		public static readonly MethodInfo XPPrecDOCreate = XmlILMethods.GetMethod(typeof(XPathPrecedingDocOrderIterator), "Create");

		public static readonly MethodInfo XPPrecDONext = XmlILMethods.GetMethod(typeof(XPathPrecedingDocOrderIterator), "MoveNext");

		public static readonly MethodInfo XPPrecMergeCreate = XmlILMethods.GetMethod(typeof(XPathPrecedingMergeIterator), "Create");

		public static readonly MethodInfo XPPrecMergeNext = XmlILMethods.GetMethod(typeof(XPathPrecedingMergeIterator), "MoveNext");

		public static readonly MethodInfo AddNewIndex = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "AddNewIndex");

		public static readonly MethodInfo ChangeTypeXsltArg = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ChangeTypeXsltArgument", new Type[]
		{
			typeof(int),
			typeof(object),
			typeof(Type)
		});

		public static readonly MethodInfo ChangeTypeXsltResult = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ChangeTypeXsltResult");

		public static readonly MethodInfo CompPos = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ComparePosition");

		public static readonly MethodInfo Context = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "get_ExternalContext");

		public static readonly MethodInfo CreateCollation = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "CreateCollation");

		public static readonly MethodInfo DocOrder = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "DocOrderDistinct");

		public static readonly MethodInfo EndRtfConstr = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "EndRtfConstruction");

		public static readonly MethodInfo EndSeqConstr = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "EndSequenceConstruction");

		public static readonly MethodInfo FindIndex = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "FindIndex");

		public static readonly MethodInfo GenId = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GenerateId");

		public static readonly MethodInfo GetAtomizedName = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetAtomizedName");

		public static readonly MethodInfo GetCollation = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetCollation");

		public static readonly MethodInfo GetEarly = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetEarlyBoundObject");

		public static readonly MethodInfo GetNameFilter = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetNameFilter");

		public static readonly MethodInfo GetOutput = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "get_Output");

		public static readonly MethodInfo GetGlobalValue = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetGlobalValue");

		public static readonly MethodInfo GetTypeFilter = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "GetTypeFilter");

		public static readonly MethodInfo GlobalComputed = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "IsGlobalComputed");

		public static readonly MethodInfo ItemMatchesCode = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "MatchesXmlType", new Type[]
		{
			typeof(XPathItem),
			typeof(XmlTypeCode)
		});

		public static readonly MethodInfo ItemMatchesType = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "MatchesXmlType", new Type[]
		{
			typeof(XPathItem),
			typeof(int)
		});

		public static readonly MethodInfo QNameEqualLit = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "IsQNameEqual", new Type[]
		{
			typeof(XPathNavigator),
			typeof(int),
			typeof(int)
		});

		public static readonly MethodInfo QNameEqualNav = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "IsQNameEqual", new Type[]
		{
			typeof(XPathNavigator),
			typeof(XPathNavigator)
		});

		public static readonly MethodInfo RtfConstr = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "TextRtfConstruction");

		public static readonly MethodInfo SendMessage = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "SendMessage");

		public static readonly MethodInfo SeqMatchesCode = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "MatchesXmlType", new Type[]
		{
			typeof(IList<XPathItem>),
			typeof(XmlTypeCode)
		});

		public static readonly MethodInfo SeqMatchesType = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "MatchesXmlType", new Type[]
		{
			typeof(IList<XPathItem>),
			typeof(int)
		});

		public static readonly MethodInfo SetGlobalValue = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "SetGlobalValue");

		public static readonly MethodInfo StartRtfConstr = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "StartRtfConstruction");

		public static readonly MethodInfo StartSeqConstr = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "StartSequenceConstruction");

		public static readonly MethodInfo TagAndMappings = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ParseTagName", new Type[]
		{
			typeof(string),
			typeof(int)
		});

		public static readonly MethodInfo TagAndNamespace = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ParseTagName", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo ThrowException = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "ThrowException");

		public static readonly MethodInfo XsltLib = XmlILMethods.GetMethod(typeof(XmlQueryRuntime), "get_XsltFunctions");

		public static readonly MethodInfo GetDataSource = XmlILMethods.GetMethod(typeof(XmlQueryContext), "GetDataSource");

		public static readonly MethodInfo GetDefaultDataSource = XmlILMethods.GetMethod(typeof(XmlQueryContext), "get_DefaultDataSource");

		public static readonly MethodInfo GetParam = XmlILMethods.GetMethod(typeof(XmlQueryContext), "GetParameter");

		public static readonly MethodInfo InvokeXsltLate = XmlILMethods.GetMethod(typeof(XmlQueryContext), "InvokeXsltLateBoundFunction");

		public static readonly MethodInfo IndexAdd = XmlILMethods.GetMethod(typeof(XmlILIndex), "Add");

		public static readonly MethodInfo IndexLookup = XmlILMethods.GetMethod(typeof(XmlILIndex), "Lookup");

		public static readonly MethodInfo ItemIsNode = XmlILMethods.GetMethod(typeof(XPathItem), "get_IsNode");

		public static readonly MethodInfo Value = XmlILMethods.GetMethod(typeof(XPathItem), "get_Value");

		public static readonly MethodInfo ValueAsAny = XmlILMethods.GetMethod(typeof(XPathItem), "ValueAs", new Type[]
		{
			typeof(Type),
			typeof(IXmlNamespaceResolver)
		});

		public static readonly MethodInfo NavClone = XmlILMethods.GetMethod(typeof(XPathNavigator), "Clone");

		public static readonly MethodInfo NavLocalName = XmlILMethods.GetMethod(typeof(XPathNavigator), "get_LocalName");

		public static readonly MethodInfo NavMoveAttr = XmlILMethods.GetMethod(typeof(XPathNavigator), "MoveToAttribute", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo NavMoveId = XmlILMethods.GetMethod(typeof(XPathNavigator), "MoveToId");

		public static readonly MethodInfo NavMoveParent = XmlILMethods.GetMethod(typeof(XPathNavigator), "MoveToParent");

		public static readonly MethodInfo NavMoveRoot = XmlILMethods.GetMethod(typeof(XPathNavigator), "MoveToRoot");

		public static readonly MethodInfo NavMoveTo = XmlILMethods.GetMethod(typeof(XPathNavigator), "MoveTo");

		public static readonly MethodInfo NavNmsp = XmlILMethods.GetMethod(typeof(XPathNavigator), "get_NamespaceURI");

		public static readonly MethodInfo NavPrefix = XmlILMethods.GetMethod(typeof(XPathNavigator), "get_Prefix");

		public static readonly MethodInfo NavSamePos = XmlILMethods.GetMethod(typeof(XPathNavigator), "IsSamePosition");

		public static readonly MethodInfo NavType = XmlILMethods.GetMethod(typeof(XPathNavigator), "get_NodeType");

		public static readonly MethodInfo StartElemLitName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElement", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartElemLocName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementLocalName", new Type[] { typeof(string) });

		public static readonly MethodInfo EndElemStackName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndElement");

		public static readonly MethodInfo StartAttrLitName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttribute", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartAttrLocName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeLocalName", new Type[] { typeof(string) });

		public static readonly MethodInfo EndAttr = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndAttribute");

		public static readonly MethodInfo Text = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteString");

		public static readonly MethodInfo NoEntText = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteRaw", new Type[] { typeof(string) });

		public static readonly MethodInfo StartTree = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "StartTree");

		public static readonly MethodInfo EndTree = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "EndTree");

		public static readonly MethodInfo StartElemLitNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementUnchecked", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartElemLocNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementUnchecked", new Type[] { typeof(string) });

		public static readonly MethodInfo StartContentUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "StartElementContentUnchecked");

		public static readonly MethodInfo EndElemLitNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndElementUnchecked", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo EndElemLocNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndElementUnchecked", new Type[] { typeof(string) });

		public static readonly MethodInfo StartAttrLitNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeUnchecked", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartAttrLocNameUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeUnchecked", new Type[] { typeof(string) });

		public static readonly MethodInfo EndAttrUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndAttributeUnchecked");

		public static readonly MethodInfo NamespaceDeclUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteNamespaceDeclarationUnchecked");

		public static readonly MethodInfo TextUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStringUnchecked");

		public static readonly MethodInfo NoEntTextUn = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteRawUnchecked");

		public static readonly MethodInfo StartRoot = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartRoot");

		public static readonly MethodInfo EndRoot = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndRoot");

		public static readonly MethodInfo StartElemCopyName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementComputed", new Type[] { typeof(XPathNavigator) });

		public static readonly MethodInfo StartElemMapName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementComputed", new Type[]
		{
			typeof(string),
			typeof(int)
		});

		public static readonly MethodInfo StartElemNmspName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementComputed", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartElemQName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartElementComputed", new Type[] { typeof(XmlQualifiedName) });

		public static readonly MethodInfo StartAttrCopyName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeComputed", new Type[] { typeof(XPathNavigator) });

		public static readonly MethodInfo StartAttrMapName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeComputed", new Type[]
		{
			typeof(string),
			typeof(int)
		});

		public static readonly MethodInfo StartAttrNmspName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeComputed", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StartAttrQName = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartAttributeComputed", new Type[] { typeof(XmlQualifiedName) });

		public static readonly MethodInfo NamespaceDecl = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteNamespaceDeclaration");

		public static readonly MethodInfo StartComment = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartComment");

		public static readonly MethodInfo CommentText = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteCommentString");

		public static readonly MethodInfo EndComment = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndComment");

		public static readonly MethodInfo StartPI = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteStartProcessingInstruction");

		public static readonly MethodInfo PIText = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteProcessingInstructionString");

		public static readonly MethodInfo EndPI = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteEndProcessingInstruction");

		public static readonly MethodInfo WriteItem = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "WriteItem");

		public static readonly MethodInfo CopyOf = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "XsltCopyOf");

		public static readonly MethodInfo StartCopy = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "StartCopy");

		public static readonly MethodInfo EndCopy = XmlILMethods.GetMethod(typeof(XmlQueryOutput), "EndCopy");

		public static readonly MethodInfo DecAdd = XmlILMethods.GetMethod(typeof(decimal), "Add");

		public static readonly MethodInfo DecCmp = XmlILMethods.GetMethod(typeof(decimal), "Compare", new Type[]
		{
			typeof(decimal),
			typeof(decimal)
		});

		public static readonly MethodInfo DecEq = XmlILMethods.GetMethod(typeof(decimal), "Equals", new Type[]
		{
			typeof(decimal),
			typeof(decimal)
		});

		public static readonly MethodInfo DecSub = XmlILMethods.GetMethod(typeof(decimal), "Subtract");

		public static readonly MethodInfo DecMul = XmlILMethods.GetMethod(typeof(decimal), "Multiply");

		public static readonly MethodInfo DecDiv = XmlILMethods.GetMethod(typeof(decimal), "Divide");

		public static readonly MethodInfo DecRem = XmlILMethods.GetMethod(typeof(decimal), "Remainder");

		public static readonly MethodInfo DecNeg = XmlILMethods.GetMethod(typeof(decimal), "Negate");

		public static readonly MethodInfo QNameEq = XmlILMethods.GetMethod(typeof(XmlQualifiedName), "Equals");

		public static readonly MethodInfo StrEq = XmlILMethods.GetMethod(typeof(string), "Equals", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StrCat2 = XmlILMethods.GetMethod(typeof(string), "Concat", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StrCat3 = XmlILMethods.GetMethod(typeof(string), "Concat", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StrCat4 = XmlILMethods.GetMethod(typeof(string), "Concat", new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StrCmp = XmlILMethods.GetMethod(typeof(string), "CompareOrdinal", new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly MethodInfo StrLen = XmlILMethods.GetMethod(typeof(string), "get_Length");

		public static readonly MethodInfo DblToDec = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDecimal", new Type[] { typeof(double) });

		public static readonly MethodInfo DblToInt = XmlILMethods.GetMethod(typeof(XsltConvert), "ToInt", new Type[] { typeof(double) });

		public static readonly MethodInfo DblToLng = XmlILMethods.GetMethod(typeof(XsltConvert), "ToLong", new Type[] { typeof(double) });

		public static readonly MethodInfo DblToStr = XmlILMethods.GetMethod(typeof(XsltConvert), "ToString", new Type[] { typeof(double) });

		public static readonly MethodInfo DecToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(decimal) });

		public static readonly MethodInfo DTToStr = XmlILMethods.GetMethod(typeof(XsltConvert), "ToString", new Type[] { typeof(DateTime) });

		public static readonly MethodInfo IntToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(int) });

		public static readonly MethodInfo LngToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(long) });

		public static readonly MethodInfo StrToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(string) });

		public static readonly MethodInfo StrToDT = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDateTime", new Type[] { typeof(string) });

		public static readonly MethodInfo ItemToBool = XmlILMethods.GetMethod(typeof(XsltConvert), "ToBoolean", new Type[] { typeof(XPathItem) });

		public static readonly MethodInfo ItemToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(XPathItem) });

		public static readonly MethodInfo ItemToStr = XmlILMethods.GetMethod(typeof(XsltConvert), "ToString", new Type[] { typeof(XPathItem) });

		public static readonly MethodInfo ItemToNode = XmlILMethods.GetMethod(typeof(XsltConvert), "ToNode", new Type[] { typeof(XPathItem) });

		public static readonly MethodInfo ItemToNodes = XmlILMethods.GetMethod(typeof(XsltConvert), "ToNodeSet", new Type[] { typeof(XPathItem) });

		public static readonly MethodInfo ItemsToBool = XmlILMethods.GetMethod(typeof(XsltConvert), "ToBoolean", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo ItemsToDbl = XmlILMethods.GetMethod(typeof(XsltConvert), "ToDouble", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo ItemsToNode = XmlILMethods.GetMethod(typeof(XsltConvert), "ToNode", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo ItemsToNodes = XmlILMethods.GetMethod(typeof(XsltConvert), "ToNodeSet", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo ItemsToStr = XmlILMethods.GetMethod(typeof(XsltConvert), "ToString", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo StrCatCat = XmlILMethods.GetMethod(typeof(StringConcat), "Concat");

		public static readonly MethodInfo StrCatClear = XmlILMethods.GetMethod(typeof(StringConcat), "Clear");

		public static readonly MethodInfo StrCatResult = XmlILMethods.GetMethod(typeof(StringConcat), "GetResult");

		public static readonly MethodInfo StrCatDelim = XmlILMethods.GetMethod(typeof(StringConcat), "set_Delimiter");

		public static readonly MethodInfo NavsToItems = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), "NavigatorsToItems");

		public static readonly MethodInfo ItemsToNavs = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), "ItemsToNavigators");

		public static readonly MethodInfo SetDod = XmlILMethods.GetMethod(typeof(XmlQueryNodeSequence), "set_IsDocOrderDistinct");

		public static readonly MethodInfo GetTypeFromHandle = XmlILMethods.GetMethod(typeof(Type), "GetTypeFromHandle");

		public static readonly MethodInfo InitializeArray = XmlILMethods.GetMethod(typeof(RuntimeHelpers), "InitializeArray");

		public static readonly Dictionary<Type, XmlILStorageMethods> StorageMethods = new Dictionary<Type, XmlILStorageMethods>();
	}
}
