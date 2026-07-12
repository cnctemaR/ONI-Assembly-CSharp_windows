using System;
using System.Collections;
using System.Collections.Generic;
using Unity;

namespace System
{
	[Serializable]
	public sealed class CharEnumerator : IEnumerator, IEnumerator<char>, IDisposable, ICloneable
	{
		internal CharEnumerator(string str)
		{
			this._str = str;
			this._index = -1;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		public bool MoveNext()
		{
			if (this._index < this._str.Length - 1)
			{
				this._index++;
				this._currentElement = this._str[this._index];
				return true;
			}
			this._index = this._str.Length;
			return false;
		}

		public void Dispose()
		{
			if (this._str != null)
			{
				this._index = this._str.Length;
			}
			this._str = null;
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public char Current
		{
			get
			{
				if (this._index == -1)
				{
					throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
				}
				if (this._index >= this._str.Length)
				{
					throw new InvalidOperationException("Enumeration already finished.");
				}
				return this._currentElement;
			}
		}

		public void Reset()
		{
			this._currentElement = '\0';
			this._index = -1;
		}

		internal CharEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string _str;

		private int _index;

		private char _currentElement;
	}
}
