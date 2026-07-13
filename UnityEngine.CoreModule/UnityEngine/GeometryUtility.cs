using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("GeometryUtilityScripting", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public sealed class GeometryUtility
	{
		public static Plane[] CalculateFrustumPlanes(Camera camera)
		{
			Plane[] array = new Plane[6];
			GeometryUtility.CalculateFrustumPlanes(camera, array.AsSpan<Plane>());
			return array;
		}

		public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
		{
			Plane[] array = new Plane[6];
			GeometryUtility.CalculateFrustumPlanes(in worldToProjectionMatrix, array.AsSpan<Plane>());
			return array;
		}

		public static Plane[] CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix)
		{
			Plane[] array = new Plane[6];
			GeometryUtility.CalculateFrustumPlanes(in worldToProjectionMatrix, array.AsSpan<Plane>());
			return array;
		}

		public static void CalculateFrustumPlanes(Camera camera, Span<Plane> planes)
		{
			Matrix4x4 matrix4x = camera.projectionMatrix * camera.worldToCameraMatrix;
			GeometryUtility.CalculateFrustumPlanes(in matrix4x, planes);
		}

		public static void CalculateFrustumPlanes(Camera camera, Plane[] planes)
		{
			Matrix4x4 matrix4x = camera.projectionMatrix * camera.worldToCameraMatrix;
			GeometryUtility.CalculateFrustumPlanes(in matrix4x, planes.AsSpan<Plane>());
		}

		public static void CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix, Span<Plane> planes)
		{
			bool flag = planes == null;
			if (flag)
			{
				throw new ArgumentNullException("planes");
			}
			bool flag2 = planes.Length != 6;
			if (flag2)
			{
				throw new ArgumentException("Planes array must be of length 6.", "planes");
			}
			GeometryUtility.Internal_ExtractPlanes(planes, in worldToProjectionMatrix);
		}

		public static void CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix, Span<Plane> planes)
		{
			bool flag = planes == null;
			if (flag)
			{
				throw new ArgumentNullException("planes");
			}
			bool flag2 = planes.Length != 6;
			if (flag2)
			{
				throw new ArgumentException("Planes array must be of length 6.", "planes");
			}
			GeometryUtility.Internal_ExtractPlanes(planes, in worldToProjectionMatrix);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix, Plane[] planes)
		{
			GeometryUtility.CalculateFrustumPlanes(in worldToProjectionMatrix, planes.AsSpan<Plane>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix, Plane[] planes)
		{
			GeometryUtility.CalculateFrustumPlanes(in worldToProjectionMatrix, planes.AsSpan<Plane>());
		}

		public static Bounds CalculateBounds(Vector3[] positions, Matrix4x4 transform)
		{
			bool flag = positions == null;
			if (flag)
			{
				throw new ArgumentNullException("positions");
			}
			bool flag2 = positions.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.", "positions");
			}
			return GeometryUtility.Internal_CalculateBounds(positions, in transform);
		}

		public static Bounds CalculateBounds(Vector3[] positions, in Matrix4x4 transform)
		{
			bool flag = positions == null;
			if (flag)
			{
				throw new ArgumentNullException("positions");
			}
			bool flag2 = positions.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.", "positions");
			}
			return GeometryUtility.Internal_CalculateBounds(positions, in transform);
		}

		public static bool TryCreatePlaneFromPolygon(Vector3[] vertices, out Plane plane)
		{
			bool flag = vertices == null || vertices.Length < 3;
			bool flag2;
			if (flag)
			{
				plane = new Plane(Vector3.up, 0f);
				flag2 = false;
			}
			else
			{
				bool flag3 = vertices.Length == 3;
				if (flag3)
				{
					Vector3 vector = vertices[0];
					Vector3 vector2 = vertices[1];
					Vector3 vector3 = vertices[2];
					plane = new Plane(in vector, in vector2, in vector3);
					flag2 = plane.normal.sqrMagnitude > 0f;
				}
				else
				{
					Vector3 zero = Vector3.zero;
					int num = vertices.Length - 1;
					Vector3 vector4 = vertices[num];
					foreach (Vector3 vector5 in vertices)
					{
						zero.x += (vector4.y - vector5.y) * (vector4.z + vector5.z);
						zero.y += (vector4.z - vector5.z) * (vector4.x + vector5.x);
						zero.z += (vector4.x - vector5.x) * (vector4.y + vector5.y);
						vector4 = vector5;
					}
					zero.Normalize();
					float num2 = 0f;
					foreach (Vector3 vector6 in vertices)
					{
						num2 -= Vector3.Dot(in zero, in vector6);
					}
					num2 /= (float)vertices.Length;
					plane = new Plane(in zero, num2);
					flag2 = plane.normal.sqrMagnitude > 0f;
				}
			}
			return flag2;
		}

		[NativeName("TestPlanesAABB")]
		private unsafe static bool Internal_TestPlanesAABB(ReadOnlySpan<Plane> planes, in Bounds bounds)
		{
			ReadOnlySpan<Plane> readOnlySpan = planes;
			bool flag;
			fixed (Plane* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				flag = GeometryUtility.Internal_TestPlanesAABB_Injected(ref managedSpanWrapper, in bounds);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
		{
			return GeometryUtility.Internal_TestPlanesAABB(planes.AsSpan<Plane>(), in bounds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TestPlanesAABB(Plane[] planes, in Bounds bounds)
		{
			return GeometryUtility.Internal_TestPlanesAABB(planes.AsSpan<Plane>(), in bounds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TestPlanesAABB(ReadOnlySpan<Plane> planes, in Bounds bounds)
		{
			return GeometryUtility.Internal_TestPlanesAABB(planes, in bounds);
		}

		[NativeName("ExtractPlanes")]
		private unsafe static void Internal_ExtractPlanes(Span<Plane> planes, in Matrix4x4 worldToProjectionMatrix)
		{
			Span<Plane> span = planes;
			fixed (Plane* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				GeometryUtility.Internal_ExtractPlanes_Injected(ref managedSpanWrapper, in worldToProjectionMatrix);
			}
		}

		[NativeName("CalculateBounds")]
		private unsafe static Bounds Internal_CalculateBounds(Vector3[] positions, in Matrix4x4 transform)
		{
			Span<Vector3> span = new Span<Vector3>(positions);
			Bounds bounds;
			fixed (Vector3* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				GeometryUtility.Internal_CalculateBounds_Injected(ref managedSpanWrapper, in transform, out bounds);
			}
			return bounds;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TestPlanesAABB_Injected(ref ManagedSpanWrapper planes, in Bounds bounds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExtractPlanes_Injected(ref ManagedSpanWrapper planes, in Matrix4x4 worldToProjectionMatrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CalculateBounds_Injected(ref ManagedSpanWrapper positions, in Matrix4x4 transform, out Bounds ret);
	}
}
