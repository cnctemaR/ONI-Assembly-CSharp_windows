using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("Script = {tag},  Language Count = {languages.Length}")]
	internal struct OTL_Script
	{
		public string tag;

		public OTL_Language[] languages;
	}
}
