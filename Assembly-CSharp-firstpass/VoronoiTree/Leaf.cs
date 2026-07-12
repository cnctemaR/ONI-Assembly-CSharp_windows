using System;
using System.Collections.Generic;
using Delaunay.Geo;

namespace VoronoiTree
{
	public class Leaf : Node
	{
		public Leaf()
			: base(Node.NodeType.Leaf)
		{
		}

		public Leaf(Diagram.Site site, Tree parent)
			: base(site, Node.NodeType.Leaf, parent)
		{
		}

		public void GetIntersectingSites(LineSegment edge, List<Diagram.Site> intersectingSites)
		{
			if (this.site == null)
			{
				return;
			}
			if (this.site.poly == null)
			{
				return;
			}
			LineSegment lineSegment = new LineSegment(null, null);
			if (!this.site.poly.ClipSegment(edge, ref lineSegment))
			{
				return;
			}
			intersectingSites.Add(this.site);
		}
	}
}
