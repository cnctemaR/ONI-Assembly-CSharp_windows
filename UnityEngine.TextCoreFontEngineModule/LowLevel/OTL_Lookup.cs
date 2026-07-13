using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("{(OTL_LookupType)lookupType}")]
	[UsedByNativeCode]
	internal struct OTL_Lookup
	{
		public uint lookupType;

		public uint lookupFlag;

		public uint markFilteringSet;
	}
}
