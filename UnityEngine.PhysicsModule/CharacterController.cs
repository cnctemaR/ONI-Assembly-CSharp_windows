using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A CharacterController allows you to easily do movement constrained by collisions without having to deal with a rigidbody.</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/CharacterController.h")]
	public class CharacterController : Collider
	{
		/// <summary>
		///   <para>Moves the character with speed.</para>
		/// </summary>
		/// <param name="speed"></param>
		public bool SimpleMove(Vector3 speed)
		{
			return this.SimpleMove_Injected(ref speed);
		}

		/// <summary>
		///   <para>A more complex move function taking absolute movement deltas.</para>
		/// </summary>
		/// <param name="motion"></param>
		public CollisionFlags Move(Vector3 motion)
		{
			return this.Move_Injected(ref motion);
		}

		/// <summary>
		///   <para>The current relative velocity of the Character (see notes).</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				Vector3 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Was the CharacterController touching the ground during the last move?</para>
		/// </summary>
		public extern bool isGrounded
		{
			[NativeName("IsGrounded")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>What part of the capsule collided with the environment during the last CharacterController.Move call.</para>
		/// </summary>
		public extern CollisionFlags collisionFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The radius of the character's capsule.</para>
		/// </summary>
		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The height of the character's capsule.</para>
		/// </summary>
		public extern float height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The center of the character's capsule relative to the transform's position.</para>
		/// </summary>
		public Vector3 center
		{
			get
			{
				Vector3 vector;
				this.get_center_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_center_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The character controllers slope limit in degrees.</para>
		/// </summary>
		public extern float slopeLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The character controllers step offset in meters.</para>
		/// </summary>
		public extern float stepOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The character's collision skin width.</para>
		/// </summary>
		public extern float skinWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets or sets the minimum move distance of the character controller.</para>
		/// </summary>
		public extern float minMoveDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines whether other rigidbodies or character controllers collide with this character controller (by default this is always enabled).</para>
		/// </summary>
		public extern bool detectCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables or disables overlap recovery.
		///  Enables or disables overlap recovery. Used to depenetrate character controllers from static objects when an overlap is detected.</para>
		/// </summary>
		public extern bool enableOverlapRecovery
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SimpleMove_Injected(ref Vector3 speed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern CollisionFlags Move_Injected(ref Vector3 motion);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_center_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_center_Injected(ref Vector3 value);
	}
}
