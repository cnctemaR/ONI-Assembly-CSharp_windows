using System;

namespace System.Xml
{
	internal struct LineInfo
	{
		public LineInfo(int lineNo, int linePos)
		{
			this.lineNo = lineNo;
			this.linePos = linePos;
		}

		public void Set(int lineNo, int linePos)
		{
			this.lineNo = lineNo;
			this.linePos = linePos;
		}

		internal int lineNo;

		internal int linePos;
	}
}
