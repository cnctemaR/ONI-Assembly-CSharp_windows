using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[StaticAccessor("GizmoBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Export/Gizmos/Gizmos.bindings.h")]
	public sealed class Gizmos
	{
		[NativeThrows]
		public static void DrawLine(Vector3 from, Vector3 to)
		{
			Gizmos.DrawLine_Injected(ref from, ref to);
		}

		[NativeThrows]
		public unsafe static void DrawLineStrip(ReadOnlySpan<Vector3> points, bool looped)
		{
			ReadOnlySpan<Vector3> readOnlySpan = points;
			fixed (Vector3* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				Gizmos.DrawLineStrip_Injected(ref managedSpanWrapper, looped);
			}
		}

		[NativeMethod(Name = "DrawLineList", ThrowsException = true)]
		internal unsafe static void DrawLineListInternal(ReadOnlySpan<Vector3> points)
		{
			ReadOnlySpan<Vector3> readOnlySpan = points;
			fixed (Vector3* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				Gizmos.DrawLineListInternal_Injected(ref managedSpanWrapper);
			}
		}

		public static void DrawLineList(ReadOnlySpan<Vector3> points)
		{
			bool flag = (points.Length & 1) != 0;
			if (flag)
			{
				throw new UnityException("You cannot draw a line list from an odd number of points, with two points per line the number of points must be even");
			}
			Gizmos.DrawLineListInternal(points);
		}

		[NativeThrows]
		public static void DrawWireSphere(Vector3 center, float radius)
		{
			Gizmos.DrawWireSphere_Injected(ref center, radius);
		}

		[NativeThrows]
		public static void DrawSphere(Vector3 center, float radius)
		{
			Gizmos.DrawSphere_Injected(ref center, radius);
		}

		[NativeThrows]
		public static void DrawWireCube(Vector3 center, Vector3 size)
		{
			Gizmos.DrawWireCube_Injected(ref center, ref size);
		}

		[NativeThrows]
		public static void DrawCube(Vector3 center, Vector3 size)
		{
			Gizmos.DrawCube_Injected(ref center, ref size);
		}

		[NativeThrows]
		public static void DrawMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawMesh_Injected(Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submeshIndex, ref position, ref rotation, ref scale);
		}

		[NativeThrows]
		public static void DrawWireMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawWireMesh_Injected(Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submeshIndex, ref position, ref rotation, ref scale);
		}

		[NativeThrows]
		public static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling)
		{
			Gizmos.DrawIcon(center, name, allowScaling, Color.white);
		}

		[NativeThrows]
		public unsafe static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling, [DefaultValue("Color(255,255,255,255)")] Color tint)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Gizmos.DrawIcon_Injected(ref center, ref managedSpanWrapper, allowScaling, ref tint);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
		{
			Gizmos.DrawGUITexture_Injected(ref screenRect, Object.MarshalledUnityObject.Marshal<Texture>(texture), leftBorder, rightBorder, topBorder, bottomBorder, Object.MarshalledUnityObject.Marshal<Material>(mat));
		}

		public static Color color
		{
			get
			{
				Color color;
				Gizmos.get_color_Injected(out color);
				return color;
			}
			set
			{
				Gizmos.set_color_Injected(ref value);
			}
		}

		public static Matrix4x4 matrix
		{
			get
			{
				Matrix4x4 matrix4x;
				Gizmos.get_matrix_Injected(out matrix4x);
				return matrix4x;
			}
			set
			{
				Gizmos.set_matrix_Injected(ref value);
			}
		}

		public static Texture exposure
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture>(Gizmos.get_exposure_Injected());
			}
			set
			{
				Gizmos.set_exposure_Injected(Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		public static extern float probeSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
		{
			Gizmos.DrawFrustum_Injected(ref center, fov, maxRange, minRange, aspect);
		}

		public static float CalculateLOD(Vector3 position, float radius)
		{
			return Gizmos.CalculateLOD_Injected(ref position, radius);
		}

		public static void DrawRay(Ray r)
		{
			Gizmos.DrawLine(r.origin, r.origin + r.direction);
		}

		public static void DrawRay(Vector3 from, Vector3 direction)
		{
			Gizmos.DrawLine(from, from + direction);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Vector3 one = Vector3.one;
			Gizmos.DrawMesh(mesh, position, rotation, one);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Gizmos.DrawMesh(mesh, position, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			Gizmos.DrawMesh(mesh, zero, identity, one);
		}

		public static void DrawMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawMesh(mesh, -1, position, rotation, scale);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
		{
			Vector3 one = Vector3.one;
			Gizmos.DrawMesh(mesh, submeshIndex, position, rotation, one);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Gizmos.DrawMesh(mesh, submeshIndex, position, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, int submeshIndex)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			Gizmos.DrawMesh(mesh, submeshIndex, zero, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Vector3 one = Vector3.one;
			Gizmos.DrawWireMesh(mesh, position, rotation, one);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh, Vector3 position)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Gizmos.DrawWireMesh(mesh, position, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			Gizmos.DrawWireMesh(mesh, zero, identity, one);
		}

		public static void DrawWireMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawWireMesh(mesh, -1, position, rotation, scale);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
		{
			Vector3 one = Vector3.one;
			Gizmos.DrawWireMesh(mesh, submeshIndex, position, rotation, one);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Gizmos.DrawWireMesh(mesh, submeshIndex, position, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawWireMesh(Mesh mesh, int submeshIndex)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			Gizmos.DrawWireMesh(mesh, submeshIndex, zero, identity, one);
		}

		[ExcludeFromDocs]
		public static void DrawIcon(Vector3 center, string name)
		{
			bool flag = true;
			Gizmos.DrawIcon(center, name, flag);
		}

		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture)
		{
			Material material = null;
			Gizmos.DrawGUITexture(screenRect, texture, material);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
		{
			Gizmos.DrawGUITexture(screenRect, texture, 0, 0, 0, 0, mat);
		}

		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material material = null;
			Gizmos.DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, material);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLine_Injected([In] ref Vector3 from, [In] ref Vector3 to);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLineStrip_Injected(ref ManagedSpanWrapper points, bool looped);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLineListInternal_Injected(ref ManagedSpanWrapper points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireSphere_Injected([In] ref Vector3 center, float radius);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawSphere_Injected([In] ref Vector3 center, float radius);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireCube_Injected([In] ref Vector3 center, [In] ref Vector3 size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawCube_Injected([In] ref Vector3 center, [In] ref Vector3 size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawMesh_Injected(IntPtr mesh, int submeshIndex, [DefaultValue("Vector3.zero")] [In] ref Vector3 position, [DefaultValue("Quaternion.identity")] [In] ref Quaternion rotation, [DefaultValue("Vector3.one")] [In] ref Vector3 scale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireMesh_Injected(IntPtr mesh, int submeshIndex, [DefaultValue("Vector3.zero")] [In] ref Vector3 position, [DefaultValue("Quaternion.identity")] [In] ref Quaternion rotation, [DefaultValue("Vector3.one")] [In] ref Vector3 scale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawIcon_Injected([In] ref Vector3 center, ref ManagedSpanWrapper name, [DefaultValue("true")] bool allowScaling, [DefaultValue("Color(255,255,255,255)")] [In] ref Color tint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawGUITexture_Injected([In] ref Rect screenRect, IntPtr texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] IntPtr mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected([In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_matrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_matrix_Injected([In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_exposure_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_exposure_Injected(IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawFrustum_Injected([In] ref Vector3 center, float fov, float maxRange, float minRange, float aspect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float CalculateLOD_Injected([In] ref Vector3 position, float radius);
	}
}
