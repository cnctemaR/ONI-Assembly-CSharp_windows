using System;

namespace System.Text
{
	[Serializable]
	public sealed class DecoderReplacementFallback : DecoderFallback
	{
		public DecoderReplacementFallback()
			: this("?")
		{
		}

		[MonoTODO]
		public DecoderReplacementFallback(string replacement)
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

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new DecoderReplacementFallbackBuffer(this);
		}

		public override bool Equals(object value)
		{
			DecoderReplacementFallback decoderReplacementFallback = value as DecoderReplacementFallback;
			return decoderReplacementFallback != null && this.replacement == decoderReplacementFallback.replacement;
		}

		public override int GetHashCode()
		{
			return this.replacement.GetHashCode();
		}

		private string replacement;
	}
}
