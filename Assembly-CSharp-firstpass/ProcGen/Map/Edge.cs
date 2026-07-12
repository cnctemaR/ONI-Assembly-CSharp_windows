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

		public Edge(Arc arc, Cell s0, Cell s1, Corner c0, Corner c1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			this.corner0 = c0;
			this.corner1 = c1;
			this.tags = new TagSet();
		}

		public void SetCorners(Corner corner0, Corner corner1)
		{
			this.corner0 = corner0;
			this.corner1 = corner1;
		}

		public Vector2 MidPoint()
		{
			return (this.corner1.position - this.corner0.position) * 0.5f + this.corner0.position;
		}

		public Corner corner0;

		public Corner corner1;
	}
}
