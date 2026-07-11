using System;
using System.Collections.Generic;

namespace Satsuma
{
	internal class LowpointDfs : Dfs
	{
		private void UpdateLowpoint(Node node, int newLowpoint)
		{
			if (this.lowpoint[node] > newLowpoint)
			{
				this.lowpoint[node] = newLowpoint;
			}
		}

		protected override void Start(out Dfs.Direction direction)
		{
			direction = Dfs.Direction.Undirected;
			this.level = new Dictionary<Node, int>();
			this.lowpoint = new Dictionary<Node, int>();
		}

		protected override bool NodeEnter(Node node, Arc arc)
		{
			this.level[node] = base.Level;
			this.lowpoint[node] = base.Level;
			return true;
		}

		protected override bool NodeExit(Node node, Arc arc)
		{
			if (arc != Arc.Invalid)
			{
				Node node2 = base.Graph.Other(arc, node);
				this.UpdateLowpoint(node2, this.lowpoint[node]);
			}
			return true;
		}

		protected override bool BackArc(Node node, Arc arc)
		{
			Node node2 = base.Graph.Other(arc, node);
			this.UpdateLowpoint(node, this.level[node2]);
			return true;
		}

		protected override void StopSearch()
		{
			this.level = null;
			this.lowpoint = null;
		}

		protected Dictionary<Node, int> level;

		protected Dictionary<Node, int> lowpoint;
	}
}
