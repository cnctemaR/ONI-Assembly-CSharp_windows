using System;
using System.Diagnostics;

namespace UnityEngine.TextCore.Text
{
	[DebuggerDisplay("Unicode ({unicode})  '{(char)unicode}'")]
	internal struct TextProcessingElement
	{
		public TextProcessingElementType elementType;

		public uint unicode;

		public int stringIndex;

		public int length;
	}
}
