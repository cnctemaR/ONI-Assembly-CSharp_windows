using System;

namespace System.Runtime.Serialization
{
	internal interface IDataNode
	{
		Type DataType { get; }

		object Value { get; set; }

		string DataContractName { get; set; }

		string DataContractNamespace { get; set; }

		string ClrTypeName { get; set; }

		string ClrAssemblyName { get; set; }

		string Id { get; set; }

		bool PreservesReferences { get; }

		void GetData(ElementData element);

		bool IsFinalValue { get; set; }

		void Clear();
	}
}
