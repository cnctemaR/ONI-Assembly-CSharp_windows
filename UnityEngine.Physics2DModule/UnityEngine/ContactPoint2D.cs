using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = false, GenerateProxy = false)]
	[NativeClass("ScriptingContactPoint2D", "struct ScriptingContactPoint2D;")]
	[NativeHeader("Modules/Physics2D/Public/PhysicsScripting2D.h")]
	public struct ContactPoint2D
	{
		public Vector2 point
		{
			get
			{
				return this.m_Point;
			}
		}

		public Vector2 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		public float normalImpulse
		{
			get
			{
				return this.m_NormalImpulse;
			}
		}

		public float tangentImpulse
		{
			get
			{
				return this.m_TangentImpulse;
			}
		}

		public Vector2 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
			}
		}

		public float friction
		{
			get
			{
				return this.m_Friction;
			}
		}

		public float bounciness
		{
			get
			{
				return this.m_Bounciness;
			}
		}

		public Collider2D collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Collider) as Collider2D;
			}
		}

		public Collider2D otherCollider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_OtherCollider) as Collider2D;
			}
		}

		public Rigidbody2D rigidbody
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Rigidbody) as Rigidbody2D;
			}
		}

		public Rigidbody2D otherRigidbody
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_OtherRigidbody) as Rigidbody2D;
			}
		}

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

		[NativeName("friction")]
		private float m_Friction;

		[NativeName("bounciness")]
		private float m_Bounciness;

		[NativeName("separation")]
		private float m_Separation;

		[NativeName("normalImpulse")]
		private float m_NormalImpulse;

		[NativeName("tangentImpulse")]
		private float m_TangentImpulse;

		[NativeName("collider")]
		private EntityId m_Collider;

		[NativeName("otherCollider")]
		private EntityId m_OtherCollider;

		[NativeName("rigidbody")]
		private EntityId m_Rigidbody;

		[NativeName("otherRigidbody")]
		private EntityId m_OtherRigidbody;

		[NativeName("enabled")]
		private int m_Enabled;
	}
}
