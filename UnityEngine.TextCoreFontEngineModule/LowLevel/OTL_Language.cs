using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("Language = {tag},  Feature Count = {featureIndexes.Length}")]
	internal struct OTL_Language
	{
		public OTL_Tag tag;

		public uint[] featureIndexes;
	}
}
