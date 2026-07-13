using System;

namespace FuzzySharp.Edits
{
	public class OpCode
	{
		public EditType EditType { get; set; }

		public int SourceBegin { get; set; }

		public int SourceEnd { get; set; }

		public int DestBegin { get; set; }

		public int DestEnd { get; set; }

		public override string ToString()
		{
			return string.Format("{0}({1},{2},{3},{4})", new object[] { this.EditType, this.SourceBegin, this.SourceEnd, this.DestBegin, this.DestEnd });
		}
	}
}
