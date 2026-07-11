using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl.XsltOld.Debugger;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class Processor : IXsltProcessor
	{
		internal XPathNavigator Current
		{
			get
			{
				ActionFrame actionFrame = (ActionFrame)this.actionStack.Peek();
				if (actionFrame == null)
				{
					return null;
				}
				return actionFrame.Node;
			}
		}

		internal Processor.ExecResult ExecutionResult
		{
			get
			{
				return this.execResult;
			}
			set
			{
				this.execResult = value;
			}
		}

		internal Stylesheet Stylesheet
		{
			get
			{
				return this.stylesheet;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		internal ArrayList SortArray
		{
			get
			{
				return this.sortArray;
			}
		}

		internal Key[] KeyList
		{
			get
			{
				return this.keyList;
			}
		}

		internal XPathNavigator GetNavigator(Uri ruri)
		{
			XPathNavigator xpathNavigator;
			if (this.documentCache != null)
			{
				xpathNavigator = this.documentCache[ruri] as XPathNavigator;
				if (xpathNavigator != null)
				{
					return xpathNavigator.Clone();
				}
			}
			else
			{
				this.documentCache = new Hashtable();
			}
			object entity = this.resolver.GetEntity(ruri, null, null);
			if (entity is Stream)
			{
				xpathNavigator = ((IXPathNavigable)Compiler.LoadDocument(new XmlTextReaderImpl(ruri.ToString(), (Stream)entity)
				{
					XmlResolver = this.resolver
				})).CreateNavigator();
			}
			else
			{
				if (!(entity is XPathNavigator))
				{
					throw XsltException.Create("Cannot resolve the referenced document '{0}'.", new string[] { ruri.ToString() });
				}
				xpathNavigator = (XPathNavigator)entity;
			}
			this.documentCache[ruri] = xpathNavigator.Clone();
			return xpathNavigator;
		}

		internal void AddSort(Sort sortinfo)
		{
			this.sortArray.Add(sortinfo);
		}

		internal void InitSortArray()
		{
			if (this.sortArray == null)
			{
				this.sortArray = new ArrayList();
				return;
			}
			this.sortArray.Clear();
		}

		internal object GetGlobalParameter(XmlQualifiedName qname)
		{
			object obj = this.args.GetParam(qname.Name, qname.Namespace);
			if (obj == null)
			{
				return null;
			}
			if (!(obj is XPathNodeIterator) && !(obj is XPathNavigator) && !(obj is bool) && !(obj is double) && !(obj is string))
			{
				if (obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is float || obj is decimal)
				{
					obj = XmlConvert.ToXPathDouble(obj);
				}
				else
				{
					obj = obj.ToString();
				}
			}
			return obj;
		}

		internal object GetExtensionObject(string nsUri)
		{
			return this.args.GetExtensionObject(nsUri);
		}

		internal object GetScriptObject(string nsUri)
		{
			return this.scriptExtensions[nsUri];
		}

		internal RootAction RootAction
		{
			get
			{
				return this.rootAction;
			}
		}

		internal XPathNavigator Document
		{
			get
			{
				return this.document;
			}
		}

		internal StringBuilder GetSharedStringBuilder()
		{
			if (this.sharedStringBuilder == null)
			{
				this.sharedStringBuilder = new StringBuilder();
			}
			else
			{
				this.sharedStringBuilder.Length = 0;
			}
			return this.sharedStringBuilder;
		}

		internal void ReleaseSharedStringBuilder()
		{
		}

		internal ArrayList NumberList
		{
			get
			{
				if (this.numberList == null)
				{
					this.numberList = new ArrayList();
				}
				return this.numberList;
			}
		}

		internal IXsltDebugger Debugger
		{
			get
			{
				return this.debugger;
			}
		}

		internal HWStack ActionStack
		{
			get
			{
				return this.actionStack;
			}
		}

		internal RecordBuilder Builder
		{
			get
			{
				return this.builder;
			}
		}

		internal XsltOutput Output
		{
			get
			{
				return this.output;
			}
		}

		public Processor(XPathNavigator doc, XsltArgumentList args, XmlResolver resolver, Stylesheet stylesheet, List<TheQuery> queryStore, RootAction rootAction, IXsltDebugger debugger)
		{
			this.stylesheet = stylesheet;
			this.queryStore = queryStore;
			this.rootAction = rootAction;
			this.queryList = new Query[queryStore.Count];
			for (int i = 0; i < queryStore.Count; i++)
			{
				this.queryList[i] = Query.Clone(queryStore[i].CompiledQuery.QueryTree);
			}
			this.xsm = new StateMachine();
			this.document = doc;
			this.builder = null;
			this.actionStack = new HWStack(10);
			this.output = this.rootAction.Output;
			this.permissions = this.rootAction.permissions;
			this.resolver = resolver ?? XmlNullResolver.Singleton;
			this.args = args ?? new XsltArgumentList();
			this.debugger = debugger;
			if (this.debugger != null)
			{
				this.debuggerStack = new HWStack(10, 1000);
				this.templateLookup = new TemplateLookupActionDbg();
			}
			if (this.rootAction.KeyList != null)
			{
				this.keyList = new Key[this.rootAction.KeyList.Count];
				for (int j = 0; j < this.keyList.Length; j++)
				{
					this.keyList[j] = this.rootAction.KeyList[j].Clone();
				}
			}
			this.scriptExtensions = new Hashtable(this.stylesheet.ScriptObjectTypes.Count);
			foreach (object obj in this.stylesheet.ScriptObjectTypes)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = (string)dictionaryEntry.Key;
				if (this.GetExtensionObject(text) != null)
				{
					throw XsltException.Create("Namespace '{0}' has a duplicate implementation.", new string[] { text });
				}
				this.scriptExtensions.Add(text, Activator.CreateInstance((Type)dictionaryEntry.Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null));
			}
			this.PushActionFrame(this.rootAction, null);
		}

		public ReaderOutput StartReader()
		{
			ReaderOutput readerOutput = new ReaderOutput(this);
			this.builder = new RecordBuilder(readerOutput, this.nameTable);
			return readerOutput;
		}

		public void Execute(Stream stream)
		{
			RecordOutput recordOutput = null;
			switch (this.output.Method)
			{
			case XsltOutput.OutputMethod.Xml:
			case XsltOutput.OutputMethod.Html:
			case XsltOutput.OutputMethod.Other:
			case XsltOutput.OutputMethod.Unknown:
				recordOutput = new TextOutput(this, stream);
				break;
			case XsltOutput.OutputMethod.Text:
				recordOutput = new TextOnlyOutput(this, stream);
				break;
			}
			this.builder = new RecordBuilder(recordOutput, this.nameTable);
			this.Execute();
		}

		public void Execute(TextWriter writer)
		{
			RecordOutput recordOutput = null;
			switch (this.output.Method)
			{
			case XsltOutput.OutputMethod.Xml:
			case XsltOutput.OutputMethod.Html:
			case XsltOutput.OutputMethod.Other:
			case XsltOutput.OutputMethod.Unknown:
				recordOutput = new TextOutput(this, writer);
				break;
			case XsltOutput.OutputMethod.Text:
				recordOutput = new TextOnlyOutput(this, writer);
				break;
			}
			this.builder = new RecordBuilder(recordOutput, this.nameTable);
			this.Execute();
		}

		public void Execute(XmlWriter writer)
		{
			this.builder = new RecordBuilder(new WriterOutput(this, writer), this.nameTable);
			this.Execute();
		}

		internal void Execute()
		{
			while (this.execResult == Processor.ExecResult.Continue)
			{
				ActionFrame actionFrame = (ActionFrame)this.actionStack.Peek();
				if (actionFrame == null)
				{
					this.builder.TheEnd();
					this.ExecutionResult = Processor.ExecResult.Done;
					break;
				}
				if (actionFrame.Execute(this))
				{
					this.actionStack.Pop();
				}
			}
			if (this.execResult == Processor.ExecResult.Interrupt)
			{
				this.execResult = Processor.ExecResult.Continue;
			}
		}

		internal ActionFrame PushNewFrame()
		{
			ActionFrame actionFrame = (ActionFrame)this.actionStack.Peek();
			ActionFrame actionFrame2 = (ActionFrame)this.actionStack.Push();
			if (actionFrame2 == null)
			{
				actionFrame2 = new ActionFrame();
				this.actionStack.AddToTop(actionFrame2);
			}
			if (actionFrame != null)
			{
				actionFrame2.Inherit(actionFrame);
			}
			return actionFrame2;
		}

		internal void PushActionFrame(Action action, XPathNodeIterator nodeSet)
		{
			this.PushNewFrame().Init(action, nodeSet);
		}

		internal void PushActionFrame(ActionFrame container)
		{
			this.PushActionFrame(container, container.NodeSet);
		}

		internal void PushActionFrame(ActionFrame container, XPathNodeIterator nodeSet)
		{
			this.PushNewFrame().Init(container, nodeSet);
		}

		internal void PushTemplateLookup(XPathNodeIterator nodeSet, XmlQualifiedName mode, Stylesheet importsOf)
		{
			this.templateLookup.Initialize(mode, importsOf);
			this.PushActionFrame(this.templateLookup, nodeSet);
		}

		internal string GetQueryExpression(int key)
		{
			return this.queryStore[key].CompiledQuery.Expression;
		}

		internal Query GetCompiledQuery(int key)
		{
			TheQuery theQuery = this.queryStore[key];
			theQuery.CompiledQuery.CheckErrors();
			Query query = Query.Clone(this.queryList[key]);
			query.SetXsltContext(new XsltCompileContext(theQuery._ScopeManager, this));
			return query;
		}

		internal Query GetValueQuery(int key)
		{
			return this.GetValueQuery(key, null);
		}

		internal Query GetValueQuery(int key, XsltCompileContext context)
		{
			TheQuery theQuery = this.queryStore[key];
			theQuery.CompiledQuery.CheckErrors();
			Query query = this.queryList[key];
			if (context == null)
			{
				context = new XsltCompileContext(theQuery._ScopeManager, this);
			}
			else
			{
				context.Reinitialize(theQuery._ScopeManager, this);
			}
			query.SetXsltContext(context);
			return query;
		}

		private XsltCompileContext GetValueOfContext()
		{
			if (this.valueOfContext == null)
			{
				this.valueOfContext = new XsltCompileContext();
			}
			return this.valueOfContext;
		}

		[Conditional("DEBUG")]
		private void RecycleValueOfContext()
		{
			if (this.valueOfContext != null)
			{
				this.valueOfContext.Recycle();
			}
		}

		private XsltCompileContext GetMatchesContext()
		{
			if (this.matchesContext == null)
			{
				this.matchesContext = new XsltCompileContext();
			}
			return this.matchesContext;
		}

		[Conditional("DEBUG")]
		private void RecycleMatchesContext()
		{
			if (this.matchesContext != null)
			{
				this.matchesContext.Recycle();
			}
		}

		internal string ValueOf(ActionFrame context, int key)
		{
			Query valueQuery = this.GetValueQuery(key, this.GetValueOfContext());
			object obj = valueQuery.Evaluate(context.NodeSet);
			string text;
			if (obj is XPathNodeIterator)
			{
				XPathNavigator xpathNavigator = valueQuery.Advance();
				text = ((xpathNavigator != null) ? this.ValueOf(xpathNavigator) : string.Empty);
			}
			else
			{
				text = XmlConvert.ToXPathString(obj);
			}
			return text;
		}

		internal string ValueOf(XPathNavigator n)
		{
			if (this.stylesheet.Whitespace && n.NodeType == XPathNodeType.Element)
			{
				StringBuilder stringBuilder = this.GetSharedStringBuilder();
				this.ElementValueWithoutWS(n, stringBuilder);
				this.ReleaseSharedStringBuilder();
				return stringBuilder.ToString();
			}
			return n.Value;
		}

		private void ElementValueWithoutWS(XPathNavigator nav, StringBuilder builder)
		{
			bool flag = this.Stylesheet.PreserveWhiteSpace(this, nav);
			if (nav.MoveToFirstChild())
			{
				do
				{
					switch (nav.NodeType)
					{
					case XPathNodeType.Element:
						this.ElementValueWithoutWS(nav, builder);
						break;
					case XPathNodeType.Text:
					case XPathNodeType.SignificantWhitespace:
						builder.Append(nav.Value);
						break;
					case XPathNodeType.Whitespace:
						if (flag)
						{
							builder.Append(nav.Value);
						}
						break;
					}
				}
				while (nav.MoveToNext());
				nav.MoveToParent();
			}
		}

		internal XPathNodeIterator StartQuery(XPathNodeIterator context, int key)
		{
			Query compiledQuery = this.GetCompiledQuery(key);
			if (compiledQuery.Evaluate(context) is XPathNodeIterator)
			{
				return new XPathSelectionIterator(context.Current, compiledQuery);
			}
			throw XsltException.Create("Expression must evaluate to a node-set.", Array.Empty<string>());
		}

		internal object Evaluate(ActionFrame context, int key)
		{
			return this.GetValueQuery(key).Evaluate(context.NodeSet);
		}

		internal object RunQuery(ActionFrame context, int key)
		{
			object obj = this.GetCompiledQuery(key).Evaluate(context.NodeSet);
			XPathNodeIterator xpathNodeIterator = obj as XPathNodeIterator;
			if (xpathNodeIterator != null)
			{
				return new XPathArrayIterator(xpathNodeIterator);
			}
			return obj;
		}

		internal string EvaluateString(ActionFrame context, int key)
		{
			object obj = this.Evaluate(context, key);
			string text = null;
			if (obj != null)
			{
				text = XmlConvert.ToXPathString(obj);
			}
			if (text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		internal bool EvaluateBoolean(ActionFrame context, int key)
		{
			object obj = this.Evaluate(context, key);
			if (obj == null)
			{
				return false;
			}
			XPathNavigator xpathNavigator = obj as XPathNavigator;
			if (xpathNavigator == null)
			{
				return Convert.ToBoolean(obj, CultureInfo.InvariantCulture);
			}
			return Convert.ToBoolean(xpathNavigator.Value, CultureInfo.InvariantCulture);
		}

		internal bool Matches(XPathNavigator context, int key)
		{
			Query valueQuery = this.GetValueQuery(key, this.GetMatchesContext());
			bool flag;
			try
			{
				flag = valueQuery.MatchNode(context) != null;
			}
			catch (XPathException)
			{
				throw XsltException.Create("'{0}' is an invalid XSLT pattern.", new string[] { this.GetQueryExpression(key) });
			}
			return flag;
		}

		internal XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		internal bool CanContinue
		{
			get
			{
				return this.execResult == Processor.ExecResult.Continue;
			}
		}

		internal bool ExecutionDone
		{
			get
			{
				return this.execResult == Processor.ExecResult.Done;
			}
		}

		internal void ResetOutput()
		{
			this.builder.Reset();
		}

		internal bool BeginEvent(XPathNodeType nodeType, string prefix, string name, string nspace, bool empty)
		{
			return this.BeginEvent(nodeType, prefix, name, nspace, empty, null, true);
		}

		internal bool BeginEvent(XPathNodeType nodeType, string prefix, string name, string nspace, bool empty, object htmlProps, bool search)
		{
			int num = this.xsm.BeginOutlook(nodeType);
			if (this.ignoreLevel > 0 || num == 16)
			{
				this.ignoreLevel++;
				return true;
			}
			switch (this.builder.BeginEvent(num, nodeType, prefix, name, nspace, empty, htmlProps, search))
			{
			case Processor.OutputResult.Continue:
				this.xsm.Begin(nodeType);
				return true;
			case Processor.OutputResult.Interrupt:
				this.xsm.Begin(nodeType);
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return true;
			case Processor.OutputResult.Overflow:
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return false;
			case Processor.OutputResult.Error:
				this.ignoreLevel++;
				return true;
			case Processor.OutputResult.Ignore:
				return true;
			default:
				return true;
			}
		}

		internal bool TextEvent(string text)
		{
			return this.TextEvent(text, false);
		}

		internal bool TextEvent(string text, bool disableOutputEscaping)
		{
			if (this.ignoreLevel > 0)
			{
				return true;
			}
			int num = this.xsm.BeginOutlook(XPathNodeType.Text);
			switch (this.builder.TextEvent(num, text, disableOutputEscaping))
			{
			case Processor.OutputResult.Continue:
				this.xsm.Begin(XPathNodeType.Text);
				return true;
			case Processor.OutputResult.Interrupt:
				this.xsm.Begin(XPathNodeType.Text);
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return true;
			case Processor.OutputResult.Overflow:
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return false;
			case Processor.OutputResult.Error:
			case Processor.OutputResult.Ignore:
				return true;
			default:
				return true;
			}
		}

		internal bool EndEvent(XPathNodeType nodeType)
		{
			if (this.ignoreLevel > 0)
			{
				this.ignoreLevel--;
				return true;
			}
			int num = this.xsm.EndOutlook(nodeType);
			switch (this.builder.EndEvent(num, nodeType))
			{
			case Processor.OutputResult.Continue:
				this.xsm.End(nodeType);
				return true;
			case Processor.OutputResult.Interrupt:
				this.xsm.End(nodeType);
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return true;
			case Processor.OutputResult.Overflow:
				this.ExecutionResult = Processor.ExecResult.Interrupt;
				return false;
			}
			return true;
		}

		internal bool CopyBeginEvent(XPathNavigator node, bool emptyflag)
		{
			switch (node.NodeType)
			{
			case XPathNodeType.Element:
			case XPathNodeType.Attribute:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				return this.BeginEvent(node.NodeType, node.Prefix, node.LocalName, node.NamespaceURI, emptyflag);
			case XPathNodeType.Namespace:
				return this.BeginEvent(XPathNodeType.Namespace, null, node.LocalName, node.Value, false);
			}
			return true;
		}

		internal bool CopyTextEvent(XPathNavigator node)
		{
			switch (node.NodeType)
			{
			case XPathNodeType.Attribute:
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
			{
				string value = node.Value;
				return this.TextEvent(value);
			}
			}
			return true;
		}

		internal bool CopyEndEvent(XPathNavigator node)
		{
			switch (node.NodeType)
			{
			case XPathNodeType.Element:
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				return this.EndEvent(node.NodeType);
			}
			return true;
		}

		internal static bool IsRoot(XPathNavigator navigator)
		{
			if (navigator.NodeType == XPathNodeType.Root)
			{
				return true;
			}
			if (navigator.NodeType == XPathNodeType.Element)
			{
				XPathNavigator xpathNavigator = navigator.Clone();
				xpathNavigator.MoveToRoot();
				return xpathNavigator.IsSamePosition(navigator);
			}
			return false;
		}

		internal void PushOutput(RecordOutput output)
		{
			this.builder.OutputState = this.xsm.State;
			RecordBuilder recordBuilder = this.builder;
			this.builder = new RecordBuilder(output, this.nameTable);
			this.builder.Next = recordBuilder;
			this.xsm.Reset();
		}

		internal RecordOutput PopOutput()
		{
			RecordBuilder recordBuilder = this.builder;
			this.builder = recordBuilder.Next;
			this.xsm.State = this.builder.OutputState;
			recordBuilder.TheEnd();
			return recordBuilder.Output;
		}

		internal bool SetDefaultOutput(XsltOutput.OutputMethod method)
		{
			if (this.Output.Method != method)
			{
				this.output = this.output.CreateDerivedOutput(method);
				return true;
			}
			return false;
		}

		internal object GetVariableValue(VariableAction variable)
		{
			int varKey = variable.VarKey;
			if (!variable.IsGlobal)
			{
				return ((ActionFrame)this.actionStack.Peek()).GetVariable(varKey);
			}
			ActionFrame actionFrame = (ActionFrame)this.actionStack[0];
			object variable2 = actionFrame.GetVariable(varKey);
			if (variable2 == VariableAction.BeingComputedMark)
			{
				throw XsltException.Create("Circular reference in the definition of variable '{0}'.", new string[] { variable.NameStr });
			}
			if (variable2 != null)
			{
				return variable2;
			}
			int length = this.actionStack.Length;
			ActionFrame actionFrame2 = this.PushNewFrame();
			actionFrame2.Inherit(actionFrame);
			actionFrame2.Init(variable, actionFrame.NodeSet);
			do
			{
				if (((ActionFrame)this.actionStack.Peek()).Execute(this))
				{
					this.actionStack.Pop();
				}
			}
			while (length < this.actionStack.Length);
			return actionFrame.GetVariable(varKey);
		}

		internal void SetParameter(XmlQualifiedName name, object value)
		{
			((ActionFrame)this.actionStack[this.actionStack.Length - 2]).SetParameter(name, value);
		}

		internal void ResetParams()
		{
			((ActionFrame)this.actionStack[this.actionStack.Length - 1]).ResetParams();
		}

		internal object GetParameter(XmlQualifiedName name)
		{
			return ((ActionFrame)this.actionStack[this.actionStack.Length - 3]).GetParameter(name);
		}

		internal void PushDebuggerStack()
		{
			Processor.DebuggerFrame debuggerFrame = (Processor.DebuggerFrame)this.debuggerStack.Push();
			if (debuggerFrame == null)
			{
				debuggerFrame = new Processor.DebuggerFrame();
				this.debuggerStack.AddToTop(debuggerFrame);
			}
			debuggerFrame.actionFrame = (ActionFrame)this.actionStack.Peek();
		}

		internal void PopDebuggerStack()
		{
			this.debuggerStack.Pop();
		}

		internal void OnInstructionExecute()
		{
			((Processor.DebuggerFrame)this.debuggerStack.Peek()).actionFrame = (ActionFrame)this.actionStack.Peek();
			this.Debugger.OnInstructionExecute(this);
		}

		internal XmlQualifiedName GetPrevioseMode()
		{
			return ((Processor.DebuggerFrame)this.debuggerStack[this.debuggerStack.Length - 2]).currentMode;
		}

		internal void SetCurrentMode(XmlQualifiedName mode)
		{
			((Processor.DebuggerFrame)this.debuggerStack[this.debuggerStack.Length - 1]).currentMode = mode;
		}

		int IXsltProcessor.StackDepth
		{
			get
			{
				return this.debuggerStack.Length;
			}
		}

		IStackFrame IXsltProcessor.GetStackFrame(int depth)
		{
			return ((Processor.DebuggerFrame)this.debuggerStack[depth]).actionFrame;
		}

		private const int StackIncrement = 10;

		private Processor.ExecResult execResult;

		private Stylesheet stylesheet;

		private RootAction rootAction;

		private Key[] keyList;

		private List<TheQuery> queryStore;

		public PermissionSet permissions;

		private XPathNavigator document;

		private HWStack actionStack;

		private HWStack debuggerStack;

		private StringBuilder sharedStringBuilder;

		private int ignoreLevel;

		private StateMachine xsm;

		private RecordBuilder builder;

		private XsltOutput output;

		private XmlNameTable nameTable = new NameTable();

		private XmlResolver resolver;

		private XsltArgumentList args;

		private Hashtable scriptExtensions;

		private ArrayList numberList;

		private TemplateLookupAction templateLookup = new TemplateLookupAction();

		private IXsltDebugger debugger;

		private Query[] queryList;

		private ArrayList sortArray;

		private Hashtable documentCache;

		private XsltCompileContext valueOfContext;

		private XsltCompileContext matchesContext;

		internal enum ExecResult
		{
			Continue,
			Interrupt,
			Done
		}

		internal enum OutputResult
		{
			Continue,
			Interrupt,
			Overflow,
			Error,
			Ignore
		}

		internal class DebuggerFrame
		{
			internal ActionFrame actionFrame;

			internal XmlQualifiedName currentMode;
		}
	}
}
