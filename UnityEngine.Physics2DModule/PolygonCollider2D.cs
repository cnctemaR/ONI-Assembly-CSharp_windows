using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Collider for 2D physics representing an arbitrary polygon defined by its vertices.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/Public/PolygonCollider2D.h")]
	public sealed class PolygonCollider2D : Collider2D
	{
		/// <summary>
		///   <para>Determines whether the PolygonCollider2D's shape is automatically updated based on a SpriteRenderer's tiling properties.</para>
		/// </summary>
		public extern bool autoTiling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Return the total number of points in the polygon in all paths.</para>
		/// </summary>
		[NativeMethod("GetPointCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetTotalPointCount();

		/// <summary>
		///   <para>Corner points that define the collider's shape in local space.</para>
		/// </summary>
		public extern Vector2[] points
		{
			[NativeMethod("GetPoints_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPoints_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The number of paths in the polygon.</para>
		/// </summary>
		public extern int pathCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets a path from the Collider by its index.</para>
		/// </summary>
		/// <param name="index">The index of the path to retrieve.</param>
		/// <returns>
		///   <para>An ordered array of the vertices or points in the selected path.</para>
		/// </returns>
		public Vector2[] GetPath(int index)
		{
			if (index >= this.pathCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("Path {0} does not exist.", index));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Path {0} does not exist; negative path index is invalid.", index));
			}
			return this.GetPath_Internal(index);
		}

		/// <summary>
		///   <para>Define a path by its constituent points.</para>
		/// </summary>
		/// <param name="index">Index of the path to set.</param>
		/// <param name="points">Points that define the path.</param>
		public void SetPath(int index, Vector2[] points)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Negative path index is invalid.", index));
			}
			this.SetPath_Internal(index, points);
		}

		[NativeMethod("GetPath_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector2[] GetPath_Internal(int index);

		[NativeMethod("SetPath_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPath_Internal(int index, Vector2[] points);

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

		/// <summary>
		///   <para>Creates as regular primitive polygon with the specified number of sides.</para>
		/// </summary>
		/// <param name="sides">The number of sides in the polygon.  This must be greater than two.</param>
		/// <param name="scale">The X/Y scale of the polygon.  These must be greater than zero.</param>
		/// <param name="offset">The X/Y offset of the polygon.</param>
		public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
		{
			if (sides < 3)
			{
				Debug.LogWarning("Cannot create a 2D polygon primitive collider with less than two sides.", this);
			}
			else if (scale.x <= 0f || scale.y <= 0f)
			{
				Debug.LogWarning("Cannot create a 2D polygon primitive collider with an axis scale less than or equal to zero.", this);
			}
			else
			{
				this.CreatePrimitive_Internal(sides, scale, offset, true);
			}
		}

		[NativeMethod("CreatePrimitive")]
		private void CreatePrimitive_Internal(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset, bool autoRefresh)
		{
			this.CreatePrimitive_Internal_Injected(sides, ref scale, ref offset, autoRefresh);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CreatePrimitive_Internal_Injected(int sides, [DefaultValue("Vector2.one")] ref Vector2 scale, [DefaultValue("Vector2.zero")] ref Vector2 offset, bool autoRefresh);
	}
}
