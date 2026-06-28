using System;
using KSerialization;
using Satsuma;

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

		public Corner corner0;

		public Corner corner1;

		public Cell site0;

		public Cell site1;
	}
}
