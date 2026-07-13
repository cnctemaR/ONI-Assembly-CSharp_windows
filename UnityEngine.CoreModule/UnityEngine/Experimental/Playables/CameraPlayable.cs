using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables
{
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Export/Director/CameraPlayable.bindings.h")]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Camera//Director/CameraPlayable.h")]
	[StaticAccessor("CameraPlayableBindings", StaticAccessorType.DoubleColon)]
	public struct CameraPlayable : IPlayable, IEquatable<CameraPlayable>
	{
		public static CameraPlayable Create(PlayableGraph graph, Camera camera)
		{
			PlayableHandle playableHandle = CameraPlayable.CreateHandle(graph, camera);
			return new CameraPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, Camera camera)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !CameraPlayable.InternalCreateCameraPlayable(ref graph, camera, ref @null);
			PlayableHandle playableHandle;
			if (flag)
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
		}

		internal CameraPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<CameraPlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an CameraPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(CameraPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator CameraPlayable(Playable playable)
		{
			return new CameraPlayable(playable.GetHandle());
		}

		public bool Equals(CameraPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public Camera GetCamera()
		{
			return CameraPlayable.GetCameraInternal(ref this.m_Handle);
		}

		public void SetCamera(Camera value)
		{
			CameraPlayable.SetCameraInternal(ref this.m_Handle, value);
		}

		[NativeThrows]
		private static Camera GetCameraInternal(ref PlayableHandle hdl)
		{
			return Unmarshal.UnmarshalUnityObject<Camera>(CameraPlayable.GetCameraInternal_Injected(ref hdl));
		}

		[NativeThrows]
		private static void SetCameraInternal(ref PlayableHandle hdl, Camera camera)
		{
			CameraPlayable.SetCameraInternal_Injected(ref hdl, Object.MarshalledUnityObject.Marshal<Camera>(camera));
		}

		[NativeThrows]
		private static bool InternalCreateCameraPlayable(ref PlayableGraph graph, Camera camera, ref PlayableHandle handle)
		{
			return CameraPlayable.InternalCreateCameraPlayable_Injected(ref graph, Object.MarshalledUnityObject.Marshal<Camera>(camera), ref handle);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateType(ref PlayableHandle hdl);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCameraInternal_Injected(ref PlayableHandle hdl);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCameraInternal_Injected(ref PlayableHandle hdl, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateCameraPlayable_Injected(ref PlayableGraph graph, IntPtr camera, ref PlayableHandle handle);

		private PlayableHandle m_Handle;
	}
}
