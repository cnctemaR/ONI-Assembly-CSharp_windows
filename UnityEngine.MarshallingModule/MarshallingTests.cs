using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal static class MarshallingTests
	{
		[FreeFunction("MarshallingTest::DisableMarshallingTestsVerification")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableMarshallingTestsVerification();
	}
}
