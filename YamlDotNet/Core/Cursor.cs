using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class Cursor
	{
		public int Index { get; set; }

		public int Line { get; set; }

		public int LineOffset { get; set; }

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

		public Mark Mark()
		{
			return new Mark(this.Index, this.Line, this.LineOffset + 1);
		}

		public void Skip()
		{
			int num = this.Index;
			this.Index = num + 1;
			num = this.LineOffset;
			this.LineOffset = num + 1;
		}

		public void SkipLineByOffset(int offset)
		{
			this.Index += offset;
			int line = this.Line;
			this.Line = line + 1;
			this.LineOffset = 0;
		}

		public void ForceSkipLineAfterNonBreak()
		{
			if (this.LineOffset != 0)
			{
				int line = this.Line;
				this.Line = line + 1;
				this.LineOffset = 0;
			}
		}
	}
}
