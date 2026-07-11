using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.IlGen
{
	internal class StaticDataManager
	{
		public int DeclareName(string name)
		{
			if (this.uniqueNames == null)
			{
				this.uniqueNames = new UniqueList<string>();
			}
			return this.uniqueNames.Add(name);
		}

		public string[] Names
		{
			get
			{
				if (this.uniqueNames == null)
				{
					return null;
				}
				return this.uniqueNames.ToArray();
			}
		}

		public int DeclareNameFilter(string locName, string nsUri)
		{
			if (this.uniqueFilters == null)
			{
				this.uniqueFilters = new UniqueList<Int32Pair>();
			}
			return this.uniqueFilters.Add(new Int32Pair(this.DeclareName(locName), this.DeclareName(nsUri)));
		}

		public Int32Pair[] NameFilters
		{
			get
			{
				if (this.uniqueFilters == null)
				{
					return null;
				}
				return this.uniqueFilters.ToArray();
			}
		}

		public int DeclarePrefixMappings(IList<QilNode> list)
		{
			StringPair[] array = new StringPair[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				QilBinary qilBinary = (QilBinary)list[i];
				array[i] = new StringPair((QilLiteral)qilBinary.Left, (QilLiteral)qilBinary.Right);
			}
			if (this.prefixMappingsList == null)
			{
				this.prefixMappingsList = new List<StringPair[]>();
			}
			this.prefixMappingsList.Add(array);
			return this.prefixMappingsList.Count - 1;
		}

		public StringPair[][] PrefixMappingsList
		{
			get
			{
				if (this.prefixMappingsList == null)
				{
					return null;
				}
				return this.prefixMappingsList.ToArray();
			}
		}

		public int DeclareGlobalValue(string name)
		{
			if (this.globalNames == null)
			{
				this.globalNames = new List<string>();
			}
			int count = this.globalNames.Count;
			this.globalNames.Add(name);
			return count;
		}

		public string[] GlobalNames
		{
			get
			{
				if (this.globalNames == null)
				{
					return null;
				}
				return this.globalNames.ToArray();
			}
		}

		public int DeclareEarlyBound(string namespaceUri, Type ebType)
		{
			if (this.earlyInfo == null)
			{
				this.earlyInfo = new UniqueList<EarlyBoundInfo>();
			}
			return this.earlyInfo.Add(new EarlyBoundInfo(namespaceUri, ebType));
		}

		public EarlyBoundInfo[] EarlyBound
		{
			get
			{
				if (this.earlyInfo != null)
				{
					return this.earlyInfo.ToArray();
				}
				return null;
			}
		}

		public int DeclareXmlType(XmlQueryType type)
		{
			if (this.uniqueXmlTypes == null)
			{
				this.uniqueXmlTypes = new UniqueList<XmlQueryType>();
			}
			return this.uniqueXmlTypes.Add(type);
		}

		public XmlQueryType[] XmlTypes
		{
			get
			{
				if (this.uniqueXmlTypes == null)
				{
					return null;
				}
				return this.uniqueXmlTypes.ToArray();
			}
		}

		public int DeclareCollation(string collation)
		{
			if (this.uniqueCollations == null)
			{
				this.uniqueCollations = new UniqueList<XmlCollation>();
			}
			return this.uniqueCollations.Add(XmlCollation.Create(collation));
		}

		public XmlCollation[] Collations
		{
			get
			{
				if (this.uniqueCollations == null)
				{
					return null;
				}
				return this.uniqueCollations.ToArray();
			}
		}

		private UniqueList<string> uniqueNames;

		private UniqueList<Int32Pair> uniqueFilters;

		private List<StringPair[]> prefixMappingsList;

		private List<string> globalNames;

		private UniqueList<EarlyBoundInfo> earlyInfo;

		private UniqueList<XmlQueryType> uniqueXmlTypes;

		private UniqueList<XmlCollation> uniqueCollations;
	}
}
