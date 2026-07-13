using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.Layout
{
	internal struct ComponentType
	{
		public static ComponentType Create<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			return new ComponentType
			{
				Size = UnsafeUtility.SizeOf<T>()
			};
		}

		public int Size;
	}
}
