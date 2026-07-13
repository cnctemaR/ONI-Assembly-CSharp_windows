using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Marshalling/MarshallingTests.h")]
	internal abstract class AbstractClass
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int MethodInAbstractClass();
	}
}
