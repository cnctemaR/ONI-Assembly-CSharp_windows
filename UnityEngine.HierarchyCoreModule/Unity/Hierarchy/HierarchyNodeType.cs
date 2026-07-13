using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeType.h")]
	public readonly struct HierarchyNodeType : IEquatable<HierarchyNodeType>
	{
		public static readonly ref HierarchyNodeType Null
		{
			get
			{
				return ref HierarchyNodeType.s_Null;
			}
		}

		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public HierarchyNodeType()
		{
			this.m_Id = 0;
		}

		internal HierarchyNodeType(int id)
		{
			this.m_Id = id;
		}

		[ExcludeFromDocs]
		public static bool operator ==(in HierarchyNodeType lhs, in HierarchyNodeType rhs)
		{
			return lhs.Id == rhs.Id;
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyNodeType lhs, in HierarchyNodeType rhs)
		{
			return !((in lhs) == (in rhs));
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyNodeType other)
		{
			return other.Id == this.Id;
		}

		[ExcludeFromDocs]
		public override string ToString()
		{
			return string.Format("{0}({1})", "HierarchyNodeType", ((in this) == HierarchyNodeType.Null) ? "Null" : this.Id);
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is HierarchyNodeType)
			{
				HierarchyNodeType hierarchyNodeType = (HierarchyNodeType)obj;
				flag = this.Equals(hierarchyNodeType);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[ExcludeFromDocs]
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		internal const int k_HierarchyNodeTypeNull = 0;

		private static readonly HierarchyNodeType s_Null;

		private readonly int m_Id;
	}
}
