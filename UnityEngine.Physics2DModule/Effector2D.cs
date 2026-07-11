using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A base class for all 2D effectors.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/Effector2D.h")]
	public class Effector2D : Behaviour
	{
		/// <summary>
		///   <para>Should the collider-mask be used or the global collision matrix?</para>
		/// </summary>
		public extern bool useColliderMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mask used to select specific layers allowed to interact with the effector.</para>
		/// </summary>
		public extern int colliderMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern bool requiresCollider
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool designedForTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool designedForNonTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
