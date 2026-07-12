using System;
using System.Diagnostics;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Language = {tag},  Feature Count = {featureIndexes.Length}")]
	[Serializable]
	internal struct OpenTypeLayoutLanguage
	{
		public string tag;

		public uint[] featureIndexes;
	}
}
