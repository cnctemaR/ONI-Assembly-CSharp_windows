using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Applies both force and torque to reduce both the linear and angular velocities to zero.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/FrictionJoint2D.h")]
	public sealed class FrictionJoint2D : AnchoredJoint2D
	{
		/// <summary>
		///   <para>The maximum force that can be generated when trying to maintain the friction joint constraint.</para>
		/// </summary>
		public extern float maxForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum torque that can be generated when trying to maintain the friction joint constraint.</para>
		/// </summary>
		public extern float maxTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
