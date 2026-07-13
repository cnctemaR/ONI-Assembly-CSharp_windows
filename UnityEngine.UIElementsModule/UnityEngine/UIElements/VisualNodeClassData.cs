using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal readonly struct VisualNodeClassData : IEnumerable<int>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.m_Alloc.IsCreated ? this.m_Alloc.Count : this.m_Fixed.Count;
			}
		}

		public int this[int index]
		{
			get
			{
				return this.m_Alloc.IsCreated ? this.m_Alloc[index] : this.m_Fixed[index];
			}
		}

		public unsafe int* GetUnsafePtr()
		{
			fixed (VisualNodeClassData* ptr = &this)
			{
				return (int*)ptr;
			}
		}

		public VisualNodeClassData.Enumerator GetEnumerator()
		{
			return new VisualNodeClassData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return new VisualNodeClassData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new VisualNodeClassData.Enumerator(this.GetUnsafePtr(), this.Count);
		}

		[FieldOffset(0)]
		private readonly VisualNodeClassDataFixed m_Fixed;

		[FieldOffset(0)]
		private readonly VisualNodeClassDataAlloc m_Alloc;

		public struct Enumerator : IEnumerator<int>, IEnumerator, IDisposable
		{
			public unsafe Enumerator(int* ptr, int count)
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

			public unsafe int Current
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

			private unsafe int* m_Ptr;

			private int m_Count;

			private int m_Index;
		}
	}
}
