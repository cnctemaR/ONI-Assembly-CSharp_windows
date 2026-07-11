using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Collision
	{
		private ContactPoint[] GetContacts_Internal()
		{
			return (this.m_LegacyContacts != null) ? this.m_LegacyContacts : this.m_RecycledContacts;
		}

		public Vector3 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return this.m_Rigidbody;
			}
		}

		public Collider collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Transform transform
		{
			get
			{
				return (!(this.rigidbody != null)) ? this.collider.transform : this.rigidbody.transform;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return (!(this.m_Rigidbody != null)) ? this.m_Collider.gameObject : this.m_Rigidbody.gameObject;
			}
		}

		public int contactCount
		{
			get
			{
				return this.m_ContactCount;
			}
		}

		public ContactPoint[] contacts
		{
			get
			{
				if (this.m_LegacyContacts == null)
				{
					this.m_LegacyContacts = new ContactPoint[this.m_ContactCount];
					Array.Copy(this.m_RecycledContacts, this.m_LegacyContacts, this.m_ContactCount);
				}
				return this.m_LegacyContacts;
			}
		}

		public ContactPoint GetContact(int index)
		{
			if (index < 0 || index >= this.m_ContactCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get contact at index {0}. There are {1} contact(s).", index, this.m_ContactCount));
			}
			return this.GetContacts_Internal()[index];
		}

		public int GetContacts(ContactPoint[] contacts)
		{
			if (contacts == null)
			{
				throw new NullReferenceException("Cannot get contacts as the provided array is NULL.");
			}
			int num = Mathf.Min(this.m_ContactCount, contacts.Length);
			Array.Copy(this.GetContacts_Internal(), contacts, num);
			return num;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.contacts.GetEnumerator();
		}

		public Vector3 impulse
		{
			get
			{
				return this.m_Impulse;
			}
		}

		[Obsolete("Use Collision.relativeVelocity instead.", false)]
		public Vector3 impactForceSum
		{
			get
			{
				return this.relativeVelocity;
			}
		}

		[Obsolete("Will always return zero.", false)]
		public Vector3 frictionForceSum
		{
			get
			{
				return Vector3.zero;
			}
		}

		[Obsolete("Please use Collision.rigidbody, Collision.transform or Collision.collider instead", false)]
		public Component other
		{
			get
			{
				return (!(this.m_Rigidbody != null)) ? this.m_Collider : this.m_Rigidbody;
			}
		}

		internal Vector3 m_Impulse;

		internal Vector3 m_RelativeVelocity;

		internal Rigidbody m_Rigidbody;

		internal Collider m_Collider;

		internal int m_ContactCount;

		internal ContactPoint[] m_RecycledContacts;

		internal ContactPoint[] m_LegacyContacts;
	}
}
