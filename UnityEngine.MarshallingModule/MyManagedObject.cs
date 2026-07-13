using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[StructLayout(LayoutKind.Sequential)]
	internal class MyManagedObject
	{
		public int value = 42;
	}
}
