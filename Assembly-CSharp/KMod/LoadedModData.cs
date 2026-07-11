using System;
using System.Collections.Generic;
using System.Reflection;

namespace KMod
{
	public class LoadedModData
	{
		public ICollection<Assembly> dlls;

		public IEnumerable<MethodBase> patched_methods;
	}
}
