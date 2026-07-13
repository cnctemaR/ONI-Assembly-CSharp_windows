using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	[StructLayout(LayoutKind.Sequential)]
	internal class DifferentMarshallingTestObject : Object
	{
	}
}
