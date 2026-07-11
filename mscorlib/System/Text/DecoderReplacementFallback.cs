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

		public DecoderReplacementFallback(string replacement)
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

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new DecoderReplacementFallbackBuffer(this);
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
			DecoderReplacementFallback decoderReplacementFallback = value as DecoderReplacementFallback;
			return decoderReplacementFallback != null && this.strDefault == decoderReplacementFallback.strDefault;
		}

		public override int GetHashCode()
		{
			return this.strDefault.GetHashCode();
		}

		private string strDefault;
	}
}
