using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal struct ScopedKnownTypes
	{
		internal void Push(Dictionary<XmlQualifiedName, DataContract> dataContractDictionary)
		{
			if (this.dataContractDictionaries == null)
			{
				this.dataContractDictionaries = new Dictionary<XmlQualifiedName, DataContract>[4];
			}
			else if (this.count == this.dataContractDictionaries.Length)
			{
				Array.Resize<Dictionary<XmlQualifiedName, DataContract>>(ref this.dataContractDictionaries, this.dataContractDictionaries.Length * 2);
			}
			Dictionary<XmlQualifiedName, DataContract>[] array = this.dataContractDictionaries;
			int num = this.count;
			this.count = num + 1;
			array[num] = dataContractDictionary;
		}

		internal void Pop()
		{
			this.count--;
		}

		internal DataContract GetDataContract(XmlQualifiedName qname)
		{
			for (int i = this.count - 1; i >= 0; i--)
			{
				DataContract dataContract;
				if (this.dataContractDictionaries[i].TryGetValue(qname, out dataContract))
				{
					return dataContract;
				}
			}
			return null;
		}

		internal Dictionary<XmlQualifiedName, DataContract>[] dataContractDictionaries;

		private int count;
	}
}
