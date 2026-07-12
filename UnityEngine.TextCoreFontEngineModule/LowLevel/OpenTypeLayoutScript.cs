using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Script = {tag},  Language Count = {languages.Count}")]
	[Serializable]
	internal struct OpenTypeLayoutScript
	{
		public string tag;

		public List<OpenTypeLayoutLanguage> languages;
	}
}
