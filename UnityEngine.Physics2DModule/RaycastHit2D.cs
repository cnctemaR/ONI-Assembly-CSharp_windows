using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information returned about an object detected by a raycast in 2D physics.</para>
	/// </summary>
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("RaycastHit2D", "struct RaycastHit2D;")]
	[NativeHeader("Runtime/Interfaces/IPhysics2D.h")]
	public struct RaycastHit2D
	{
		/// <summary>
		///   <para>The centroid of the primitive used to perform the cast.</para>
		/// </summary>
		public Vector2 centroid
		{
			get
			{
				return this.m_Centroid;
			}
			set
			{
				this.m_Centroid = value;
			}
		}

		/// <summary>
		///   <para>The point in world space where the ray hit the collider's surface.</para>
		/// </summary>
		public Vector2 point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		/// <summary>
		///   <para>The normal vector of the surface hit by the ray.</para>
		/// </summary>
		public Vector2 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		/// <summary>
		///   <para>The distance from the ray origin to the impact point.</para>
		/// </summary>
		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = value;
			}
		}

		/// <summary>
		///   <para>Fraction of the distance along the ray that the hit occurred.</para>
		/// </summary>
		public float fraction
		{
			get
			{
				return this.m_Fraction;
			}
			set
			{
				this.m_Fraction = value;
			}
		}

		/// <summary>
		///   <para>The collider hit by the ray.</para>
		/// </summary>
		public Collider2D collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.m_Collider) as Collider2D;
			}
		}

		/// <summary>
		///   <para>The Rigidbody2D attached to the object that was hit.</para>
		/// </summary>
		public Rigidbody2D rigidbody
		{
			get
			{
				return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
			}
		}

		/// <summary>
		///   <para>The Transform of the object that was hit.</para>
		/// </summary>
		public Transform transform
		{
			get
			{
				Rigidbody2D rigidbody = this.rigidbody;
				Transform transform;
				if (rigidbody != null)
				{
					transform = rigidbody.transform;
				}
				else if (this.collider != null)
				{
					transform = this.collider.transform;
				}
				else
				{
					transform = null;
				}
				return transform;
			}
		}

		public static implicit operator bool(RaycastHit2D hit)
		{
			return hit.collider != null;
		}

		public int CompareTo(RaycastHit2D other)
		{
			int num;
			if (this.collider == null)
			{
				num = 1;
			}
			else if (other.collider == null)
			{
				num = -1;
			}
			else
			{
				num = this.fraction.CompareTo(other.fraction);
			}
			return num;
		}

		[NativeName("centroid")]
		private Vector2 m_Centroid;

		[NativeName("point")]
		private Vector2 m_Point;

		[NativeName("normal")]
		private Vector2 m_Normal;

		[NativeName("distance")]
		private float m_Distance;

		[NativeName("fraction")]
		private float m_Fraction;

		[NativeName("collider")]
		private int m_Collider;
	}
}
