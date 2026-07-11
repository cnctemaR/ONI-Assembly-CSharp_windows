using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Export/Director/CameraPlayable.bindings.h")]
	[NativeHeader("Runtime/Camera//Director/CameraPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[StaticAccessor("CameraPlayableBindings", StaticAccessorType.DoubleColon)]
	public struct CameraPlayable : IPlayable, IEquatable<CameraPlayable>
	{
		internal CameraPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<CameraPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an CameraPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static CameraPlayable Create(PlayableGraph graph, Camera camera)
		{
			PlayableHandle playableHandle = CameraPlayable.CreateHandle(graph, camera);
			return new CameraPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, Camera camera)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!CameraPlayable.InternalCreateCameraPlayable(ref graph, camera, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Camera GetCameraInternal(ref PlayableHandle hdl);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCameraInternal(ref PlayableHandle hdl, Camera camera);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateCameraPlayable(ref PlayableGraph graph, Camera camera, ref PlayableHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateType(ref PlayableHandle hdl);

		private PlayableHandle m_Handle;
	}
}
