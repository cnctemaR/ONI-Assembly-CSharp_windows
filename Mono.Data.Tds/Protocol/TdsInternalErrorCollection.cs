using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public sealed class TdsInternalErrorCollection : IEnumerable
	{
		public TdsInternalErrorCollection()
		{
			this.list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public TdsInternalError this[int index]
		{
			get
			{
				return (TdsInternalError)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public int Add(TdsInternalError error)
		{
			return this.list.Add(error);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		private ArrayList list;
	}
}
