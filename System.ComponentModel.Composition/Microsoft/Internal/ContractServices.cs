using System;
using System.ComponentModel.Composition.Primitives;

namespace Microsoft.Internal
{
	internal static class ContractServices
	{
		public static bool TryCast(Type contractType, object value, out object result)
		{
			if (value == null)
			{
				result = null;
				return true;
			}
			if (contractType.IsInstanceOfType(value))
			{
				result = value;
				return true;
			}
			if (typeof(Delegate).IsAssignableFrom(contractType))
			{
				ExportedDelegate exportedDelegate = value as ExportedDelegate;
				if (exportedDelegate != null)
				{
					result = exportedDelegate.CreateDelegate(contractType.UnderlyingSystemType);
					return result != null;
				}
			}
			result = null;
			return false;
		}
	}
}
