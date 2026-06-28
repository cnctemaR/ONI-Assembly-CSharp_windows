using System;
using System.Collections.Generic;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	internal class DefaultContractResolverState
	{
		public Dictionary<ResolverContractKey, JsonContract> ContractCache;

		public PropertyNameTable NameTable = new PropertyNameTable();
	}
}
