using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Export/Director/PlayableOutputHandle.bindings.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	public struct PlayableOutputHandle : IEquatable<PlayableOutputHandle>
	{
		public static PlayableOutputHandle Null
		{
			get
			{
				return PlayableOutputHandle.m_Null;
			}
		}

		[VisibleToOtherModules]
		internal bool IsPlayableOutputOfType<T>()
		{
			return this.GetPlayableOutputType() == typeof(T);
		}

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode() ^ this.m_Version.GetHashCode();
		}

		public static bool operator ==(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return PlayableOutputHandle.CompareVersion(lhs, rhs);
		}

		public static bool operator !=(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return !PlayableOutputHandle.CompareVersion(lhs, rhs);
		}

		public override bool Equals(object p)
		{
			return p is PlayableOutputHandle && this.Equals((PlayableOutputHandle)p);
		}

		public bool Equals(PlayableOutputHandle other)
		{
			return PlayableOutputHandle.CompareVersion(this, other);
		}

		internal static bool CompareVersion(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
		}

		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsNull();

		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsValid();

		[FreeFunction("PlayableOutputHandleBindings::GetPlayableOutputType", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetPlayableOutputType();

		[FreeFunction("PlayableOutputHandleBindings::GetReferenceObject", HasExplicitThis = true, ThrowsException = true)]
		internal Object GetReferenceObject()
		{
			return Unmarshal.UnmarshalUnityObject<Object>(PlayableOutputHandle.GetReferenceObject_Injected(ref this));
		}

		[FreeFunction("PlayableOutputHandleBindings::SetReferenceObject", HasExplicitThis = true, ThrowsException = true)]
		internal void SetReferenceObject(Object target)
		{
			PlayableOutputHandle.SetReferenceObject_Injected(ref this, Object.MarshalledUnityObject.Marshal<Object>(target));
		}

		[FreeFunction("PlayableOutputHandleBindings::GetUserData", HasExplicitThis = true, ThrowsException = true)]
		internal Object GetUserData()
		{
			return Unmarshal.UnmarshalUnityObject<Object>(PlayableOutputHandle.GetUserData_Injected(ref this));
		}

		[FreeFunction("PlayableOutputHandleBindings::SetUserData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetUserData([Writable] Object target);

		[FreeFunction("PlayableOutputHandleBindings::GetSourcePlayable", HasExplicitThis = true, ThrowsException = true)]
		internal PlayableHandle GetSourcePlayable()
		{
			PlayableHandle playableHandle;
			PlayableOutputHandle.GetSourcePlayable_Injected(ref this, out playableHandle);
			return playableHandle;
		}

		[FreeFunction("PlayableOutputHandleBindings::SetSourcePlayable", HasExplicitThis = true, ThrowsException = true)]
		internal void SetSourcePlayable(PlayableHandle target, int port)
		{
			PlayableOutputHandle.SetSourcePlayable_Injected(ref this, ref target, port);
		}

		[FreeFunction("PlayableOutputHandleBindings::GetSourceOutputPort", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetSourceOutputPort();

		[FreeFunction("PlayableOutputHandleBindings::GetWeight", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetWeight();

		[FreeFunction("PlayableOutputHandleBindings::SetWeight", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetWeight(float weight);

		[FreeFunction("PlayableOutputHandleBindings::PushNotification", HasExplicitThis = true, ThrowsException = true)]
		internal void PushNotification(PlayableHandle origin, INotification notification, object context)
		{
			PlayableOutputHandle.PushNotification_Injected(ref this, ref origin, notification, context);
		}

		[FreeFunction("PlayableOutputHandleBindings::GetNotificationReceivers", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal extern INotificationReceiver[] GetNotificationReceivers();

		[FreeFunction("PlayableOutputHandleBindings::AddNotificationReceiver", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddNotificationReceiver(INotificationReceiver receiver);

		[FreeFunction("PlayableOutputHandleBindings::RemoveNotificationReceiver", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveNotificationReceiver(INotificationReceiver receiver);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetReferenceObject_Injected(ref PlayableOutputHandle _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetReferenceObject_Injected(ref PlayableOutputHandle _unity_self, IntPtr target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetUserData_Injected(ref PlayableOutputHandle _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, [In] ref PlayableHandle target, int port);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PushNotification_Injected(ref PlayableOutputHandle _unity_self, [In] ref PlayableHandle origin, INotification notification, object context);

		internal IntPtr m_Handle;

		internal uint m_Version;

		private static readonly PlayableOutputHandle m_Null = default(PlayableOutputHandle);
	}
}
