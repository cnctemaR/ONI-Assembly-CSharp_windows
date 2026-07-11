using System;

namespace System.Xml.Schema
{
	internal class AxisElement
	{
		internal DoubleLinkAxis CurNode
		{
			get
			{
				return this.curNode;
			}
		}

		internal AxisElement(DoubleLinkAxis node, int depth)
		{
			this.curNode = node;
			this.curDepth = depth;
			this.rootDepth = depth;
			this.isMatch = false;
		}

		internal void SetDepth(int depth)
		{
			this.curDepth = depth;
			this.rootDepth = depth;
		}

		internal void MoveToParent(int depth, ForwardAxis parent)
		{
			if (depth != this.curDepth - 1)
			{
				if (depth == this.curDepth && this.isMatch)
				{
					this.isMatch = false;
				}
				return;
			}
			if (this.curNode.Input == parent.RootNode && parent.IsDss)
			{
				this.curNode = parent.RootNode;
				this.rootDepth = (this.curDepth = -1);
				return;
			}
			if (this.curNode.Input != null)
			{
				this.curNode = (DoubleLinkAxis)this.curNode.Input;
				this.curDepth--;
				return;
			}
		}

		internal bool MoveToChild(string name, string URN, int depth, ForwardAxis parent)
		{
			if (Asttree.IsAttribute(this.curNode))
			{
				return false;
			}
			if (this.isMatch)
			{
				this.isMatch = false;
			}
			if (!AxisStack.Equal(this.curNode.Name, this.curNode.Urn, name, URN))
			{
				return false;
			}
			if (this.curDepth == -1)
			{
				this.SetDepth(depth);
			}
			else if (depth > this.curDepth)
			{
				return false;
			}
			if (this.curNode == parent.TopNode)
			{
				this.isMatch = true;
				return true;
			}
			DoubleLinkAxis doubleLinkAxis = (DoubleLinkAxis)this.curNode.Next;
			if (Asttree.IsAttribute(doubleLinkAxis))
			{
				this.isMatch = true;
				return false;
			}
			this.curNode = doubleLinkAxis;
			this.curDepth++;
			return false;
		}

		internal DoubleLinkAxis curNode;

		internal int rootDepth;

		internal int curDepth;

		internal bool isMatch;
	}
}
