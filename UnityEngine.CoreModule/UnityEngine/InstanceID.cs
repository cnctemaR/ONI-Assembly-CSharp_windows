using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Obsolete("Obsolete - Please use EntityId instead.")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 4)]
	public struct InstanceID : IEquatable<InstanceID>, IComparable<InstanceID>
	{
		public static InstanceID None
		{
			get
			{
				return default(InstanceID);
			}
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is InstanceID)
			{
				InstanceID instanceID = (InstanceID)obj;
				flag = this.Equals(instanceID);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(InstanceID other)
		{
			return this.m_Data == other.m_Data;
		}

		public int CompareTo(InstanceID other)
		{
			return this.m_Data.CompareTo(other.m_Data);
		}

		public static bool operator ==(InstanceID left, InstanceID right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InstanceID left, InstanceID right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(InstanceID left, InstanceID right)
		{
			return left.m_Data < right.m_Data;
		}

		public static bool operator >(InstanceID left, InstanceID right)
		{
			return left.m_Data > right.m_Data;
		}

		public static bool operator <=(InstanceID left, InstanceID right)
		{
			return left.m_Data <= right.m_Data;
		}

		public static bool operator >=(InstanceID left, InstanceID right)
		{
			return left.m_Data >= right.m_Data;
		}

		public override int GetHashCode()
		{
			uint num = (uint)this.m_Data;
			num = num + 2127912214U + (num << 12);
			num = num ^ 3345072700U ^ (num >> 19);
			num = num + 374761393U + (num << 5);
			num = (num + 3550635116U) ^ (num << 9);
			num = num + 4251993797U + (num << 3);
			return (int)(num ^ 3042594569U ^ (num >> 16));
		}

		public bool IsValid()
		{
			return this != InstanceID.None;
		}

		public bool Equals(int other)
		{
			return this.m_Data == other;
		}

		public static implicit operator int(InstanceID entityId)
		{
			return entityId.m_Data;
		}

		public static implicit operator InstanceID(int intValue)
		{
			return new InstanceID
			{
				m_Data = intValue
			};
		}

		public static implicit operator EntityId(InstanceID entityId)
		{
			return entityId;
		}

		public static implicit operator InstanceID(EntityId entityId)
		{
			return new InstanceID
			{
				m_Data = entityId
			};
		}

		public override string ToString()
		{
			return this.m_Data.ToString();
		}

		public string ToString(string format)
		{
			return this.m_Data.ToString(format);
		}

		[SerializeField]
		private int m_Data;
	}
}
