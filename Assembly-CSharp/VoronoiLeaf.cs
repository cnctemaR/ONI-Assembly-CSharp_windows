using System;
using System.Collections.Generic;
using Delaunay.Geo;

public class VoronoiLeaf : VoronoiNode
{
	public VoronoiLeaf()
		: base(VoronoiNode.NodeType.Leaf)
	{
	}

	public VoronoiLeaf(VoronoiDiagram.Site site, VoronoiTree parent)
		: base(site, VoronoiNode.NodeType.Leaf, parent)
	{
	}

	public override VoronoiTree Split(VoronoiNode.SplitType splitType = (VoronoiNode.SplitType)0, TagSet dontCopyTags = null, TagSet moveTags = null, VoronoiNode.NodeTypeOverride typeOverride = null)
	{
		VoronoiTree voronoiTree = base.parent.ReplaceLeafWithTree(this);
		voronoiTree.Split(splitType, dontCopyTags, moveTags, typeOverride);
		return voronoiTree;
	}

	public void GetIntersectingSites(LineSegment edge, List<VoronoiDiagram.Site> intersectingSites)
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

	public override void Draw(int depth = 0)
	{
		if (depth > VoronoiNode.maxDepth || this.site.poly == null)
		{
			return;
		}
		base.Draw(depth);
	}
}
