using System;

namespace System.Xml.Schema
{
	internal class IdRefNode
	{
		internal IdRefNode(IdRefNode next, string id, int lineNo, int linePos)
		{
			this.Id = id;
			this.LineNo = lineNo;
			this.LinePos = linePos;
			this.Next = next;
		}

		internal string Id;

		internal int LineNo;

		internal int LinePos;

		internal IdRefNode Next;
	}
}
