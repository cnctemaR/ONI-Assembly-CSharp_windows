using System;

namespace UnityEngine
{
	[Serializable]
	public struct LazyLoadReference<T> where T : Object
	{
		public bool isSet
		{
			get
			{
				return this.m_InstanceID != EntityId.None;
			}
		}

		public bool isBroken
		{
			get
			{
				return this.m_InstanceID != EntityId.None && !Object.DoesObjectWithInstanceIDExist(this.m_InstanceID);
			}
		}

		public T asset
		{
			get
			{
				bool flag = this.m_InstanceID == EntityId.None;
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
				bool flag = value == null;
				if (flag)
				{
					this.m_InstanceID = EntityId.None;
				}
				else
				{
					bool flag2 = !Object.IsPersistent(value);
					if (flag2)
					{
						throw new ArgumentException("Object that does not belong to a persisted asset cannot be set as the target of a LazyLoadReference.");
					}
					this.m_InstanceID = value.GetEntityId();
				}
			}
		}

		public EntityId entityId
		{
			get
			{
				return this.m_InstanceID;
			}
			set
			{
				this.m_InstanceID = value;
			}
		}

		[Obsolete("Use entityId instead, this will be removed in a future version", false)]
		public int instanceID
		{
			get
			{
				return this.entityId;
			}
			set
			{
				this.entityId = value;
			}
		}

		public LazyLoadReference(T asset)
		{
			bool flag = asset == null;
			if (flag)
			{
				this.m_InstanceID = EntityId.None;
			}
			else
			{
				bool flag2 = !Object.IsPersistent(asset);
				if (flag2)
				{
					throw new ArgumentException("Object that does not belong to a persisted asset cannot be set as the target of a LazyLoadReference.");
				}
				this.m_InstanceID = asset.GetEntityId();
			}
		}

		public LazyLoadReference(EntityId entityId)
		{
			this.m_InstanceID = entityId;
		}

		[Obsolete("Use LazyLoadReference(EntityId entityId) instead, this will be removed in a future version", false)]
		public LazyLoadReference(int instanceID)
		{
			this.m_InstanceID = instanceID;
		}

		public static implicit operator LazyLoadReference<T>(T asset)
		{
			return new LazyLoadReference<T>
			{
				asset = asset
			};
		}

		public static implicit operator LazyLoadReference<T>(EntityId entityId)
		{
			return new LazyLoadReference<T>
			{
				m_InstanceID = entityId
			};
		}

		[Obsolete("Use LazyLoadReference(EntityId entityId) instead, this will be removed in a future version", false)]
		public static implicit operator LazyLoadReference<T>(int instanceID)
		{
			return new LazyLoadReference<T>
			{
				m_InstanceID = instanceID
			};
		}

		[SerializeField]
		private int m_InstanceID;
	}
}
