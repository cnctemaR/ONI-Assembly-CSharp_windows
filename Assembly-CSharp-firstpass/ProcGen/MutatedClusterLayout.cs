using System;
using System.Diagnostics;
using ObjectCloner;

namespace ProcGen
{
	[DebuggerDisplay("{layout.name}")]
	public class MutatedClusterLayout
	{
		public MutatedClusterLayout(ClusterLayout layout)
		{
			this.layout = SerializingCloner.Copy<ClusterLayout>(layout);
		}

		public ClusterLayout layout;
	}
}
