using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[ExcludeFromObjectFactory]
	[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
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
