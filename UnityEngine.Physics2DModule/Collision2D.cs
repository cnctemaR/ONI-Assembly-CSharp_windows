using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Collision details returned by 2D physics callback functions.</para>
	/// </summary>
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Collision2D
	{
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
		///   <para>The Transform of the incoming object involved in the collision.</para>
		/// </summary>
		public Transform transform
		{
			get
			{
				return (!(this.rigidbody != null)) ? this.collider.transform : this.rigidbody.transform;
			}
		}

		/// <summary>
		///   <para>The incoming GameObject involved in the collision.</para>
		/// </summary>
		public GameObject gameObject
		{
			get
			{
				return (!(this.rigidbody != null)) ? this.collider.gameObject : this.rigidbody.gameObject;
			}
		}

		/// <summary>
		///   <para>The relative linear velocity of the two colliding objects (Read Only).</para>
		/// </summary>
		public Vector2 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
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

		/// <summary>
		///   <para>The specific points of contact with the incoming Collider2D. You should avoid using this as it produces memory garbage. Use GetContact or GetContacts instead.</para>
		/// </summary>
		public ContactPoint2D[] contacts
		{
			get
			{
				if (this.m_LegacyContactArray == null)
				{
					this.m_LegacyContactArray = new ContactPoint2D[this.m_ContactCount];
					if (this.m_ContactCount > 0)
					{
						for (int i = 0; i < this.m_ContactCount; i++)
						{
							this.m_LegacyContactArray[i] = this.m_CachedContactPoints[i];
						}
					}
				}
				return this.m_LegacyContactArray;
			}
		}

		/// <summary>
		///   <para>Gets the number of contacts for this collision.</para>
		/// </summary>
		public int contactCount
		{
			get
			{
				return this.m_ContactCount;
			}
		}

		/// <summary>
		///   <para>Gets the contact point at the specified index.</para>
		/// </summary>
		/// <param name="index">The index of the contact to retrieve.</param>
		/// <returns>
		///   <para>The contact at the specified index.</para>
		/// </returns>
		public ContactPoint2D GetContact(int index)
		{
			if (index < 0 || index >= this.m_ContactCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get contact at index {0}. There are {1} contact(s).", index, this.m_ContactCount));
			}
			return this.m_CachedContactPoints[index];
		}

		/// <summary>
		///   <para>Retrieves all contact points in for contacts between collider and otherCollider.</para>
		/// </summary>
		/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of contacts placed in the contacts array.</para>
		/// </returns>
		public int GetContacts(ContactPoint2D[] contacts)
		{
			if (contacts == null)
			{
				throw new ArgumentNullException("Cannot get contacts into a NULL array.");
			}
			int num = Mathf.Min(contacts.Length, this.m_ContactCount);
			int num2;
			if (num == 0)
			{
				num2 = 0;
			}
			else if (this.m_LegacyContactArray != null)
			{
				Array.Copy(this.m_LegacyContactArray, contacts, num);
				num2 = num;
			}
			else
			{
				if (this.m_ContactCount > 0)
				{
					for (int i = 0; i < num; i++)
					{
						contacts[i] = this.m_CachedContactPoints[i];
					}
				}
				num2 = num;
			}
			return num2;
		}

		internal int m_Collider;

		internal int m_OtherCollider;

		internal int m_Rigidbody;

		internal int m_OtherRigidbody;

		internal Vector2 m_RelativeVelocity;

		internal int m_Enabled;

		internal int m_ContactCount;

		internal CachedContactPoints2D m_CachedContactPoints;

		internal ContactPoint2D[] m_LegacyContactArray;
	}
}
