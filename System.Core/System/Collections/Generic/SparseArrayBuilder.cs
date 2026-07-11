using System;

namespace System.Collections.Generic
{
	internal struct SparseArrayBuilder<T>
	{
		public SparseArrayBuilder(bool initialize)
		{
			this = default(SparseArrayBuilder<T>);
			this._builder = new LargeArrayBuilder<T>(true);
		}

		public int Count
		{
			get
			{
				return checked(this._builder.Count + this._reservedCount);
			}
		}

		public ArrayBuilder<Marker> Markers
		{
			get
			{
				return this._markers;
			}
		}

		public void Add(T item)
		{
			this._builder.Add(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			this._builder.AddRange(items);
		}

		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			int num = 0;
			CopyPosition copyPosition = CopyPosition.Start;
			for (int i = 0; i < this._markers.Count; i++)
			{
				Marker marker = this._markers[i];
				int num2 = Math.Min(marker.Index - num, count);
				if (num2 > 0)
				{
					copyPosition = this._builder.CopyTo(copyPosition, array, arrayIndex, num2);
					arrayIndex += num2;
					num += num2;
					count -= num2;
				}
				if (count == 0)
				{
					return;
				}
				int num3 = Math.Min(marker.Count, count);
				arrayIndex += num3;
				num += num3;
				count -= num3;
			}
			if (count > 0)
			{
				this._builder.CopyTo(copyPosition, array, arrayIndex, count);
			}
		}

		public void Reserve(int count)
		{
			this._markers.Add(new Marker(count, this.Count));
			checked
			{
				this._reservedCount += count;
			}
		}

		public bool ReserveOrAdd(IEnumerable<T> items)
		{
			int num;
			if (EnumerableHelpers.TryGetCount<T>(items, out num))
			{
				if (num > 0)
				{
					this.Reserve(num);
					return true;
				}
			}
			else
			{
				this.AddRange(items);
			}
			return false;
		}

		public T[] ToArray()
		{
			if (this._markers.Count == 0)
			{
				return this._builder.ToArray();
			}
			T[] array = new T[this.Count];
			this.CopyTo(array, 0, array.Length);
			return array;
		}

		private LargeArrayBuilder<T> _builder;

		private ArrayBuilder<Marker> _markers;

		private int _reservedCount;
	}
}
