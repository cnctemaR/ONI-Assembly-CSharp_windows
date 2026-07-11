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

		public EncoderReplacementFallback(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			bool flag = false;
			for (int i = 0; i < replacement.Length; i++)
			{
				if (char.IsSurrogate(replacement, i))
				{
					if (char.IsHighSurrogate(replacement, i))
					{
						if (flag)
						{
							break;
						}
						flag = true;
					}
					else
					{
						if (!flag)
						{
							flag = true;
							break;
						}
						flag = false;
					}
				}
				else if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				throw new ArgumentException(Environment.GetResourceString("String contains invalid Unicode code points.", new object[] { "replacement" }));
			}
			this.strDefault = replacement;
		}

		public string DefaultString
		{
			get
			{
				return this.strDefault;
			}
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new EncoderReplacementFallbackBuffer(this);
		}

		public override int MaxCharCount
		{
			get
			{
				return this.strDefault.Length;
			}
		}

		public override bool Equals(object value)
		{
			EncoderReplacementFallback encoderReplacementFallback = value as EncoderReplacementFallback;
			return encoderReplacementFallback != null && this.strDefault == encoderReplacementFallback.strDefault;
		}

		public override int GetHashCode()
		{
			return this.strDefault.GetHashCode();
		}

		private string strDefault;
	}
}
