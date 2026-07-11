using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Gizmos are used to give visual debugging or setup aids in the Scene view.</para>
	/// </summary>
	[NativeHeader("Runtime/Export/Gizmos.bindings.h")]
	[StaticAccessor("GizmoBindings", StaticAccessorType.DoubleColon)]
	public sealed class Gizmos
	{
		/// <summary>
		///   <para>Draws a line starting at from towards to.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		[NativeThrows]
		public static void DrawLine(Vector3 from, Vector3 to)
		{
			Gizmos.DrawLine_Injected(ref from, ref to);
		}

		/// <summary>
		///   <para>Draws a wireframe sphere with center and radius.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		[NativeThrows]
		public static void DrawWireSphere(Vector3 center, float radius)
		{
			Gizmos.DrawWireSphere_Injected(ref center, radius);
		}

		/// <summary>
		///   <para>Draws a solid sphere with center and radius.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		[NativeThrows]
		public static void DrawSphere(Vector3 center, float radius)
		{
			Gizmos.DrawSphere_Injected(ref center, radius);
		}

		/// <summary>
		///   <para>Draw a wireframe box with center and size.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="size"></param>
		[NativeThrows]
		public static void DrawWireCube(Vector3 center, Vector3 size)
		{
			Gizmos.DrawWireCube_Injected(ref center, ref size);
		}

		/// <summary>
		///   <para>Draw a solid box with center and size.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="size"></param>
		[NativeThrows]
		public static void DrawCube(Vector3 center, Vector3 size)
		{
			Gizmos.DrawCube_Injected(ref center, ref size);
		}

		/// <summary>
		///   <para>Draws a mesh.</para>
		/// </summary>
		/// <param name="mesh">Mesh to draw as a gizmo.</param>
		/// <param name="position">Position (default is zero).</param>
		/// <param name="rotation">Rotation (default is no rotation).</param>
		/// <param name="scale">Scale (default is no scale).</param>
		/// <param name="submeshIndex">Submesh to draw (default is -1, which draws whole mesh).</param>
		[NativeThrows]
		public static void DrawMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawMesh_Injected(mesh, submeshIndex, ref position, ref rotation, ref scale);
		}

		/// <summary>
		///   <para>Draws a wireframe mesh.</para>
		/// </summary>
		/// <param name="mesh">Mesh to draw as a gizmo.</param>
		/// <param name="position">Position (default is zero).</param>
		/// <param name="rotation">Rotation (default is no rotation).</param>
		/// <param name="scale">Scale (default is no scale).</param>
		/// <param name="submeshIndex">Submesh to draw (default is -1, which draws whole mesh).</param>
		[NativeThrows]
		public static void DrawWireMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
		{
			Gizmos.DrawWireMesh_Injected(mesh, submeshIndex, ref position, ref rotation, ref scale);
		}

		/// <summary>
		///   <para>Draw an icon at a position in the Scene view.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="name"></param>
		/// <param name="allowScaling"></param>
		[NativeThrows]
		public static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling)
		{
			Gizmos.DrawIcon_Injected(ref center, name, allowScaling);
		}

		/// <summary>
		///   <para>Draw a texture in the Scene.</para>
		/// </summary>
		/// <param name="screenRect">The size and position of the texture on the "screen" defined by the XY plane.</param>
		/// <param name="texture">The texture to be displayed.</param>
		/// <param name="mat">An optional material to apply the texture.</param>
		/// <param name="leftBorder">Inset from the rectangle's left edge.</param>
		/// <param name="rightBorder">Inset from the rectangle's right edge.</param>
		/// <param name="topBorder">Inset from the rectangle's top edge.</param>
		/// <param name="bottomBorder">Inset from the rectangle's bottom edge.</param>
		[NativeThrows]
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
		{
			Gizmos.DrawGUITexture_Injected(ref screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		/// <summary>
		///   <para>Sets the color for the gizmos that will be drawn next.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Set the gizmo matrix used to draw all gizmos.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Draw a camera frustum using the currently set Gizmos.matrix for it's location and rotation.</para>
		/// </summary>
		/// <param name="center">The apex of the truncated pyramid.</param>
		/// <param name="fov">Vertical field of view (ie, the angle at the apex in degrees).</param>
		/// <param name="maxRange">Distance of the frustum's far plane.</param>
		/// <param name="minRange">Distance of the frustum's near plane.</param>
		/// <param name="aspect">Width/height ratio.</param>
		public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
		{
			Gizmos.DrawFrustum_Injected(ref center, fov, maxRange, minRange, aspect);
		}

		/// <summary>
		///   <para>Draws a ray starting at from to from + direction.</para>
		/// </summary>
		/// <param name="r"></param>
		/// <param name="from"></param>
		/// <param name="direction"></param>
		public static void DrawRay(Ray r)
		{
			Gizmos.DrawLine(r.origin, r.origin + r.direction);
		}

		/// <summary>
		///   <para>Draws a ray starting at from to from + direction.</para>
		/// </summary>
		/// <param name="r"></param>
		/// <param name="from"></param>
		/// <param name="direction"></param>
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

		/// <summary>
		///   <para>Draws a mesh.</para>
		/// </summary>
		/// <param name="mesh">Mesh to draw as a gizmo.</param>
		/// <param name="position">Position (default is zero).</param>
		/// <param name="rotation">Rotation (default is no rotation).</param>
		/// <param name="scale">Scale (default is no scale).</param>
		/// <param name="submeshIndex">Submesh to draw (default is -1, which draws whole mesh).</param>
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

		/// <summary>
		///   <para>Draws a wireframe mesh.</para>
		/// </summary>
		/// <param name="mesh">Mesh to draw as a gizmo.</param>
		/// <param name="position">Position (default is zero).</param>
		/// <param name="rotation">Rotation (default is no rotation).</param>
		/// <param name="scale">Scale (default is no scale).</param>
		/// <param name="submeshIndex">Submesh to draw (default is -1, which draws whole mesh).</param>
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

		/// <summary>
		///   <para>Draw an icon at a position in the Scene view.</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="name"></param>
		/// <param name="allowScaling"></param>
		[ExcludeFromDocs]
		public static void DrawIcon(Vector3 center, string name)
		{
			bool flag = true;
			Gizmos.DrawIcon(center, name, flag);
		}

		/// <summary>
		///   <para>Draw a texture in the Scene.</para>
		/// </summary>
		/// <param name="screenRect">The size and position of the texture on the "screen" defined by the XY plane.</param>
		/// <param name="texture">The texture to be displayed.</param>
		/// <param name="mat">An optional material to apply the texture.</param>
		/// <param name="leftBorder">Inset from the rectangle's left edge.</param>
		/// <param name="rightBorder">Inset from the rectangle's right edge.</param>
		/// <param name="topBorder">Inset from the rectangle's top edge.</param>
		/// <param name="bottomBorder">Inset from the rectangle's bottom edge.</param>
		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture)
		{
			Material material = null;
			Gizmos.DrawGUITexture(screenRect, texture, material);
		}

		/// <summary>
		///   <para>Draw a texture in the Scene.</para>
		/// </summary>
		/// <param name="screenRect">The size and position of the texture on the "screen" defined by the XY plane.</param>
		/// <param name="texture">The texture to be displayed.</param>
		/// <param name="mat">An optional material to apply the texture.</param>
		/// <param name="leftBorder">Inset from the rectangle's left edge.</param>
		/// <param name="rightBorder">Inset from the rectangle's right edge.</param>
		/// <param name="topBorder">Inset from the rectangle's top edge.</param>
		/// <param name="bottomBorder">Inset from the rectangle's bottom edge.</param>
		public static void DrawGUITexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
		{
			Gizmos.DrawGUITexture(screenRect, texture, 0, 0, 0, 0, mat);
		}

		/// <summary>
		///   <para>Draw a texture in the Scene.</para>
		/// </summary>
		/// <param name="screenRect">The size and position of the texture on the "screen" defined by the XY plane.</param>
		/// <param name="texture">The texture to be displayed.</param>
		/// <param name="mat">An optional material to apply the texture.</param>
		/// <param name="leftBorder">Inset from the rectangle's left edge.</param>
		/// <param name="rightBorder">Inset from the rectangle's right edge.</param>
		/// <param name="topBorder">Inset from the rectangle's top edge.</param>
		/// <param name="bottomBorder">Inset from the rectangle's bottom edge.</param>
		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material material = null;
			Gizmos.DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, material);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLine_Injected(ref Vector3 from, ref Vector3 to);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireSphere_Injected(ref Vector3 center, float radius);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawSphere_Injected(ref Vector3 center, float radius);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireCube_Injected(ref Vector3 center, ref Vector3 size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawCube_Injected(ref Vector3 center, ref Vector3 size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawMesh_Injected(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] ref Vector3 position, [DefaultValue("Quaternion.identity")] ref Quaternion rotation, [DefaultValue("Vector3.one")] ref Vector3 scale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireMesh_Injected(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] ref Vector3 position, [DefaultValue("Quaternion.identity")] ref Quaternion rotation, [DefaultValue("Vector3.one")] ref Vector3 scale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawIcon_Injected(ref Vector3 center, string name, [DefaultValue("true")] bool allowScaling);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawGUITexture_Injected(ref Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_matrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_matrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawFrustum_Injected(ref Vector3 center, float fov, float maxRange, float minRange, float aspect);
	}
}
