using System;
using System.Collections;
using System.Text;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class RecordBuilder
	{
		internal RecordBuilder(RecordOutput output, XmlNameTable nameTable)
		{
			this.output = output;
			this.nameTable = ((nameTable != null) ? nameTable : new NameTable());
			this.atoms = new OutKeywords(this.nameTable);
			this.scopeManager = new OutputScopeManager(this.nameTable, this.atoms);
		}

		internal int OutputState
		{
			get
			{
				return this.outputState;
			}
			set
			{
				this.outputState = value;
			}
		}

		internal RecordBuilder Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		internal RecordOutput Output
		{
			get
			{
				return this.output;
			}
		}

		internal BuilderInfo MainNode
		{
			get
			{
				return this.mainNode;
			}
		}

		internal ArrayList AttributeList
		{
			get
			{
				return this.attributeList;
			}
		}

		internal int AttributeCount
		{
			get
			{
				return this.attributeCount;
			}
		}

		internal OutputScopeManager Manager
		{
			get
			{
				return this.scopeManager;
			}
		}

		private void ValueAppend(string s, bool disableOutputEscaping)
		{
			this.currentInfo.ValueAppend(s, disableOutputEscaping);
		}

		private bool CanOutput(int state)
		{
			if (this.recordState == 0 || (state & 8192) == 0)
			{
				return true;
			}
			this.recordState = 2;
			this.FinalizeRecord();
			this.SetEmptyFlag(state);
			return this.output.RecordDone(this) == Processor.OutputResult.Continue;
		}

		internal Processor.OutputResult BeginEvent(int state, XPathNodeType nodeType, string prefix, string name, string nspace, bool empty, object htmlProps, bool search)
		{
			if (!this.CanOutput(state))
			{
				return Processor.OutputResult.Overflow;
			}
			this.AdjustDepth(state);
			this.ResetRecord(state);
			this.PopElementScope();
			prefix = ((prefix != null) ? this.nameTable.Add(prefix) : this.atoms.Empty);
			name = ((name != null) ? this.nameTable.Add(name) : this.atoms.Empty);
			nspace = ((nspace != null) ? this.nameTable.Add(nspace) : this.atoms.Empty);
			switch (nodeType)
			{
			case XPathNodeType.Element:
				this.mainNode.htmlProps = htmlProps as HtmlElementProps;
				this.mainNode.search = search;
				this.BeginElement(prefix, name, nspace, empty);
				break;
			case XPathNodeType.Attribute:
				this.BeginAttribute(prefix, name, nspace, htmlProps, search);
				break;
			case XPathNodeType.Namespace:
				this.BeginNamespace(name, nspace);
				break;
			case XPathNodeType.ProcessingInstruction:
				if (!this.BeginProcessingInstruction(prefix, name, nspace))
				{
					return Processor.OutputResult.Error;
				}
				break;
			case XPathNodeType.Comment:
				this.BeginComment();
				break;
			}
			return this.CheckRecordBegin(state);
		}

		internal Processor.OutputResult TextEvent(int state, string text, bool disableOutputEscaping)
		{
			if (!this.CanOutput(state))
			{
				return Processor.OutputResult.Overflow;
			}
			this.AdjustDepth(state);
			this.ResetRecord(state);
			this.PopElementScope();
			if ((state & 8192) != 0)
			{
				this.currentInfo.Depth = this.recordDepth;
				this.currentInfo.NodeType = XmlNodeType.Text;
			}
			this.ValueAppend(text, disableOutputEscaping);
			return this.CheckRecordBegin(state);
		}

		internal Processor.OutputResult EndEvent(int state, XPathNodeType nodeType)
		{
			if (!this.CanOutput(state))
			{
				return Processor.OutputResult.Overflow;
			}
			this.AdjustDepth(state);
			this.PopElementScope();
			this.popScope = (state & 65536) != 0;
			if ((state & 4096) != 0 && this.mainNode.IsEmptyTag)
			{
				return Processor.OutputResult.Continue;
			}
			this.ResetRecord(state);
			if ((state & 8192) != 0 && nodeType == XPathNodeType.Element)
			{
				this.EndElement();
			}
			return this.CheckRecordEnd(state);
		}

		internal void Reset()
		{
			if (this.recordState == 2)
			{
				this.recordState = 0;
			}
		}

		internal void TheEnd()
		{
			if (this.recordState == 1)
			{
				this.recordState = 2;
				this.FinalizeRecord();
				this.output.RecordDone(this);
			}
			this.output.TheEnd();
		}

		private int FindAttribute(string name, string nspace, ref string prefix)
		{
			for (int i = 0; i < this.attributeCount; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)this.attributeList[i];
				if (Ref.Equal(builderInfo.LocalName, name))
				{
					if (Ref.Equal(builderInfo.NamespaceURI, nspace))
					{
						return i;
					}
					if (Ref.Equal(builderInfo.Prefix, prefix))
					{
						prefix = string.Empty;
					}
				}
			}
			return -1;
		}

		private void BeginElement(string prefix, string name, string nspace, bool empty)
		{
			this.currentInfo.NodeType = XmlNodeType.Element;
			this.currentInfo.Prefix = prefix;
			this.currentInfo.LocalName = name;
			this.currentInfo.NamespaceURI = nspace;
			this.currentInfo.Depth = this.recordDepth;
			this.currentInfo.IsEmptyTag = empty;
			this.scopeManager.PushScope(name, nspace, prefix);
		}

		private void EndElement()
		{
			OutputScope currentElementScope = this.scopeManager.CurrentElementScope;
			this.currentInfo.NodeType = XmlNodeType.EndElement;
			this.currentInfo.Prefix = currentElementScope.Prefix;
			this.currentInfo.LocalName = currentElementScope.Name;
			this.currentInfo.NamespaceURI = currentElementScope.Namespace;
			this.currentInfo.Depth = this.recordDepth;
		}

		private int NewAttribute()
		{
			if (this.attributeCount >= this.attributeList.Count)
			{
				this.attributeList.Add(new BuilderInfo());
			}
			int num = this.attributeCount;
			this.attributeCount = num + 1;
			return num;
		}

		private void BeginAttribute(string prefix, string name, string nspace, object htmlAttrProps, bool search)
		{
			int num = this.FindAttribute(name, nspace, ref prefix);
			if (num == -1)
			{
				num = this.NewAttribute();
			}
			BuilderInfo builderInfo = (BuilderInfo)this.attributeList[num];
			builderInfo.Initialize(prefix, name, nspace);
			builderInfo.Depth = this.recordDepth;
			builderInfo.NodeType = XmlNodeType.Attribute;
			builderInfo.htmlAttrProps = htmlAttrProps as HtmlAttributeProps;
			builderInfo.search = search;
			this.currentInfo = builderInfo;
		}

		private void BeginNamespace(string name, string nspace)
		{
			bool flag = false;
			if (Ref.Equal(name, this.atoms.Empty))
			{
				if (!Ref.Equal(nspace, this.scopeManager.DefaultNamespace) && !Ref.Equal(this.mainNode.NamespaceURI, this.atoms.Empty))
				{
					this.DeclareNamespace(nspace, name);
				}
			}
			else
			{
				string text = this.scopeManager.ResolveNamespace(name, out flag);
				if (text != null)
				{
					if (!Ref.Equal(nspace, text) && !flag)
					{
						this.DeclareNamespace(nspace, name);
					}
				}
				else
				{
					this.DeclareNamespace(nspace, name);
				}
			}
			this.currentInfo = this.dummy;
			this.currentInfo.NodeType = XmlNodeType.Attribute;
		}

		private bool BeginProcessingInstruction(string prefix, string name, string nspace)
		{
			this.currentInfo.NodeType = XmlNodeType.ProcessingInstruction;
			this.currentInfo.Prefix = prefix;
			this.currentInfo.LocalName = name;
			this.currentInfo.NamespaceURI = nspace;
			this.currentInfo.Depth = this.recordDepth;
			return true;
		}

		private void BeginComment()
		{
			this.currentInfo.NodeType = XmlNodeType.Comment;
			this.currentInfo.Depth = this.recordDepth;
		}

		private void AdjustDepth(int state)
		{
			int num = state & 768;
			if (num == 256)
			{
				this.recordDepth++;
				return;
			}
			if (num != 512)
			{
				return;
			}
			this.recordDepth--;
		}

		private void ResetRecord(int state)
		{
			if ((state & 8192) != 0)
			{
				this.attributeCount = 0;
				this.namespaceCount = 0;
				this.currentInfo = this.mainNode;
				this.currentInfo.Initialize(this.atoms.Empty, this.atoms.Empty, this.atoms.Empty);
				this.currentInfo.NodeType = XmlNodeType.None;
				this.currentInfo.IsEmptyTag = false;
				this.currentInfo.htmlProps = null;
				this.currentInfo.htmlAttrProps = null;
			}
		}

		private void PopElementScope()
		{
			if (this.popScope)
			{
				this.scopeManager.PopScope();
				this.popScope = false;
			}
		}

		private Processor.OutputResult CheckRecordBegin(int state)
		{
			if ((state & 16384) != 0)
			{
				this.recordState = 2;
				this.FinalizeRecord();
				this.SetEmptyFlag(state);
				return this.output.RecordDone(this);
			}
			this.recordState = 1;
			return Processor.OutputResult.Continue;
		}

		private Processor.OutputResult CheckRecordEnd(int state)
		{
			if ((state & 16384) != 0)
			{
				this.recordState = 2;
				this.FinalizeRecord();
				this.SetEmptyFlag(state);
				return this.output.RecordDone(this);
			}
			return Processor.OutputResult.Continue;
		}

		private void SetEmptyFlag(int state)
		{
			if ((state & 1024) != 0)
			{
				this.mainNode.IsEmptyTag = false;
			}
		}

		private void AnalyzeSpaceLang()
		{
			for (int i = 0; i < this.attributeCount; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)this.attributeList[i];
				if (Ref.Equal(builderInfo.Prefix, this.atoms.Xml))
				{
					OutputScope currentElementScope = this.scopeManager.CurrentElementScope;
					if (Ref.Equal(builderInfo.LocalName, this.atoms.Lang))
					{
						currentElementScope.Lang = builderInfo.Value;
					}
					else if (Ref.Equal(builderInfo.LocalName, this.atoms.Space))
					{
						currentElementScope.Space = RecordBuilder.TranslateXmlSpace(builderInfo.Value);
					}
				}
			}
		}

		private void FixupElement()
		{
			if (Ref.Equal(this.mainNode.NamespaceURI, this.atoms.Empty))
			{
				this.mainNode.Prefix = this.atoms.Empty;
			}
			if (Ref.Equal(this.mainNode.Prefix, this.atoms.Empty))
			{
				if (!Ref.Equal(this.mainNode.NamespaceURI, this.scopeManager.DefaultNamespace))
				{
					this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
				}
			}
			else
			{
				bool flag = false;
				string text = this.scopeManager.ResolveNamespace(this.mainNode.Prefix, out flag);
				if (text != null)
				{
					if (!Ref.Equal(this.mainNode.NamespaceURI, text))
					{
						if (flag)
						{
							this.mainNode.Prefix = this.GetPrefixForNamespace(this.mainNode.NamespaceURI);
						}
						else
						{
							this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
						}
					}
				}
				else
				{
					this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
				}
			}
			this.scopeManager.CurrentElementScope.Prefix = this.mainNode.Prefix;
		}

		private void FixupAttributes(int attributeCount)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)this.attributeList[i];
				if (Ref.Equal(builderInfo.NamespaceURI, this.atoms.Empty))
				{
					builderInfo.Prefix = this.atoms.Empty;
				}
				else if (Ref.Equal(builderInfo.Prefix, this.atoms.Empty))
				{
					builderInfo.Prefix = this.GetPrefixForNamespace(builderInfo.NamespaceURI);
				}
				else
				{
					bool flag = false;
					string text = this.scopeManager.ResolveNamespace(builderInfo.Prefix, out flag);
					if (text != null)
					{
						if (!Ref.Equal(builderInfo.NamespaceURI, text))
						{
							if (flag)
							{
								builderInfo.Prefix = this.GetPrefixForNamespace(builderInfo.NamespaceURI);
							}
							else
							{
								this.DeclareNamespace(builderInfo.NamespaceURI, builderInfo.Prefix);
							}
						}
					}
					else
					{
						this.DeclareNamespace(builderInfo.NamespaceURI, builderInfo.Prefix);
					}
				}
			}
		}

		private void AppendNamespaces()
		{
			for (int i = this.namespaceCount - 1; i >= 0; i--)
			{
				((BuilderInfo)this.attributeList[this.NewAttribute()]).Initialize((BuilderInfo)this.namespaceList[i]);
			}
		}

		private void AnalyzeComment()
		{
			StringBuilder stringBuilder = null;
			string value = this.mainNode.Value;
			bool flag = false;
			int i = 0;
			int num = 0;
			while (i < value.Length)
			{
				if (value[i] == '-')
				{
					if (flag)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(value, num, i, 2 * value.Length);
						}
						else
						{
							stringBuilder.Append(value, num, i - num);
						}
						stringBuilder.Append(" -");
						num = i + 1;
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
				i++;
			}
			if (stringBuilder != null)
			{
				if (num < value.Length)
				{
					stringBuilder.Append(value, num, value.Length - num);
				}
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				this.mainNode.Value = stringBuilder.ToString();
				return;
			}
			if (flag)
			{
				this.mainNode.ValueAppend(" ", false);
			}
		}

		private void AnalyzeProcessingInstruction()
		{
			StringBuilder stringBuilder = null;
			string value = this.mainNode.Value;
			bool flag = false;
			int i = 0;
			int num = 0;
			while (i < value.Length)
			{
				char c = value[i];
				if (c != '>')
				{
					flag = c == '?';
				}
				else
				{
					if (flag)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(value, num, i, 2 * value.Length);
						}
						else
						{
							stringBuilder.Append(value, num, i - num);
						}
						stringBuilder.Append(" >");
						num = i + 1;
					}
					flag = false;
				}
				i++;
			}
			if (stringBuilder != null)
			{
				if (num < value.Length)
				{
					stringBuilder.Append(value, num, value.Length - num);
				}
				this.mainNode.Value = stringBuilder.ToString();
			}
		}

		private void FinalizeRecord()
		{
			XmlNodeType nodeType = this.mainNode.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				int num = this.attributeCount;
				this.FixupElement();
				this.FixupAttributes(num);
				this.AnalyzeSpaceLang();
				this.AppendNamespaces();
				return;
			}
			if (nodeType == XmlNodeType.ProcessingInstruction)
			{
				this.AnalyzeProcessingInstruction();
				return;
			}
			if (nodeType != XmlNodeType.Comment)
			{
				return;
			}
			this.AnalyzeComment();
		}

		private int NewNamespace()
		{
			if (this.namespaceCount >= this.namespaceList.Count)
			{
				this.namespaceList.Add(new BuilderInfo());
			}
			int num = this.namespaceCount;
			this.namespaceCount = num + 1;
			return num;
		}

		private void DeclareNamespace(string nspace, string prefix)
		{
			int num = this.NewNamespace();
			BuilderInfo builderInfo = (BuilderInfo)this.namespaceList[num];
			if (prefix == this.atoms.Empty)
			{
				builderInfo.Initialize(this.atoms.Empty, this.atoms.Xmlns, this.atoms.XmlnsNamespace);
			}
			else
			{
				builderInfo.Initialize(this.atoms.Xmlns, prefix, this.atoms.XmlnsNamespace);
			}
			builderInfo.Depth = this.recordDepth;
			builderInfo.NodeType = XmlNodeType.Attribute;
			builderInfo.Value = nspace;
			this.scopeManager.PushNamespace(prefix, nspace);
		}

		private string DeclareNewNamespace(string nspace)
		{
			string text = this.scopeManager.GeneratePrefix("xp_{0}");
			this.DeclareNamespace(nspace, text);
			return text;
		}

		internal string GetPrefixForNamespace(string nspace)
		{
			string text = null;
			if (this.scopeManager.FindPrefix(nspace, out text))
			{
				return text;
			}
			return this.DeclareNewNamespace(nspace);
		}

		private static XmlSpace TranslateXmlSpace(string space)
		{
			if (space == "default")
			{
				return XmlSpace.Default;
			}
			if (space == "preserve")
			{
				return XmlSpace.Preserve;
			}
			return XmlSpace.None;
		}

		private int outputState;

		private RecordBuilder next;

		private RecordOutput output;

		private XmlNameTable nameTable;

		private OutKeywords atoms;

		private OutputScopeManager scopeManager;

		private BuilderInfo mainNode = new BuilderInfo();

		private ArrayList attributeList = new ArrayList();

		private int attributeCount;

		private ArrayList namespaceList = new ArrayList();

		private int namespaceCount;

		private BuilderInfo dummy = new BuilderInfo();

		private BuilderInfo currentInfo;

		private bool popScope;

		private int recordState;

		private int recordDepth;

		private const int NoRecord = 0;

		private const int SomeRecord = 1;

		private const int HaveRecord = 2;

		private const char s_Minus = '-';

		private const string s_Space = " ";

		private const string s_SpaceMinus = " -";

		private const char s_Question = '?';

		private const char s_Greater = '>';

		private const string s_SpaceGreater = " >";

		private const string PrefixFormat = "xp_{0}";
	}
}
