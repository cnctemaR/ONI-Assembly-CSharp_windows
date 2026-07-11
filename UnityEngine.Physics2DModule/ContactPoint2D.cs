using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Details about a specific point of contact involved in a 2D physics collision.</para>
	/// </summary>
	[NativeClass("ScriptingContactPoint2D", "struct ScriptingContactPoint2D;")]
	[NativeHeader("Modules/Physics2D/Public/PhysicsScripting2D.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct ContactPoint2D
	{
		/// <summary>
		///   <para>The point of contact between the two colliders in world space.</para>
		/// </summary>
		public Vector2 point
		{
			get
			{
				return this.m_Point;
			}
		}

		/// <summary>
		///   <para>Surface normal at the contact point.</para>
		/// </summary>
		public Vector2 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		/// <summary>
		///   <para>Gets the distance between the colliders at the contact point.</para>
		/// </summary>
		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		/// <summary>
		///   <para>Gets the impulse force applied at the contact point along the ContactPoint2D.normal.</para>
		/// </summary>
		public float normalImpulse
		{
			get
			{
				return this.m_NormalImpulse;
			}
		}

		/// <summary>
		///   <para>Gets the impulse force applied at the contact point which is perpendicular to the ContactPoint2D.normal.</para>
		/// </summary>
		public float tangentImpulse
		{
			get
			{
				return this.m_TangentImpulse;
			}
		}

		/// <summary>
		///   <para>Gets the relative velocity of the two colliders at the contact point (Read Only).</para>
		/// </summary>
		public Vector2 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
			}
		}

		/// <summary>
		///   <para>The incoming Collider2D involved in the collision with the otherCollider.</para>
		/// </summary>
		public Collider2D collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Collider) as Collider2D;
			}
		}

		/// <summary>
		///   <para>The other Collider2D involved in the collision with the collider.</para>
		/// </summary>
		public Collider2D otherCollider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_OtherCollider) as Collider2D;
			}
		}

		/// <summary>
		///   <para>The incoming Rigidbody2D involved in the collision with the otherRigidbody.</para>
		/// </summary>
		public Rigidbody2D rigidbody
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Rigidbody) as Rigidbody2D;
			}
		}

		/// <summary>
		///   <para>The other Rigidbody2D involved in the collision with the rigidbody.</para>
		/// </summary>
		public Rigidbody2D otherRigidbody
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_OtherRigidbody) as Rigidbody2D;
			}
		}

		/// <summary>
		///   <para>Indicates whether the collision response or reaction is enabled or disabled.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return this.m_Enabled == 1;
			}
		}

		[NativeName("point")]
		private Vector2 m_Point;

		[NativeName("normal")]
		private Vector2 m_Normal;

		[NativeName("relativeVelocity")]
		private Vector2 m_RelativeVelocity;

		[NativeName("separation")]
		private float m_Separation;

		[NativeName("normalImpulse")]
		private float m_NormalImpulse;

		[NativeName("tangentImpulse")]
		private float m_TangentImpulse;

		[NativeName("collider")]
		private int m_Collider;

		[NativeName("otherCollider")]
		private int m_OtherCollider;

		[NativeName("rigidbody")]
		private int m_Rigidbody;

		[NativeName("otherRigidbody")]
		private int m_OtherRigidbody;

		[NativeName("enabled")]
		private int m_Enabled;
	}
}
