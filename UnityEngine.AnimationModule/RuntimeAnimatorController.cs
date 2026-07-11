using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Animation/RuntimeAnimatorController.h")]
	[ExcludeFromObjectFactory]
	[UsedByNativeCode]
	public class RuntimeAnimatorController : Object
	{
		protected RuntimeAnimatorController()
		{
		}

		public extern AnimationClip[] animationClips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
