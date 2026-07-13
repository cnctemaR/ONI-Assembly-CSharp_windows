using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal struct VisualNodeProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		internal unsafe VisualNodeProperty(VisualNodePropertyData* data)
		{
			this.m_Data = data;
		}

		public unsafe ref T this[VisualNodeHandle handle]
		{
			get
			{
				Debug.Assert(handle.Id > 0);
				return ref *(T*)((byte*)this.m_Data->Ptr + (IntPtr)(handle.Id - 1) * (IntPtr)sizeof(T));
			}
		}

		private unsafe readonly VisualNodePropertyData* m_Data;
	}
}
