using System;
using System.Reflection;

namespace System.Runtime.Serialization
{
	internal sealed class ValueTypeFixupInfo
	{
		public ValueTypeFixupInfo(long containerID, FieldInfo member, int[] parentIndex)
		{
			if (member == null && parentIndex == null)
			{
				throw new ArgumentException("When supplying the ID of a containing object, the FieldInfo that identifies the current field within that object must also be supplied.");
			}
			if (containerID == 0L && member == null)
			{
				this._containerID = containerID;
				this._parentField = member;
				this._parentIndex = parentIndex;
			}
			if (member != null)
			{
				if (parentIndex != null)
				{
					throw new ArgumentException("Cannot supply both a MemberInfo and an Array to indicate the parent of a value type.");
				}
				if (member.FieldType.IsValueType && containerID == 0L)
				{
					throw new ArgumentException("When supplying a FieldInfo for fixing up a nested type, a valid ID for that containing object must also be supplied.");
				}
			}
			this._containerID = containerID;
			this._parentField = member;
			this._parentIndex = parentIndex;
		}

		public long ContainerID
		{
			get
			{
				return this._containerID;
			}
		}

		public FieldInfo ParentField
		{
			get
			{
				return this._parentField;
			}
		}

		public int[] ParentIndex
		{
			get
			{
				return this._parentIndex;
			}
		}

		private readonly long _containerID;

		private readonly FieldInfo _parentField;

		private readonly int[] _parentIndex;
	}
}
