using System;
using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Edge : Arc
	{
		public Edge()
			: base(WorldGenTags.Edge.Name)
		{
			this.tags = new TagSet();
		}

		public Edge(Corner c0, Corner c1)
			: base(WorldGenTags.Edge.Name)
		{
			this.corner0 = c0;
			this.corner1 = c1;
			c0.Add(this);
			c1.Add(this);
			this.tags = new TagSet();
		}

		public Edge(Arc arc, Corner c0, Corner c1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			this.corner0 = c0;
			this.corner1 = c1;
			c0.Add(this);
			c1.Add(this);
			this.tags = new TagSet();
		}

		public Edge(Arc arc, Corner c0, Corner c1, Cell s0, Cell s1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			this.corner0 = c0;
			this.corner1 = c1;
			this.site0 = s0;
			this.site1 = s1;
			c0.Add(this);
			c1.Add(this);
			s0.Add(this);
			s1.Add(this);
			this.tags = new TagSet();
		}

		public void SetSite0(Cell s0)
		{
			this.site0 = s0;
			s0.Add(this);
		}

		public void SetSite1(Cell s1)
		{
			this.site1 = s1;
			s1.Add(this);
		}

		public Vector2 MidPoint()
		{
			return (this.corner1.position - this.corner0.position) * 0.5f + this.corner0.position;
		}

		public Corner corner0;

		public Corner corner1;

		public Cell site0;

		public Cell site1;
	}
}
