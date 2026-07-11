using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class GroupEnumerator : IEnumerator
	{
		internal GroupEnumerator(GroupCollection rgc)
		{
			this._curindex = -1;
			this._rgc = rgc;
		}

		public bool MoveNext()
		{
			int count = this._rgc.Count;
			if (this._curindex >= count)
			{
				return false;
			}
			this._curindex++;
			return this._curindex < count;
		}

		public object Current
		{
			get
			{
				return this.Capture;
			}
		}

		public Capture Capture
		{
			get
			{
				if (this._curindex < 0 || this._curindex >= this._rgc.Count)
				{
					throw new InvalidOperationException(global::SR.GetString("Enumeration has either not started or has already finished."));
				}
				return this._rgc[this._curindex];
			}
		}

		public void Reset()
		{
			this._curindex = -1;
		}

		internal GroupCollection _rgc;

		internal int _curindex;
	}
}
