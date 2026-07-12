using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[DebuggerDisplay("{(OTL_LookupType)lookupType}")]
	internal struct OTL_Lookup
	{
		public uint lookupType;

		public uint lookupFlag;

		public uint markFilteringSet;
	}
}
