using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/BlobObject/BlobObject.h")]
	internal class BlobObject : Object
	{
		public BlobObject()
		{
			BlobObject.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] BlobObject self);

		[NativeMethod(IsThreadSafe = true)]
		internal unsafe void* GetBlobData(out ulong typeHash, out uint size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BlobObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return BlobObject.GetBlobData_Injected(intPtr, out typeHash, out size);
		}

		[NativeMethod(IsThreadSafe = false)]
		internal unsafe void SetBlobData(ulong typeHash, void* ptr, uint size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BlobObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BlobObject.SetBlobData_Injected(intPtr, typeHash, ptr, size);
		}

		[NativeMethod(IsThreadSafe = false)]
		internal IntPtr GetRootReference()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BlobObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return BlobObject.GetRootReference_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = false)]
		internal void SetNestedReferenceValue(ref FixedBlobObjectReference blobObjectReference, BlobObject blobObject)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BlobObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BlobObject.SetNestedReferenceValue_Injected(intPtr, ref blobObjectReference, Object.MarshalledUnityObject.Marshal<BlobObject>(blobObject));
		}

		[NativeMethod(IsThreadSafe = false)]
		internal void ReinitializeNestedReferences()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BlobObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BlobObject.ReinitializeNestedReferences_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetBlobData_Injected(IntPtr _unity_self, out ulong typeHash, out uint size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBlobData_Injected(IntPtr _unity_self, ulong typeHash, void* ptr, uint size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRootReference_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNestedReferenceValue_Injected(IntPtr _unity_self, ref FixedBlobObjectReference blobObjectReference, IntPtr blobObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReinitializeNestedReferences_Injected(IntPtr _unity_self);
	}
}
