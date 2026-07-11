using System;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	public class EventCounter : IDisposable
	{
		public EventCounter(string name, EventSource eventSource)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			this.InitializeBuffer();
			this._name = name;
			this._group = EventCounterGroup.GetEventCounterGroup(eventSource);
			this._group.Add(this);
			this._min = float.PositiveInfinity;
			this._max = float.NegativeInfinity;
		}

		public void WriteMetric(float value)
		{
			this.Enqueue(value);
		}

		public void Dispose()
		{
			EventCounterGroup group = this._group;
			if (group != null)
			{
				group.Remove(this);
				this._group = null;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"EventCounter '",
				this._name,
				"' Count ",
				this._count,
				" Mean ",
				((double)this._sum / (double)this._count).ToString("n3")
			});
		}

		private object MyLock
		{
			get
			{
				return this._bufferedValues;
			}
		}

		private void InitializeBuffer()
		{
			this._bufferedValues = new float[10];
			for (int i = 0; i < this._bufferedValues.Length; i++)
			{
				this._bufferedValues[i] = float.NegativeInfinity;
			}
		}

		private void Enqueue(float value)
		{
			int num = this._bufferedValuesIndex;
			float num2;
			do
			{
				num2 = Interlocked.CompareExchange(ref this._bufferedValues[num], value, float.NegativeInfinity);
				num++;
				if (this._bufferedValues.Length <= num)
				{
					object myLock = this.MyLock;
					lock (myLock)
					{
						this.Flush();
					}
					num = 0;
				}
			}
			while (num2 != float.NegativeInfinity);
			this._bufferedValuesIndex = num;
		}

		private void Flush()
		{
			for (int i = 0; i < this._bufferedValues.Length; i++)
			{
				float num = Interlocked.Exchange(ref this._bufferedValues[i], float.NegativeInfinity);
				if (num != float.NegativeInfinity)
				{
					this.OnMetricWritten(num);
				}
			}
			this._bufferedValuesIndex = 0;
		}

		private void OnMetricWritten(float value)
		{
			this._sum += value;
			this._sumSquared += value * value;
			if (value > this._max)
			{
				this._max = value;
			}
			if (value < this._min)
			{
				this._min = value;
			}
			this._count++;
		}

		internal EventCounterPayload GetEventCounterPayload()
		{
			object myLock = this.MyLock;
			EventCounterPayload eventCounterPayload2;
			lock (myLock)
			{
				this.Flush();
				EventCounterPayload eventCounterPayload = new EventCounterPayload();
				eventCounterPayload.Name = this._name;
				eventCounterPayload.Count = this._count;
				if (0 < this._count)
				{
					eventCounterPayload.Mean = this._sum / (float)this._count;
					eventCounterPayload.StandardDeviation = (float)Math.Sqrt((double)(this._sumSquared / (float)this._count - this._sum * this._sum / (float)this._count / (float)this._count));
				}
				else
				{
					eventCounterPayload.Mean = 0f;
					eventCounterPayload.StandardDeviation = 0f;
				}
				eventCounterPayload.Min = this._min;
				eventCounterPayload.Max = this._max;
				this.ResetStatistics();
				eventCounterPayload2 = eventCounterPayload;
			}
			return eventCounterPayload2;
		}

		private void ResetStatistics()
		{
			this._count = 0;
			this._sum = 0f;
			this._sumSquared = 0f;
			this._min = float.PositiveInfinity;
			this._max = float.NegativeInfinity;
		}

		private readonly string _name;

		private EventCounterGroup _group;

		private const int BufferedSize = 10;

		private const float UnusedBufferSlotValue = float.NegativeInfinity;

		private const int UnsetIndex = -1;

		private volatile float[] _bufferedValues;

		private volatile int _bufferedValuesIndex;

		private int _count;

		private float _sum;

		private float _sumSquared;

		private float _min;

		private float _max;
	}
}
