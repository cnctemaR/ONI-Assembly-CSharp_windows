using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The runtime representation of the AnimatorController. Use this representation to change the Animator Controller during runtime.</para>
	/// </summary>
	[UsedByNativeCode]
	[ExcludeFromObjectFactory]
	[NativeHeader("Runtime/Animation/RuntimeAnimatorController.h")]
	public class RuntimeAnimatorController : Object
	{
		protected RuntimeAnimatorController()
		{
		}

		/// <summary>
		///   <para>Retrieves all AnimationClip used by the controller.</para>
		/// </summary>
		public extern AnimationClip[] animationClips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
