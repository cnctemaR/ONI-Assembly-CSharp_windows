using System;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Capture
	{
		internal Capture(string text)
			: this(text, 0, 0)
		{
		}

		internal Capture(string text, int index, int length)
		{
			this.text = text;
			this.index = index;
			this.length = length;
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public string Value
		{
			get
			{
				return (this.text != null) ? this.text.Substring(this.index, this.length) : string.Empty;
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		internal string Text
		{
			get
			{
				return this.text;
			}
		}

		internal int index;

		internal int length;

		internal string text;
	}
}
