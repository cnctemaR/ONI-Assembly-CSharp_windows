using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A Collider that can merge other Colliders together.</para>
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[NativeHeader("Modules/Physics2D/Public/CompositeCollider2D.h")]
	public sealed class CompositeCollider2D : Collider2D
	{
		/// <summary>
		///   <para>Specifies the type of geometry the Composite Collider should generate.</para>
		/// </summary>
		public extern CompositeCollider2D.GeometryType geometryType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specifies when to generate the Composite Collider geometry.</para>
		/// </summary>
		public extern CompositeCollider2D.GenerationType generationType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls the minimum distance allowed between generated vertices.</para>
		/// </summary>
		public extern float vertexDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls the radius of all edges created by the Collider.</para>
		/// </summary>
		public extern float edgeRadius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Regenerates the Composite Collider geometry.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GenerateGeometry();

		/// <summary>
		///   <para>Gets the number of points in the specified path from the Collider by its index.</para>
		/// </summary>
		/// <param name="index">The index of the path from 0 to pathCount.</param>
		/// <returns>
		///   <para>Returns the number of points in the path specified by index.</para>
		/// </returns>
		public int GetPathPointCount(int index)
		{
			int num = this.pathCount - 1;
			if (index < 0 || index > num)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Path index {0} must be in the range of 0 to {1}.", index, num));
			}
			return this.GetPathPointCount_Internal(index);
		}

		[NativeMethod("GetPathPointCount_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetPathPointCount_Internal(int index);

		/// <summary>
		///   <para>The number of paths in the Collider.</para>
		/// </summary>
		public extern int pathCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the total number of points in all the paths within the Collider.</para>
		/// </summary>
		public extern int pointCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets a path from the Collider by its index.</para>
		/// </summary>
		/// <param name="index">The index of the path from 0 to pathCount.</param>
		/// <param name="points">An ordered array of the vertices or points in the selected path.</param>
		/// <returns>
		///   <para>Returns the number of points placed in the points array.</para>
		/// </returns>
		public int GetPath(int index, Vector2[] points)
		{
			if (index < 0 || index >= this.pathCount)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Path index {0} must be in the range of 0 to {1}.", index, this.pathCount - 1));
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			return this.GetPath_Internal(index, points);
		}

		[NativeMethod("GetPath_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetPath_Internal(int index, [Out] Vector2[] points);

		/// <summary>
		///   <para>Specifies the type of geometry the Composite Collider generates.</para>
		/// </summary>
		public enum GeometryType
		{
			/// <summary>
			///   <para>Sets the Composite Collider to generate closed outlines for the merged Collider geometry consisting of only edges.</para>
			/// </summary>
			Outlines,
			/// <summary>
			///   <para>Sets the Composite Collider to generate closed outlines for the merged Collider geometry consisting of convex polygon shapes.</para>
			/// </summary>
			Polygons
		}

		/// <summary>
		///   <para>Specifies when to generate the Composite Collider geometry.</para>
		/// </summary>
		public enum GenerationType
		{
			/// <summary>
			///   <para>Sets the Composite Collider geometry to update synchronously immediately when a Collider used by the Composite Collider changes.</para>
			/// </summary>
			Synchronous,
			/// <summary>
			///   <para>Sets the Composite Collider geometry to not automatically update when a Collider used by the Composite Collider changes.</para>
			/// </summary>
			Manual
		}
	}
}
