using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class CharEnumerator : IEnumerator, IDisposable, ICloneable, IEnumerator<char>
	{
		internal CharEnumerator(string s)
		{
			this.str = s;
			this.index = -1;
			this.length = s.Length;
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		void IDisposable.Dispose()
		{
		}

		public char Current
		{
			get
			{
				if (this.index == -1 || this.index >= this.length)
				{
					throw new InvalidOperationException(Locale.GetText("The position is not valid."));
				}
				return this.str[this.index];
			}
		}

		public object Clone()
		{
			return new CharEnumerator(this.str)
			{
				index = this.index
			};
		}

		public bool MoveNext()
		{
			this.index++;
			if (this.index >= this.length)
			{
				this.index = this.length;
				return false;
			}
			return true;
		}

		public void Reset()
		{
			this.index = -1;
		}

		private string str;

		private int index;

		private int length;
	}
}
