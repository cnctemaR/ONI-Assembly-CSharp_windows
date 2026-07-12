using System;

namespace System.IO.Compression
{
	internal sealed class InflaterManaged
	{
		internal InflaterManaged(IFileFormatReader reader, bool deflate64)
		{
			this._output = new OutputWindow();
			this._input = new InputBuffer();
			this._codeList = new byte[320];
			this._codeLengthTreeCodeLength = new byte[19];
			this._deflate64 = deflate64;
			if (reader != null)
			{
				this._formatReader = reader;
				this._hasFormatReader = true;
			}
			this.Reset();
		}

		private void Reset()
		{
			this._state = (this._hasFormatReader ? InflaterState.ReadingHeader : InflaterState.ReadingBFinal);
		}

		public void SetInput(byte[] inputBytes, int offset, int length)
		{
			this._input.SetInput(inputBytes, offset, length);
		}

		public bool Finished()
		{
			return this._state == InflaterState.Done || this._state == InflaterState.VerifyingFooter;
		}

		public int AvailableOutput
		{
			get
			{
				return this._output.AvailableBytes;
			}
		}

		public int Inflate(byte[] bytes, int offset, int length)
		{
			int num = 0;
			do
			{
				int num2 = this._output.CopyTo(bytes, offset, length);
				if (num2 > 0)
				{
					if (this._hasFormatReader)
					{
						this._formatReader.UpdateWithBytesRead(bytes, offset, num2);
					}
					offset += num2;
					num += num2;
					length -= num2;
				}
			}
			while (length != 0 && !this.Finished() && this.Decode());
			if (this._state == InflaterState.VerifyingFooter && this._output.AvailableBytes == 0)
			{
				this._formatReader.Validate();
			}
			return num;
		}

		private bool Decode()
		{
			bool flag = false;
			if (this.Finished())
			{
				return true;
			}
			if (this._hasFormatReader)
			{
				if (this._state == InflaterState.ReadingHeader)
				{
					if (!this._formatReader.ReadHeader(this._input))
					{
						return false;
					}
					this._state = InflaterState.ReadingBFinal;
				}
				else if (this._state == InflaterState.StartReadingFooter || this._state == InflaterState.ReadingFooter)
				{
					if (!this._formatReader.ReadFooter(this._input))
					{
						return false;
					}
					this._state = InflaterState.VerifyingFooter;
					return true;
				}
			}
			if (this._state == InflaterState.ReadingBFinal)
			{
				if (!this._input.EnsureBitsAvailable(1))
				{
					return false;
				}
				this._bfinal = this._input.GetBits(1);
				this._state = InflaterState.ReadingBType;
			}
			if (this._state == InflaterState.ReadingBType)
			{
				if (!this._input.EnsureBitsAvailable(2))
				{
					this._state = InflaterState.ReadingBType;
					return false;
				}
				this._blockType = (BlockType)this._input.GetBits(2);
				if (this._blockType == BlockType.Dynamic)
				{
					this._state = InflaterState.ReadingNumLitCodes;
				}
				else if (this._blockType == BlockType.Static)
				{
					this._literalLengthTree = HuffmanTree.StaticLiteralLengthTree;
					this._distanceTree = HuffmanTree.StaticDistanceTree;
					this._state = InflaterState.DecodeTop;
				}
				else
				{
					if (this._blockType != BlockType.Uncompressed)
					{
						throw new InvalidDataException("Unknown block type. Stream might be corrupted.");
					}
					this._state = InflaterState.UncompressedAligning;
				}
			}
			bool flag2;
			if (this._blockType == BlockType.Dynamic)
			{
				if (this._state < InflaterState.DecodeTop)
				{
					flag2 = this.DecodeDynamicBlockHeader();
				}
				else
				{
					flag2 = this.DecodeBlock(out flag);
				}
			}
			else if (this._blockType == BlockType.Static)
			{
				flag2 = this.DecodeBlock(out flag);
			}
			else
			{
				if (this._blockType != BlockType.Uncompressed)
				{
					throw new InvalidDataException("Unknown block type. Stream might be corrupted.");
				}
				flag2 = this.DecodeUncompressedBlock(out flag);
			}
			if (flag && this._bfinal != 0)
			{
				if (this._hasFormatReader)
				{
					this._state = InflaterState.StartReadingFooter;
				}
				else
				{
					this._state = InflaterState.Done;
				}
			}
			return flag2;
		}

		private bool DecodeUncompressedBlock(out bool end_of_block)
		{
			end_of_block = false;
			for (;;)
			{
				switch (this._state)
				{
				case InflaterState.UncompressedAligning:
					this._input.SkipToByteBoundary();
					this._state = InflaterState.UncompressedByte1;
					goto IL_0043;
				case InflaterState.UncompressedByte1:
				case InflaterState.UncompressedByte2:
				case InflaterState.UncompressedByte3:
				case InflaterState.UncompressedByte4:
					goto IL_0043;
				case InflaterState.DecodingUncompressed:
					goto IL_00D1;
				}
				break;
				IL_0043:
				int bits = this._input.GetBits(8);
				if (bits < 0)
				{
					return false;
				}
				this._blockLengthBuffer[this._state - InflaterState.UncompressedByte1] = (byte)bits;
				if (this._state == InflaterState.UncompressedByte4)
				{
					this._blockLength = (int)this._blockLengthBuffer[0] + (int)this._blockLengthBuffer[1] * 256;
					int num = (int)this._blockLengthBuffer[2] + (int)this._blockLengthBuffer[3] * 256;
					if ((ushort)this._blockLength != (ushort)(~(ushort)num))
					{
						goto Block_4;
					}
				}
				this._state++;
			}
			throw new InvalidDataException("Decoder is in some unknown state. This might be caused by corrupted data.");
			Block_4:
			throw new InvalidDataException("Block length does not match with its complement.");
			IL_00D1:
			int num2 = this._output.CopyFrom(this._input, this._blockLength);
			this._blockLength -= num2;
			if (this._blockLength == 0)
			{
				this._state = InflaterState.ReadingBFinal;
				end_of_block = true;
				return true;
			}
			return this._output.FreeBytes == 0;
		}

		private bool DecodeBlock(out bool end_of_block_code_seen)
		{
			end_of_block_code_seen = false;
			int i = this._output.FreeBytes;
			while (i > 65536)
			{
				switch (this._state)
				{
				case InflaterState.DecodeTop:
				{
					int num = this._literalLengthTree.GetNextSymbol(this._input);
					if (num < 0)
					{
						return false;
					}
					if (num < 256)
					{
						this._output.Write((byte)num);
						i--;
						continue;
					}
					if (num == 256)
					{
						end_of_block_code_seen = true;
						this._state = InflaterState.ReadingBFinal;
						return true;
					}
					num -= 257;
					if (num < 8)
					{
						num += 3;
						this._extraBits = 0;
					}
					else if (!this._deflate64 && num == 28)
					{
						num = 258;
						this._extraBits = 0;
					}
					else
					{
						if (num < 0 || num >= InflaterManaged.s_extraLengthBits.Length)
						{
							throw new InvalidDataException("Found invalid data while decoding.");
						}
						this._extraBits = (int)InflaterManaged.s_extraLengthBits[num];
					}
					this._length = num;
					goto IL_00E5;
				}
				case InflaterState.HaveInitialLength:
					goto IL_00E5;
				case InflaterState.HaveFullLength:
					goto IL_0150;
				case InflaterState.HaveDistCode:
					break;
				default:
					throw new InvalidDataException("Decoder is in some unknown state. This might be caused by corrupted data.");
				}
				IL_01B2:
				int num2;
				if (this._distanceCode > 3)
				{
					this._extraBits = this._distanceCode - 2 >> 1;
					int bits = this._input.GetBits(this._extraBits);
					if (bits < 0)
					{
						return false;
					}
					num2 = InflaterManaged.s_distanceBasePosition[this._distanceCode] + bits;
				}
				else
				{
					num2 = this._distanceCode + 1;
				}
				this._output.WriteLengthDistance(this._length, num2);
				i -= this._length;
				this._state = InflaterState.DecodeTop;
				continue;
				IL_0150:
				if (this._blockType == BlockType.Dynamic)
				{
					this._distanceCode = this._distanceTree.GetNextSymbol(this._input);
				}
				else
				{
					this._distanceCode = this._input.GetBits(5);
					if (this._distanceCode >= 0)
					{
						this._distanceCode = (int)InflaterManaged.s_staticDistanceTreeTable[this._distanceCode];
					}
				}
				if (this._distanceCode < 0)
				{
					return false;
				}
				this._state = InflaterState.HaveDistCode;
				goto IL_01B2;
				IL_00E5:
				if (this._extraBits > 0)
				{
					this._state = InflaterState.HaveInitialLength;
					int bits2 = this._input.GetBits(this._extraBits);
					if (bits2 < 0)
					{
						return false;
					}
					if (this._length < 0 || this._length >= InflaterManaged.s_lengthBase.Length)
					{
						throw new InvalidDataException("Found invalid data while decoding.");
					}
					this._length = InflaterManaged.s_lengthBase[this._length] + bits2;
				}
				this._state = InflaterState.HaveFullLength;
				goto IL_0150;
			}
			return true;
		}

		private bool DecodeDynamicBlockHeader()
		{
			switch (this._state)
			{
			case InflaterState.ReadingNumLitCodes:
				this._literalLengthCodeCount = this._input.GetBits(5);
				if (this._literalLengthCodeCount < 0)
				{
					return false;
				}
				this._literalLengthCodeCount += 257;
				this._state = InflaterState.ReadingNumDistCodes;
				goto IL_0062;
			case InflaterState.ReadingNumDistCodes:
				goto IL_0062;
			case InflaterState.ReadingNumCodeLengthCodes:
				goto IL_0094;
			case InflaterState.ReadingCodeLengthCodes:
				break;
			case InflaterState.ReadingTreeCodesBefore:
			case InflaterState.ReadingTreeCodesAfter:
				goto IL_035A;
			default:
				throw new InvalidDataException("Decoder is in some unknown state. This might be caused by corrupted data.");
			}
			IL_0105:
			while (this._loopCounter < this._codeLengthCodeCount)
			{
				int bits = this._input.GetBits(3);
				if (bits < 0)
				{
					return false;
				}
				this._codeLengthTreeCodeLength[(int)InflaterManaged.s_codeOrder[this._loopCounter]] = (byte)bits;
				this._loopCounter++;
			}
			for (int i = this._codeLengthCodeCount; i < InflaterManaged.s_codeOrder.Length; i++)
			{
				this._codeLengthTreeCodeLength[(int)InflaterManaged.s_codeOrder[i]] = 0;
			}
			this._codeLengthTree = new HuffmanTree(this._codeLengthTreeCodeLength);
			this._codeArraySize = this._literalLengthCodeCount + this._distanceCodeCount;
			this._loopCounter = 0;
			this._state = InflaterState.ReadingTreeCodesBefore;
			IL_035A:
			while (this._loopCounter < this._codeArraySize)
			{
				if (this._state == InflaterState.ReadingTreeCodesBefore && (this._lengthCode = this._codeLengthTree.GetNextSymbol(this._input)) < 0)
				{
					return false;
				}
				if (this._lengthCode <= 15)
				{
					byte[] codeList = this._codeList;
					int num = this._loopCounter;
					this._loopCounter = num + 1;
					codeList[num] = (byte)this._lengthCode;
				}
				else if (this._lengthCode == 16)
				{
					if (!this._input.EnsureBitsAvailable(2))
					{
						this._state = InflaterState.ReadingTreeCodesAfter;
						return false;
					}
					if (this._loopCounter == 0)
					{
						throw new InvalidDataException();
					}
					byte b = this._codeList[this._loopCounter - 1];
					int num2 = this._input.GetBits(2) + 3;
					if (this._loopCounter + num2 > this._codeArraySize)
					{
						throw new InvalidDataException();
					}
					for (int j = 0; j < num2; j++)
					{
						byte[] codeList2 = this._codeList;
						int num = this._loopCounter;
						this._loopCounter = num + 1;
						codeList2[num] = b;
					}
				}
				else if (this._lengthCode == 17)
				{
					if (!this._input.EnsureBitsAvailable(3))
					{
						this._state = InflaterState.ReadingTreeCodesAfter;
						return false;
					}
					int num2 = this._input.GetBits(3) + 3;
					if (this._loopCounter + num2 > this._codeArraySize)
					{
						throw new InvalidDataException();
					}
					for (int k = 0; k < num2; k++)
					{
						byte[] codeList3 = this._codeList;
						int num = this._loopCounter;
						this._loopCounter = num + 1;
						codeList3[num] = 0;
					}
				}
				else
				{
					if (!this._input.EnsureBitsAvailable(7))
					{
						this._state = InflaterState.ReadingTreeCodesAfter;
						return false;
					}
					int num2 = this._input.GetBits(7) + 11;
					if (this._loopCounter + num2 > this._codeArraySize)
					{
						throw new InvalidDataException();
					}
					for (int l = 0; l < num2; l++)
					{
						byte[] codeList4 = this._codeList;
						int num = this._loopCounter;
						this._loopCounter = num + 1;
						codeList4[num] = 0;
					}
				}
				this._state = InflaterState.ReadingTreeCodesBefore;
			}
			byte[] array = new byte[288];
			byte[] array2 = new byte[32];
			Array.Copy(this._codeList, 0, array, 0, this._literalLengthCodeCount);
			Array.Copy(this._codeList, this._literalLengthCodeCount, array2, 0, this._distanceCodeCount);
			if (array[256] == 0)
			{
				throw new InvalidDataException();
			}
			this._literalLengthTree = new HuffmanTree(array);
			this._distanceTree = new HuffmanTree(array2);
			this._state = InflaterState.DecodeTop;
			return true;
			IL_0062:
			this._distanceCodeCount = this._input.GetBits(5);
			if (this._distanceCodeCount < 0)
			{
				return false;
			}
			this._distanceCodeCount++;
			this._state = InflaterState.ReadingNumCodeLengthCodes;
			IL_0094:
			this._codeLengthCodeCount = this._input.GetBits(4);
			if (this._codeLengthCodeCount < 0)
			{
				return false;
			}
			this._codeLengthCodeCount += 4;
			this._loopCounter = 0;
			this._state = InflaterState.ReadingCodeLengthCodes;
			goto IL_0105;
		}

		public void Dispose()
		{
		}

		private static readonly byte[] s_extraLengthBits = new byte[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 2, 2, 2, 2, 3, 3, 3, 3,
			4, 4, 4, 4, 5, 5, 5, 5, 16
		};

		private static readonly int[] s_lengthBase = new int[]
		{
			3, 4, 5, 6, 7, 8, 9, 10, 11, 13,
			15, 17, 19, 23, 27, 31, 35, 43, 51, 59,
			67, 83, 99, 115, 131, 163, 195, 227, 3
		};

		private static readonly int[] s_distanceBasePosition = new int[]
		{
			1, 2, 3, 4, 5, 7, 9, 13, 17, 25,
			33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
			1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577,
			32769, 49153
		};

		private static readonly byte[] s_codeOrder = new byte[]
		{
			16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
			11, 4, 12, 3, 13, 2, 14, 1, 15
		};

		private static readonly byte[] s_staticDistanceTreeTable = new byte[]
		{
			0, 16, 8, 24, 4, 20, 12, 28, 2, 18,
			10, 26, 6, 22, 14, 30, 1, 17, 9, 25,
			5, 21, 13, 29, 3, 19, 11, 27, 7, 23,
			15, 31
		};

		private readonly OutputWindow _output;

		private readonly InputBuffer _input;

		private HuffmanTree _literalLengthTree;

		private HuffmanTree _distanceTree;

		private InflaterState _state;

		private bool _hasFormatReader;

		private int _bfinal;

		private BlockType _blockType;

		private readonly byte[] _blockLengthBuffer = new byte[4];

		private int _blockLength;

		private int _length;

		private int _distanceCode;

		private int _extraBits;

		private int _loopCounter;

		private int _literalLengthCodeCount;

		private int _distanceCodeCount;

		private int _codeLengthCodeCount;

		private int _codeArraySize;

		private int _lengthCode;

		private readonly byte[] _codeList;

		private readonly byte[] _codeLengthTreeCodeLength;

		private readonly bool _deflate64;

		private HuffmanTree _codeLengthTree;

		private IFileFormatReader _formatReader;
	}
}
