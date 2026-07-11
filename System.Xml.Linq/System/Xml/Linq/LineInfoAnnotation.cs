using System;

namespace System.Xml.Linq
{
	internal class LineInfoAnnotation
	{
		public LineInfoAnnotation(int lineNumber, int linePosition)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		internal int lineNumber;

		internal int linePosition;
	}
}
