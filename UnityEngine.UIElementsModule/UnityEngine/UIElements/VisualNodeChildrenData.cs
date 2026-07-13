using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal readonly struct VisualNodeChildrenData : IEnumerable<VisualNodeHandle>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.m_Alloc.IsCreated ? this.m_Alloc.Count : this.m_Fixed.Count;
			}
		}

		public VisualNodeHandle this[int index]
		{
			get
			{
				return this.m_Alloc.IsCreated ? this.m_Alloc[index] : this.m_Fixed[index];
			}
		}

		public VisualNodeHandle ElementAt(int index)
		{
			return this.m_Alloc.IsCreated ? this.m_Alloc[index] : this.m_Fixed[index];
		}

		public unsafe VisualNodeHandle* GetUnsafePtr()
		{
			fixed (VisualNodeChildrenData* ptr = &this)
			{
				return (VisualNodeHandle*)ptr;
			}
		}

		public VisualNodeChildrenData.Enumerator GetEnumerator()
		{
			return new VisualNodeChildrenData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		IEnumerator<VisualNodeHandle> IEnumerable<VisualNodeHandle>.GetEnumerator()
		{
			return new VisualNodeChildrenData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new VisualNodeChildrenData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		[FieldOffset(0)]
		private readonly VisualNodeChildrenFixed m_Fixed;

		[FieldOffset(0)]
		private readonly VisualNodeChildrenAlloc m_Alloc;

		public struct Enumerator : IEnumerator<VisualNodeHandle>, IEnumerator, IDisposable
		{
			public unsafe Enumerator(VisualNodeHandle* ptr, int count)
			{
				this.m_Ptr = ptr;
				this.m_Count = count;
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Count;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public unsafe VisualNodeHandle Current
			{
				get
				{
					return this.m_Ptr[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this.m_Ptr = null;
			}

			private unsafe VisualNodeHandle* m_Ptr;

			private int m_Count;

			private int m_Index;
		}
	}
}
