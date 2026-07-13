using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshingSubsystem.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRMeshSubsystem : IntegratedSubsystem<XRMeshSubsystemDescriptor>
	{
		public bool TryGetMeshInfos(List<MeshInfo> meshInfosOut)
		{
			bool flag = meshInfosOut == null;
			if (flag)
			{
				throw new ArgumentNullException("meshInfosOut");
			}
			return this.GetMeshInfosAsList(meshInfosOut);
		}

		private unsafe bool GetMeshInfosAsList(List<MeshInfo> meshInfos)
		{
			bool meshInfosAsList_Injected;
			try
			{
				IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (meshInfos != null)
				{
					fixed (MeshInfo[] array = NoAllocHelpers.ExtractArrayFromList<MeshInfo>(meshInfos))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, meshInfos.Count);
					}
				}
				meshInfosAsList_Injected = XRMeshSubsystem.GetMeshInfosAsList_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<MeshInfo>(meshInfos);
			}
			return meshInfosAsList_Injected;
		}

		private MeshInfo[] GetMeshInfosAsFixedArray()
		{
			MeshInfo[] array2;
			try
			{
				IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				XRMeshSubsystem.GetMeshInfosAsFixedArray_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MeshInfo[] array;
				blittableArrayWrapper.Unmarshal<MeshInfo>(ref array);
				array2 = array;
			}
			return array2;
		}

		public void GenerateMeshAsync(MeshId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			this.GenerateMeshAsync(meshId, mesh, meshCollider, attributes, onMeshGenerationComplete, MeshGenerationOptions.None);
		}

		public void GenerateMeshAsync(MeshId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete, MeshGenerationOptions options)
		{
			IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRMeshSubsystem.GenerateMeshAsync_Injected(intPtr, ref meshId, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), Object.MarshalledUnityObject.Marshal<MeshCollider>(meshCollider), attributes, onMeshGenerationComplete, options);
		}

		[RequiredByNativeCode]
		private void InvokeMeshReadyDelegate(MeshGenerationResult result, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			bool flag = onMeshGenerationComplete != null;
			if (flag)
			{
				onMeshGenerationComplete(result);
			}
		}

		public float meshDensity
		{
			get
			{
				IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRMeshSubsystem.get_meshDensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRMeshSubsystem.set_meshDensity_Injected(intPtr, value);
			}
		}

		public bool SetBoundingVolume(Vector3 origin, Vector3 extents)
		{
			IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRMeshSubsystem.SetBoundingVolume_Injected(intPtr, ref origin, ref extents);
		}

		public unsafe NativeArray<MeshTransform> GetUpdatedMeshTransforms(Allocator allocator)
		{
			NativeArray<MeshTransform> nativeArray2;
			using (XRMeshSubsystem.MeshTransformList meshTransformList = new XRMeshSubsystem.MeshTransformList(this.GetUpdatedMeshTransforms()))
			{
				NativeArray<MeshTransform> nativeArray = new NativeArray<MeshTransform>(meshTransformList.Count, allocator, NativeArrayOptions.UninitializedMemory);
				UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr<MeshTransform>(), meshTransformList.Data.ToPointer(), (long)(meshTransformList.Count * sizeof(MeshTransform)));
				nativeArray2 = nativeArray;
			}
			return nativeArray2;
		}

		private IntPtr GetUpdatedMeshTransforms()
		{
			IntPtr intPtr = XRMeshSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRMeshSubsystem.GetUpdatedMeshTransforms_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetMeshInfosAsList_Injected(IntPtr _unity_self, ref BlittableListWrapper meshInfos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMeshInfosAsFixedArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateMeshAsync_Injected(IntPtr _unity_self, [In] ref MeshId meshId, IntPtr mesh, IntPtr meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete, MeshGenerationOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_meshDensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_meshDensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetBoundingVolume_Injected(IntPtr _unity_self, [In] ref Vector3 origin, [In] ref Vector3 extents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetUpdatedMeshTransforms_Injected(IntPtr _unity_self);

		[NativeConditional("ENABLE_XR")]
		private readonly struct MeshTransformList : IDisposable
		{
			public MeshTransformList(IntPtr self)
			{
				this.m_Self = self;
			}

			public int Count
			{
				get
				{
					return XRMeshSubsystem.MeshTransformList.GetLength(this.m_Self);
				}
			}

			public IntPtr Data
			{
				get
				{
					return XRMeshSubsystem.MeshTransformList.GetData(this.m_Self);
				}
			}

			public void Dispose()
			{
				XRMeshSubsystem.MeshTransformList.Dispose(this.m_Self);
			}

			[FreeFunction("UnityXRMeshTransformList_get_Length")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetLength(IntPtr self);

			[FreeFunction("UnityXRMeshTransformList_get_Data")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetData(IntPtr self);

			[FreeFunction("UnityXRMeshTransformList_Dispose")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void Dispose(IntPtr self);

			private readonly IntPtr m_Self;
		}

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRMeshSubsystem subsystem)
			{
				return subsystem.m_Ptr;
			}
		}
	}
}
