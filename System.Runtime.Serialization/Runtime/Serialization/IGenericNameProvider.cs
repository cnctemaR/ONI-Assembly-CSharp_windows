using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal interface IGenericNameProvider
	{
		int GetParameterCount();

		IList<int> GetNestedParameterCounts();

		string GetParameterName(int paramIndex);

		string GetNamespaces();

		string GetGenericTypeName();

		bool ParametersFromBuiltInNamespaces { get; }
	}
}
