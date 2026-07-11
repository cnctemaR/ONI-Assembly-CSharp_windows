using System;
using System.Collections.Generic;

namespace System.Xml.Schema
{
	internal sealed class SequenceNode : InteriorNode
	{
		public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
		{
			Stack<SequenceNode.SequenceConstructPosContext> stack = new Stack<SequenceNode.SequenceConstructPosContext>();
			SequenceNode.SequenceConstructPosContext sequenceConstructPosContext = new SequenceNode.SequenceConstructPosContext(this, firstpos, lastpos);
			SequenceNode sequenceNode;
			for (;;)
			{
				sequenceNode = sequenceConstructPosContext.this_;
				sequenceConstructPosContext.lastposLeft = new BitSet(lastpos.Count);
				if (!(sequenceNode.LeftChild is SequenceNode))
				{
					break;
				}
				stack.Push(sequenceConstructPosContext);
				sequenceConstructPosContext = new SequenceNode.SequenceConstructPosContext((SequenceNode)sequenceNode.LeftChild, sequenceConstructPosContext.firstpos, sequenceConstructPosContext.lastposLeft);
			}
			sequenceNode.LeftChild.ConstructPos(sequenceConstructPosContext.firstpos, sequenceConstructPosContext.lastposLeft, followpos);
			for (;;)
			{
				sequenceConstructPosContext.firstposRight = new BitSet(firstpos.Count);
				sequenceNode.RightChild.ConstructPos(sequenceConstructPosContext.firstposRight, sequenceConstructPosContext.lastpos, followpos);
				if (sequenceNode.LeftChild.IsNullable && !sequenceNode.RightChild.IsRangeNode)
				{
					sequenceConstructPosContext.firstpos.Or(sequenceConstructPosContext.firstposRight);
				}
				if (sequenceNode.RightChild.IsNullable)
				{
					sequenceConstructPosContext.lastpos.Or(sequenceConstructPosContext.lastposLeft);
				}
				for (int num = sequenceConstructPosContext.lastposLeft.NextSet(-1); num != -1; num = sequenceConstructPosContext.lastposLeft.NextSet(num))
				{
					followpos[num].Or(sequenceConstructPosContext.firstposRight);
				}
				if (sequenceNode.RightChild.IsRangeNode)
				{
					((LeafRangeNode)sequenceNode.RightChild).NextIteration = sequenceConstructPosContext.firstpos.Clone();
				}
				if (stack.Count == 0)
				{
					break;
				}
				sequenceConstructPosContext = stack.Pop();
				sequenceNode = sequenceConstructPosContext.this_;
			}
		}

		public override bool IsNullable
		{
			get
			{
				SequenceNode sequenceNode = this;
				while (!sequenceNode.RightChild.IsRangeNode || !(((LeafRangeNode)sequenceNode.RightChild).Min == 0m))
				{
					if (!sequenceNode.RightChild.IsNullable && !sequenceNode.RightChild.IsRangeNode)
					{
						return false;
					}
					SyntaxTreeNode leftChild = sequenceNode.LeftChild;
					sequenceNode = leftChild as SequenceNode;
					if (sequenceNode == null)
					{
						return leftChild.IsNullable;
					}
				}
				return true;
			}
		}

		public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
		{
			base.ExpandTreeNoRecursive(parent, symbols, positions);
		}

		private struct SequenceConstructPosContext
		{
			public SequenceConstructPosContext(SequenceNode node, BitSet firstpos, BitSet lastpos)
			{
				this.this_ = node;
				this.firstpos = firstpos;
				this.lastpos = lastpos;
				this.lastposLeft = null;
				this.firstposRight = null;
			}

			public SequenceNode this_;

			public BitSet firstpos;

			public BitSet lastpos;

			public BitSet lastposLeft;

			public BitSet firstposRight;
		}
	}
}
