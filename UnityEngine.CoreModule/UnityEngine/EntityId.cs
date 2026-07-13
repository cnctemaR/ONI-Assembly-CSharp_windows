using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeClass("EntityId")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 4)]
	public struct EntityId : IEquatable<EntityId>, IComparable<EntityId>
	{
		public static EntityId None
		{
			get
			{
				return default(EntityId);
			}
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is EntityId)
			{
				EntityId entityId = (EntityId)obj;
				flag = this.Equals(entityId);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(EntityId other)
		{
			return this.m_Data == other.m_Data;
		}

		public int CompareTo(EntityId other)
		{
			return this.m_Data.CompareTo(other.m_Data);
		}

		public static bool operator ==(EntityId left, EntityId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EntityId left, EntityId right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(EntityId left, EntityId right)
		{
			return left.m_Data < right.m_Data;
		}

		public static bool operator >(EntityId left, EntityId right)
		{
			return left.m_Data > right.m_Data;
		}

		public static bool operator <=(EntityId left, EntityId right)
		{
			return left.m_Data <= right.m_Data;
		}

		public static bool operator >=(EntityId left, EntityId right)
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
			return this != EntityId.None;
		}

		public bool Equals(int other)
		{
			return this.m_Data == other;
		}

		public static implicit operator int(EntityId entityId)
		{
			return entityId.m_Data;
		}

		public static implicit operator EntityId(int intValue)
		{
			return new EntityId
			{
				m_Data = intValue
			};
		}

		public static implicit operator EntityId(InstanceID entityId)
		{
			return new EntityId
			{
				m_Data = entityId
			};
		}

		public static implicit operator InstanceID(EntityId entityId)
		{
			return entityId;
		}

		public override string ToString()
		{
			return this.m_Data.ToString();
		}

		public string ToString(string format)
		{
			return this.m_Data.ToString(format);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static EntityId From(int input)
		{
			return new EntityId
			{
				m_Data = input
			};
		}

		internal static EntityId From(ulong input)
		{
			return new EntityId
			{
				m_Data = (int)input
			};
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static EntityId Parse(string input)
		{
			EntityId entityId = EntityId.None;
			int num;
			bool flag = int.TryParse(input, out num);
			if (flag)
			{
				entityId = EntityId.From(num);
			}
			return entityId;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int GetRawData()
		{
			return this.m_Data;
		}

		[SerializeField]
		private int m_Data;
	}
}
