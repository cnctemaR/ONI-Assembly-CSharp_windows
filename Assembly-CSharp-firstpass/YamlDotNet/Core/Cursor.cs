using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Cursor
	{
		public Cursor()
		{
			this.Line = 1;
		}

		public Cursor(Cursor cursor)
		{
			this.Index = cursor.Index;
			this.Line = cursor.Line;
			this.LineOffset = cursor.LineOffset;
		}

		public int Index { get; set; }

		public int Line { get; set; }

		public int LineOffset { get; set; }

		public Mark Mark()
		{
			return new Mark(this.Index, this.Line, this.LineOffset + 1);
		}

		public void Skip()
		{
			this.Index++;
			this.LineOffset++;
		}

		public void SkipLineByOffset(int offset)
		{
			this.Index += offset;
			this.Line++;
			this.LineOffset = 0;
		}

		public void ForceSkipLineAfterNonBreak()
		{
			if (this.LineOffset != 0)
			{
				this.Line++;
				this.LineOffset = 0;
			}
		}
	}
}
