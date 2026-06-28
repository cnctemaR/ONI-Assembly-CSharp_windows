using System;
using System.Runtime.CompilerServices;
using Unity.Bindings;

namespace UnityEngine
{
	internal class PropertyNameUtils
	{
		public static PropertyName PropertyNameFromString([NativeParameter(Unmarshalled = true)] string name)
		{
			PropertyName propertyName;
			PropertyNameUtils.PropertyNameFromString_Injected(name, out propertyName);
			return propertyName;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PropertyNameFromString_Injected(string name, out PropertyName ret);
	}
}
