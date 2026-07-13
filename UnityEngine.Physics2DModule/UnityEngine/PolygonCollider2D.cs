using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/PolygonCollider2D.h")]
	public sealed class PolygonCollider2D : Collider2D
	{
		public bool useDelaunayMesh
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PolygonCollider2D.get_useDelaunayMesh_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PolygonCollider2D.set_useDelaunayMesh_Injected(intPtr, value);
			}
		}

		public bool autoTiling
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PolygonCollider2D.get_autoTiling_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PolygonCollider2D.set_autoTiling_Injected(intPtr, value);
			}
		}

		[NativeMethod("GetPointCount")]
		public int GetTotalPointCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PolygonCollider2D.GetTotalPointCount_Injected(intPtr);
		}

		public unsafe Vector2[] points
		{
			[NativeMethod("GetPoints_Binding")]
			get
			{
				Vector2[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					PolygonCollider2D.get_points_Injected(intPtr, out blittableArrayWrapper);
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
			[NativeMethod("SetPoints_Binding")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector2> span = new Span<Vector2>(value);
				fixed (Vector2* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					PolygonCollider2D.set_points_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public int pathCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PolygonCollider2D.get_pathCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PolygonCollider2D.set_pathCount_Injected(intPtr, value);
			}
		}

		public Vector2[] GetPath(int index)
		{
			bool flag = index >= this.pathCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Path {0} does not exist.", index));
			}
			bool flag2 = index < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Path {0} does not exist; negative path index is invalid.", index));
			}
			return this.GetPath_Internal(index);
		}

		[NativeMethod("GetPath_Binding")]
		private Vector2[] GetPath_Internal(int index)
		{
			Vector2[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				PolygonCollider2D.GetPath_Internal_Injected(intPtr, index, out blittableArrayWrapper);
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

		public void SetPath(int index, Vector2[] points)
		{
			bool flag = index < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Negative path index {0} is invalid.", index));
			}
			this.SetPath_Internal(index, points);
		}

		[NativeMethod("SetPath_Binding")]
		private unsafe void SetPath_Internal(int index, [NotNull] Vector2[] points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector2> span = new Span<Vector2>(points);
			fixed (Vector2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				PolygonCollider2D.SetPath_Internal_Injected(intPtr, index, ref managedSpanWrapper);
			}
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
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
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
					pathList_Internal_Injected = PolygonCollider2D.GetPathList_Internal_Injected(intPtr, index, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(points);
			}
			return pathList_Internal_Injected;
		}

		public void SetPath(int index, List<Vector2> points)
		{
			bool flag = index < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Negative path index {0} is invalid.", index));
			}
			this.SetPathList_Internal(index, points);
		}

		[NativeMethod("SetPathList_Binding")]
		private unsafe void SetPathList_Internal(int index, [NotNull] List<Vector2> points)
		{
			if (points == null)
			{
				ThrowHelper.ThrowArgumentNullException(points, "points");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
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
					PolygonCollider2D.SetPathList_Internal_Injected(intPtr, index, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(points);
			}
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides)
		{
			this.CreatePrimitive(sides, Vector2.one, Vector2.zero);
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides, Vector2 scale)
		{
			this.CreatePrimitive(sides, scale, Vector2.zero);
		}

		public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
		{
			bool flag = sides < 3;
			if (flag)
			{
				Debug.LogWarning("Cannot create a 2D polygon primitive collider with less than two sides.", this);
			}
			else
			{
				bool flag2 = scale.x <= 0f || scale.y <= 0f;
				if (flag2)
				{
					Debug.LogWarning("Cannot create a 2D polygon primitive collider with an axis scale less than or equal to zero.", this);
				}
				else
				{
					this.CreatePrimitive_Internal(sides, scale, offset, true);
				}
			}
		}

		[NativeMethod("CreatePrimitive")]
		private void CreatePrimitive_Internal(int sides, Vector2 scale, Vector2 offset, bool recreateCollider)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PolygonCollider2D.CreatePrimitive_Internal_Injected(intPtr, sides, ref scale, ref offset, recreateCollider);
		}

		public bool CreateFromSprite(Sprite sprite, [DefaultValue("0.25f")] float detail = 0.25f, [DefaultValue("200")] byte alphaTolerance = 200, [DefaultValue("true")] bool holeDetection = true, [DefaultValue("true")] bool usePhysicsShapes = true)
		{
			bool flag = sprite == null;
			bool flag2;
			if (flag)
			{
				Debug.LogWarning("Sprite cannot be NULL.", this);
				flag2 = false;
			}
			else
			{
				bool flag3 = detail < 0f || detail > 1f;
				if (flag3)
				{
					Debug.LogWarning("Detail must be in the range [0, 1].", this);
					flag2 = false;
				}
				else
				{
					flag2 = this.CreateFromSprite_Internal(sprite, detail, alphaTolerance, holeDetection, true, usePhysicsShapes);
				}
			}
			return flag2;
		}

		[NativeMethod("CreateFromSprite")]
		private bool CreateFromSprite_Internal([NotNull] Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, bool recreateCollider, bool usePhysicsShapes)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PolygonCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			return PolygonCollider2D.CreateFromSprite_Internal_Injected(intPtr, intPtr2, detail, alphaTolerance, holeDetection, recreateCollider, usePhysicsShapes);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useDelaunayMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useDelaunayMesh_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoTiling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoTiling_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTotalPointCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_points_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_points_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pathCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pathCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPath_Internal_Injected(IntPtr _unity_self, int index, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPath_Internal_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreatePrimitive_Internal_Injected(IntPtr _unity_self, int sides, [In] ref Vector2 scale, [In] ref Vector2 offset, bool recreateCollider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateFromSprite_Internal_Injected(IntPtr _unity_self, IntPtr sprite, float detail, byte alphaTolerance, bool holeDetection, bool recreateCollider, bool usePhysicsShapes);
	}
}
