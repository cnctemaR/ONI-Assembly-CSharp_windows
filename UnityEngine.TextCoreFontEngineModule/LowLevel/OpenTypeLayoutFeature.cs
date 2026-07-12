using System;
using System.Diagnostics;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Feature = {tag},  Lookup Count = {lookupIndexes.Length}")]
	[Serializable]
	internal struct OpenTypeLayoutFeature
	{
		public string tag;

		public uint[] lookupIndexes;
	}
}
