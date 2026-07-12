using System;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	internal struct OTL_Table
	{
		public OTL_Script[] scripts;

		public OTL_Feature[] features;

		public OTL_Lookup[] lookups;
	}
}
