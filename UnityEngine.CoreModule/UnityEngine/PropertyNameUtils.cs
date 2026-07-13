using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/PropertyName.h")]
	internal class PropertyNameUtils
	{
		[FreeFunction("PropertyNameFromStringICall", IsThreadSafe = true)]
		public unsafe static PropertyName PropertyNameFromString(string name)
		{
			PropertyName propertyName2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				PropertyName propertyName;
				PropertyNameUtils.PropertyNameFromString_Injected(ref managedSpanWrapper, out propertyName);
			}
			finally
			{
				char* ptr = null;
				PropertyName propertyName;
				propertyName2 = propertyName;
			}
			return propertyName2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PropertyNameFromString_Injected(ref ManagedSpanWrapper name, out PropertyName ret);
	}
}
