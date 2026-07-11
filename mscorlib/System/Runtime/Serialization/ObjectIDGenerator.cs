using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[Serializable]
	public class ObjectIDGenerator
	{
		public ObjectIDGenerator()
		{
			this.m_currentCount = 1;
			this.m_currentSize = ObjectIDGenerator.sizes[0];
			this.m_ids = new long[this.m_currentSize * 4];
			this.m_objs = new object[this.m_currentSize * 4];
		}

		private int FindElement(object obj, out bool found)
		{
			int num = RuntimeHelpers.GetHashCode(obj);
			int num2 = 1 + (num & int.MaxValue) % (this.m_currentSize - 2);
			int i;
			for (;;)
			{
				int num3 = (num & int.MaxValue) % this.m_currentSize * 4;
				for (i = num3; i < num3 + 4; i++)
				{
					if (this.m_objs[i] == null)
					{
						goto Block_1;
					}
					if (this.m_objs[i] == obj)
					{
						goto Block_2;
					}
				}
				num += num2;
			}
			Block_1:
			found = false;
			return i;
			Block_2:
			found = true;
			return i;
		}

		public virtual long GetId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", Environment.GetResourceString("Object cannot be null."));
			}
			bool flag;
			int num = this.FindElement(obj, out flag);
			long num3;
			if (!flag)
			{
				this.m_objs[num] = obj;
				long[] ids = this.m_ids;
				int num2 = num;
				int currentCount = this.m_currentCount;
				this.m_currentCount = currentCount + 1;
				ids[num2] = (long)currentCount;
				num3 = this.m_ids[num];
				if (this.m_currentCount > this.m_currentSize * 4 / 2)
				{
					this.Rehash();
				}
			}
			else
			{
				num3 = this.m_ids[num];
			}
			firstTime = !flag;
			return num3;
		}

		public virtual long HasId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", Environment.GetResourceString("Object cannot be null."));
			}
			bool flag;
			int num = this.FindElement(obj, out flag);
			if (flag)
			{
				firstTime = false;
				return this.m_ids[num];
			}
			firstTime = true;
			return 0L;
		}

		private void Rehash()
		{
			int num = 0;
			int currentSize = this.m_currentSize;
			while (num < ObjectIDGenerator.sizes.Length && ObjectIDGenerator.sizes[num] <= currentSize)
			{
				num++;
			}
			if (num == ObjectIDGenerator.sizes.Length)
			{
				throw new SerializationException(Environment.GetResourceString("The internal array cannot expand to greater than Int32.MaxValue elements."));
			}
			this.m_currentSize = ObjectIDGenerator.sizes[num];
			long[] array = new long[this.m_currentSize * 4];
			object[] array2 = new object[this.m_currentSize * 4];
			long[] ids = this.m_ids;
			object[] objs = this.m_objs;
			this.m_ids = array;
			this.m_objs = array2;
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] != null)
				{
					bool flag;
					int num2 = this.FindElement(objs[i], out flag);
					this.m_objs[num2] = objs[i];
					this.m_ids[num2] = ids[i];
				}
			}
		}

		private const int numbins = 4;

		internal int m_currentCount;

		internal int m_currentSize;

		internal long[] m_ids;

		internal object[] m_objs;

		private static readonly int[] sizes = new int[]
		{
			5, 11, 29, 47, 97, 197, 397, 797, 1597, 3203,
			6421, 12853, 25717, 51437, 102877, 205759, 411527, 823117, 1646237, 3292489,
			6584983
		};
	}
}
