using System;

namespace UnityEngine
{
	[Serializable]
	public struct LazyLoadReference<T> where T : Object
	{
		internal int instanceID
		{
			get
			{
				return this.m_InstanceID;
			}
		}

		public bool isSet
		{
			get
			{
				return this.m_InstanceID == 0;
			}
		}

		public bool isBroken
		{
			get
			{
				return this.m_InstanceID != 0 && !Object.DoesObjectWithInstanceIDExist(this.m_InstanceID);
			}
		}

		public T asset
		{
			get
			{
				bool flag = this.m_InstanceID == 0;
				T t;
				if (flag)
				{
					t = default(T);
				}
				else
				{
					t = (T)((object)Object.ForceLoadFromInstanceID(this.m_InstanceID));
				}
				return t;
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					bool flag2 = !Object.IsPersistent(value);
					if (flag2)
					{
						throw new ArgumentException("Object that does not belong to a persisted asset cannot be set as the target of a LazyLoadReference.");
					}
					this.m_InstanceID = value.GetInstanceID();
				}
				else
				{
					this.m_InstanceID = 0;
				}
			}
		}

		private const int kInstanceID_None = 0;

		[SerializeField]
		private int m_InstanceID;
	}
}
