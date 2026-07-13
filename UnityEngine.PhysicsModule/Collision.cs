using System;
using System.Collections.Generic;

namespace UnityEngine
{
	public class Collision
	{
		public Vector3 impulse
		{
			get
			{
				return this.m_Pair.impulseSum;
			}
		}

		public Vector3 relativeVelocity
		{
			get
			{
				return this.m_Flipped ? this.m_Header.m_RelativeVelocity : (-this.m_Header.m_RelativeVelocity);
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return this.body as Rigidbody;
			}
		}

		public ArticulationBody articulationBody
		{
			get
			{
				return this.body as ArticulationBody;
			}
		}

		public Component body
		{
			get
			{
				return this.m_Flipped ? this.m_Header.body : this.m_Header.otherBody;
			}
		}

		public Collider collider
		{
			get
			{
				return this.m_Flipped ? this.m_Pair.collider : this.m_Pair.otherCollider;
			}
		}

		public Transform transform
		{
			get
			{
				return (this.rigidbody != null) ? this.rigidbody.transform : this.collider.transform;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return (this.body != null) ? this.body.gameObject : this.collider.gameObject;
			}
		}

		internal bool Flipped
		{
			get
			{
				return this.m_Flipped;
			}
			set
			{
				this.m_Flipped = value;
			}
		}

		public int contactCount
		{
			get
			{
				return (int)this.m_Pair.m_NbPoints;
			}
		}

		public ContactPoint[] contacts
		{
			get
			{
				bool flag = this.m_LegacyContacts == null;
				if (flag)
				{
					this.m_LegacyContacts = new ContactPoint[this.m_Pair.m_NbPoints];
					this.m_Pair.ExtractContactsArray(this.m_LegacyContacts, this.m_Flipped);
				}
				return this.m_LegacyContacts;
			}
		}

		public Collision()
		{
			this.m_Header = default(ContactPairHeader);
			this.m_Pair = default(ContactPair);
			this.m_Flipped = false;
			this.m_LegacyContacts = null;
		}

		internal Collision(in ContactPairHeader header, in ContactPair pair, bool flipped)
		{
			this.m_LegacyContacts = new ContactPoint[pair.m_NbPoints];
			pair.ExtractContactsArray(this.m_LegacyContacts, flipped);
			this.m_Header = header;
			this.m_Pair = pair;
			this.m_Flipped = flipped;
		}

		internal void Reuse(in ContactPairHeader header, in ContactPair pair)
		{
			this.m_Header = header;
			this.m_Pair = pair;
			this.m_LegacyContacts = null;
			this.m_Flipped = false;
		}

		public unsafe ContactPoint GetContact(int index)
		{
			bool flag = index < 0 || index >= this.contactCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get contact at index {0}. There are {1} contact(s).", index, this.contactCount));
			}
			bool flag2 = this.m_LegacyContacts != null;
			ContactPoint contactPoint;
			if (flag2)
			{
				contactPoint = this.m_LegacyContacts[index];
			}
			else
			{
				float num = (this.m_Flipped ? (-1f) : 1f);
				ContactPairPoint* contactPoint_Internal = this.m_Pair.GetContactPoint_Internal(index);
				contactPoint = new ContactPoint(contactPoint_Internal->m_Position, contactPoint_Internal->m_Normal * num, contactPoint_Internal->m_Impulse, contactPoint_Internal->m_Separation, this.m_Flipped ? this.m_Pair.otherColliderEntityId : this.m_Pair.colliderEntityId, this.m_Flipped ? this.m_Pair.colliderEntityId : this.m_Pair.otherColliderEntityId);
			}
			return contactPoint;
		}

		public int GetContacts(ContactPoint[] contacts)
		{
			bool flag = contacts == null;
			if (flag)
			{
				throw new NullReferenceException("Cannot get contacts as the provided array is NULL.");
			}
			bool flag2 = this.m_LegacyContacts != null;
			int num2;
			if (flag2)
			{
				int num = Mathf.Min(this.m_LegacyContacts.Length, contacts.Length);
				Array.Copy(this.m_LegacyContacts, contacts, num);
				num2 = num;
			}
			else
			{
				num2 = this.m_Pair.ExtractContactsArray(contacts, this.m_Flipped);
			}
			return num2;
		}

		public int GetContacts(List<ContactPoint> contacts)
		{
			bool flag = contacts == null;
			if (flag)
			{
				throw new NullReferenceException("Cannot get contacts as the provided list is NULL.");
			}
			contacts.Clear();
			bool flag2 = this.m_LegacyContacts != null;
			int num;
			if (flag2)
			{
				contacts.AddRange(this.m_LegacyContacts);
				num = this.m_LegacyContacts.Length;
			}
			else
			{
				int nbPoints = (int)this.m_Pair.m_NbPoints;
				bool flag3 = nbPoints == 0;
				if (flag3)
				{
					num = 0;
				}
				else
				{
					bool flag4 = contacts.Capacity < nbPoints;
					if (flag4)
					{
						contacts.Capacity = nbPoints;
					}
					num = this.m_Pair.ExtractContacts(contacts, this.m_Flipped);
				}
			}
			return num;
		}

		private ContactPairHeader m_Header;

		private ContactPair m_Pair;

		private bool m_Flipped;

		private ContactPoint[] m_LegacyContacts = null;
	}
}
