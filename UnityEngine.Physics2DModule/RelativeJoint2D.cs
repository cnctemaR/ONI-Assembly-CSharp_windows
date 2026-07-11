using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Keeps two Rigidbody2D at their relative orientations.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/RelativeJoint2D.h")]
	public sealed class RelativeJoint2D : Joint2D
	{
		/// <summary>
		///   <para>The maximum force that can be generated when trying to maintain the relative joint constraint.</para>
		/// </summary>
		public extern float maxForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum torque that can be generated when trying to maintain the relative joint constraint.</para>
		/// </summary>
		public extern float maxTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Scales both the linear and angular forces used to correct the required relative orientation.</para>
		/// </summary>
		public extern float correctionScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should both the linearOffset and angularOffset be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current linear offset between the Rigidbody2D that the joint connects.</para>
		/// </summary>
		public Vector2 linearOffset
		{
			get
			{
				Vector2 vector;
				this.get_linearOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_linearOffset_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The current angular offset between the Rigidbody2D that the joint connects.</para>
		/// </summary>
		public extern float angularOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The world-space position that is currently trying to be maintained.</para>
		/// </summary>
		public Vector2 target
		{
			get
			{
				Vector2 vector;
				this.get_target_Injected(out vector);
				return vector;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_linearOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_linearOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_target_Injected(out Vector2 ret);
	}
}
