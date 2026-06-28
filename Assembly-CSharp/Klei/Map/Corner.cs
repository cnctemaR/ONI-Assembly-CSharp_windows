using System;
using System.Collections.Generic;
using KSerialization;
using Satsuma;

namespace Klei.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Corner : Node
	{
		public Corner()
			: base(WorldGenTags.Corner.Name)
		{
			this.Init();
		}

		public Corner(Node node)
			: base(node, WorldGenTags.Corner.Name)
		{
			this.Init();
		}

		private void Init()
		{
			this.edges = new List<Edge>();
			this.cells = new List<Cell>();
			this.tags = new TagSet();
		}

		public void Add(Edge e)
		{
			if (this.edges.Find((Edge edge) => (e.corner0 == edge.corner0 && e.corner1 == edge.corner1) || (e.corner1 == edge.corner0 && e.corner0 == edge.corner1)) == null)
			{
				this.edges.Add(e);
				if (e.site0.position == this.position && this.cells.Find((Cell site) => e.site0 == site) == null)
				{
					this.cells.Add(e.site0);
					e.site0.Add(this);
				}
				if (e.site1.position == this.position && this.cells.Find((Cell site) => e.site1 == site) == null)
				{
					this.cells.Add(e.site1);
					e.site1.Add(this);
				}
			}
		}

		public List<Edge> edges;

		public List<Cell> cells;
	}
}
