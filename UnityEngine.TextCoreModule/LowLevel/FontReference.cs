using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("{familyName} - {styleName}")]
	internal struct FontReference
	{
		public string familyName;

		public string styleName;

		public int faceIndex;

		public string filePath;
	}
}
