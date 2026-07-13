using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Camera/CullingGroup.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class CullingGroup : IDisposable
	{
		public CullingGroup()
		{
			this.m_Ptr = CullingGroup.Init(this);
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					this.FinalizerFailure();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		[FreeFunction("CullingGroup_Bindings::Dispose", HasExplicitThis = true)]
		private void DisposeInternal()
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.DisposeInternal_Injected(intPtr);
		}

		public void Dispose()
		{
			this.DisposeInternal();
			this.m_Ptr = IntPtr.Zero;
		}

		public CullingGroup.StateChanged onStateChanged
		{
			get
			{
				return this.m_OnStateChanged;
			}
			set
			{
				this.m_OnStateChanged = value;
			}
		}

		public bool enabled
		{
			get
			{
				IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CullingGroup.get_enabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CullingGroup.set_enabled_Injected(intPtr, value);
			}
		}

		public Camera targetCamera
		{
			get
			{
				IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Camera>(CullingGroup.get_targetCamera_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CullingGroup.set_targetCamera_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(value));
			}
		}

		public void SetBoundingSpheres([UnityMarshalAs(NativeType.ScriptingObjectPtr)] BoundingSphere[] array)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.SetBoundingSpheres_Injected(intPtr, array);
		}

		public void SetBoundingSphereCount(int count)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.SetBoundingSphereCount_Injected(intPtr, count);
		}

		public void EraseSwapBack(int index)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.EraseSwapBack_Injected(intPtr, index);
		}

		public static void EraseSwapBack<T>(int index, T[] myArray, ref int size)
		{
			size--;
			myArray[index] = myArray[size];
		}

		public int QueryIndices(bool visible, int[] result, int firstIndex)
		{
			return this.QueryIndices(visible, -1, CullingQueryOptions.IgnoreDistance, result, firstIndex);
		}

		public int QueryIndices(int distanceIndex, int[] result, int firstIndex)
		{
			return this.QueryIndices(false, distanceIndex, CullingQueryOptions.IgnoreVisibility, result, firstIndex);
		}

		public int QueryIndices(bool visible, int distanceIndex, int[] result, int firstIndex)
		{
			return this.QueryIndices(visible, distanceIndex, CullingQueryOptions.Normal, result, firstIndex);
		}

		[NativeThrows]
		[FreeFunction("CullingGroup_Bindings::QueryIndices", HasExplicitThis = true)]
		private unsafe int QueryIndices(bool visible, int distanceIndex, CullingQueryOptions options, int[] result, int firstIndex)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = new Span<int>(result);
			int num;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = CullingGroup.QueryIndices_Injected(intPtr, visible, distanceIndex, options, ref managedSpanWrapper, firstIndex);
			}
			return num;
		}

		[FreeFunction("CullingGroup_Bindings::IsVisible", HasExplicitThis = true)]
		[NativeThrows]
		public bool IsVisible(int index)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CullingGroup.IsVisible_Injected(intPtr, index);
		}

		[NativeThrows]
		[FreeFunction("CullingGroup_Bindings::GetDistance", HasExplicitThis = true)]
		public int GetDistance(int index)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CullingGroup.GetDistance_Injected(intPtr, index);
		}

		[FreeFunction("CullingGroup_Bindings::SetBoundingDistances", HasExplicitThis = true)]
		public unsafe void SetBoundingDistances(float[] distances)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(distances);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CullingGroup.SetBoundingDistances_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction("CullingGroup_Bindings::SetDistanceReferencePoint", HasExplicitThis = true)]
		private void SetDistanceReferencePoint_InternalVector3(Vector3 point)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.SetDistanceReferencePoint_InternalVector3_Injected(intPtr, ref point);
		}

		[NativeMethod("SetDistanceReferenceTransform")]
		private void SetDistanceReferencePoint_InternalTransform(Transform transform)
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.SetDistanceReferencePoint_InternalTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(transform));
		}

		public void SetDistanceReferencePoint(Vector3 point)
		{
			this.SetDistanceReferencePoint_InternalVector3(point);
		}

		public void SetDistanceReferencePoint(Transform transform)
		{
			this.SetDistanceReferencePoint_InternalTransform(transform);
		}

		[RequiredByNativeCode]
		[SecuritySafeCritical]
		private unsafe static void SendEvents(CullingGroup cullingGroup, IntPtr eventsPtr, int count)
		{
			CullingGroupEvent* ptr = (CullingGroupEvent*)eventsPtr.ToPointer();
			bool flag = cullingGroup.m_OnStateChanged == null;
			if (!flag)
			{
				for (int i = 0; i < count; i++)
				{
					cullingGroup.m_OnStateChanged(ptr[i]);
				}
			}
		}

		[FreeFunction("CullingGroup_Bindings::Init")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Init(object scripting);

		[FreeFunction("CullingGroup_Bindings::FinalizerFailure", HasExplicitThis = true, IsThreadSafe = true)]
		private void FinalizerFailure()
		{
			IntPtr intPtr = CullingGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CullingGroup.FinalizerFailure_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_targetCamera_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetCamera_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoundingSpheres_Injected(IntPtr _unity_self, BoundingSphere[] array);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoundingSphereCount_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EraseSwapBack_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int QueryIndices_Injected(IntPtr _unity_self, bool visible, int distanceIndex, CullingQueryOptions options, ref ManagedSpanWrapper result, int firstIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsVisible_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDistance_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoundingDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper distances);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDistanceReferencePoint_InternalVector3_Injected(IntPtr _unity_self, [In] ref Vector3 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDistanceReferencePoint_InternalTransform_Injected(IntPtr _unity_self, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FinalizerFailure_Injected(IntPtr _unity_self);

		internal IntPtr m_Ptr;

		private CullingGroup.StateChanged m_OnStateChanged = null;

		public delegate void StateChanged(CullingGroupEvent sphere);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(CullingGroup cullingGroup)
			{
				return cullingGroup.m_Ptr;
			}
		}
	}
}
