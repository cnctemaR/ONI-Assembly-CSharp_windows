using System;
using System.Collections;
using System.Collections.Generic;
using Unity;

namespace System.Net.Http.Headers
{
	public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
	{
		internal HttpHeaderValueCollection(HttpHeaders headers, HeaderInfo headerInfo)
		{
			this.list = new List<T>();
			this.headers = headers;
			this.headerInfo = headerInfo;
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		internal List<string> InvalidValues
		{
			get
			{
				return this.invalidValues;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(T item)
		{
			this.list.Add(item);
		}

		internal void AddRange(List<T> values)
		{
			this.list.AddRange(values);
		}

		internal void AddInvalidValue(string invalidValue)
		{
			if (this.invalidValues == null)
			{
				this.invalidValues = new List<string>();
			}
			this.invalidValues.Add(invalidValue);
		}

		public void Clear()
		{
			this.list.Clear();
			this.invalidValues = null;
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public void ParseAdd(string input)
		{
			this.headers.AddValue(input, this.headerInfo, false);
		}

		public bool Remove(T item)
		{
			return this.list.Remove(item);
		}

		public override string ToString()
		{
			string text = string.Join<T>(this.headerInfo.Separator, this.list);
			if (this.invalidValues != null)
			{
				text += string.Join(this.headerInfo.Separator, this.invalidValues);
			}
			return text;
		}

		public bool TryParseAdd(string input)
		{
			return this.headers.AddValue(input, this.headerInfo, true);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal T Find(Predicate<T> predicate)
		{
			return this.list.Find(predicate);
		}

		internal void Remove(Predicate<T> predicate)
		{
			T t = this.Find(predicate);
			if (t != null)
			{
				this.Remove(t);
			}
		}

		internal HttpHeaderValueCollection()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly List<T> list;

		private readonly HttpHeaders headers;

		private readonly HeaderInfo headerInfo;

		private List<string> invalidValues;
	}
}
