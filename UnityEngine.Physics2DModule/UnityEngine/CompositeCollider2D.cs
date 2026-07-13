using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/CompositeCollider2D.h")]
	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class CompositeCollider2D : Collider2D
	{
		public CompositeCollider2D.GeometryType geometryType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_geometryType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_geometryType_Injected(intPtr, value);
			}
		}

		public CompositeCollider2D.GenerationType generationType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_generationType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_generationType_Injected(intPtr, value);
			}
		}

		public bool useDelaunayMesh
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_useDelaunayMesh_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_useDelaunayMesh_Injected(intPtr, value);
			}
		}

		public float vertexDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_vertexDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_vertexDistance_Injected(intPtr, value);
			}
		}

		public float edgeRadius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_edgeRadius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_edgeRadius_Injected(intPtr, value);
			}
		}

		public float offsetDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_offsetDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CompositeCollider2D.set_offsetDistance_Injected(intPtr, value);
			}
		}

		public void GenerateGeometry()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CompositeCollider2D.GenerateGeometry_Injected(intPtr);
		}

		[NativeMethod("GetCompositedColliders_Binding")]
		public int GetCompositedColliders([NotNull] List<Collider2D> colliders)
		{
			if (colliders == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliders, "colliders");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CompositeCollider2D.GetCompositedColliders_Injected(intPtr, colliders);
		}

		public int GetPathPointCount(int index)
		{
			int num = this.pathCount - 1;
			bool flag = index < 0 || index > num;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Path index {0} must be in the range of 0 to {1}.", index, num));
			}
			return this.GetPathPointCount_Internal(index);
		}

		[NativeMethod("GetPathPointCount_Binding")]
		private int GetPathPointCount_Internal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CompositeCollider2D.GetPathPointCount_Internal_Injected(intPtr, index);
		}

		public int pathCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_pathCount_Injected(intPtr);
			}
		}

		public int pointCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CompositeCollider2D.get_pointCount_Injected(intPtr);
			}
		}

		public int GetPath(int index, Vector2[] points)
		{
			bool flag = index < 0 || index >= this.pathCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Path index {0} must be in the range of 0 to {1}.", index, this.pathCount - 1));
			}
			bool flag2 = points == null;
			if (flag2)
			{
				throw new ArgumentNullException("points");
			}
			return this.GetPathArray_Internal(index, points);
		}

		[NativeMethod("GetPathArray_Binding")]
		private unsafe int GetPathArray_Internal(int index, [NotNull] Vector2[] points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector2> span = new Span<Vector2>(points);
			int pathArray_Internal_Injected;
			fixed (Vector2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				pathArray_Internal_Injected = CompositeCollider2D.GetPathArray_Internal_Injected(intPtr, index, ref managedSpanWrapper);
			}
			return pathArray_Internal_Injected;
		}

		public int GetPath(int index, List<Vector2> points)
		{
			bool flag = index < 0 || index >= this.pathCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Path index {0} must be in the range of 0 to {1}.", index, this.pathCount - 1));
			}
			bool flag2 = points == null;
			if (flag2)
			{
				throw new ArgumentNullException("points");
			}
			return this.GetPathList_Internal(index, points);
		}

		[NativeMethod("GetPathList_Binding")]
		private unsafe int GetPathList_Internal(int index, [NotNull] List<Vector2> points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			int pathList_Internal_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CompositeCollider2D>(this);
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
					pathList_Internal_Injected = CompositeCollider2D.GetPathList_Internal_Injected(intPtr, index, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(points);
			}
			return pathList_Internal_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CompositeCollider2D.GeometryType get_geometryType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_geometryType_Injected(IntPtr _unity_self, CompositeCollider2D.GeometryType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CompositeCollider2D.GenerationType get_generationType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_generationType_Injected(IntPtr _unity_self, CompositeCollider2D.GenerationType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useDelaunayMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useDelaunayMesh_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_vertexDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vertexDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_edgeRadius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_edgeRadius_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_offsetDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_offsetDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateGeometry_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCompositedColliders_Injected(IntPtr _unity_self, List<Collider2D> colliders);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPathPointCount_Internal_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pathCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pointCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPathArray_Internal_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);

		public enum GeometryType
		{
			Outlines,
			Polygons
		}

		public enum GenerationType
		{
			Synchronous,
			Manual
		}
	}
}
