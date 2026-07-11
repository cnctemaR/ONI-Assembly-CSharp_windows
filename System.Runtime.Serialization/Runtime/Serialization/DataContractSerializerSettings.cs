using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Runtime.Serialization
{
	public class DataContractSerializerSettings
	{
		public XmlDictionaryString RootName { get; set; }

		public XmlDictionaryString RootNamespace { get; set; }

		public IEnumerable<Type> KnownTypes { get; set; }

		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.maxItemsInObjectGraph;
			}
			set
			{
				this.maxItemsInObjectGraph = value;
			}
		}

		public bool IgnoreExtensionDataObject { get; set; }

		public bool PreserveObjectReferences { get; set; }

		public IDataContractSurrogate DataContractSurrogate { get; set; }

		public DataContractResolver DataContractResolver { get; set; }

		public bool SerializeReadOnlyTypes { get; set; }

		private int maxItemsInObjectGraph = int.MaxValue;
	}
}
