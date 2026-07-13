using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Jobs
{
	[NativeType(Header = "Runtime/Transform/ScriptBindings/TransformAccess.bindings.h", CodegenOptions = CodegenOptions.Custom)]
	public struct TransformAccessArray : IDisposable
	{
		public TransformAccessArray(Transform[] transforms, int desiredJobCount = -1)
		{
			TransformAccessArray.Allocate(transforms.Length, desiredJobCount, out this);
			TransformAccessArray.SetTransforms(this.m_TransformArray, transforms);
		}

		public TransformAccessArray(NativeArray<TransformHandle> transformHandles, int desiredJobCount = -1)
		{
			TransformAccessArray.Allocate(transformHandles.Length, desiredJobCount, out this);
			TransformAccessArray.SetTransformHandles(this.m_TransformArray, transformHandles.GetUnsafeReadOnlyPtr<TransformHandle>(), transformHandles.Length);
		}

		public TransformAccessArray(int capacity, int desiredJobCount = -1)
		{
			TransformAccessArray.Allocate(capacity, desiredJobCount, out this);
		}

		public static void Allocate(int capacity, int desiredJobCount, out TransformAccessArray array)
		{
			array.m_TransformArray = TransformAccessArray.Create(capacity, desiredJobCount);
			UnsafeUtility.LeakRecord(array.m_TransformArray, LeakCategory.TransformAccessArray, 0);
		}

		public bool isCreated
		{
			get
			{
				return this.m_TransformArray != IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			UnsafeUtility.LeakErase(this.m_TransformArray, LeakCategory.TransformAccessArray);
			TransformAccessArray.DestroyTransformAccessArray(this.m_TransformArray);
			this.m_TransformArray = IntPtr.Zero;
		}

		internal IntPtr GetTransformAccessArrayForSchedule()
		{
			return this.m_TransformArray;
		}

		public Transform this[int index]
		{
			get
			{
				return TransformAccessArray.GetTransform(this.m_TransformArray, index);
			}
			set
			{
				TransformAccessArray.SetTransform(this.m_TransformArray, index, value);
			}
		}

		public TransformHandle GetTransformHandle(int index)
		{
			return TransformAccessArray.GetTransformHandleInternal(this.m_TransformArray, index);
		}

		public void SetTransformHandle(int index, TransformHandle transformHandle)
		{
			TransformAccessArray.SetTransformHandleInternal(this.m_TransformArray, index, transformHandle);
		}

		public int capacity
		{
			get
			{
				return TransformAccessArray.GetCapacity(this.m_TransformArray);
			}
			set
			{
				TransformAccessArray.SetCapacity(this.m_TransformArray, value);
			}
		}

		public int length
		{
			get
			{
				return TransformAccessArray.GetLength(this.m_TransformArray);
			}
		}

		public void Add(Transform transform)
		{
			TransformAccessArray.Add(this.m_TransformArray, transform);
		}

		[Obsolete("TransformAccessArray.Add(int) is obsolete. Use TransformAccessArray.Add(EntityId) instead.")]
		public void Add(int instanceId)
		{
			TransformAccessArray.AddInstanceId(this.m_TransformArray, instanceId);
		}

		public void Add(TransformHandle transformHandle)
		{
			TransformAccessArray.AddTransformHandle(this.m_TransformArray, transformHandle);
		}

		public void Add(EntityId entityId)
		{
			TransformAccessArray.AddInstanceId(this.m_TransformArray, entityId);
		}

		public void RemoveAtSwapBack(int index)
		{
			TransformAccessArray.RemoveAtSwapBack(this.m_TransformArray, index);
		}

		public void SetTransforms(Transform[] transforms)
		{
			TransformAccessArray.SetTransforms(this.m_TransformArray, transforms);
		}

		public void SetTransformHandles(NativeArray<TransformHandle> transformHandles)
		{
			TransformAccessArray.SetTransformHandles(this.m_TransformArray, transformHandles.GetUnsafeReadOnlyPtr<TransformHandle>(), transformHandles.Length);
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::Create", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(int capacity, int desiredJobCount);

		[NativeMethod(Name = "DestroyTransformAccessArray", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyTransformAccessArray(IntPtr transformArray);

		[NativeMethod(Name = "TransformAccessArrayBindings::SetTransforms", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransforms(IntPtr transformArrayIntPtr, Transform[] transforms);

		[NativeMethod(Name = "TransformAccessArrayBindings::SetTransformHandles", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetTransformHandles(IntPtr transformArrayIntPtr, void* transformHandles, int count);

		[NativeMethod(Name = "TransformAccessArrayBindings::AddTransform", IsFreeFunction = true)]
		private static void Add(IntPtr transformArrayIntPtr, Transform transform)
		{
			TransformAccessArray.Add_Injected(transformArrayIntPtr, Object.MarshalledUnityObject.Marshal<Transform>(transform));
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::AddTransformHandle", IsFreeFunction = true)]
		private static void AddTransformHandle(IntPtr transformArrayIntPtr, TransformHandle transformHandle)
		{
			TransformAccessArray.AddTransformHandle_Injected(transformArrayIntPtr, ref transformHandle);
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::AddTransformInstanceId", IsFreeFunction = true)]
		private static void AddInstanceId(IntPtr transformArrayIntPtr, EntityId instanceId)
		{
			TransformAccessArray.AddInstanceId_Injected(transformArrayIntPtr, ref instanceId);
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::RemoveAtSwapBack", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveAtSwapBack(IntPtr transformArrayIntPtr, int index);

		[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedTransformAccess", IsThreadSafe = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSortedTransformAccess(IntPtr transformArrayIntPtr);

		[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedToUserIndex", IsThreadSafe = true, IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSortedToUserIndex(IntPtr transformArrayIntPtr);

		[NativeMethod(Name = "TransformAccessArrayBindings::GetLength", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLength(IntPtr transformArrayIntPtr);

		[NativeMethod(Name = "TransformAccessArrayBindings::GetCapacity", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCapacity(IntPtr transformArrayIntPtr);

		[NativeMethod(Name = "TransformAccessArrayBindings::SetCapacity", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCapacity(IntPtr transformArrayIntPtr, int capacity);

		[NativeMethod(Name = "TransformAccessArrayBindings::GetTransform", IsFreeFunction = true, ThrowsException = true)]
		internal static Transform GetTransform(IntPtr transformArrayIntPtr, int index)
		{
			return Unmarshal.UnmarshalUnityObject<Transform>(TransformAccessArray.GetTransform_Injected(transformArrayIntPtr, index));
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::SetTransform", IsFreeFunction = true, ThrowsException = true)]
		internal static void SetTransform(IntPtr transformArrayIntPtr, int index, Transform transform)
		{
			TransformAccessArray.SetTransform_Injected(transformArrayIntPtr, index, Object.MarshalledUnityObject.Marshal<Transform>(transform));
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::GetTransformHandle", IsFreeFunction = true, ThrowsException = true)]
		internal static TransformHandle GetTransformHandleInternal(IntPtr transformArrayIntPtr, int index)
		{
			TransformHandle transformHandle;
			TransformAccessArray.GetTransformHandleInternal_Injected(transformArrayIntPtr, index, out transformHandle);
			return transformHandle;
		}

		[NativeMethod(Name = "TransformAccessArrayBindings::SetTransformHandle", IsFreeFunction = true, ThrowsException = true)]
		internal static void SetTransformHandleInternal(IntPtr transformArrayIntPtr, int index, TransformHandle transformHandle)
		{
			TransformAccessArray.SetTransformHandleInternal_Injected(transformArrayIntPtr, index, ref transformHandle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Add_Injected(IntPtr transformArrayIntPtr, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTransformHandle_Injected(IntPtr transformArrayIntPtr, [In] ref TransformHandle transformHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddInstanceId_Injected(IntPtr transformArrayIntPtr, [In] ref EntityId instanceId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTransform_Injected(IntPtr transformArrayIntPtr, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransform_Injected(IntPtr transformArrayIntPtr, int index, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTransformHandleInternal_Injected(IntPtr transformArrayIntPtr, int index, out TransformHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransformHandleInternal_Injected(IntPtr transformArrayIntPtr, int index, [In] ref TransformHandle transformHandle);

		private IntPtr m_TransformArray;
	}
}
