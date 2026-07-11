using System;
using System.Globalization;
using System.Text;

namespace System.Xml
{
	internal class CharEntityEncoderFallbackBuffer : EncoderFallbackBuffer
	{
		internal CharEntityEncoderFallbackBuffer(CharEntityEncoderFallback parent)
		{
			this.parent = parent;
		}

		public override bool Fallback(char charUnknown, int index)
		{
			if (this.charEntityIndex >= 0)
			{
				new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknown, index);
			}
			if (this.parent.CanReplaceAt(index))
			{
				this.charEntity = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", new object[] { (int)charUnknown });
				this.charEntityIndex = 0;
				return true;
			}
			new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknown, index);
			return false;
		}

		public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
		{
			if (!char.IsSurrogatePair(charUnknownHigh, charUnknownLow))
			{
				throw XmlConvert.CreateInvalidSurrogatePairException(charUnknownHigh, charUnknownLow);
			}
			if (this.charEntityIndex >= 0)
			{
				new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknownHigh, charUnknownLow, index);
			}
			if (this.parent.CanReplaceAt(index))
			{
				this.charEntity = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", new object[] { this.SurrogateCharToUtf32(charUnknownHigh, charUnknownLow) });
				this.charEntityIndex = 0;
				return true;
			}
			new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknownHigh, charUnknownLow, index);
			return false;
		}

		public override char GetNextChar()
		{
			if (this.charEntityIndex == this.charEntity.Length)
			{
				this.charEntityIndex = -1;
			}
			if (this.charEntityIndex == -1)
			{
				return '\0';
			}
			string text = this.charEntity;
			int num = this.charEntityIndex;
			this.charEntityIndex = num + 1;
			return text[num];
		}

		public override bool MovePrevious()
		{
			if (this.charEntityIndex == -1)
			{
				return false;
			}
			if (this.charEntityIndex > 0)
			{
				this.charEntityIndex--;
				return true;
			}
			return false;
		}

		public override int Remaining
		{
			get
			{
				if (this.charEntityIndex == -1)
				{
					return 0;
				}
				return this.charEntity.Length - this.charEntityIndex;
			}
		}

		public override void Reset()
		{
			this.charEntityIndex = -1;
		}

		private int SurrogateCharToUtf32(char highSurrogate, char lowSurrogate)
		{
			return XmlCharType.CombineSurrogateChar((int)lowSurrogate, (int)highSurrogate);
		}

		private CharEntityEncoderFallback parent;

		private string charEntity = string.Empty;

		private int charEntityIndex = -1;
	}
}
