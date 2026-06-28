using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct RaycastHit2D
	{
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

		public Collider2D collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Rigidbody2D rigidbody
		{
			get
			{
				return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
			}
		}

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

		private Vector2 m_Centroid;

		private Vector2 m_Point;

		private Vector2 m_Normal;

		private float m_Distance;

		private float m_Fraction;

		private Collider2D m_Collider;
	}
}
