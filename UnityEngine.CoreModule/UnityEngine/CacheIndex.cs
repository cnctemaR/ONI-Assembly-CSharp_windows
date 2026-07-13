using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[Obsolete("This struct is not for public use.")]
	public struct CacheIndex
	{
		public string name;

		public int bytesUsed;

		public int expires;
	}
}
