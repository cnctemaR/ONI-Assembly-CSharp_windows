using System;
using System.Collections;
using System.Collections.Generic;

public class KSplitCompactedVector<Header, Payload> : KCompactedVectorBase, ICollection, IEnumerable
{
	public KSplitCompactedVector(int initial_count = 0)
		: base(initial_count)
	{
		this.headers = new List<Header>(initial_count);
		this.payloads = new List<Payload>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(Header header, ref Payload payload)
	{
		this.headers.Add(header);
		this.payloads.Add(payload);
		return base.Allocate(this.headers.Count - 1);
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		int num = this.headers.Count - 1;
		int num2;
		bool flag = base.Free(handle, num, out num2);
		if (flag)
		{
			if (num2 < num)
			{
				this.headers[num2] = this.headers[num];
				this.payloads[num2] = this.payloads[num];
			}
			this.headers.RemoveAt(num);
			this.payloads.RemoveAt(num);
		}
		if (!flag)
		{
			return handle;
		}
		return HandleVector<int>.InvalidHandle;
	}

	public void GetData(HandleVector<int>.Handle handle, out Header header, out Payload payload)
	{
		int num = base.ComputeIndex(handle);
		header = this.headers[num];
		payload = this.payloads[num];
	}

	public Header GetHeader(HandleVector<int>.Handle handle)
	{
		return this.headers[base.ComputeIndex(handle)];
	}

	public Payload GetPayload(HandleVector<int>.Handle handle)
	{
		return this.payloads[base.ComputeIndex(handle)];
	}

	public void SetData(HandleVector<int>.Handle handle, Header new_data0, ref Payload new_data1)
	{
		int num = base.ComputeIndex(handle);
		this.headers[num] = new_data0;
		this.payloads[num] = new_data1;
	}

	public void SetHeader(HandleVector<int>.Handle handle, Header new_data)
	{
		this.headers[base.ComputeIndex(handle)] = new_data;
	}

	public void SetPayload(HandleVector<int>.Handle handle, ref Payload new_data)
	{
		this.payloads[base.ComputeIndex(handle)] = new_data;
	}

	public new virtual void Clear()
	{
		base.Clear();
		this.headers.Clear();
		this.payloads.Clear();
	}

	public int Count
	{
		get
		{
			return this.headers.Count;
		}
	}

	public void GetDataLists(out List<Header> headers, out List<Payload> payloads)
	{
		headers = this.headers;
		payloads = this.payloads;
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}

	public IEnumerator GetEnumerator()
	{
		return new KSplitCompactedVector<Header, Payload>.Enumerator(this.headers.GetEnumerator(), this.payloads.GetEnumerator());
	}

	protected List<Header> headers;

	protected List<Payload> payloads;

	private struct Enumerator : IEnumerator
	{
		public object Current
		{
			get
			{
				return new KSplitCompactedVector<Header, Payload>.Enumerator.Value
				{
					header = this.headerCurrent.Current,
					payload = this.payloadCurrent.Current
				};
			}
		}

		public Enumerator(List<Header>.Enumerator headerEnumerator, List<Payload>.Enumerator payloadEnumerator)
		{
			this.headerBegin = headerEnumerator;
			this.payloadBegin = payloadEnumerator;
			this.Reset();
		}

		public bool MoveNext()
		{
			return this.headerCurrent.MoveNext() && this.payloadCurrent.MoveNext();
		}

		public void Reset()
		{
			this.headerCurrent = this.headerBegin;
			this.payloadCurrent = this.payloadBegin;
		}

		private readonly List<Header>.Enumerator headerBegin;

		private readonly List<Payload>.Enumerator payloadBegin;

		private List<Header>.Enumerator headerCurrent;

		private List<Payload>.Enumerator payloadCurrent;

		public struct Value
		{
			public Header header;

			public Payload payload;
		}
	}
}
