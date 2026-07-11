using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class MatchCollection : ICollection, IEnumerable
	{
		internal MatchCollection(Regex regex, string input, int beginning, int length, int startat)
		{
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat", global::SR.GetString("Start index cannot be less than 0 or greater than input length."));
			}
			this._regex = regex;
			this._input = input;
			this._beginning = beginning;
			this._length = length;
			this._startat = startat;
			this._prevlen = -1;
			this._matches = new ArrayList();
			this._done = false;
		}

		internal Match GetMatch(int i)
		{
			if (i < 0)
			{
				return null;
			}
			if (this._matches.Count > i)
			{
				return (Match)this._matches[i];
			}
			if (this._done)
			{
				return null;
			}
			for (;;)
			{
				Match match = this._regex.Run(false, this._prevlen, this._input, this._beginning, this._length, this._startat);
				if (!match.Success)
				{
					break;
				}
				this._matches.Add(match);
				this._prevlen = match._length;
				this._startat = match._textpos;
				if (this._matches.Count > i)
				{
					return match;
				}
			}
			this._done = true;
			return null;
		}

		public int Count
		{
			get
			{
				if (this._done)
				{
					return this._matches.Count;
				}
				this.GetMatch(MatchCollection.infinite);
				return this._matches.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public virtual Match this[int i]
		{
			get
			{
				Match match = this.GetMatch(i);
				if (match == null)
				{
					throw new ArgumentOutOfRangeException("i");
				}
				return match;
			}
		}

		public void CopyTo(Array array, int arrayIndex)
		{
			if (array != null && array.Rank != 1)
			{
				throw new ArgumentException(global::SR.GetString("Only single dimensional arrays are supported for the requested action."));
			}
			int count = this.Count;
			try
			{
				this._matches.CopyTo(array, arrayIndex);
			}
			catch (ArrayTypeMismatchException ex)
			{
				throw new ArgumentException(global::SR.GetString("Target array type is not compatible with the type of items in the collection."), ex);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new MatchEnumerator(this);
		}

		internal Regex _regex;

		internal ArrayList _matches;

		internal bool _done;

		internal string _input;

		internal int _beginning;

		internal int _length;

		internal int _startat;

		internal int _prevlen;

		private static int infinite = int.MaxValue;
	}
}
