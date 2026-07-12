using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.LowLevel
{
	[Serializable]
	internal struct OpenTypeLayoutTable
	{
		public List<OpenTypeLayoutScript> scripts;

		public List<OpenTypeLayoutFeature> features;

		[SerializeReference]
		public List<OpenTypeLayoutLookup> lookups;
	}
}
