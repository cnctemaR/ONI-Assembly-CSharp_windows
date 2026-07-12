using System;

namespace UnityEngine.TextCore.Text
{
	internal struct TextBackingContainer
	{
		public uint[] Text
		{
			get
			{
				return this.m_Array;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_Array.Length;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
			set
			{
				this.m_Count = value;
			}
		}

		public uint this[int index]
		{
			get
			{
				return this.m_Array[index];
			}
			set
			{
				bool flag = index >= this.m_Array.Length;
				if (flag)
				{
					this.Resize(index);
				}
				this.m_Array[index] = value;
			}
		}

		public TextBackingContainer(int size)
		{
			this.m_Array = new uint[size];
			this.m_Count = 0;
		}

		public void Resize(int size)
		{
			size = Mathf.NextPowerOfTwo(size + 1);
			Array.Resize<uint>(ref this.m_Array, size);
		}

		private uint[] m_Array;

		private int m_Count;
	}
}
