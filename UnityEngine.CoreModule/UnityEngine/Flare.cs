using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A flare asset. Read more about flares in the.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/Flare.h")]
	public sealed class Flare : Object
	{
		public Flare()
		{
			Flare.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Flare self);
	}
}
