using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class GenericInfo : IGenericNameProvider
	{
		internal GenericInfo(XmlQualifiedName stableName, string genericTypeName)
		{
			this.stableName = stableName;
			this.genericTypeName = genericTypeName;
			this.nestedParamCounts = new List<int>();
			this.nestedParamCounts.Add(0);
		}

		internal void Add(GenericInfo actualParamInfo)
		{
			if (this.paramGenericInfos == null)
			{
				this.paramGenericInfos = new List<GenericInfo>();
			}
			this.paramGenericInfos.Add(actualParamInfo);
		}

		internal void AddToLevel(int level, int count)
		{
			if (level >= this.nestedParamCounts.Count)
			{
				do
				{
					this.nestedParamCounts.Add((level == this.nestedParamCounts.Count) ? count : 0);
				}
				while (level >= this.nestedParamCounts.Count);
				return;
			}
			this.nestedParamCounts[level] = this.nestedParamCounts[level] + count;
		}

		internal XmlQualifiedName GetExpandedStableName()
		{
			if (this.paramGenericInfos == null)
			{
				return this.stableName;
			}
			return new XmlQualifiedName(DataContract.EncodeLocalName(DataContract.ExpandGenericParameters(XmlConvert.DecodeName(this.stableName.Name), this)), this.stableName.Namespace);
		}

		internal string GetStableNamespace()
		{
			return this.stableName.Namespace;
		}

		internal XmlQualifiedName StableName
		{
			get
			{
				return this.stableName;
			}
		}

		internal IList<GenericInfo> Parameters
		{
			get
			{
				return this.paramGenericInfos;
			}
		}

		public int GetParameterCount()
		{
			return this.paramGenericInfos.Count;
		}

		public IList<int> GetNestedParameterCounts()
		{
			return this.nestedParamCounts;
		}

		public string GetParameterName(int paramIndex)
		{
			return this.paramGenericInfos[paramIndex].GetExpandedStableName().Name;
		}

		public string GetNamespaces()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.paramGenericInfos.Count; i++)
			{
				stringBuilder.Append(" ").Append(this.paramGenericInfos[i].GetStableNamespace());
			}
			return stringBuilder.ToString();
		}

		public string GetGenericTypeName()
		{
			return this.genericTypeName;
		}

		public bool ParametersFromBuiltInNamespaces
		{
			get
			{
				bool flag = true;
				int num = 0;
				while (num < this.paramGenericInfos.Count && flag)
				{
					flag = DataContract.IsBuiltInNamespace(this.paramGenericInfos[num].GetStableNamespace());
					num++;
				}
				return flag;
			}
		}

		private string genericTypeName;

		private XmlQualifiedName stableName;

		private List<GenericInfo> paramGenericInfos;

		private List<int> nestedParamCounts;
	}
}
