using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	internal abstract class HeaderInfo
	{
		protected HeaderInfo(string name, HttpHeaderKind headerKind)
		{
			this.Name = name;
			this.HeaderKind = headerKind;
		}

		public static HeaderInfo CreateSingle<T>(string name, TryParseDelegate<T> parser, HttpHeaderKind headerKind, Func<object, string> toString = null)
		{
			return new HeaderInfo.HeaderTypeInfo<T, object>(name, parser, headerKind)
			{
				CustomToString = toString
			};
		}

		public static HeaderInfo CreateMulti<T>(string name, TryParseListDelegate<T> elementParser, HttpHeaderKind headerKind, int minimalCount = 1, string separator = ", ") where T : class
		{
			return new HeaderInfo.CollectionHeaderTypeInfo<T, T>(name, elementParser, headerKind, minimalCount, separator);
		}

		public object CreateCollection(HttpHeaders headers)
		{
			return this.CreateCollection(headers, this);
		}

		public Func<object, string> CustomToString { get; private set; }

		public virtual string Separator
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public abstract void AddToCollection(object collection, object value);

		protected abstract object CreateCollection(HttpHeaders headers, HeaderInfo headerInfo);

		public abstract List<string> ToStringCollection(object collection);

		public abstract bool TryParse(string value, out object result);

		public bool AllowsMany;

		public readonly HttpHeaderKind HeaderKind;

		public readonly string Name;

		private class HeaderTypeInfo<T, U> : HeaderInfo where U : class
		{
			public HeaderTypeInfo(string name, TryParseDelegate<T> parser, HttpHeaderKind headerKind)
				: base(name, headerKind)
			{
				this.parser = parser;
			}

			public override void AddToCollection(object collection, object value)
			{
				HttpHeaderValueCollection<U> httpHeaderValueCollection = (HttpHeaderValueCollection<U>)collection;
				List<U> list = value as List<U>;
				if (list != null)
				{
					httpHeaderValueCollection.AddRange(list);
					return;
				}
				httpHeaderValueCollection.Add((U)((object)value));
			}

			protected override object CreateCollection(HttpHeaders headers, HeaderInfo headerInfo)
			{
				return new HttpHeaderValueCollection<U>(headers, headerInfo);
			}

			public override List<string> ToStringCollection(object collection)
			{
				if (collection == null)
				{
					return null;
				}
				HttpHeaderValueCollection<U> httpHeaderValueCollection = (HttpHeaderValueCollection<U>)collection;
				if (httpHeaderValueCollection.Count != 0)
				{
					List<string> list = new List<string>();
					foreach (U u in httpHeaderValueCollection)
					{
						list.Add(u.ToString());
					}
					if (httpHeaderValueCollection.InvalidValues != null)
					{
						list.AddRange(httpHeaderValueCollection.InvalidValues);
					}
					return list;
				}
				if (httpHeaderValueCollection.InvalidValues == null)
				{
					return null;
				}
				return new List<string>(httpHeaderValueCollection.InvalidValues);
			}

			public override bool TryParse(string value, out object result)
			{
				T t;
				bool flag = this.parser(value, out t);
				result = t;
				return flag;
			}

			private readonly TryParseDelegate<T> parser;
		}

		private class CollectionHeaderTypeInfo<T, U> : HeaderInfo.HeaderTypeInfo<T, U> where U : class
		{
			public CollectionHeaderTypeInfo(string name, TryParseListDelegate<T> parser, HttpHeaderKind headerKind, int minimalCount, string separator)
				: base(name, null, headerKind)
			{
				this.parser = parser;
				this.minimalCount = minimalCount;
				this.AllowsMany = true;
				this.separator = separator;
			}

			public override string Separator
			{
				get
				{
					return this.separator;
				}
			}

			public override bool TryParse(string value, out object result)
			{
				List<T> list;
				if (!this.parser(value, this.minimalCount, out list))
				{
					result = null;
					return false;
				}
				result = list;
				return true;
			}

			private readonly int minimalCount;

			private readonly string separator;

			private readonly TryParseListDelegate<T> parser;
		}
	}
}
