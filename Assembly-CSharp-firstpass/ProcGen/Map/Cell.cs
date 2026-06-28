using System;
using System.Collections.Generic;
using KSerialization;
using Satsuma;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Cell : Node
	{
		public Cell()
		{
			base.SetType(WorldGenTags.Cell.Name);
			this.Init();
		}

		public Cell(Node node)
			: base(node, WorldGenTags.Cell.Name)
		{
			this.Init();
		}

		public Cell(Node node)
			: base(node.node, WorldGenTags.Cell.Name)
		{
			this.Init();
		}

		private void Init()
		{
			this.edges = new List<Edge>();
			this.corners = new List<Corner>();
			this.neighbors = new List<Cell>();
			this.tags = new TagSet();
		}

		public void Add(Edge e)
		{
			if (this.edges.Find((Edge edge) => (e.corner0 == edge.corner0 && e.corner1 == edge.corner1) || (e.corner1 == edge.corner0 && e.corner0 == edge.corner1)) == null)
			{
				this.edges.Add(e);
			}
		}

		public void Add(Corner c)
		{
			if (!this.corners.Contains(c))
			{
				this.corners.Add(c);
			}
		}

		public void Add(Cell c)
		{
			if (!this.neighbors.Contains(c))
			{
				this.neighbors.Add(c);
				c.Add(this);
			}
		}

		public List<Cell> neighbors;

		public List<Edge> edges;

		public List<Corner> corners;
	}
}
