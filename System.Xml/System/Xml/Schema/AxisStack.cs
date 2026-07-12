using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal class AxisStack
	{
		internal ForwardAxis Subtree
		{
			get
			{
				return this._subtree;
			}
		}

		internal int Length
		{
			get
			{
				return this._stack.Count;
			}
		}

		public AxisStack(ForwardAxis faxis, ActiveAxis parent)
		{
			this._subtree = faxis;
			this._stack = new ArrayList();
			this._parent = parent;
			if (!faxis.IsDss)
			{
				this.Push(1);
			}
		}

		internal void Push(int depth)
		{
			AxisElement axisElement = new AxisElement(this._subtree.RootNode, depth);
			this._stack.Add(axisElement);
		}

		internal void Pop()
		{
			this._stack.RemoveAt(this.Length - 1);
		}

		internal static bool Equal(string thisname, string thisURN, string name, string URN)
		{
			if (thisURN == null)
			{
				if (URN != null && URN.Length != 0)
				{
					return false;
				}
			}
			else if (thisURN.Length != 0 && thisURN != URN)
			{
				return false;
			}
			return thisname.Length == 0 || !(thisname != name);
		}

		internal void MoveToParent(string name, string URN, int depth)
		{
			if (this._subtree.IsSelfAxis)
			{
				return;
			}
			for (int i = 0; i < this._stack.Count; i++)
			{
				((AxisElement)this._stack[i]).MoveToParent(depth, this._subtree);
			}
			if (this._subtree.IsDss && AxisStack.Equal(this._subtree.RootNode.Name, this._subtree.RootNode.Urn, name, URN))
			{
				this.Pop();
			}
		}

		internal bool MoveToChild(string name, string URN, int depth)
		{
			bool flag = false;
			if (this._subtree.IsDss && AxisStack.Equal(this._subtree.RootNode.Name, this._subtree.RootNode.Urn, name, URN))
			{
				this.Push(-1);
			}
			for (int i = 0; i < this._stack.Count; i++)
			{
				if (((AxisElement)this._stack[i]).MoveToChild(name, URN, depth, this._subtree))
				{
					flag = true;
				}
			}
			return flag;
		}

		internal bool MoveToAttribute(string name, string URN, int depth)
		{
			if (!this._subtree.IsAttribute)
			{
				return false;
			}
			if (!AxisStack.Equal(this._subtree.TopNode.Name, this._subtree.TopNode.Urn, name, URN))
			{
				return false;
			}
			bool flag = false;
			if (this._subtree.TopNode.Input == null)
			{
				return this._subtree.IsDss || depth == 1;
			}
			for (int i = 0; i < this._stack.Count; i++)
			{
				AxisElement axisElement = (AxisElement)this._stack[i];
				if (axisElement.isMatch && axisElement.CurNode == this._subtree.TopNode.Input)
				{
					flag = true;
				}
			}
			return flag;
		}

		private ArrayList _stack;

		private ForwardAxis _subtree;

		private ActiveAxis _parent;
	}
}
