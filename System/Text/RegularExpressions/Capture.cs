using System;
using Unity;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Capture
	{
		internal Capture(string text, int i, int l)
		{
			this._text = text;
			this._index = i;
			this._length = l;
		}

		public int Index
		{
			get
			{
				return this._index;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public string Value
		{
			get
			{
				return this._text.Substring(this._index, this._length);
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		internal string GetOriginalString()
		{
			return this._text;
		}

		internal string GetLeftSubstring()
		{
			return this._text.Substring(0, this._index);
		}

		internal string GetRightSubstring()
		{
			return this._text.Substring(this._index + this._length, this._text.Length - this._index - this._length);
		}

		internal Capture()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal string _text;

		internal int _index;

		internal int _length;
	}
}
