using System;
using System.Collections.Generic;

public class FloatHOTQueue<TValue>
{
	public KeyValuePair<float, TValue> Dequeue()
	{
		if (this.hotQueue.Count == 0)
		{
			FloatHOTQueue<TValue>.PriorityQueue priorityQueue = this.hotQueue;
			this.hotQueue = this.coldQueue;
			this.coldQueue = priorityQueue;
			this.hotThreshold = this.coldThreshold;
		}
		this.count--;
		return this.hotQueue.Dequeue();
	}

	public void Enqueue(float priority, TValue value)
	{
		if (priority <= this.hotThreshold)
		{
			this.hotQueue.Enqueue(priority, value);
		}
		else
		{
			this.coldQueue.Enqueue(priority, value);
			this.coldThreshold = Math.Max(this.coldThreshold, priority);
		}
		this.count++;
	}

	public KeyValuePair<float, TValue> Peek()
	{
		if (this.hotQueue.Count == 0)
		{
			FloatHOTQueue<TValue>.PriorityQueue priorityQueue = this.hotQueue;
			this.hotQueue = this.coldQueue;
			this.coldQueue = priorityQueue;
			this.hotThreshold = this.coldThreshold;
		}
		return this.hotQueue.Peek();
	}

	public void Clear()
	{
		this.count = 0;
		this.hotThreshold = float.MinValue;
		this.hotQueue.Clear();
		this.coldThreshold = float.MinValue;
		this.coldQueue.Clear();
	}

	public int Count
	{
		get
		{
			return this.count;
		}
	}

	private FloatHOTQueue<TValue>.PriorityQueue hotQueue = new FloatHOTQueue<TValue>.PriorityQueue();

	private FloatHOTQueue<TValue>.PriorityQueue coldQueue = new FloatHOTQueue<TValue>.PriorityQueue();

	private float hotThreshold = float.MinValue;

	private float coldThreshold = float.MinValue;

	private int count;

	private class PriorityQueue
	{
		public PriorityQueue()
		{
			this._baseHeap = new List<KeyValuePair<float, TValue>>();
		}

		public void Enqueue(float priority, TValue value)
		{
			this.Insert(priority, value);
		}

		public KeyValuePair<float, TValue> Dequeue()
		{
			KeyValuePair<float, TValue> keyValuePair = this._baseHeap[0];
			this.DeleteRoot();
			return keyValuePair;
		}

		public KeyValuePair<float, TValue> Peek()
		{
			if (this.Count > 0)
			{
				return this._baseHeap[0];
			}
			throw new InvalidOperationException("Priority queue is empty");
		}

		private void ExchangeElements(int pos1, int pos2)
		{
			KeyValuePair<float, TValue> keyValuePair = this._baseHeap[pos1];
			this._baseHeap[pos1] = this._baseHeap[pos2];
			this._baseHeap[pos2] = keyValuePair;
		}

		private void Insert(float priority, TValue value)
		{
			KeyValuePair<float, TValue> keyValuePair = new KeyValuePair<float, TValue>(priority, value);
			this._baseHeap.Add(keyValuePair);
			this.HeapifyFromEndToBeginning(this._baseHeap.Count - 1);
		}

		private int HeapifyFromEndToBeginning(int pos)
		{
			if (pos >= this._baseHeap.Count)
			{
				return -1;
			}
			while (pos > 0)
			{
				int num = (pos - 1) / 2;
				if (this._baseHeap[num].Key - this._baseHeap[pos].Key <= 0f)
				{
					break;
				}
				this.ExchangeElements(num, pos);
				pos = num;
			}
			return pos;
		}

		private void DeleteRoot()
		{
			if (this._baseHeap.Count <= 1)
			{
				this._baseHeap.Clear();
				return;
			}
			this._baseHeap[0] = this._baseHeap[this._baseHeap.Count - 1];
			this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
			this.HeapifyFromBeginningToEnd(0);
		}

		private void HeapifyFromBeginningToEnd(int pos)
		{
			int count = this._baseHeap.Count;
			if (pos >= count)
			{
				return;
			}
			for (;;)
			{
				int num = pos;
				int num2 = 2 * pos + 1;
				int num3 = 2 * pos + 2;
				if (num2 < count && this._baseHeap[num].Key - this._baseHeap[num2].Key > 0f)
				{
					num = num2;
				}
				if (num3 < count && this._baseHeap[num].Key - this._baseHeap[num3].Key > 0f)
				{
					num = num3;
				}
				if (num == pos)
				{
					break;
				}
				this.ExchangeElements(num, pos);
				pos = num;
			}
		}

		public void Clear()
		{
			this._baseHeap.Clear();
		}

		public int Count
		{
			get
			{
				return this._baseHeap.Count;
			}
		}

		private List<KeyValuePair<float, TValue>> _baseHeap;
	}
}
