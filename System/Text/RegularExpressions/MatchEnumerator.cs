using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	internal class MatchEnumerator : IEnumerator
	{
		internal MatchEnumerator(MatchCollection matchcoll)
		{
			this._matchcoll = matchcoll;
		}

		public bool MoveNext()
		{
			if (this._done)
			{
				return false;
			}
			this._match = this._matchcoll.GetMatch(this._curindex);
			this._curindex++;
			if (this._match == null)
			{
				this._done = true;
				return false;
			}
			return true;
		}

		public object Current
		{
			get
			{
				if (this._match == null)
				{
					throw new InvalidOperationException(global::SR.GetString("Enumeration has either not started or has already finished."));
				}
				return this._match;
			}
		}

		public void Reset()
		{
			this._curindex = 0;
			this._done = false;
			this._match = null;
		}

		internal MatchCollection _matchcoll;

		internal Match _match;

		internal int _curindex;

		internal bool _done;
	}
}
