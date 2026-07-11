using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Collider for 2D physics representing an arbitrary set of connected edges (lines) defined by its vertices.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/Public/EdgeCollider2D.h")]
	public sealed class EdgeCollider2D : Collider2D
	{
		/// <summary>
		///   <para>Reset to a single edge consisting of two points.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reset();

		/// <summary>
		///   <para>Controls the radius of all edges created by the collider.</para>
		/// </summary>
		public extern float edgeRadius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the number of edges.</para>
		/// </summary>
		public extern int edgeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the number of points.</para>
		/// </summary>
		public extern int pointCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get or set the points defining multiple continuous edges.</para>
		/// </summary>
		public extern Vector2[] points
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
