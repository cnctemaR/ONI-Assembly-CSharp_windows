using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class DelegateTests
	{
		public static int A()
		{
			return 882;
		}

		[MonoPInvokeCallback(typeof(DelegateTests.SomeDelegateFunctionPtr))]
		public static int B()
		{
			return 883;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ReturnDelegate(DelegateTests.SomeDelegate someDelegate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ReturnDelegateFunctionPtr(DelegateTests.SomeDelegateFunctionPtr SomeDelegateFunctionPtr);

		public delegate int SomeDelegate();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int SomeDelegateFunctionPtr();
	}
}
