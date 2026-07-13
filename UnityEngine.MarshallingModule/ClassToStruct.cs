using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeAsStruct]
	[ExcludeFromDocs]
	[StructLayout(LayoutKind.Sequential)]
	internal class ClassToStruct
	{
		public int intField;

		public string stringField;
	}
}
