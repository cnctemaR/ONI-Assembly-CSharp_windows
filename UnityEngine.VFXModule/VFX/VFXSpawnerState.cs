using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RequiredByNativeCode]
	[NativeType(Header = "Modules/VFX/Public/VFXSpawnerState.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class VFXSpawnerState : IDisposable
	{
		public VFXSpawnerState()
			: this(VFXSpawnerState.Internal_Create(), true)
		{
		}

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
			VFXSpawnerState vfxspawnerState = new VFXSpawnerState(IntPtr.Zero, false);
			vfxspawnerState.PrepareWrapper();
			return vfxspawnerState;
		}

		private void PrepareWrapper()
		{
			bool owner = this.m_Owner;
			if (owner)
			{
				throw new Exception("VFXSpawnerState : SetWrapValue is reserved to CreateWrapper object");
			}
			bool flag = this.m_WrapEventAttribute != null;
			if (flag)
			{
				throw new Exception("VFXSpawnerState : Unexpected calling twice prepare wrapper");
			}
			this.m_WrapEventAttribute = VFXEventAttribute.CreateEventAttributeWrapper();
		}

		[RequiredByNativeCode]
		internal void SetWrapValue(IntPtr ptrToSpawnerState, IntPtr ptrToEventAttribute)
		{
			bool owner = this.m_Owner;
			if (owner)
			{
				throw new Exception("VFXSpawnerState : SetWrapValue is reserved to CreateWrapper object");
			}
			bool flag = this.m_WrapEventAttribute == null;
			if (flag)
			{
				throw new Exception("VFXSpawnerState : Missing PrepareWrapper");
			}
			this.m_Ptr = ptrToSpawnerState;
			this.m_WrapEventAttribute.SetWrapValue(ptrToEventAttribute);
		}

		internal IntPtr GetPtr()
		{
			return this.m_Ptr;
		}

		private void Release()
		{
			bool flag = this.m_Ptr != IntPtr.Zero && this.m_Owner;
			if (flag)
			{
				VFXSpawnerState.Internal_Destroy(this.m_Ptr);
			}
			this.m_Ptr = IntPtr.Zero;
			this.m_WrapEventAttribute = null;
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

		public bool playing
		{
			get
			{
				return this.loopState == VFXSpawnerLoopState.Looping;
			}
			set
			{
				this.loopState = (value ? VFXSpawnerLoopState.Looping : VFXSpawnerLoopState.Finished);
			}
		}

		public bool newLoop
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_newLoop_Injected(intPtr);
			}
		}

		public VFXSpawnerLoopState loopState
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_loopState_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_loopState_Injected(intPtr, value);
			}
		}

		public float spawnCount
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_spawnCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_spawnCount_Injected(intPtr, value);
			}
		}

		public float deltaTime
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_deltaTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_deltaTime_Injected(intPtr, value);
			}
		}

		public float totalTime
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_totalTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_totalTime_Injected(intPtr, value);
			}
		}

		public float delayBeforeLoop
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_delayBeforeLoop_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_delayBeforeLoop_Injected(intPtr, value);
			}
		}

		public float loopDuration
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_loopDuration_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_loopDuration_Injected(intPtr, value);
			}
		}

		public float delayAfterLoop
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_delayAfterLoop_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_delayAfterLoop_Injected(intPtr, value);
			}
		}

		public int loopIndex
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_loopIndex_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_loopIndex_Injected(intPtr, value);
			}
		}

		public int loopCount
		{
			get
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VFXSpawnerState.get_loopCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VFXSpawnerState.set_loopCount_Injected(intPtr, value);
			}
		}

		internal VFXEventAttribute Internal_GetVFXEventAttribute()
		{
			IntPtr intPtr = VFXSpawnerState.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = VFXSpawnerState.Internal_GetVFXEventAttribute_Injected(intPtr);
			return (intPtr2 == 0) ? null : VFXEventAttribute.BindingsMarshaller.ConvertToManaged(intPtr2);
		}

		public VFXEventAttribute vfxEventAttribute
		{
			get
			{
				bool flag = !this.m_Owner && this.m_WrapEventAttribute != null;
				VFXEventAttribute vfxeventAttribute;
				if (flag)
				{
					vfxeventAttribute = this.m_WrapEventAttribute;
				}
				else
				{
					vfxeventAttribute = this.Internal_GetVFXEventAttribute();
				}
				return vfxeventAttribute;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_newLoop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VFXSpawnerLoopState get_loopState_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopState_Injected(IntPtr _unity_self, VFXSpawnerLoopState value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_spawnCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spawnCount_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_deltaTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_deltaTime_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_totalTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_totalTime_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_delayBeforeLoop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_delayBeforeLoop_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_loopDuration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopDuration_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_delayAfterLoop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_delayAfterLoop_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_loopIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopIndex_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_loopCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetVFXEventAttribute_Injected(IntPtr _unity_self);

		private IntPtr m_Ptr;

		private bool m_Owner;

		private VFXEventAttribute m_WrapEventAttribute;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VFXSpawnerState vfxSpawnerState)
			{
				return vfxSpawnerState.m_Ptr;
			}
		}
	}
}
