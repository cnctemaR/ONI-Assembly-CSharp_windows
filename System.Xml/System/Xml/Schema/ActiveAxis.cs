using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal class ActiveAxis
	{
		public int CurrentDepth
		{
			get
			{
				return this.currentDepth;
			}
		}

		internal void Reactivate()
		{
			this.isActive = true;
			this.currentDepth = -1;
		}

		internal ActiveAxis(Asttree axisTree)
		{
			this.axisTree = axisTree;
			this.currentDepth = -1;
			this.axisStack = new ArrayList(axisTree.SubtreeArray.Count);
			for (int i = 0; i < axisTree.SubtreeArray.Count; i++)
			{
				AxisStack axisStack = new AxisStack((ForwardAxis)axisTree.SubtreeArray[i], this);
				this.axisStack.Add(axisStack);
			}
			this.isActive = true;
		}

		public bool MoveToStartElement(string localname, string URN)
		{
			if (!this.isActive)
			{
				return false;
			}
			this.currentDepth++;
			bool flag = false;
			for (int i = 0; i < this.axisStack.Count; i++)
			{
				AxisStack axisStack = (AxisStack)this.axisStack[i];
				if (axisStack.Subtree.IsSelfAxis)
				{
					if (axisStack.Subtree.IsDss || this.CurrentDepth == 0)
					{
						flag = true;
					}
				}
				else if (this.CurrentDepth != 0 && axisStack.MoveToChild(localname, URN, this.currentDepth))
				{
					flag = true;
				}
			}
			return flag;
		}

		public virtual bool EndElement(string localname, string URN)
		{
			if (this.currentDepth == 0)
			{
				this.isActive = false;
				this.currentDepth--;
			}
			if (!this.isActive)
			{
				return false;
			}
			for (int i = 0; i < this.axisStack.Count; i++)
			{
				((AxisStack)this.axisStack[i]).MoveToParent(localname, URN, this.currentDepth);
			}
			this.currentDepth--;
			return false;
		}

		public bool MoveToAttribute(string localname, string URN)
		{
			if (!this.isActive)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < this.axisStack.Count; i++)
			{
				if (((AxisStack)this.axisStack[i]).MoveToAttribute(localname, URN, this.currentDepth + 1))
				{
					flag = true;
				}
			}
			return flag;
		}

		private int currentDepth;

		private bool isActive;

		private Asttree axisTree;

		private ArrayList axisStack;
	}
}
