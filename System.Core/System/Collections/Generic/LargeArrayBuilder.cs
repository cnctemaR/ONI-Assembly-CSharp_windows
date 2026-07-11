using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	internal struct LargeArrayBuilder<T>
	{
		public LargeArrayBuilder(bool initialize)
		{
			this = new LargeArrayBuilder<T>(int.MaxValue);
		}

		public LargeArrayBuilder(int maxCapacity)
		{
			this = default(LargeArrayBuilder<T>);
			this._first = (this._current = Array.Empty<T>());
			this._maxCapacity = maxCapacity;
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			if (this._index == this._current.Length)
			{
				this.AllocateBuffer();
			}
			T[] current = this._current;
			int index = this._index;
			this._index = index + 1;
			current[index] = item;
			this._count++;
		}

		public void AddRange(IEnumerable<T> items)
		{
			using (IEnumerator<T> enumerator = items.GetEnumerator())
			{
				T[] array = this._current;
				int num = this._index;
				while (enumerator.MoveNext())
				{
					if (num == array.Length)
					{
						this._count += num - this._index;
						this._index = num;
						this.AllocateBuffer();
						array = this._current;
						num = this._index;
					}
					array[num++] = enumerator.Current;
				}
				this._count += num - this._index;
				this._index = num;
			}
		}

		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			int num = 0;
			while (count > 0)
			{
				T[] buffer = this.GetBuffer(num);
				int num2 = Math.Min(count, buffer.Length);
				Array.Copy(buffer, 0, array, arrayIndex, num2);
				count -= num2;
				arrayIndex += num2;
				num++;
			}
		}

		public CopyPosition CopyTo(CopyPosition position, T[] array, int arrayIndex, int count)
		{
			LargeArrayBuilder<T>.<>c__DisplayClass15_0 CS$<>8__locals1;
			CS$<>8__locals1.count = count;
			CS$<>8__locals1.array = array;
			CS$<>8__locals1.arrayIndex = arrayIndex;
			int num = position.Row;
			int column = position.Column;
			T[] array2 = this.GetBuffer(num);
			int num2 = LargeArrayBuilder<T>.<CopyTo>g__CopyToCore|15_0(array2, column, ref CS$<>8__locals1);
			if (CS$<>8__locals1.count == 0)
			{
				return new CopyPosition(num, column + num2).Normalize(array2.Length);
			}
			do
			{
				array2 = this.GetBuffer(++num);
				num2 = LargeArrayBuilder<T>.<CopyTo>g__CopyToCore|15_0(array2, 0, ref CS$<>8__locals1);
			}
			while (CS$<>8__locals1.count > 0);
			return new CopyPosition(num, num2).Normalize(array2.Length);
		}

		public T[] GetBuffer(int index)
		{
			if (index == 0)
			{
				return this._first;
			}
			if (index > this._buffers.Count)
			{
				return this._current;
			}
			return this._buffers[index - 1];
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SlowAdd(T item)
		{
			this.Add(item);
		}

		public T[] ToArray()
		{
			T[] array;
			if (this.TryMove(out array))
			{
				return array;
			}
			array = new T[this._count];
			this.CopyTo(array, 0, this._count);
			return array;
		}

		public bool TryMove(out T[] array)
		{
			array = this._first;
			return this._count == this._first.Length;
		}

		private void AllocateBuffer()
		{
			if (this._count < 8)
			{
				int num = Math.Min((this._count == 0) ? 4 : (this._count * 2), this._maxCapacity);
				this._current = new T[num];
				Array.Copy(this._first, 0, this._current, 0, this._count);
				this._first = this._current;
				return;
			}
			int num2;
			if (this._count == 8)
			{
				num2 = 8;
			}
			else
			{
				this._buffers.Add(this._current);
				num2 = Math.Min(this._count, this._maxCapacity - this._count);
			}
			this._current = new T[num2];
			this._index = 0;
		}

		[CompilerGenerated]
		internal static int <CopyTo>g__CopyToCore|15_0(T[] sourceBuffer, int sourceIndex, ref LargeArrayBuilder<T>.<>c__DisplayClass15_0 A_2)
		{
			int num = Math.Min(sourceBuffer.Length - sourceIndex, A_2.count);
			Array.Copy(sourceBuffer, sourceIndex, A_2.array, A_2.arrayIndex, num);
			A_2.arrayIndex += num;
			A_2.count -= num;
			return num;
		}

		private const int StartingCapacity = 4;

		private const int ResizeLimit = 8;

		private readonly int _maxCapacity;

		private T[] _first;

		private ArrayBuilder<T[]> _buffers;

		private T[] _current;

		private int _index;

		private int _count;
	}
}
