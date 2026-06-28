using System;
using System.Collections.Generic;
using Delaunay.Geo;
using UnityEngine;

namespace VoronoiTree
{
	public class Tree : Node
	{
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

		public SeededRandom myRandom { get; private set; }

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
			int num;
			if (this.children == null)
			{
				num = 0;
			}
			else
			{
				num = this.children.Count;
			}
			return num;
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
			Node node;
			if (childIndex < this.children.Count)
			{
				node = this.children[childIndex];
			}
			else
			{
				node = null;
			}
			return node;
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
			bool flag;
			if (depth > Node.maxDepth || this.site.poly == null || this.children == null)
			{
				flag = false;
			}
			else
			{
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
					}
					if (base.ComputeNodePD(list, 500, 0.2f))
					{
						for (int k = 0; k < this.children.Count; k++)
						{
							if (this.children[k].type == Node.NodeType.Internal)
							{
								Tree tree = this.children[k] as Tree;
								if (!tree.ComputeChildrenRecursive(depth + 1, pd))
								{
									return false;
								}
							}
						}
					}
				}
				else if (base.ComputeNode(list))
				{
					for (int l = 0; l < this.children.Count; l++)
					{
						if (this.children[l].type == Node.NodeType.Internal)
						{
							Tree tree2 = this.children[l] as Tree;
							if (!tree2.ComputeChildrenRecursive(depth + 1, false))
							{
								return false;
							}
						}
					}
				}
				flag = true;
			}
			return flag;
		}

		public bool ComputeChildren(int seed, bool place = false, bool pd = false)
		{
			bool flag;
			if (this.site.poly == null || this.children == null)
			{
				flag = false;
			}
			else
			{
				List<Diagram.Site> list = new List<Diagram.Site>();
				for (int i = 0; i < this.children.Count; i++)
				{
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
				flag = true;
			}
			return flag;
		}

		public int Count()
		{
			int num;
			if (this.children == null || this.children.Count == 0)
			{
				num = 0;
			}
			else
			{
				int num2 = this.children.Count;
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i].type == Node.NodeType.Internal)
					{
						Tree tree = this.children[i] as Tree;
						num2 += tree.Count();
					}
				}
				num = num2;
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
						Tree tree = this.children[i] as Tree;
						tree.Reset();
					}
				}
			}
			base.Reset(null);
		}

		public int MaxDepth(int depth = 0)
		{
			int num;
			if (this.children == null || this.children.Count == 0)
			{
				num = depth;
			}
			else
			{
				int num2 = depth + 1;
				int num3 = num2;
				for (int i = 0; i < this.children.Count; i++)
				{
					int num4 = num3 + 1;
					if (this.children[i].type == Node.NodeType.Internal)
					{
						Tree tree = this.children[i] as Tree;
						num4 = tree.MaxDepth(num3);
					}
					if (num4 > num2)
					{
						num2 = num4;
					}
				}
				num = num2;
			}
			return num;
		}

		public void RelaxRecursive(int depth, int iterations = -1, float minEnergy = 1f, bool pd = false)
		{
			if (this.dontRelaxChildren || this.site.poly == null || this.children == null || this.children.Count == 0)
			{
				this.visited = Node.VisitedType.MissingData;
			}
			else
			{
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
		}

		public float Relax(int depth, int relaxDepth, bool pd = false)
		{
			float num;
			if (this.dontRelaxChildren || depth > Node.maxDepth || depth > relaxDepth || this.site.poly == null || this.children == null || this.children.Count == 0)
			{
				num = 0f;
			}
			else
			{
				float num2 = 0f;
				if (depth < relaxDepth)
				{
					for (int i = 0; i < this.children.Count; i++)
					{
						if (this.children[i].type == Node.NodeType.Internal)
						{
							Tree tree = this.children[i] as Tree;
							num2 += tree.Relax(depth + 1, relaxDepth, false);
						}
					}
					num = num2;
				}
				else
				{
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
							num2 += Vector2.Distance(this.children[k].site.position, list[k].poly.Centroid());
							this.children[k].site.position = list[k].poly.Centroid();
							if (this.children[k].type == Node.NodeType.Internal)
							{
								Tree tree2 = this.children[k] as Tree;
								if (!tree2.ComputeChildren(depth, false, false))
								{
									return 0f;
								}
							}
						}
					}
					num = num2;
				}
			}
			return num;
		}

		public Node GetNodeForPoint(Vector2 point, bool stopAtFirstChild = false)
		{
			Node node;
			if (this.site.poly == null)
			{
				node = null;
			}
			else if (this.children == null || this.children.Count == 0)
			{
				node = this;
			}
			else
			{
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
					node = this;
				}
				else
				{
					node = null;
				}
			}
			return node;
		}

		public Node GetNodeForSite(Diagram.Site target)
		{
			Node node;
			if (this.site == target)
			{
				node = this;
			}
			else if (this.site.poly == null || this.children == null || this.children.Count == 0)
			{
				node = null;
			}
			else
			{
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
							Tree tree = this.children[i] as Tree;
							return tree.GetNodeForSite(target);
						}
						return this.children[i];
					}
					else
					{
						i++;
					}
				}
				node = null;
			}
			return node;
		}

		public void GetIntersectingLeafSites(LineSegment edge, List<Diagram.Site> intersectingSites)
		{
			LineSegment lineSegment = new LineSegment(null, null);
			if ((this.site.poly.Contains(edge.p0.Value) | this.site.poly.Contains(edge.p1.Value)) || this.site.poly.ClipSegment(edge, ref lineSegment))
			{
				if (this.children.Count == 0)
				{
					intersectingSites.Add(this.site);
				}
				else
				{
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

		public override Tree Split(Node.SplitCommand cmd)
		{
			if (cmd.SplitFunction != null)
			{
				cmd.SplitFunction(this, cmd);
			}
			this.ComputeChildrenRecursive(0, false);
			this.RelaxRecursive(0, 3, 1f, false);
			return this;
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
				if (this.children[i] != null)
				{
					if (this.children[i].type == Node.NodeType.Internal)
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

		public void GetLeafNodes(List<Node> nodes, Tree.LeafNodeTest test = null)
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
			}
			else
			{
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
		}

		protected List<Node> children = null;

		public bool dontRelaxChildren = false;

		public delegate bool LeafNodeTest(Node node);
	}
}
