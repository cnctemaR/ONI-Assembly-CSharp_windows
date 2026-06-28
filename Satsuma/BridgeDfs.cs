using System;
using System.Collections.Generic;

namespace Satsuma
{
	internal class BridgeDfs : LowpointDfs
	{
		protected override void Start(out Dfs.Direction direction)
		{
			base.Start(out direction);
			this.ComponentCount = 0;
			this.Bridges = new HashSet<Arc>();
		}

		protected override bool NodeExit(Node node, Arc arc)
		{
			if (arc == Arc.Invalid)
			{
				this.ComponentCount++;
			}
			else if (this.lowpoint[node] == base.Level)
			{
				this.Bridges.Add(arc);
				this.ComponentCount++;
			}
			return base.NodeExit(node, arc);
		}

		public int ComponentCount;

		public HashSet<Arc> Bridges;
	}
}
