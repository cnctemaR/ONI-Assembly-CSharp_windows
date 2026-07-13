using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class MarshallingTests2
	{
		public static void ParameterNonBlittableStructReuse(StructCoreString param)
		{
			MarshallingTests2.ParameterNonBlittableStructReuse_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterNonBlittableStructReuse_Injected([In] ref StructCoreString param);
	}
}
