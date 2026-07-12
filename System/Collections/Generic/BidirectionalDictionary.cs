using System;

namespace System.Collections.Generic
{
	internal sealed class BidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>, IEnumerable
	{
		public BidirectionalDictionary(int capacity)
		{
			this._forward = new Dictionary<T1, T2>(capacity);
			this._backward = new Dictionary<T2, T1>(capacity);
		}

		public int Count
		{
			get
			{
				return this._forward.Count;
			}
		}

		public void Add(T1 item1, T2 item2)
		{
			this._forward.Add(item1, item2);
			this._backward.Add(item2, item1);
		}

		public bool TryGetForward(T1 item1, out T2 item2)
		{
			return this._forward.TryGetValue(item1, out item2);
		}

		public bool TryGetBackward(T2 item2, out T1 item1)
		{
			return this._backward.TryGetValue(item2, out item1);
		}

		public Dictionary<T1, T2>.Enumerator GetEnumerator()
		{
			return this._forward.GetEnumerator();
		}

		IEnumerator<KeyValuePair<T1, T2>> IEnumerable<KeyValuePair<T1, T2>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly Dictionary<T1, T2> _forward;

		private readonly Dictionary<T2, T1> _backward;
	}
}
