using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>This class defines a pair of clips used by AnimatorOverrideController.</para>
	/// </summary>
	[Obsolete("This class is not used anymore. See AnimatorOverrideController.GetOverrides() and AnimatorOverrideController.ApplyOverrides()")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationClipPair
	{
		/// <summary>
		///   <para>The original clip from the controller.</para>
		/// </summary>
		public AnimationClip originalClip;

		/// <summary>
		///   <para>The override animation clip.</para>
		/// </summary>
		public AnimationClip overrideClip;
	}
}
