using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Xsl.IlGen;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlQueryStaticData
	{
		public XmlQueryStaticData(XmlWriterSettings defaultWriterSettings, IList<WhitespaceRule> whitespaceRules, StaticDataManager staticData)
		{
			this.defaultWriterSettings = defaultWriterSettings;
			this.whitespaceRules = whitespaceRules;
			this.names = staticData.Names;
			this.prefixMappingsList = staticData.PrefixMappingsList;
			this.filters = staticData.NameFilters;
			this.types = staticData.XmlTypes;
			this.collations = staticData.Collations;
			this.globalNames = staticData.GlobalNames;
			this.earlyBound = staticData.EarlyBound;
		}

		public XmlQueryStaticData(byte[] data, Type[] ebTypes)
		{
			XmlQueryDataReader xmlQueryDataReader = new XmlQueryDataReader(new MemoryStream(data, false));
			if ((xmlQueryDataReader.ReadInt32Encoded() & -256) > 0)
			{
				throw new NotSupportedException();
			}
			this.defaultWriterSettings = new XmlWriterSettings(xmlQueryDataReader);
			int num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.whitespaceRules = new WhitespaceRule[num];
				for (int i = 0; i < num; i++)
				{
					this.whitespaceRules[i] = new WhitespaceRule(xmlQueryDataReader);
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.names = new string[num];
				for (int j = 0; j < num; j++)
				{
					this.names[j] = xmlQueryDataReader.ReadString();
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.prefixMappingsList = new StringPair[num][];
				for (int k = 0; k < num; k++)
				{
					int num2 = xmlQueryDataReader.ReadInt32();
					this.prefixMappingsList[k] = new StringPair[num2];
					for (int l = 0; l < num2; l++)
					{
						this.prefixMappingsList[k][l] = new StringPair(xmlQueryDataReader.ReadString(), xmlQueryDataReader.ReadString());
					}
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.filters = new Int32Pair[num];
				for (int m = 0; m < num; m++)
				{
					this.filters[m] = new Int32Pair(xmlQueryDataReader.ReadInt32Encoded(), xmlQueryDataReader.ReadInt32Encoded());
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.types = new XmlQueryType[num];
				for (int n = 0; n < num; n++)
				{
					this.types[n] = XmlQueryTypeFactory.Deserialize(xmlQueryDataReader);
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.collations = new XmlCollation[num];
				for (int num3 = 0; num3 < num; num3++)
				{
					this.collations[num3] = new XmlCollation(xmlQueryDataReader);
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.globalNames = new string[num];
				for (int num4 = 0; num4 < num; num4++)
				{
					this.globalNames[num4] = xmlQueryDataReader.ReadString();
				}
			}
			num = xmlQueryDataReader.ReadInt32();
			if (num != 0)
			{
				this.earlyBound = new EarlyBoundInfo[num];
				for (int num5 = 0; num5 < num; num5++)
				{
					this.earlyBound[num5] = new EarlyBoundInfo(xmlQueryDataReader.ReadString(), ebTypes[num5]);
				}
			}
			xmlQueryDataReader.Close();
		}

		public void GetObjectData(out byte[] data, out Type[] ebTypes)
		{
			MemoryStream memoryStream = new MemoryStream(4096);
			XmlQueryDataWriter xmlQueryDataWriter = new XmlQueryDataWriter(memoryStream);
			xmlQueryDataWriter.WriteInt32Encoded(0);
			this.defaultWriterSettings.GetObjectData(xmlQueryDataWriter);
			if (this.whitespaceRules == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.whitespaceRules.Count);
				foreach (WhitespaceRule whitespaceRule in this.whitespaceRules)
				{
					whitespaceRule.GetObjectData(xmlQueryDataWriter);
				}
			}
			if (this.names == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.names.Length);
				foreach (string text in this.names)
				{
					xmlQueryDataWriter.Write(text);
				}
			}
			if (this.prefixMappingsList == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.prefixMappingsList.Length);
				foreach (StringPair[] array3 in this.prefixMappingsList)
				{
					xmlQueryDataWriter.Write(array3.Length);
					foreach (StringPair stringPair in array3)
					{
						xmlQueryDataWriter.Write(stringPair.Left);
						xmlQueryDataWriter.Write(stringPair.Right);
					}
				}
			}
			if (this.filters == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.filters.Length);
				foreach (Int32Pair int32Pair in this.filters)
				{
					xmlQueryDataWriter.WriteInt32Encoded(int32Pair.Left);
					xmlQueryDataWriter.WriteInt32Encoded(int32Pair.Right);
				}
			}
			if (this.types == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.types.Length);
				foreach (XmlQueryType xmlQueryType in this.types)
				{
					XmlQueryTypeFactory.Serialize(xmlQueryDataWriter, xmlQueryType);
				}
			}
			if (this.collations == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.collations.Length);
				XmlCollation[] array7 = this.collations;
				for (int i = 0; i < array7.Length; i++)
				{
					array7[i].GetObjectData(xmlQueryDataWriter);
				}
			}
			if (this.globalNames == null)
			{
				xmlQueryDataWriter.Write(0);
			}
			else
			{
				xmlQueryDataWriter.Write(this.globalNames.Length);
				foreach (string text2 in this.globalNames)
				{
					xmlQueryDataWriter.Write(text2);
				}
			}
			if (this.earlyBound == null)
			{
				xmlQueryDataWriter.Write(0);
				ebTypes = null;
			}
			else
			{
				xmlQueryDataWriter.Write(this.earlyBound.Length);
				ebTypes = new Type[this.earlyBound.Length];
				int num = 0;
				foreach (EarlyBoundInfo earlyBoundInfo in this.earlyBound)
				{
					xmlQueryDataWriter.Write(earlyBoundInfo.NamespaceUri);
					ebTypes[num++] = earlyBoundInfo.EarlyBoundType;
				}
			}
			xmlQueryDataWriter.Close();
			data = memoryStream.ToArray();
		}

		public XmlWriterSettings DefaultWriterSettings
		{
			get
			{
				return this.defaultWriterSettings;
			}
		}

		public IList<WhitespaceRule> WhitespaceRules
		{
			get
			{
				return this.whitespaceRules;
			}
		}

		public string[] Names
		{
			get
			{
				return this.names;
			}
		}

		public StringPair[][] PrefixMappingsList
		{
			get
			{
				return this.prefixMappingsList;
			}
		}

		public Int32Pair[] Filters
		{
			get
			{
				return this.filters;
			}
		}

		public XmlQueryType[] Types
		{
			get
			{
				return this.types;
			}
		}

		public XmlCollation[] Collations
		{
			get
			{
				return this.collations;
			}
		}

		public string[] GlobalNames
		{
			get
			{
				return this.globalNames;
			}
		}

		public EarlyBoundInfo[] EarlyBound
		{
			get
			{
				return this.earlyBound;
			}
		}

		public const string DataFieldName = "staticData";

		public const string TypesFieldName = "ebTypes";

		private const int CurrentFormatVersion = 0;

		private XmlWriterSettings defaultWriterSettings;

		private IList<WhitespaceRule> whitespaceRules;

		private string[] names;

		private StringPair[][] prefixMappingsList;

		private Int32Pair[] filters;

		private XmlQueryType[] types;

		private XmlCollation[] collations;

		private string[] globalNames;

		private EarlyBoundInfo[] earlyBound;
	}
}
