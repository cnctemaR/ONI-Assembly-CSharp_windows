using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/PropertyName.h")]
	internal class PropertyNameUtils
	{
		[FreeFunction]
		public static PropertyName PropertyNameFromString([Unmarshalled] string name)
		{
			PropertyName propertyName;
			PropertyNameUtils.PropertyNameFromString_Injected(name, out propertyName);
			return propertyName;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PropertyNameFromString_Injected(string name, out PropertyName ret);
	}
}
