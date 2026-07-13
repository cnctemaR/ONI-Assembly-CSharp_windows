using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyId.h")]
	internal readonly struct HierarchyPropertyId : IEquatable<HierarchyPropertyId>
	{
		public static readonly ref HierarchyPropertyId Null
		{
			get
			{
				return ref HierarchyPropertyId.s_Null;
			}
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public HierarchyPropertyId()
		{
			this.m_Id = 0;
		}

		internal HierarchyPropertyId(int id)
		{
			this.m_Id = id;
		}

		public static bool operator ==(in HierarchyPropertyId lhs, in HierarchyPropertyId rhs)
		{
			return lhs.Id == rhs.Id;
		}

		public static bool operator !=(in HierarchyPropertyId lhs, in HierarchyPropertyId rhs)
		{
			return !((in lhs) == (in rhs));
		}

		public bool Equals(HierarchyPropertyId other)
		{
			return other.Id == this.Id;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", "HierarchyPropertyId", ((in this) == HierarchyPropertyId.Null) ? "Null" : this.Id);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is HierarchyPropertyId)
			{
				HierarchyPropertyId hierarchyPropertyId = (HierarchyPropertyId)obj;
				flag = this.Equals(hierarchyPropertyId);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		private const int k_HierarchyPropertyIdNull = 0;

		private static readonly HierarchyPropertyId s_Null;

		private readonly int m_Id;
	}
}
