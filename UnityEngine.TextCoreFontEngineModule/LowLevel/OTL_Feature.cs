using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Feature = {tag},  Lookup Count = {lookupIndexes.Length}")]
	[UsedByNativeCode]
	internal struct OTL_Feature
	{
		public string tag;

		public uint[] lookupIndexes;
	}
}
