using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml.Xsl.XsltOld.Debugger;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class ActionFrame : IStackFrame
	{
		internal PrefixQName CalulatedName
		{
			get
			{
				return this.calulatedName;
			}
			set
			{
				this.calulatedName = value;
			}
		}

		internal string StoredOutput
		{
			get
			{
				return this.storedOutput;
			}
			set
			{
				this.storedOutput = value;
			}
		}

		internal int State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		internal int Counter
		{
			get
			{
				return this.counter;
			}
			set
			{
				this.counter = value;
			}
		}

		internal ActionFrame Container
		{
			get
			{
				return this.container;
			}
		}

		internal XPathNavigator Node
		{
			get
			{
				if (this.nodeSet != null)
				{
					return this.nodeSet.Current;
				}
				return null;
			}
		}

		internal XPathNodeIterator NodeSet
		{
			get
			{
				return this.nodeSet;
			}
		}

		internal XPathNodeIterator NewNodeSet
		{
			get
			{
				return this.newNodeSet;
			}
		}

		internal int IncrementCounter()
		{
			int num = this.counter + 1;
			this.counter = num;
			return num;
		}

		internal void AllocateVariables(int count)
		{
			if (0 < count)
			{
				this.variables = new object[count];
				return;
			}
			this.variables = null;
		}

		internal object GetVariable(int index)
		{
			return this.variables[index];
		}

		internal void SetVariable(int index, object value)
		{
			this.variables[index] = value;
		}

		internal void SetParameter(XmlQualifiedName name, object value)
		{
			if (this.withParams == null)
			{
				this.withParams = new Hashtable();
			}
			this.withParams[name] = value;
		}

		internal void ResetParams()
		{
			if (this.withParams != null)
			{
				this.withParams.Clear();
			}
		}

		internal object GetParameter(XmlQualifiedName name)
		{
			if (this.withParams != null)
			{
				return this.withParams[name];
			}
			return null;
		}

		internal void InitNodeSet(XPathNodeIterator nodeSet)
		{
			this.nodeSet = nodeSet;
		}

		internal void InitNewNodeSet(XPathNodeIterator nodeSet)
		{
			this.newNodeSet = nodeSet;
		}

		internal void SortNewNodeSet(Processor proc, ArrayList sortarray)
		{
			int count = sortarray.Count;
			XPathSortComparer xpathSortComparer = new XPathSortComparer(count);
			for (int i = 0; i < count; i++)
			{
				Sort sort = (Sort)sortarray[i];
				Query compiledQuery = proc.GetCompiledQuery(sort.select);
				xpathSortComparer.AddSort(compiledQuery, new XPathComparerHelper(sort.order, sort.caseOrder, sort.lang, sort.dataType));
			}
			List<SortKey> list = new List<SortKey>();
			while (this.NewNextNode(proc))
			{
				XPathNodeIterator xpathNodeIterator = this.nodeSet;
				this.nodeSet = this.newNodeSet;
				SortKey sortKey = new SortKey(count, list.Count, this.newNodeSet.Current.Clone());
				for (int j = 0; j < count; j++)
				{
					sortKey[j] = xpathSortComparer.Expression(j).Evaluate(this.newNodeSet);
				}
				list.Add(sortKey);
				this.nodeSet = xpathNodeIterator;
			}
			list.Sort(xpathSortComparer);
			this.newNodeSet = new ActionFrame.XPathSortArrayIterator(list);
		}

		internal void Finished()
		{
			this.State = -1;
		}

		internal void Inherit(ActionFrame parent)
		{
			this.variables = parent.variables;
		}

		private void Init(Action action, ActionFrame container, XPathNodeIterator nodeSet)
		{
			this.state = 0;
			this.action = action;
			this.container = container;
			this.currentAction = 0;
			this.nodeSet = nodeSet;
			this.newNodeSet = null;
		}

		internal void Init(Action action, XPathNodeIterator nodeSet)
		{
			this.Init(action, null, nodeSet);
		}

		internal void Init(ActionFrame containerFrame, XPathNodeIterator nodeSet)
		{
			this.Init(containerFrame.GetAction(0), containerFrame, nodeSet);
		}

		internal void SetAction(Action action)
		{
			this.SetAction(action, 0);
		}

		internal void SetAction(Action action, int state)
		{
			this.action = action;
			this.state = state;
		}

		private Action GetAction(int actionIndex)
		{
			return ((ContainerAction)this.action).GetAction(actionIndex);
		}

		internal void Exit()
		{
			this.Finished();
			this.container = null;
		}

		internal bool Execute(Processor processor)
		{
			if (this.action == null)
			{
				return true;
			}
			this.action.Execute(processor, this);
			if (this.State == -1)
			{
				if (this.container != null)
				{
					this.currentAction++;
					this.action = this.container.GetAction(this.currentAction);
					this.State = 0;
				}
				else
				{
					this.action = null;
				}
				return this.action == null;
			}
			return false;
		}

		internal bool NextNode(Processor proc)
		{
			bool flag = this.nodeSet.MoveNext();
			if (flag && proc.Stylesheet.Whitespace)
			{
				XPathNodeType xpathNodeType = this.nodeSet.Current.NodeType;
				if (xpathNodeType == XPathNodeType.Whitespace)
				{
					XPathNavigator xpathNavigator = this.nodeSet.Current.Clone();
					bool flag2;
					do
					{
						xpathNavigator.MoveTo(this.nodeSet.Current);
						xpathNavigator.MoveToParent();
						flag2 = !proc.Stylesheet.PreserveWhiteSpace(proc, xpathNavigator) && (flag = this.nodeSet.MoveNext());
						xpathNodeType = this.nodeSet.Current.NodeType;
					}
					while (flag2 && xpathNodeType == XPathNodeType.Whitespace);
				}
			}
			return flag;
		}

		internal bool NewNextNode(Processor proc)
		{
			bool flag = this.newNodeSet.MoveNext();
			if (flag && proc.Stylesheet.Whitespace)
			{
				XPathNodeType xpathNodeType = this.newNodeSet.Current.NodeType;
				if (xpathNodeType == XPathNodeType.Whitespace)
				{
					XPathNavigator xpathNavigator = this.newNodeSet.Current.Clone();
					bool flag2;
					do
					{
						xpathNavigator.MoveTo(this.newNodeSet.Current);
						xpathNavigator.MoveToParent();
						flag2 = !proc.Stylesheet.PreserveWhiteSpace(proc, xpathNavigator) && (flag = this.newNodeSet.MoveNext());
						xpathNodeType = this.newNodeSet.Current.NodeType;
					}
					while (flag2 && xpathNodeType == XPathNodeType.Whitespace);
				}
			}
			return flag;
		}

		XPathNavigator IStackFrame.Instruction
		{
			get
			{
				if (this.action == null)
				{
					return null;
				}
				return this.action.GetDbgData(this).StyleSheet;
			}
		}

		XPathNodeIterator IStackFrame.NodeSet
		{
			get
			{
				return this.nodeSet.Clone();
			}
		}

		int IStackFrame.GetVariablesCount()
		{
			if (this.action == null)
			{
				return 0;
			}
			return this.action.GetDbgData(this).Variables.Length;
		}

		XPathNavigator IStackFrame.GetVariable(int varIndex)
		{
			return this.action.GetDbgData(this).Variables[varIndex].GetDbgData(null).StyleSheet;
		}

		object IStackFrame.GetVariableValue(int varIndex)
		{
			return this.GetVariable(this.action.GetDbgData(this).Variables[varIndex].VarKey);
		}

		private int state;

		private int counter;

		private object[] variables;

		private Hashtable withParams;

		private Action action;

		private ActionFrame container;

		private int currentAction;

		private XPathNodeIterator nodeSet;

		private XPathNodeIterator newNodeSet;

		private PrefixQName calulatedName;

		private string storedOutput;

		private class XPathSortArrayIterator : XPathArrayIterator
		{
			public XPathSortArrayIterator(List<SortKey> list)
				: base(list)
			{
			}

			public XPathSortArrayIterator(ActionFrame.XPathSortArrayIterator it)
				: base(it)
			{
			}

			public override XPathNodeIterator Clone()
			{
				return new ActionFrame.XPathSortArrayIterator(this);
			}

			public override XPathNavigator Current
			{
				get
				{
					return ((SortKey)this.list[this.index - 1]).Node;
				}
			}
		}
	}
}
