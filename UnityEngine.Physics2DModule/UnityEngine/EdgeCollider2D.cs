using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/EdgeCollider2D.h")]
	public sealed class EdgeCollider2D : Collider2D
	{
		public void Reset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			EdgeCollider2D.Reset_Injected(intPtr);
		}

		public float edgeRadius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return EdgeCollider2D.get_edgeRadius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				EdgeCollider2D.set_edgeRadius_Injected(intPtr, value);
			}
		}

		public int edgeCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return EdgeCollider2D.get_edgeCount_Injected(intPtr);
			}
		}

		public int pointCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return EdgeCollider2D.get_pointCount_Injected(intPtr);
			}
		}

		public unsafe Vector2[] points
		{
			get
			{
				Vector2[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					EdgeCollider2D.get_points_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector2[] array;
					blittableArrayWrapper.Unmarshal<Vector2>(ref array);
					array2 = array;
				}
				return array2;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector2> span = new Span<Vector2>(value);
				fixed (Vector2* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					EdgeCollider2D.set_points_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[NativeMethod("GetPoints_Binding")]
		public unsafe int GetPoints([NotNull] List<Vector2> points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			int points_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector2[] array = NoAllocHelpers.ExtractArrayFromList<Vector2>(points))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, points.Count);
					points_Injected = EdgeCollider2D.GetPoints_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(points);
			}
			return points_Injected;
		}

		[NativeMethod("SetPoints_Binding")]
		public unsafe bool SetPoints([NotNull] List<Vector2> points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector2[] array = NoAllocHelpers.ExtractArrayFromList<Vector2>(points))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, points.Count);
					flag = EdgeCollider2D.SetPoints_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(points);
			}
			return flag;
		}

		public bool useAdjacentStartPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return EdgeCollider2D.get_useAdjacentStartPoint_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				EdgeCollider2D.set_useAdjacentStartPoint_Injected(intPtr, value);
			}
		}

		public bool useAdjacentEndPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return EdgeCollider2D.get_useAdjacentEndPoint_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				EdgeCollider2D.set_useAdjacentEndPoint_Injected(intPtr, value);
			}
		}

		public Vector2 adjacentStartPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				EdgeCollider2D.get_adjacentStartPoint_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				EdgeCollider2D.set_adjacentStartPoint_Injected(intPtr, ref value);
			}
		}

		public Vector2 adjacentEndPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				EdgeCollider2D.get_adjacentEndPoint_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<EdgeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				EdgeCollider2D.set_adjacentEndPoint_Injected(intPtr, ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_edgeRadius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_edgeRadius_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_edgeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pointCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_points_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_points_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPoints_Injected(IntPtr _unity_self, ref BlittableListWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPoints_Injected(IntPtr _unity_self, ref BlittableListWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useAdjacentStartPoint_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useAdjacentStartPoint_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useAdjacentEndPoint_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useAdjacentEndPoint_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_adjacentStartPoint_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_adjacentStartPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_adjacentEndPoint_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_adjacentEndPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);
	}
}
