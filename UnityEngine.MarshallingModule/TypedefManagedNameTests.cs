using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeType(Header = "Modules/Marshalling/MarshallingTests.h")]
	internal class TypedefManagedNameTests
	{
		public static void ParameterStructWithTypedefManagedName(StructWithTypedefManagedName param)
		{
			TypedefManagedNameTests.ParameterStructWithTypedefManagedName_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithTypedefManagedName_Injected([In] ref StructWithTypedefManagedName param);
	}
}
