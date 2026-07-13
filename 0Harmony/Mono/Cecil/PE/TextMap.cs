using System;

namespace Mono.Cecil.PE
{
	internal sealed class TextMap
	{
		public void AddMap(TextSegment segment, int length)
		{
			this.map[(int)segment] = new Range(this.GetStart(segment), (uint)length);
		}

		private uint AlignUp(uint value, uint align)
		{
			align -= 1U;
			return (value + align) & ~align;
		}

		public void AddMap(TextSegment segment, int length, int align)
		{
			uint num2;
			if (segment != TextSegment.ImportAddressTable)
			{
				int num = segment - TextSegment.CLIHeader;
				Range range = this.map[num];
				num2 = this.AlignUp(range.Start + range.Length, (uint)align);
				this.map[num].Length = num2 - range.Start;
			}
			else
			{
				num2 = 8192U;
			}
			this.map[(int)segment] = new Range(num2, (uint)length);
		}

		public void AddMap(TextSegment segment, Range range)
		{
			this.map[(int)segment] = range;
		}

		public Range GetRange(TextSegment segment)
		{
			return this.map[(int)segment];
		}

		public DataDirectory GetDataDirectory(TextSegment segment)
		{
			Range range = this.map[(int)segment];
			return new DataDirectory((range.Length == 0U) ? 0U : range.Start, range.Length);
		}

		public uint GetRVA(TextSegment segment)
		{
			return this.map[(int)segment].Start;
		}

		public uint GetNextRVA(TextSegment segment)
		{
			return this.map[(int)segment].Start + this.map[(int)segment].Length;
		}

		public int GetLength(TextSegment segment)
		{
			return (int)this.map[(int)segment].Length;
		}

		private uint GetStart(TextSegment segment)
		{
			if (segment != TextSegment.ImportAddressTable)
			{
				return this.ComputeStart((int)segment);
			}
			return 8192U;
		}

		private uint ComputeStart(int index)
		{
			index--;
			return this.map[index].Start + this.map[index].Length;
		}

		public uint GetLength()
		{
			Range range = this.map[16];
			return range.Start - 8192U + range.Length;
		}

		private readonly Range[] map = new Range[17];
	}
}
