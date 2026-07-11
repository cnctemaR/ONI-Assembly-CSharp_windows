using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class InsertionTsp<TNode> : ITsp<TNode>
	{
		public InsertionTsp(IEnumerable<TNode> nodes, Func<TNode, TNode, double> cost, TspSelectionRule selectionRule = TspSelectionRule.Farthest)
		{
			this.Nodes = nodes;
			this.Cost = cost;
			this.SelectionRule = selectionRule;
			this.tour = new LinkedList<TNode>();
			this.tourNodes = new Dictionary<TNode, LinkedListNode<TNode>>();
			this.insertableNodes = new HashSet<TNode>();
			this.insertableNodeQueue = new PriorityQueue<TNode, double>();
			this.Clear();
		}

		public IEnumerable<TNode> Nodes { get; private set; }

		public Func<TNode, TNode, double> Cost { get; private set; }

		public TspSelectionRule SelectionRule { get; private set; }

		public IEnumerable<TNode> Tour
		{
			get
			{
				return this.tour;
			}
		}

		public double TourCost { get; private set; }

		private double PriorityFromCost(double c)
		{
			TspSelectionRule selectionRule = this.SelectionRule;
			if (selectionRule != TspSelectionRule.Farthest)
			{
				return c;
			}
			return -c;
		}

		public void Clear()
		{
			this.tour.Clear();
			this.TourCost = 0.0;
			this.tourNodes.Clear();
			this.insertableNodes.Clear();
			this.insertableNodeQueue.Clear();
			if (this.Nodes.Any<TNode>())
			{
				TNode tnode = this.Nodes.First<TNode>();
				this.tour.AddFirst(tnode);
				this.tourNodes[tnode] = this.tour.AddFirst(tnode);
				foreach (TNode tnode2 in this.Nodes)
				{
					if (!tnode2.Equals(tnode))
					{
						this.insertableNodes.Add(tnode2);
						this.insertableNodeQueue[tnode2] = this.PriorityFromCost(this.Cost(tnode, tnode2));
					}
				}
			}
		}

		public bool Insert(TNode node)
		{
			if (!this.insertableNodes.Contains(node))
			{
				return false;
			}
			this.insertableNodes.Remove(node);
			this.insertableNodeQueue.Remove(node);
			LinkedListNode<TNode> linkedListNode = null;
			double num = double.PositiveInfinity;
			for (LinkedListNode<TNode> linkedListNode2 = this.tour.First; linkedListNode2 != this.tour.Last; linkedListNode2 = linkedListNode2.Next)
			{
				LinkedListNode<TNode> next = linkedListNode2.Next;
				double num2 = this.Cost(linkedListNode2.Value, node) + this.Cost(node, next.Value);
				if (linkedListNode2 != next)
				{
					num2 -= this.Cost(linkedListNode2.Value, next.Value);
				}
				if (num2 < num)
				{
					num = num2;
					linkedListNode = linkedListNode2;
				}
			}
			this.tourNodes[node] = this.tour.AddAfter(linkedListNode, node);
			this.TourCost += num;
			foreach (TNode tnode in this.insertableNodes)
			{
				double num3 = this.PriorityFromCost(this.Cost(node, tnode));
				if (num3 < this.insertableNodeQueue[tnode])
				{
					this.insertableNodeQueue[tnode] = num3;
				}
			}
			return true;
		}

		public bool Insert()
		{
			if (this.insertableNodes.Count == 0)
			{
				return false;
			}
			this.Insert(this.insertableNodeQueue.Peek());
			return true;
		}

		public void Run()
		{
			while (this.Insert())
			{
			}
		}

		private LinkedList<TNode> tour;

		private Dictionary<TNode, LinkedListNode<TNode>> tourNodes;

		private HashSet<TNode> insertableNodes;

		private PriorityQueue<TNode, double> insertableNodeQueue;
	}
}
