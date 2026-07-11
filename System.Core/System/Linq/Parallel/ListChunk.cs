using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class ListChunk<TInputOutput> : IEnumerable<TInputOutput>, IEnumerable
	{
		internal ListChunk(int size)
		{
			this._chunk = new TInputOutput[size];
			this._chunkCount = 0;
			this._tailChunk = this;
		}

		internal void Add(TInputOutput e)
		{
			ListChunk<TInputOutput> listChunk = this._tailChunk;
			if (listChunk._chunkCount == listChunk._chunk.Length)
			{
				this._tailChunk = new ListChunk<TInputOutput>(listChunk._chunkCount * 2);
				listChunk = (listChunk._nextChunk = this._tailChunk);
			}
			TInputOutput[] chunk = listChunk._chunk;
			ListChunk<TInputOutput> listChunk2 = listChunk;
			int chunkCount = listChunk2._chunkCount;
			listChunk2._chunkCount = chunkCount + 1;
			chunk[chunkCount] = e;
		}

		internal ListChunk<TInputOutput> Next
		{
			get
			{
				return this._nextChunk;
			}
		}

		internal int Count
		{
			get
			{
				return this._chunkCount;
			}
		}

		public IEnumerator<TInputOutput> GetEnumerator()
		{
			for (ListChunk<TInputOutput> curr = this; curr != null; curr = curr._nextChunk)
			{
				int num;
				for (int i = 0; i < curr._chunkCount; i = num + 1)
				{
					yield return curr._chunk[i];
					num = i;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TInputOutput>)this).GetEnumerator();
		}

		internal TInputOutput[] _chunk;

		private int _chunkCount;

		private ListChunk<TInputOutput> _nextChunk;

		private ListChunk<TInputOutput> _tailChunk;
	}
}
