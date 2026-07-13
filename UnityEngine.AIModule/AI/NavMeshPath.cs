using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[NativeHeader("Modules/AI/NavMeshPath.bindings.h")]
	[MovedFrom("UnityEngine")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class NavMeshPath
	{
		public NavMeshPath()
		{
			this.m_Ptr = NavMeshPath.InitializeNavMeshPath();
		}

		~NavMeshPath()
		{
			NavMeshPath.DestroyNavMeshPath(this.m_Ptr);
			this.m_Ptr = IntPtr.Zero;
		}

		[FreeFunction("NavMeshPathScriptBindings::InitializeNavMeshPath")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitializeNavMeshPath();

		[FreeFunction("NavMeshPathScriptBindings::DestroyNavMeshPath", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyNavMeshPath(IntPtr ptr);

		[FreeFunction("NavMeshPathScriptBindings::GetCornersNonAlloc", HasExplicitThis = true)]
		public unsafe int GetCornersNonAlloc([Out] Vector3[] results)
		{
			int cornersNonAlloc_Injected;
			try
			{
				IntPtr intPtr = NavMeshPath.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (results != null)
				{
					fixed (Vector3[] array = results)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				cornersNonAlloc_Injected = NavMeshPath.GetCornersNonAlloc_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				Vector3[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector3>(ref array);
			}
			return cornersNonAlloc_Injected;
		}

		[FreeFunction("NavMeshPathScriptBindings::CalculateCornersInternal", HasExplicitThis = true)]
		private Vector3[] CalculateCornersInternal()
		{
			Vector3[] array2;
			try
			{
				IntPtr intPtr = NavMeshPath.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				NavMeshPath.CalculateCornersInternal_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector3[] array;
				blittableArrayWrapper.Unmarshal<Vector3>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("NavMeshPathScriptBindings::ClearCornersInternal", HasExplicitThis = true)]
		private void ClearCornersInternal()
		{
			IntPtr intPtr = NavMeshPath.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshPath.ClearCornersInternal_Injected(intPtr);
		}

		public void ClearCorners()
		{
			this.ClearCornersInternal();
			this.m_Corners = null;
		}

		private void CalculateCorners()
		{
			bool flag = this.m_Corners == null;
			if (flag)
			{
				this.m_Corners = this.CalculateCornersInternal();
			}
		}

		public Vector3[] corners
		{
			get
			{
				this.CalculateCorners();
				return this.m_Corners;
			}
		}

		public NavMeshPathStatus status
		{
			get
			{
				IntPtr intPtr = NavMeshPath.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshPath.get_status_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCornersNonAlloc_Injected(IntPtr _unity_self, out BlittableArrayWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateCornersInternal_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearCornersInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NavMeshPathStatus get_status_Injected(IntPtr _unity_self);

		internal IntPtr m_Ptr;

		internal Vector3[] m_Corners;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(NavMeshPath navMeshPath)
			{
				return navMeshPath.m_Ptr;
			}
		}
	}
}
