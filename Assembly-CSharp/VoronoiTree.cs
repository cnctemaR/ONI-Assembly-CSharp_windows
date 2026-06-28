using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Klei;
using UnityEngine;

public class VoronoiTree : VoronoiNode
{
	public VoronoiTree()
		: base(VoronoiNode.NodeType.Internal)
	{
		this.children = new List<VoronoiNode>();
	}

	public VoronoiTree(VoronoiDiagram.Site site, VoronoiTree parent)
		: base(site, VoronoiNode.NodeType.Internal, parent)
	{
		this.children = new List<VoronoiNode>();
	}

	public VoronoiTree(VoronoiDiagram.Site site, List<VoronoiNode> children, VoronoiTree parent)
		: base(site, VoronoiNode.NodeType.Internal, parent)
	{
		this.children = children;
	}

	public VoronoiNode GetChildByID(uint id)
	{
		return this.children.Find((VoronoiNode s) => s.site.id == id);
	}

	public int ChildCount()
	{
		return this.children.Count;
	}

	public VoronoiTree GetChildContainingLeaf(VoronoiLeaf leaf)
	{
		Vector2 vector = leaf.site.poly.Centroid();
		for (int i = 0; i < this.children.Count; i++)
		{
			VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
			if (voronoiTree != null && voronoiTree.site.poly.Contains(vector))
			{
				return voronoiTree;
			}
		}
		return null;
	}

	public VoronoiNode GetChild(int childIndex)
	{
		if (childIndex < this.children.Count)
		{
			return this.children[childIndex];
		}
		return null;
	}

	public void AddChild(VoronoiNode child)
	{
		if (child.site.id > VoronoiNode.maxIndex)
		{
			VoronoiNode.maxIndex = child.site.id;
		}
		this.children.Add(child);
		child.SetParent(this);
	}

	public VoronoiNode AddSite(VoronoiDiagram.Site site, VoronoiNode.NodeType type)
	{
		VoronoiNode voronoiNode;
		if (type == VoronoiNode.NodeType.Internal)
		{
			voronoiNode = new VoronoiTree(site, this);
		}
		else
		{
			voronoiNode = new VoronoiLeaf(site, this);
		}
		this.AddChild(voronoiNode);
		return voronoiNode;
	}

	public bool ComputeChildrenRecursive(int depth)
	{
		if (depth > VoronoiNode.maxDepth || this.site.poly == null || this.children == null)
		{
			return false;
		}
		List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
		for (int i = 0; i < this.children.Count; i++)
		{
			list.Add(this.children[i].site);
		}
		if (base.ComputeNode(list))
		{
			for (int j = 0; j < this.children.Count; j++)
			{
				if (this.children[j].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = this.children[j] as VoronoiTree;
					if (!voronoiTree.ComputeChildrenRecursive(depth + 1))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool ComputeChildren()
	{
		if (this.site.poly == null || this.children == null)
		{
			return false;
		}
		List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
		for (int i = 0; i < this.children.Count; i++)
		{
			list.Add(this.children[i].site);
		}
		if (base.ComputeNode(list))
		{
		}
		return true;
	}

	public int Count()
	{
		if (this.children == null || this.children.Count == 0)
		{
			return 0;
		}
		int num = this.children.Count;
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
				num += voronoiTree.Count();
			}
		}
		return num;
	}

	public void Reset()
	{
		if (this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
					voronoiTree.Reset();
				}
			}
		}
		base.Reset(null);
	}

	public int MaxDepth(int depth = 0)
	{
		if (this.children == null || this.children.Count == 0)
		{
			return depth;
		}
		int num = depth + 1;
		int num2 = num;
		for (int i = 0; i < this.children.Count; i++)
		{
			int num3 = num2 + 1;
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
				num3 = voronoiTree.MaxDepth(num2);
			}
			if (num3 > num)
			{
				num = num3;
			}
		}
		return num;
	}

	public void RelaxRecursive(int depth, int iterations = -1, float minEnergy = 1f)
	{
		if (this.dontRelaxChildren || this.site.poly == null || this.children == null || this.children.Count == 0)
		{
			this.visited = VoronoiNode.VisitedType.MissingData;
			return;
		}
		List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
		for (int i = 0; i < this.children.Count; i++)
		{
			list.Add(this.children[i].site);
		}
		float num = float.MaxValue;
		int num2 = 0;
		while (num2 < iterations && num > minEnergy)
		{
			float num3 = 0f;
			for (int j = 0; j < this.children.Count; j++)
			{
				num3 += Vector2.Distance(this.children[j].site.position, list[j].poly.Centroid());
				this.children[j].site.position = list[j].poly.Centroid();
			}
			num = num3;
			if (!base.ComputeNode(list))
			{
				this.visited = VoronoiNode.VisitedType.Error;
				return;
			}
			num2++;
		}
		for (int k = 0; k < this.children.Count; k++)
		{
			if (this.children[k].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = this.children[k] as VoronoiTree;
				if (voronoiTree.ComputeChildren())
				{
					voronoiTree.RelaxRecursive(depth + 1, iterations, minEnergy);
				}
			}
		}
		this.visited = VoronoiNode.VisitedType.VisitedSuccess;
	}

	public float Relax(int depth, int relaxDepth)
	{
		if (this.dontRelaxChildren || depth > VoronoiNode.maxDepth || depth > relaxDepth || this.site.poly == null || this.children == null || this.children.Count == 0)
		{
			return 0f;
		}
		float num = 0f;
		if (depth < relaxDepth)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
					num += voronoiTree.Relax(depth + 1, relaxDepth);
				}
			}
			return num;
		}
		if (depth == relaxDepth)
		{
			List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
			for (int j = 0; j < this.children.Count; j++)
			{
				list.Add(this.children[j].site);
			}
			if (!base.ComputeNode(list))
			{
				return 0f;
			}
			for (int k = 0; k < this.children.Count; k++)
			{
				num += Vector2.Distance(this.children[k].site.position, list[k].poly.Centroid());
				this.children[k].site.position = list[k].poly.Centroid();
				if (this.children[k].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree2 = this.children[k] as VoronoiTree;
					if (!voronoiTree2.ComputeChildren())
					{
						return 0f;
					}
				}
			}
		}
		return num;
	}

	public VoronoiNode GetNodeForPoint(Vector2 point, bool stopAtFirstChild = false)
	{
		if (this.site.poly == null)
		{
			return null;
		}
		if (this.children == null || this.children.Count == 0)
		{
			return this;
		}
		int i = 0;
		while (i < this.children.Count)
		{
			if (this.children[i].site.poly.Contains(point))
			{
				if (this.children[i].type != VoronoiNode.NodeType.Internal)
				{
					return this.children[i];
				}
				VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
				if (stopAtFirstChild)
				{
					return this.children[i];
				}
				return voronoiTree.GetNodeForPoint(point, false);
			}
			else
			{
				i++;
			}
		}
		if (this.site.poly.Contains(point))
		{
			return this;
		}
		return null;
	}

	public VoronoiNode GetNodeForSite(VoronoiDiagram.Site target)
	{
		if (this.site == target)
		{
			return this;
		}
		if (this.site.poly == null || this.children == null || this.children.Count == 0)
		{
			return null;
		}
		int i = 0;
		while (i < this.children.Count)
		{
			if (this.children[i].site == target)
			{
				return this.children[i];
			}
			if (this.children[i].site.poly.Contains(target.position))
			{
				if (this.children[i].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = this.children[i] as VoronoiTree;
					return voronoiTree.GetNodeForSite(target);
				}
				return this.children[i];
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	public void GetIntersectingLeafSites(LineSegment edge, List<VoronoiDiagram.Site> intersectingSites)
	{
		LineSegment lineSegment = new LineSegment(null, null);
		if (!(this.site.poly.Contains(edge.p0.Value) | this.site.poly.Contains(edge.p1.Value)) && !this.site.poly.ClipSegment(edge, ref lineSegment))
		{
			return;
		}
		if (this.children.Count == 0)
		{
			intersectingSites.Add(this.site);
			return;
		}
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				((VoronoiTree)this.children[i]).GetIntersectingLeafSites(edge, intersectingSites);
			}
			else
			{
				((VoronoiLeaf)this.children[i]).GetIntersectingSites(edge, intersectingSites);
			}
		}
	}

	public void GetIntersectingLeafNodes(LineSegment edge, List<VoronoiLeaf> intersectingNodes)
	{
		LineSegment lineSegment = new LineSegment(null, null);
		for (int i = 0; i < this.children.Count; i++)
		{
			if ((this.children[i].site.poly.Contains(edge.p0.Value) | this.children[i].site.poly.Contains(edge.p1.Value)) || this.children[i].site.poly.ClipSegment(edge, ref lineSegment))
			{
				if (this.children[i].type == VoronoiNode.NodeType.Internal)
				{
					((VoronoiTree)this.children[i]).GetIntersectingLeafNodes(edge, intersectingNodes);
				}
				else
				{
					intersectingNodes.Add((VoronoiLeaf)this.children[i]);
				}
			}
		}
	}

	public override VoronoiTree Split(VoronoiNode.SplitType splitType = (VoronoiNode.SplitType)0, TagSet dontCopyTags = null, TagSet moveTags = null, VoronoiNode.NodeTypeOverride typeOverride = null)
	{
		Node node;
		if (this.tags.Contains(WorldGenTags.Overworld))
		{
			node = WorldGen.WorldLayout.overworldGraph.FindNodeByID(this.site.id);
		}
		else
		{
			node = WorldGen.WorldLayout.localGraph.FindNodeByID(this.site.id);
		}
		TagSet tagSet = new TagSet(this.tags);
		if (dontCopyTags != null)
		{
			tagSet.Remove(dontCopyTags);
			if (moveTags != null)
			{
				tagSet.Remove(moveTags);
			}
		}
		TagSet tagSet2 = new TagSet();
		if (moveTags != null)
		{
			for (int i = 0; i < moveTags.Count; i++)
			{
				Tag tag = moveTags[i];
				if (this.tags.Contains(tag))
				{
					this.tags.Remove(tag);
					tagSet2.Add(tag);
				}
			}
		}
		List<Vector2> list = new List<Vector2>();
		if (tagSet.Contains(WorldGenTags.Feature))
		{
			Node node2 = WorldGen.WorldLayout.localGraph.AddNode(node.type);
			node2.SetPosition((!tagSet.Contains(WorldGenTags.StartLocation)) ? this.site.position : this.site.poly.Centroid());
			VoronoiNode voronoiNode = this.AddSite(new VoronoiDiagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiNode.NodeType.Leaf);
			if (tagSet != null && tagSet.Count != 0)
			{
				voronoiNode.SetTags(tagSet);
			}
			tagSet.Remove(WorldGenTags.Feature);
			tagSet.Remove(new Tag(node.type));
			list.Add(node2.position);
		}
		float num = WorldGen.Settings.defaults.GetFloat("SplitDensityMin");
		float num2 = WorldGen.Settings.defaults.GetFloat("SplitDensityMax");
		if (this.tags.Contains(WorldGenTags.UltraHighDensitySplit))
		{
			num = WorldGen.Settings.defaults.GetFloat("UltraHighSplitDensityMin");
			num2 = WorldGen.Settings.defaults.GetFloat("UltraHighSplitDensityMax");
		}
		else if (this.tags.Contains(WorldGenTags.VeryHighDensitySplit))
		{
			num = WorldGen.Settings.defaults.GetFloat("VeryHighSplitDensityMin");
			num2 = WorldGen.Settings.defaults.GetFloat("VeryHighSplitDensityMax");
		}
		else if (this.tags.Contains(WorldGenTags.HighDensitySplit))
		{
			num = WorldGen.Settings.defaults.GetFloat("HighSplitDensityMin");
			num2 = WorldGen.Settings.defaults.GetFloat("HighSplitDensityMax");
		}
		else if (this.tags.Contains(WorldGenTags.MediumDensitySplit))
		{
			num = WorldGen.Settings.defaults.GetFloat("MediumSplitDensityMin");
			num2 = WorldGen.Settings.defaults.GetFloat("MediumSplitDensityMax");
		}
		float num3 = WorldGen.RandomRange(num, num2);
		List<Vector2> randomPoints = PointGenerator.GetRandomPoints(this.site.poly, num3, 1f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, true, true);
		for (int j = 0; j < randomPoints.Count; j++)
		{
			Node node3 = WorldGen.WorldLayout.localGraph.AddNode((typeOverride != null) ? typeOverride(randomPoints[j]) : node.type);
			node3.SetPosition(randomPoints[j]);
			VoronoiNode voronoiNode2 = this.AddSite(new VoronoiDiagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiNode.NodeType.Leaf);
			if (tagSet != null && tagSet.Count != 0)
			{
				voronoiNode2.SetTags(tagSet);
			}
		}
		for (int k = 0; k < tagSet2.Count; k++)
		{
			Tag tag2 = tagSet2[k];
			this.children[(int)WorldGen.RandomRange(0f, (float)this.children.Count)].AddTag(tag2);
		}
		this.ComputeChildrenRecursive(0);
		this.RelaxRecursive(0, 3, 1f);
		return this;
	}

	public VoronoiTree ReplaceLeafWithTree(VoronoiLeaf leaf)
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] == leaf)
			{
				this.children[i] = new VoronoiTree(leaf.site, this);
				this.children[i].log = leaf.log;
				if (leaf.tags != null)
				{
					this.children[i].SetTags(leaf.tags);
				}
				return this.children[i] as VoronoiTree;
			}
		}
		return null;
	}

	public VoronoiLeaf ReplaceTreeWithLeaf(VoronoiTree tree)
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] == tree)
			{
				this.children[i] = new VoronoiLeaf(tree.site, this);
				this.children[i].log = tree.log;
				if (tree.tags != null)
				{
					this.children[i].SetTags(tree.tags);
				}
				return this.children[i] as VoronoiLeaf;
			}
		}
		return null;
	}

	public void ForceLowestToLeaf()
	{
		List<VoronoiNode> list = new List<VoronoiNode>();
		this.GetLeafNodes(list, null);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].type == VoronoiNode.NodeType.Internal)
			{
				list[i].parent.ReplaceTreeWithLeaf(list[i] as VoronoiTree);
			}
		}
	}

	public void Collapse()
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] != null)
			{
				if (this.children[i].type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = (VoronoiTree)this.children[i];
					if (voronoiTree.ChildCount() > 1)
					{
						voronoiTree.Collapse();
					}
					else
					{
						VoronoiNode voronoiNode = this.children[i];
						this.children[i] = new VoronoiLeaf(voronoiTree.site, this);
						this.children[i].log = voronoiNode.log;
						if (voronoiTree.tags != null)
						{
							this.children[i].SetTags(voronoiTree.tags);
						}
					}
				}
			}
		}
	}

	public void UpdateTags()
	{
		base.PushTagsToSiteNode();
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				(this.children[i] as VoronoiTree).UpdateTags();
			}
			else
			{
				this.children[i].PushTagsToSiteNode();
			}
		}
	}

	public void GetLeafNodes(List<VoronoiNode> nodes, VoronoiTree.LeafNodeTest test = null)
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = (VoronoiTree)this.children[i];
				if (voronoiTree.ChildCount() > 0)
				{
					voronoiTree.GetLeafNodes(nodes, test);
				}
				else if (test == null || test(this.children[i]))
				{
					nodes.Add(this.children[i]);
				}
			}
			else if (test == null || test(this.children[i]))
			{
				nodes.Add(this.children[i]);
			}
		}
	}

	public void GetInternalNodes(List<VoronoiTree> nodes)
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = (VoronoiTree)this.children[i];
				nodes.Add(voronoiTree);
				if (voronoiTree.ChildCount() > 0)
				{
					voronoiTree.GetInternalNodes(nodes);
				}
			}
		}
	}

	public void ResetParentPointer()
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = (VoronoiTree)this.children[i];
				if (voronoiTree.ChildCount() > 0)
				{
					voronoiTree.ResetParentPointer();
				}
			}
			this.children[i].SetParent(this);
		}
	}

	public void AddTagToChildren(Tag tag)
	{
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].AddTag(tag);
		}
	}

	public void GetNodesWithTag(Tag tag, List<VoronoiNode> nodes)
	{
		if (this.children.Count == 0 && this.tags.Contains(tag))
		{
			nodes.Add(this);
			return;
		}
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				((VoronoiTree)this.children[i]).GetNodesWithTag(tag, nodes);
			}
			else if (this.children[i].tags.Contains(tag))
			{
				nodes.Add(this.children[i]);
			}
		}
	}

	public override void Draw(int depth = 0)
	{
		if (depth > VoronoiNode.maxDepth || this.site.poly == null || this.children == null)
		{
			return;
		}
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].Draw(depth + 1);
		}
		base.Draw(depth);
	}

	protected List<VoronoiNode> children;

	public bool dontRelaxChildren;

	public delegate bool LeafNodeTest(VoronoiNode node);
}
