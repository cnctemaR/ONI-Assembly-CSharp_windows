using System;
using System.Collections.Generic;
using Delaunay.Geo;
using UnityEngine;

public class VoronoiTree : VoronoiNode
{
	public VoronoiTree()
		: base(VoronoiNode.NodeType.Internal)
	{
		this.children = new List<VoronoiNode>();
		this.SetSeed(0);
	}

	public VoronoiTree(int seed = 0)
		: base(VoronoiNode.NodeType.Internal)
	{
		this.children = new List<VoronoiNode>();
		this.SetSeed(seed);
	}

	public VoronoiTree(VoronoiDiagram.Site site, VoronoiTree parent, int seed = 0)
		: base(site, VoronoiNode.NodeType.Internal, parent)
	{
		this.children = new List<VoronoiNode>();
		this.SetSeed(seed);
	}

	public VoronoiTree(VoronoiDiagram.Site site, List<VoronoiNode> children, VoronoiTree parent, int seed = 0)
		: base(site, VoronoiNode.NodeType.Internal, parent)
	{
		if (children == null)
		{
			children = new List<VoronoiNode>();
		}
		this.children = children;
		this.SetSeed(seed);
	}

	public SeededRandom myRandom { get; private set; }

	public void SetSeed(int seed)
	{
		this.myRandom = new SeededRandom(seed);
	}

	public VoronoiNode GetChildByID(uint id)
	{
		return this.children.Find((VoronoiNode s) => s.site.id == id);
	}

	public int ChildCount()
	{
		if (this.children == null)
		{
			return 0;
		}
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
			voronoiNode = new VoronoiTree(site, this, this.myRandom.seed + this.ChildCount());
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
		if (base.ComputeNode(list, depth))
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

	public bool ComputeChildren(int seed)
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
		if (base.ComputeNode(list, seed))
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
			if (!base.ComputeNode(list, depth))
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
				if (voronoiTree.ComputeChildren(depth))
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
			if (!base.ComputeNode(list, depth))
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
					if (!voronoiTree2.ComputeChildren(depth))
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

	public override VoronoiTree Split(VoronoiNode.SplitCommand cmd)
	{
		if (cmd.SplitFunction != null)
		{
			cmd.SplitFunction(this, cmd);
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
				this.children[i] = new VoronoiTree(leaf.site, this, this.myRandom.seed + i);
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

	public void VisitAll(Action<VoronoiNode> action)
	{
		action(this);
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i].type == VoronoiNode.NodeType.Internal)
			{
				(this.children[i] as VoronoiTree).VisitAll(action);
			}
			else
			{
				action(this.children[i]);
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

	protected List<VoronoiNode> children;

	public bool dontRelaxChildren;

	public delegate bool LeafNodeTest(VoronoiNode node);
}
