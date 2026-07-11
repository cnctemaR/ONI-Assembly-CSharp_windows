using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class BiEdgeConnectedComponents
	{
		public BiEdgeConnectedComponents(IGraph graph, BiEdgeConnectedComponents.Flags flags = BiEdgeConnectedComponents.Flags.None)
		{
			this.Graph = graph;
			BridgeDfs bridgeDfs = new BridgeDfs();
			bridgeDfs.Run(graph, null);
			this.Count = bridgeDfs.ComponentCount;
			if ((flags & BiEdgeConnectedComponents.Flags.CreateBridges) != BiEdgeConnectedComponents.Flags.None)
			{
				this.Bridges = bridgeDfs.Bridges;
			}
			if ((flags & BiEdgeConnectedComponents.Flags.CreateComponents) != BiEdgeConnectedComponents.Flags.None)
			{
				Subgraph subgraph = new Subgraph(graph);
				foreach (Arc arc in bridgeDfs.Bridges)
				{
					subgraph.Enable(arc, false);
				}
				this.Components = new ConnectedComponents(subgraph, ConnectedComponents.Flags.CreateComponents).Components;
			}
		}

		public IGraph Graph { get; private set; }

		public int Count { get; private set; }

		public List<HashSet<Node>> Components { get; private set; }

		public HashSet<Arc> Bridges { get; private set; }

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateComponents = 1,
			CreateBridges = 2
		}
	}
}
