using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	internal class CaptureEnumerator : IEnumerator
	{
		internal CaptureEnumerator(CaptureCollection rcc)
		{
			this._curindex = -1;
			this._rcc = rcc;
		}

		public bool MoveNext()
		{
			int count = this._rcc.Count;
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
				if (this._curindex < 0 || this._curindex >= this._rcc.Count)
				{
					throw new InvalidOperationException(global::SR.GetString("Enumeration has either not started or has already finished."));
				}
				return this._rcc[this._curindex];
			}
		}

		public void Reset()
		{
			this._curindex = -1;
		}

		internal CaptureCollection _rcc;

		internal int _curindex;
	}
}
