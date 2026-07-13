using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal class ParentOfNested
	{
		internal class Nested
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern int MethodInNested();
		}
	}
}
