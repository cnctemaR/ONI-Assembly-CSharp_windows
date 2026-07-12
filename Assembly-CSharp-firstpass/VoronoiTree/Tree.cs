using System;
using System.Collections.Generic;
using Delaunay.Geo;
using UnityEngine;

namespace VoronoiTree
{
	public class Tree : Node
	{
		public SeededRandom myRandom { get; private set; }

		public Tree()
			: base(Node.NodeType.Internal)
		{
			this.children = new List<Node>();
			this.SetSeed(0);
		}

		public Tree(int seed = 0)
			: base(Node.NodeType.Internal)
		{
			this.children = new List<Node>();
			this.SetSeed(seed);
		}

		public Tree(Diagram.Site site, Tree parent, int seed = 0)
			: base(site, Node.NodeType.Internal, parent)
		{
			this.children = new List<Node>();
			this.SetSeed(seed);
		}

		public Tree(Diagram.Site site, List<Node> children, Tree parent, int seed = 0)
			: base(site, Node.NodeType.Internal, parent)
		{
			if (children == null)
			{
				children = new List<Node>();
			}
			this.children = children;
			this.SetSeed(seed);
		}

		public void SetSeed(int seed)
		{
			this.myRandom = new SeededRandom(seed);
		}

		public Node GetChildByID(uint id)
		{
			return this.children.Find((Node s) => s.site.id == id);
		}

		public int ChildCount()
		{
			if (this.children == null)
			{
				return 0;
			}
			return this.children.Count;
		}

		public Tree GetChildContainingLeaf(Leaf leaf)
		{
			Vector2 vector = leaf.site.poly.Centroid();
			for (int i = 0; i < this.children.Count; i++)
			{
				Tree tree = this.children[i] as Tree;
				if (tree != null && tree.site.poly.Contains(vector))
				{
					return tree;
				}
			}
			return null;
		}

		public Node GetChild(int childIndex)
		{
			if (childIndex < this.children.Count)
			{
				return this.children[childIndex];
			}
			return null;
		}

		public void AddChild(Node child)
		{
			if (child.site.id > Node.maxIndex)
			{
				Node.maxIndex = child.site.id;
			}
			this.children.Add(child);
			child.SetParent(this);
		}

		public Node AddSite(Diagram.Site site, Node.NodeType type)
		{
			Node node;
			if (type == Node.NodeType.Internal)
			{
				node = new Tree(site, this, this.myRandom.seed + this.ChildCount());
			}
			else
			{
				node = new Leaf(site, this);
			}
			this.AddChild(node);
			return node;
		}

		public bool ComputeChildrenRecursive(int depth, bool pd = false)
		{
			if (depth > Node.maxDepth || this.site.poly == null || this.children == null)
			{
				return false;
			}
			List<Diagram.Site> list = new List<Diagram.Site>();
			for (int i = 0; i < this.children.Count; i++)
			{
				list.Add(this.children[i].site);
			}
			base.PlaceSites(list, depth);
			if (pd)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (!this.site.poly.Contains(list[j].position))
					{
						global::Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", new object[]
						{
							list[j].id,
							list[j].position
						});
					}
				}
				if (base.ComputeNodePD(list, 500, 0.2f))
				{
					for (int k = 0; k < this.children.Count; k++)
					{
						if (this.children[k].type == Node.NodeType.Internal && !(this.children[k] as Tree).ComputeChildrenRecursive(depth + 1, pd))
						{
							return false;
						}
					}
				}
			}
			else if (base.ComputeNode(list))
			{
				for (int l = 0; l < this.children.Count; l++)
				{
					if (this.children[l].type == Node.NodeType.Internal && !(this.children[l] as Tree).ComputeChildrenRecursive(depth + 1, false))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool ComputeChildren(int seed, bool place = false, bool pd = false)
		{
			if (this.site.poly == null || this.children == null)
			{
				return false;
			}
			List<Diagram.Site> list = new List<Diagram.Site>();
			for (int i = 0; i < this.children.Count; i++)
			{
				if (place || !this.site.poly.Contains(this.children[i].site.position))
				{
					global::Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", new object[]
					{
						this.children[i].site.id,
						this.children[i].site.position
					});
				}
				list.Add(this.children[i].site);
			}
			if (place)
			{
				base.PlaceSites(list, seed);
			}
			if (pd)
			{
				base.ComputeNodePD(list, 500, 0.2f);
			}
			else
			{
				base.ComputeNode(list);
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
				if (this.children[i].type == Node.NodeType.Internal)
				{
					Tree tree = this.children[i] as Tree;
					num += tree.Count();
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
					if (this.children[i].type == Node.NodeType.Internal)
					{
						(this.children[i] as Tree).Reset();
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
				if (this.children[i].type == Node.NodeType.Internal)
				{
					num3 = (this.children[i] as Tree).MaxDepth(num2);
				}
				if (num3 > num)
				{
					num = num3;
				}
			}
			return num;
		}

		public void RelaxRecursive(int depth, int iterations = -1, float minEnergy = 1f, bool pd = false)
		{
			if (this.dontRelaxChildren || this.site.poly == null || this.children == null || this.children.Count == 0)
			{
				this.visited = Node.VisitedType.MissingData;
				return;
			}
			List<Diagram.Site> list = new List<Diagram.Site>();
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
				base.PlaceSites(list, depth);
				if (pd)
				{
					if (!base.ComputeNodePD(list, 500, 0.2f))
					{
						this.visited = Node.VisitedType.Error;
						return;
					}
				}
				else if (!base.ComputeNode(list))
				{
					this.visited = Node.VisitedType.Error;
					return;
				}
				num2++;
			}
			for (int k = 0; k < this.children.Count; k++)
			{
				if (this.children[k].type == Node.NodeType.Internal)
				{
					Tree tree = this.children[k] as Tree;
					if (tree.ComputeChildren(depth, false, false))
					{
						tree.RelaxRecursive(depth + 1, iterations, minEnergy, false);
					}
				}
			}
			this.visited = Node.VisitedType.VisitedSuccess;
		}

		public float Relax(int depth, int relaxDepth, bool pd = false)
		{
			if (this.dontRelaxChildren || depth > Node.maxDepth || depth > relaxDepth || this.site.poly == null || this.children == null || this.children.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			if (depth < relaxDepth)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i].type == Node.NodeType.Internal)
					{
						Tree tree = this.children[i] as Tree;
						num += tree.Relax(depth + 1, relaxDepth, false);
					}
				}
				return num;
			}
			if (depth == relaxDepth)
			{
				List<Diagram.Site> list = new List<Diagram.Site>();
				for (int j = 0; j < this.children.Count; j++)
				{
					list.Add(this.children[j].site);
				}
				if (pd)
				{
					if (!base.ComputeNodePD(list, 500, 0.2f))
					{
						return 0f;
					}
				}
				else
				{
					base.PlaceSites(list, depth);
					if (!base.ComputeNode(list))
					{
						return 0f;
					}
				}
				for (int k = 0; k < this.children.Count; k++)
				{
					num += Vector2.Distance(this.children[k].site.position, list[k].poly.Centroid());
					this.children[k].site.position = list[k].poly.Centroid();
					if (this.children[k].type == Node.NodeType.Internal && !(this.children[k] as Tree).ComputeChildren(depth, false, false))
					{
						return 0f;
					}
				}
			}
			return num;
		}

		public Node GetNodeForPoint(Vector2 point, bool stopAtFirstChild = false)
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
					if (this.children[i].type != Node.NodeType.Internal)
					{
						return this.children[i];
					}
					Tree tree = this.children[i] as Tree;
					if (stopAtFirstChild)
					{
						return this.children[i];
					}
					return tree.GetNodeForPoint(point, false);
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

		public Node GetNodeForSite(Diagram.Site target)
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
					if (this.children[i].type == Node.NodeType.Internal)
					{
						return (this.children[i] as Tree).GetNodeForSite(target);
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

		public void GetIntersectingLeafSites(LineSegment edge, List<Diagram.Site> intersectingSites)
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
				if (this.children[i].type == Node.NodeType.Internal)
				{
					((Tree)this.children[i]).GetIntersectingLeafSites(edge, intersectingSites);
				}
				else
				{
					((Leaf)this.children[i]).GetIntersectingSites(edge, intersectingSites);
				}
			}
		}

		public void GetIntersectingLeafNodes(LineSegment edge, List<Leaf> intersectingNodes)
		{
			LineSegment lineSegment = new LineSegment(null, null);
			for (int i = 0; i < this.children.Count; i++)
			{
				if ((this.children[i].site.poly.Contains(edge.p0.Value) | this.children[i].site.poly.Contains(edge.p1.Value)) || this.children[i].site.poly.ClipSegment(edge, ref lineSegment))
				{
					if (this.children[i].type == Node.NodeType.Internal)
					{
						((Tree)this.children[i]).GetIntersectingLeafNodes(edge, intersectingNodes);
					}
					else
					{
						intersectingNodes.Add((Leaf)this.children[i]);
					}
				}
			}
		}

		public Tree ReplaceLeafWithTree(Leaf leaf)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i] == leaf)
				{
					this.children[i] = new Tree(leaf.site, this, this.myRandom.seed + i);
					this.children[i].log = leaf.log;
					if (leaf.tags != null)
					{
						this.children[i].SetTags(leaf.tags);
					}
					return this.children[i] as Tree;
				}
			}
			return null;
		}

		public Leaf ReplaceTreeWithLeaf(Tree tree)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i] == tree)
				{
					this.children[i] = new Leaf(tree.site, this);
					this.children[i].log = tree.log;
					if (tree.tags != null)
					{
						this.children[i].SetTags(tree.tags);
					}
					return this.children[i] as Leaf;
				}
			}
			return null;
		}

		public void ForceLowestToLeaf()
		{
			List<Node> list = new List<Node>();
			this.GetLeafNodes(list, null);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].type == Node.NodeType.Internal)
				{
					list[i].parent.ReplaceTreeWithLeaf(list[i] as Tree);
				}
			}
		}

		public void Collapse()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i] != null && this.children[i].type == Node.NodeType.Internal)
				{
					Tree tree = (Tree)this.children[i];
					if (tree.ChildCount() > 1)
					{
						tree.Collapse();
					}
					else
					{
						Node node = this.children[i];
						this.children[i] = new Leaf(tree.site, this);
						this.children[i].log = node.log;
						if (tree.tags != null)
						{
							this.children[i].SetTags(tree.tags);
						}
					}
				}
			}
		}

		public void VisitAll(Action<Node> action)
		{
			action(this);
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					(this.children[i] as Tree).VisitAll(action);
				}
				else
				{
					action(this.children[i]);
				}
			}
		}

		public List<Node> ImmediateChildren()
		{
			return new List<Node>(this.children);
		}

		public void GetLeafNodes(List<Node> nodes, Tree.NodeTest test = null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					Tree tree = (Tree)this.children[i];
					if (tree.ChildCount() > 0)
					{
						tree.GetLeafNodes(nodes, test);
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

		public void GetInternalNodes(List<Tree> nodes)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					Tree tree = (Tree)this.children[i];
					nodes.Add(tree);
					if (tree.ChildCount() > 0)
					{
						tree.GetInternalNodes(nodes);
					}
				}
			}
		}

		public void GetInternalNonLeafNodes(List<Node> nodes, Tree.NodeTest test = null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal && (test == null || test(this.children[i])))
				{
					nodes.Add(this.children[i]);
				}
			}
		}

		public void ResetParentPointer()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					Tree tree = (Tree)this.children[i];
					if (tree.ChildCount() > 0)
					{
						tree.ResetParentPointer();
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

		public void GetNodesWithTag(Tag tag, List<Node> nodes)
		{
			if (this.children.Count == 0 && this.tags.Contains(tag))
			{
				nodes.Add(this);
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					((Tree)this.children[i]).GetNodesWithTag(tag, nodes);
				}
				else if (this.children[i].tags.Contains(tag))
				{
					nodes.Add(this.children[i]);
				}
			}
		}

		public void GetNodesWithoutTag(Tag tag, List<Node> nodes)
		{
			if (this.children.Count == 0 && !this.tags.Contains(tag))
			{
				nodes.Add(this);
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].type == Node.NodeType.Internal)
				{
					((Tree)this.children[i]).GetNodesWithoutTag(tag, nodes);
				}
				else if (!this.children[i].tags.Contains(tag))
				{
					nodes.Add(this.children[i]);
				}
			}
		}

		protected List<Node> children;

		public bool dontRelaxChildren;

		public delegate bool NodeTest(Node node);
	}
}
