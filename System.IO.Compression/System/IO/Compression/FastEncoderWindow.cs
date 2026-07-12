using System;
using System.Diagnostics;

namespace System.IO.Compression
{
	internal sealed class FastEncoderWindow
	{
		public FastEncoderWindow()
		{
			this.ResetWindow();
		}

		public int BytesAvailable
		{
			get
			{
				return this._bufEnd - this._bufPos;
			}
		}

		public DeflateInput UnprocessedInput
		{
			get
			{
				return new DeflateInput
				{
					Buffer = this._window,
					StartIndex = this._bufPos,
					Count = this._bufEnd - this._bufPos
				};
			}
		}

		public void FlushWindow()
		{
			this.ResetWindow();
		}

		private void ResetWindow()
		{
			this._window = new byte[16646];
			this._prev = new ushort[8450];
			this._lookup = new ushort[2048];
			this._bufPos = 8192;
			this._bufEnd = this._bufPos;
		}

		public int FreeWindowSpace
		{
			get
			{
				return 16384 - this._bufEnd;
			}
		}

		public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
		{
			Array.Copy(inputBuffer, startIndex, this._window, this._bufEnd, count);
			this._bufEnd += count;
		}

		public void MoveWindows()
		{
			Array.Copy(this._window, this._bufPos - 8192, this._window, 0, 8192);
			for (int i = 0; i < 2048; i++)
			{
				int num = (int)(this._lookup[i] - 8192);
				if (num <= 0)
				{
					this._lookup[i] = 0;
				}
				else
				{
					this._lookup[i] = (ushort)num;
				}
			}
			for (int i = 0; i < 8192; i++)
			{
				long num2 = (long)((ulong)this._prev[i] - 8192UL);
				if (num2 <= 0L)
				{
					this._prev[i] = 0;
				}
				else
				{
					this._prev[i] = (ushort)num2;
				}
			}
			this._bufPos = 8192;
			this._bufEnd = this._bufPos;
		}

		private uint HashValue(uint hash, byte b)
		{
			return (hash << 4) ^ (uint)b;
		}

		private uint InsertString(ref uint hash)
		{
			hash = this.HashValue(hash, this._window[this._bufPos + 2]);
			uint num = (uint)this._lookup[(int)(hash & 2047U)];
			this._lookup[(int)(hash & 2047U)] = (ushort)this._bufPos;
			this._prev[this._bufPos & 8191] = (ushort)num;
			return num;
		}

		private void InsertStrings(ref uint hash, int matchLen)
		{
			if (this._bufEnd - this._bufPos <= matchLen)
			{
				this._bufPos += matchLen - 1;
				return;
			}
			while (--matchLen > 0)
			{
				this.InsertString(ref hash);
				this._bufPos++;
			}
		}

		internal bool GetNextSymbolOrMatch(Match match)
		{
			uint num = this.HashValue(0U, this._window[this._bufPos]);
			num = this.HashValue(num, this._window[this._bufPos + 1]);
			int num2 = 0;
			int num3;
			if (this._bufEnd - this._bufPos <= 3)
			{
				num3 = 0;
			}
			else
			{
				int num4 = (int)this.InsertString(ref num);
				if (num4 != 0)
				{
					num3 = this.FindMatch(num4, out num2, 32, 32);
					if (this._bufPos + num3 > this._bufEnd)
					{
						num3 = this._bufEnd - this._bufPos;
					}
				}
				else
				{
					num3 = 0;
				}
			}
			if (num3 < 3)
			{
				match.State = MatchState.HasSymbol;
				match.Symbol = this._window[this._bufPos];
				this._bufPos++;
			}
			else
			{
				this._bufPos++;
				if (num3 <= 6)
				{
					int num5 = 0;
					int num6 = (int)this.InsertString(ref num);
					int num7;
					if (num6 != 0)
					{
						num7 = this.FindMatch(num6, out num5, (num3 < 4) ? 32 : 8, 32);
						if (this._bufPos + num7 > this._bufEnd)
						{
							num7 = this._bufEnd - this._bufPos;
						}
					}
					else
					{
						num7 = 0;
					}
					if (num7 > num3)
					{
						match.State = MatchState.HasSymbolAndMatch;
						match.Symbol = this._window[this._bufPos - 1];
						match.Position = num5;
						match.Length = num7;
						this._bufPos++;
						num3 = num7;
						this.InsertStrings(ref num, num3);
					}
					else
					{
						match.State = MatchState.HasMatch;
						match.Position = num2;
						match.Length = num3;
						num3--;
						this._bufPos++;
						this.InsertStrings(ref num, num3);
					}
				}
				else
				{
					match.State = MatchState.HasMatch;
					match.Position = num2;
					match.Length = num3;
					this.InsertStrings(ref num, num3);
				}
			}
			if (this._bufPos == 16384)
			{
				this.MoveWindows();
			}
			return true;
		}

		private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
		{
			int num = 0;
			int num2 = 0;
			int num3 = this._bufPos - 8192;
			byte b = this._window[this._bufPos];
			while (search > num3)
			{
				if (this._window[search + num] == b)
				{
					int num4 = 0;
					while (num4 < 258 && this._window[this._bufPos + num4] == this._window[search + num4])
					{
						num4++;
					}
					if (num4 > num)
					{
						num = num4;
						num2 = search;
						if (num4 > 32)
						{
							break;
						}
						b = this._window[this._bufPos + num4];
					}
				}
				if (--searchDepth == 0)
				{
					break;
				}
				search = (int)this._prev[search & 8191];
			}
			matchPos = this._bufPos - num2 - 1;
			if (num == 3 && matchPos >= 16384)
			{
				return 0;
			}
			return num;
		}

		[Conditional("DEBUG")]
		private void DebugAssertVerifyHashes()
		{
		}

		[Conditional("DEBUG")]
		private void DebugAssertRecalculatedHashesAreEqual(int position1, int position2, string message = "")
		{
		}

		private byte[] _window;

		private int _bufPos;

		private int _bufEnd;

		private const int FastEncoderHashShift = 4;

		private const int FastEncoderHashtableSize = 2048;

		private const int FastEncoderHashMask = 2047;

		private const int FastEncoderWindowSize = 8192;

		private const int FastEncoderWindowMask = 8191;

		private const int FastEncoderMatch3DistThreshold = 16384;

		internal const int MaxMatch = 258;

		internal const int MinMatch = 3;

		private const int SearchDepth = 32;

		private const int GoodLength = 4;

		private const int NiceLength = 32;

		private const int LazyMatchThreshold = 6;

		private ushort[] _prev;

		private ushort[] _lookup;
	}
}
