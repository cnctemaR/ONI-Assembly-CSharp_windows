using System;

namespace System.Text
{
	[Serializable]
	public sealed class EncoderReplacementFallback : EncoderFallback
	{
		public EncoderReplacementFallback()
			: this("?")
		{
		}

		[MonoTODO]
		public EncoderReplacementFallback(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException();
			}
			this.replacement = replacement;
		}

		public string DefaultString
		{
			get
			{
				return this.replacement;
			}
		}

		public override int MaxCharCount
		{
			get
			{
				return this.replacement.Length;
			}
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new EncoderReplacementFallbackBuffer(this);
		}

		public override bool Equals(object value)
		{
			EncoderReplacementFallback encoderReplacementFallback = value as EncoderReplacementFallback;
			return encoderReplacementFallback != null && this.replacement == encoderReplacementFallback.replacement;
		}

		public override int GetHashCode()
		{
			return this.replacement.GetHashCode();
		}

		private string replacement;
	}
}
