using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	internal struct PlayableOutput
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidInternal(ref PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object GetInternalReferenceObject(ref PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetInternalReferenceObject(ref PlayableOutput output, Object target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object GetInternalUserData(ref PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetInternalUserData(ref PlayableOutput output, [Writable] Object target);

		internal static PlayableHandle InternalGetSourcePlayable(ref PlayableOutput output)
		{
			PlayableHandle playableHandle;
			PlayableOutput.INTERNAL_CALL_InternalGetSourcePlayable(ref output, out playableHandle);
			return playableHandle;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalGetSourcePlayable(ref PlayableOutput output, out PlayableHandle value);

		internal static void InternalSetSourcePlayable(ref PlayableOutput output, ref PlayableHandle target)
		{
			PlayableOutput.INTERNAL_CALL_InternalSetSourcePlayable(ref output, ref target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetSourcePlayable(ref PlayableOutput output, ref PlayableHandle target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetSourceInputPort(ref PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSetSourceInputPort(ref PlayableOutput output, int port);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSetWeight(ref PlayableOutput output, float weight);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float InternalGetWeight(ref PlayableOutput output);

		internal IntPtr m_Handle;

		internal int m_Version;
	}
}
