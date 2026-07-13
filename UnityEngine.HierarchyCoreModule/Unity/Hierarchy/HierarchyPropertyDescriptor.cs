using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyDescriptor.h")]
	public struct HierarchyPropertyDescriptor
	{
		public int Size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size = value;
			}
		}

		public HierarchyPropertyStorageType Type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		private int m_Size;

		private HierarchyPropertyStorageType m_Type;
	}
}
