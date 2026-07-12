using System;

namespace UnityEngine
{
	public struct ColliderHit
	{
		public int instanceID
		{
			get
			{
				return this.m_ColliderInstanceID;
			}
		}

		public Collider collider
		{
			get
			{
				return Object.FindObjectFromInstanceID(this.instanceID) as Collider;
			}
		}

		private int m_ColliderInstanceID;
	}
}
