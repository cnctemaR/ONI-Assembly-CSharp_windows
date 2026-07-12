using System;

namespace System.IO.Compression
{
	internal sealed class FastEncoder
	{
		public FastEncoder()
		{
			this._inputWindow = new FastEncoderWindow();
			this._currentMatch = new Match();
		}

		internal int BytesInHistory
		{
			get
			{
				return this._inputWindow.BytesAvailable;
			}
		}

		internal DeflateInput UnprocessedInput
		{
			get
			{
				return this._inputWindow.UnprocessedInput;
			}
		}

		internal void FlushInput()
		{
			this._inputWindow.FlushWindow();
		}

		internal double LastCompressionRatio
		{
			get
			{
				return this._lastCompressionRatio;
			}
		}

		internal void GetBlock(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
			FastEncoder.WriteDeflatePreamble(output);
			this.GetCompressedOutput(input, output, maxBytesToCopy);
			this.WriteEndOfBlock(output);
		}

		internal void GetCompressedData(DeflateInput input, OutputBuffer output)
		{
			this.GetCompressedOutput(input, output, -1);
		}

		internal void GetBlockHeader(OutputBuffer output)
		{
			FastEncoder.WriteDeflatePreamble(output);
		}

		internal void GetBlockFooter(OutputBuffer output)
		{
			this.WriteEndOfBlock(output);
		}

		private void GetCompressedOutput(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
			int bytesWritten = output.BytesWritten;
			int num = 0;
			int num2 = this.BytesInHistory + input.Count;
			do
			{
				int num3 = ((input.Count < this._inputWindow.FreeWindowSpace) ? input.Count : this._inputWindow.FreeWindowSpace);
				if (maxBytesToCopy >= 1)
				{
					num3 = Math.Min(num3, maxBytesToCopy - num);
				}
				if (num3 > 0)
				{
					this._inputWindow.CopyBytes(input.Buffer, input.StartIndex, num3);
					input.ConsumeBytes(num3);
					num += num3;
				}
				this.GetCompressedOutput(output);
			}
			while (this.SafeToWriteTo(output) && this.InputAvailable(input) && (maxBytesToCopy < 1 || num < maxBytesToCopy));
			int num4 = output.BytesWritten - bytesWritten;
			int num5 = this.BytesInHistory + input.Count;
			int num6 = num2 - num5;
			if (num4 != 0)
			{
				this._lastCompressionRatio = (double)num4 / (double)num6;
			}
		}

		private void GetCompressedOutput(OutputBuffer output)
		{
			while (this._inputWindow.BytesAvailable > 0 && this.SafeToWriteTo(output))
			{
				this._inputWindow.GetNextSymbolOrMatch(this._currentMatch);
				if (this._currentMatch.State == MatchState.HasSymbol)
				{
					FastEncoder.WriteChar(this._currentMatch.Symbol, output);
				}
				else if (this._currentMatch.State == MatchState.HasMatch)
				{
					FastEncoder.WriteMatch(this._currentMatch.Length, this._currentMatch.Position, output);
				}
				else
				{
					FastEncoder.WriteChar(this._currentMatch.Symbol, output);
					FastEncoder.WriteMatch(this._currentMatch.Length, this._currentMatch.Position, output);
				}
			}
		}

		private bool InputAvailable(DeflateInput input)
		{
			return input.Count > 0 || this.BytesInHistory > 0;
		}

		private bool SafeToWriteTo(OutputBuffer output)
		{
			return output.FreeBytes > 16;
		}

		private void WriteEndOfBlock(OutputBuffer output)
		{
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[256];
			int num2 = (int)(num & 31U);
			output.WriteBits(num2, num >> 5);
		}

		internal static void WriteMatch(int matchLen, int matchPos, OutputBuffer output)
		{
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[254 + matchLen];
			int num2 = (int)(num & 31U);
			if (num2 <= 16)
			{
				output.WriteBits(num2, num >> 5);
			}
			else
			{
				output.WriteBits(16, (num >> 5) & 65535U);
				output.WriteBits(num2 - 16, num >> 21);
			}
			num = FastEncoderStatics.FastEncoderDistanceCodeInfo[FastEncoderStatics.GetSlot(matchPos)];
			output.WriteBits((int)(num & 15U), num >> 8);
			int num3 = (int)((num >> 4) & 15U);
			if (num3 != 0)
			{
				output.WriteBits(num3, (uint)(matchPos & (int)FastEncoderStatics.BitMask[num3]));
			}
		}

		internal static void WriteChar(byte b, OutputBuffer output)
		{
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[(int)b];
			output.WriteBits((int)(num & 31U), num >> 5);
		}

		internal static void WriteDeflatePreamble(OutputBuffer output)
		{
			output.WriteBytes(FastEncoderStatics.FastEncoderTreeStructureData, 0, FastEncoderStatics.FastEncoderTreeStructureData.Length);
			output.WriteBits(9, 34U);
		}

		private readonly FastEncoderWindow _inputWindow;

		private readonly Match _currentMatch;

		private double _lastCompressionRatio;
	}
}
