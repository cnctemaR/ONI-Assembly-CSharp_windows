using System;
using System.Reflection;

namespace System.Runtime.Serialization
{
	internal class ValueTypeFixupInfo
	{
		public ValueTypeFixupInfo(long containerID, FieldInfo member, int[] parentIndex)
		{
			if (member == null && parentIndex == null)
			{
				throw new ArgumentException(Environment.GetResourceString("When supplying the ID of a containing object, the FieldInfo that identifies the current field within that object must also be supplied."));
			}
			if (containerID == 0L && member == null)
			{
				this.m_containerID = containerID;
				this.m_parentField = member;
				this.m_parentIndex = parentIndex;
			}
			if (member != null)
			{
				if (parentIndex != null)
				{
					throw new ArgumentException(Environment.GetResourceString("Cannot supply both a MemberInfo and an Array to indicate the parent of a value type."));
				}
				if (member.FieldType.IsValueType && containerID == 0L)
				{
					throw new ArgumentException(Environment.GetResourceString("When supplying a FieldInfo for fixing up a nested type, a valid ID for that containing object must also be supplied."));
				}
			}
			this.m_containerID = containerID;
			this.m_parentField = member;
			this.m_parentIndex = parentIndex;
		}

		public long ContainerID
		{
			get
			{
				return this.m_containerID;
			}
		}

		public FieldInfo ParentField
		{
			get
			{
				return this.m_parentField;
			}
		}

		public int[] ParentIndex
		{
			get
			{
				return this.m_parentIndex;
			}
		}

		private long m_containerID;

		private FieldInfo m_parentField;

		private int[] m_parentIndex;
	}
}
