using System;

namespace System.Text
{
	public sealed class DecoderReplacementFallbackBuffer : DecoderFallbackBuffer
	{
		public DecoderReplacementFallbackBuffer(DecoderReplacementFallback fallback)
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
				return (!this.fallback_assigned) ? 0 : (this.replacement.Length - this.current);
			}
		}

		public override bool Fallback(byte[] bytesUnknown, int index)
		{
			if (bytesUnknown == null)
			{
				throw new ArgumentNullException("bytesUnknown");
			}
			if (this.fallback_assigned && this.Remaining != 0)
			{
				throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
			}
			if (index < 0 || bytesUnknown.Length < index)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.fallback_assigned = true;
			this.current = 0;
			return this.replacement.Length > 0;
		}

		public override char GetNextChar()
		{
			if (!this.fallback_assigned)
			{
				return '\0';
			}
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
			this.fallback_assigned = false;
			this.current = 0;
		}

		private bool fallback_assigned;

		private int current;

		private string replacement;
	}
}
