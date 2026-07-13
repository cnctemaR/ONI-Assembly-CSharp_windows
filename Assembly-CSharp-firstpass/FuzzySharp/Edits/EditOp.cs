using System;

namespace FuzzySharp.Edits
{
	public class EditOp
	{
		public EditType EditType { get; set; }

		public int SourcePos { get; set; }

		public int DestPos { get; set; }

		public override string ToString()
		{
			return string.Format("{0}({1}, {2})", this.EditType, this.SourcePos, this.DestPos);
		}
	}
}
