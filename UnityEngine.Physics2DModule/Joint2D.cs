using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Parent class for joints to connect Rigidbody2D objects.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/Joint2D.h")]
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D))]
	public class Joint2D : Behaviour
	{
		/// <summary>
		///   <para>The Rigidbody2D attached to the Joint2D.</para>
		/// </summary>
		public extern Rigidbody2D attachedRigidbody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The Rigidbody2D object to which the other end of the joint is attached (ie, the object without the joint component).</para>
		/// </summary>
		public extern Rigidbody2D connectedBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should the two rigid bodies connected with this joint collide with each other?</para>
		/// </summary>
		public extern bool enableCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The force that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The torque that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the reaction force of the joint.</para>
		/// </summary>
		public Vector2 reactionForce
		{
			[NativeMethod("GetReactionForceFixedTime")]
			get
			{
				Vector2 vector;
				this.get_reactionForce_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Gets the reaction torque of the joint.</para>
		/// </summary>
		public extern float reactionTorque
		{
			[NativeMethod("GetReactionTorqueFixedTime")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the reaction force of the joint given the specified timeStep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the reaction force for.</param>
		/// <returns>
		///   <para>The reaction force of the joint in the specified timeStep.</para>
		/// </returns>
		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 vector;
			this.GetReactionForce_Injected(timeStep, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Gets the reaction torque of the joint given the specified timeStep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the reaction torque for.</param>
		/// <returns>
		///   <para>The reaction torque of the joint in the specified timeStep.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetReactionTorque(float timeStep);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_reactionForce_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetReactionForce_Injected(float timeStep, out Vector2 ret);
	}
}
