using System;
using Unity;

namespace System.Text.RegularExpressions
{
	public class Capture
	{
		internal Capture(string text, int index, int length)
		{
			this.Text = text;
			this.Index = index;
			this.Length = length;
		}

		public int Index { get; private protected set; }

		public int Length { get; private protected set; }

		protected internal string Text { internal get; private protected set; }

		public string Value
		{
			get
			{
				return this.Text.Substring(this.Index, this.Length);
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		internal ReadOnlySpan<char> GetLeftSubstring()
		{
			return this.Text.AsSpan(0, this.Index);
		}

		internal ReadOnlySpan<char> GetRightSubstring()
		{
			return this.Text.AsSpan(this.Index + this.Length, this.Text.Length - this.Index - this.Length);
		}

		internal Capture()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
