using System;

namespace System.Text
{
	public sealed class EncoderReplacementFallbackBuffer : EncoderFallbackBuffer
	{
		public EncoderReplacementFallbackBuffer(EncoderReplacementFallback fallback)
		{
			if (fallback == null)
			{
				throw new ArgumentNullException("fallback");
			}
			this.replacement = fallback.DefaultString;
			this.current = 0;
		}

		public override int Remaining
		{
			get
			{
				return this.replacement.Length - this.current;
			}
		}

		public override bool Fallback(char charUnknown, int index)
		{
			return this.Fallback(index);
		}

		public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
		{
			return this.Fallback(index);
		}

		private bool Fallback(int index)
		{
			if (this.fallback_assigned && this.Remaining != 0)
			{
				throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.fallback_assigned = true;
			this.current = 0;
			return this.replacement.Length > 0;
		}

		public override char GetNextChar()
		{
			if (this.current >= this.replacement.Length)
			{
				return '\0';
			}
			return this.replacement[this.current++];
		}

		public override bool MovePrevious()
		{
			if (this.current == 0)
			{
				return false;
			}
			this.current--;
			return true;
		}

		public override void Reset()
		{
			this.current = 0;
		}

		private string replacement;

		private int current;

		private bool fallback_assigned;
	}
}
