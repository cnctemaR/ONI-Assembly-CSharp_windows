using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("Feature = {tag},  Lookup Count = {lookupIndexes.Length}")]
	internal struct OTL_Feature
	{
		public OTL_Tag tag;

		public uint[] lookupIndexes;
	}
}
