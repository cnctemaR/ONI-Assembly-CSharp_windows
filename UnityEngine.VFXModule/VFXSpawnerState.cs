using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.VFX
{
	[RequiredByNativeCode]
	[NativeType(Header = "Modules/VFX/Public/VFXSpawnerState.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class VFXSpawnerState : IDisposable
	{
		internal VFXSpawnerState(IntPtr ptr, bool owner)
		{
			this.m_Ptr = ptr;
			this.m_Owner = owner;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create();

		[RequiredByNativeCode]
		internal static VFXSpawnerState CreateSpawnerStateWrapper()
		{
			return new VFXSpawnerState(IntPtr.Zero, false);
		}

		[RequiredByNativeCode]
		internal void SetWrapValue(IntPtr ptr)
		{
			if (this.m_Owner)
			{
				throw new Exception("VFXSpawnerState : SetWrapValue is reserved to CreateWrapper object");
			}
			this.m_Ptr = ptr;
		}

		private void Release()
		{
			if (this.m_Ptr != IntPtr.Zero && this.m_Owner)
			{
				VFXSpawnerState.Internal_Destroy(this.m_Ptr);
			}
			this.m_Ptr = IntPtr.Zero;
		}

		~VFXSpawnerState()
		{
			this.Release();
		}

		public void Dispose()
		{
			this.Release();
			GC.SuppressFinalize(this);
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		public extern bool playing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float spawnCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float deltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float totalTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern VFXEventAttribute vfxEventAttribute
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private IntPtr m_Ptr;

		private bool m_Owner;
	}
}
