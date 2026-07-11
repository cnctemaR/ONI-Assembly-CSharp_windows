using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlQueryOutput : XmlWriter
	{
		internal XmlQueryOutput(XmlQueryRuntime runtime, XmlSequenceWriter seqwrt)
		{
			this.runtime = runtime;
			this.seqwrt = seqwrt;
			this.xstate = XmlState.WithinSequence;
		}

		internal XmlQueryOutput(XmlQueryRuntime runtime, XmlEventCache xwrt)
		{
			this.runtime = runtime;
			this.xwrt = xwrt;
			this.xstate = XmlState.WithinContent;
			this.depth = 1;
			this.rootType = XPathNodeType.Root;
		}

		internal XmlSequenceWriter SequenceWriter
		{
			get
			{
				return this.seqwrt;
			}
		}

		internal XmlRawWriter Writer
		{
			get
			{
				return this.xwrt;
			}
			set
			{
				IRemovableWriter removableWriter = value as IRemovableWriter;
				if (removableWriter != null)
				{
					removableWriter.OnRemoveWriterEvent = new OnRemoveWriter(this.SetWrappedWriter);
				}
				this.xwrt = value;
			}
		}

		private void SetWrappedWriter(XmlRawWriter writer)
		{
			if (this.Writer is XmlAttributeCache)
			{
				this.attrCache = (XmlAttributeCache)this.Writer;
			}
			this.Writer = writer;
		}

		public override void WriteStartDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteStartDocument(bool standalone)
		{
			throw new NotSupportedException();
		}

		public override void WriteEndDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new NotSupportedException();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.ConstructWithinContent(XPathNodeType.Element);
			this.WriteStartElementUnchecked(prefix, localName, ns);
			this.WriteNamespaceDeclarationUnchecked(prefix, ns);
			if (this.attrCache == null)
			{
				this.attrCache = new XmlAttributeCache();
			}
			this.attrCache.Init(this.Writer);
			this.Writer = this.attrCache;
			this.attrCache = null;
			this.PushElementNames(prefix, localName, ns);
		}

		public override void WriteEndElement()
		{
			if (this.xstate == XmlState.EnumAttrs)
			{
				this.StartElementContentUnchecked();
			}
			string text;
			string text2;
			string text3;
			this.PopElementNames(out text, out text2, out text3);
			this.WriteEndElementUnchecked(text, text2, text3);
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElement();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (prefix.Length == 5 && prefix == "xmlns")
			{
				this.WriteStartNamespace(localName);
				return;
			}
			this.ConstructInEnumAttrs(XPathNodeType.Attribute);
			if (ns.Length != 0 && this.depth != 0)
			{
				prefix = this.CheckAttributePrefix(prefix, ns);
			}
			this.WriteStartAttributeUnchecked(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			if (this.xstate == XmlState.WithinNmsp)
			{
				this.WriteEndNamespace();
				return;
			}
			this.WriteEndAttributeUnchecked();
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		public override void WriteComment(string text)
		{
			this.WriteStartComment();
			this.WriteCommentString(text);
			this.WriteEndComment();
		}

		public override void WriteProcessingInstruction(string target, string text)
		{
			this.WriteStartProcessingInstruction(target);
			this.WriteProcessingInstructionString(text);
			this.WriteEndProcessingInstruction();
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotSupportedException();
		}

		public override void WriteCharEntity(char ch)
		{
			throw new NotSupportedException();
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			throw new NotSupportedException();
		}

		public override void WriteWhitespace(string ws)
		{
			throw new NotSupportedException();
		}

		public override void WriteString(string text)
		{
			this.WriteString(text, false);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data, true);
		}

		public override void WriteCData(string text)
		{
			this.WriteString(text, false);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override WriteState WriteState
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override void Close()
		{
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			throw new NotSupportedException();
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override string XmlLang
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public void StartTree(XPathNodeType rootType)
		{
			this.Writer = this.seqwrt.StartTree(rootType, this.nsmgr, this.runtime.NameTable);
			this.rootType = rootType;
			this.xstate = ((rootType == XPathNodeType.Attribute || rootType == XPathNodeType.Namespace) ? XmlState.EnumAttrs : XmlState.WithinContent);
		}

		public void EndTree()
		{
			this.seqwrt.EndTree();
			this.xstate = XmlState.WithinSequence;
			this.Writer = null;
		}

		public void WriteStartElementUnchecked(string prefix, string localName, string ns)
		{
			if (this.nsmgr != null)
			{
				this.nsmgr.PushScope();
			}
			this.Writer.WriteStartElement(prefix, localName, ns);
			this.usedPrefixes.Clear();
			this.usedPrefixes[prefix] = ns;
			this.xstate = XmlState.EnumAttrs;
			this.depth++;
		}

		public void WriteStartElementUnchecked(string localName)
		{
			this.WriteStartElementUnchecked(string.Empty, localName, string.Empty);
		}

		public void StartElementContentUnchecked()
		{
			if (this.cntNmsp != 0)
			{
				this.WriteCachedNamespaces();
			}
			this.Writer.StartElementContent();
			this.xstate = XmlState.WithinContent;
		}

		public void WriteEndElementUnchecked(string prefix, string localName, string ns)
		{
			this.Writer.WriteEndElement(prefix, localName, ns);
			this.xstate = XmlState.WithinContent;
			this.depth--;
			if (this.nsmgr != null)
			{
				this.nsmgr.PopScope();
			}
		}

		public void WriteEndElementUnchecked(string localName)
		{
			this.WriteEndElementUnchecked(string.Empty, localName, string.Empty);
		}

		public void WriteStartAttributeUnchecked(string prefix, string localName, string ns)
		{
			this.Writer.WriteStartAttribute(prefix, localName, ns);
			this.xstate = XmlState.WithinAttr;
			this.depth++;
		}

		public void WriteStartAttributeUnchecked(string localName)
		{
			this.WriteStartAttributeUnchecked(string.Empty, localName, string.Empty);
		}

		public void WriteEndAttributeUnchecked()
		{
			this.Writer.WriteEndAttribute();
			this.xstate = XmlState.EnumAttrs;
			this.depth--;
		}

		public void WriteNamespaceDeclarationUnchecked(string prefix, string ns)
		{
			if (this.depth == 0)
			{
				this.Writer.WriteNamespaceDeclaration(prefix, ns);
				return;
			}
			if (this.nsmgr == null)
			{
				if (ns.Length == 0 && prefix.Length == 0)
				{
					return;
				}
				this.nsmgr = new XmlNamespaceManager(this.runtime.NameTable);
				this.nsmgr.PushScope();
			}
			if (this.nsmgr.LookupNamespace(prefix) != ns)
			{
				this.AddNamespace(prefix, ns);
			}
			this.usedPrefixes[prefix] = ns;
		}

		public void WriteStringUnchecked(string text)
		{
			this.Writer.WriteString(text);
		}

		public void WriteRawUnchecked(string text)
		{
			this.Writer.WriteRaw(text);
		}

		public void WriteStartRoot()
		{
			if (this.xstate != XmlState.WithinSequence)
			{
				this.ThrowInvalidStateError(XPathNodeType.Root);
			}
			this.StartTree(XPathNodeType.Root);
			this.depth++;
		}

		public void WriteEndRoot()
		{
			this.depth--;
			this.EndTree();
		}

		public void WriteStartElementLocalName(string localName)
		{
			this.WriteStartElement(string.Empty, localName, string.Empty);
		}

		public void WriteStartAttributeLocalName(string localName)
		{
			this.WriteStartAttribute(string.Empty, localName, string.Empty);
		}

		public void WriteStartElementComputed(string tagName, int prefixMappingsIndex)
		{
			this.WriteStartComputed(XPathNodeType.Element, tagName, prefixMappingsIndex);
		}

		public void WriteStartElementComputed(string tagName, string ns)
		{
			this.WriteStartComputed(XPathNodeType.Element, tagName, ns);
		}

		public void WriteStartElementComputed(XPathNavigator navigator)
		{
			this.WriteStartComputed(XPathNodeType.Element, navigator);
		}

		public void WriteStartElementComputed(XmlQualifiedName name)
		{
			this.WriteStartComputed(XPathNodeType.Element, name);
		}

		public void WriteStartAttributeComputed(string tagName, int prefixMappingsIndex)
		{
			this.WriteStartComputed(XPathNodeType.Attribute, tagName, prefixMappingsIndex);
		}

		public void WriteStartAttributeComputed(string tagName, string ns)
		{
			this.WriteStartComputed(XPathNodeType.Attribute, tagName, ns);
		}

		public void WriteStartAttributeComputed(XPathNavigator navigator)
		{
			this.WriteStartComputed(XPathNodeType.Attribute, navigator);
		}

		public void WriteStartAttributeComputed(XmlQualifiedName name)
		{
			this.WriteStartComputed(XPathNodeType.Attribute, name);
		}

		public void WriteNamespaceDeclaration(string prefix, string ns)
		{
			this.ConstructInEnumAttrs(XPathNodeType.Namespace);
			if (this.nsmgr == null)
			{
				this.WriteNamespaceDeclarationUnchecked(prefix, ns);
			}
			else
			{
				string text = this.nsmgr.LookupNamespace(prefix);
				if (ns != text)
				{
					if (text != null && this.usedPrefixes.ContainsKey(prefix))
					{
						throw new XslTransformException("Cannot construct namespace declaration xmlns{0}{1}='{2}'. Prefix '{1}' is already mapped to namespace '{3}'.", new string[]
						{
							(prefix.Length == 0) ? "" : ":",
							prefix,
							ns,
							text
						});
					}
					this.AddNamespace(prefix, ns);
				}
			}
			if (this.depth == 0)
			{
				this.EndTree();
			}
			this.usedPrefixes[prefix] = ns;
		}

		public void WriteStartNamespace(string prefix)
		{
			this.ConstructInEnumAttrs(XPathNodeType.Namespace);
			this.piTarget = prefix;
			this.nodeText.Clear();
			this.xstate = XmlState.WithinNmsp;
			this.depth++;
		}

		public void WriteNamespaceString(string text)
		{
			this.nodeText.ConcatNoDelimiter(text);
		}

		public void WriteEndNamespace()
		{
			this.xstate = XmlState.EnumAttrs;
			this.depth--;
			this.WriteNamespaceDeclaration(this.piTarget, this.nodeText.GetResult());
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		public void WriteStartComment()
		{
			this.ConstructWithinContent(XPathNodeType.Comment);
			this.nodeText.Clear();
			this.xstate = XmlState.WithinComment;
			this.depth++;
		}

		public void WriteCommentString(string text)
		{
			this.nodeText.ConcatNoDelimiter(text);
		}

		public void WriteEndComment()
		{
			this.Writer.WriteComment(this.nodeText.GetResult());
			this.xstate = XmlState.WithinContent;
			this.depth--;
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		public void WriteStartProcessingInstruction(string target)
		{
			this.ConstructWithinContent(XPathNodeType.ProcessingInstruction);
			ValidateNames.ValidateNameThrow("", target, "", XPathNodeType.ProcessingInstruction, ValidateNames.Flags.AllExceptPrefixMapping);
			this.piTarget = target;
			this.nodeText.Clear();
			this.xstate = XmlState.WithinPI;
			this.depth++;
		}

		public void WriteProcessingInstructionString(string text)
		{
			this.nodeText.ConcatNoDelimiter(text);
		}

		public void WriteEndProcessingInstruction()
		{
			this.Writer.WriteProcessingInstruction(this.piTarget, this.nodeText.GetResult());
			this.xstate = XmlState.WithinContent;
			this.depth--;
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		public void WriteItem(XPathItem item)
		{
			if (!item.IsNode)
			{
				this.seqwrt.WriteItem(item);
				return;
			}
			XPathNavigator xpathNavigator = (XPathNavigator)item;
			if (this.xstate == XmlState.WithinSequence)
			{
				this.seqwrt.WriteItem(xpathNavigator);
				return;
			}
			this.CopyNode(xpathNavigator);
		}

		public void XsltCopyOf(XPathNavigator navigator)
		{
			RtfNavigator rtfNavigator = navigator as RtfNavigator;
			if (rtfNavigator != null)
			{
				rtfNavigator.CopyToWriter(this);
				return;
			}
			if (navigator.NodeType == XPathNodeType.Root)
			{
				if (navigator.MoveToFirstChild())
				{
					do
					{
						this.CopyNode(navigator);
					}
					while (navigator.MoveToNext());
					navigator.MoveToParent();
					return;
				}
			}
			else
			{
				this.CopyNode(navigator);
			}
		}

		public bool StartCopy(XPathNavigator navigator)
		{
			if (navigator.NodeType == XPathNodeType.Root)
			{
				return true;
			}
			if (this.StartCopy(navigator, true))
			{
				this.CopyNamespaces(navigator, XPathNamespaceScope.ExcludeXml);
				return true;
			}
			return false;
		}

		public void EndCopy(XPathNavigator navigator)
		{
			if (navigator.NodeType == XPathNodeType.Element)
			{
				this.WriteEndElement();
			}
		}

		private void AddNamespace(string prefix, string ns)
		{
			this.nsmgr.AddNamespace(prefix, ns);
			this.cntNmsp++;
			this.usedPrefixes[prefix] = ns;
		}

		private void WriteString(string text, bool disableOutputEscaping)
		{
			switch (this.xstate)
			{
			case XmlState.WithinSequence:
				this.StartTree(XPathNodeType.Text);
				break;
			case XmlState.EnumAttrs:
				this.StartElementContentUnchecked();
				break;
			case XmlState.WithinContent:
				break;
			case XmlState.WithinAttr:
				this.WriteStringUnchecked(text);
				goto IL_0071;
			case XmlState.WithinNmsp:
				this.WriteNamespaceString(text);
				goto IL_0071;
			case XmlState.WithinComment:
				this.WriteCommentString(text);
				goto IL_0071;
			case XmlState.WithinPI:
				this.WriteProcessingInstructionString(text);
				goto IL_0071;
			default:
				goto IL_0071;
			}
			if (disableOutputEscaping)
			{
				this.WriteRawUnchecked(text);
			}
			else
			{
				this.WriteStringUnchecked(text);
			}
			IL_0071:
			if (this.depth == 0)
			{
				this.EndTree();
			}
		}

		private void CopyNode(XPathNavigator navigator)
		{
			int num = this.depth;
			for (;;)
			{
				IL_0007:
				if (this.StartCopy(navigator, this.depth == num))
				{
					XPathNodeType nodeType = navigator.NodeType;
					if (navigator.MoveToFirstAttribute())
					{
						do
						{
							this.StartCopy(navigator, false);
						}
						while (navigator.MoveToNextAttribute());
						navigator.MoveToParent();
					}
					this.CopyNamespaces(navigator, (this.depth - 1 == num) ? XPathNamespaceScope.ExcludeXml : XPathNamespaceScope.Local);
					this.StartElementContentUnchecked();
					if (navigator.MoveToFirstChild())
					{
						continue;
					}
					this.EndCopy(navigator, this.depth - 1 == num);
				}
				while (this.depth != num)
				{
					if (navigator.MoveToNext())
					{
						goto IL_0007;
					}
					navigator.MoveToParent();
					this.EndCopy(navigator, this.depth - 1 == num);
				}
				break;
			}
		}

		private bool StartCopy(XPathNavigator navigator, bool callChk)
		{
			bool flag = false;
			switch (navigator.NodeType)
			{
			case XPathNodeType.Root:
				this.ThrowInvalidStateError(XPathNodeType.Root);
				break;
			case XPathNodeType.Element:
				if (callChk)
				{
					this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				}
				else
				{
					this.WriteStartElementUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				}
				flag = true;
				break;
			case XPathNodeType.Attribute:
				if (callChk)
				{
					this.WriteStartAttribute(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				}
				else
				{
					this.WriteStartAttributeUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				}
				this.WriteString(navigator.Value);
				if (callChk)
				{
					this.WriteEndAttribute();
				}
				else
				{
					this.WriteEndAttributeUnchecked();
				}
				break;
			case XPathNodeType.Namespace:
				if (callChk)
				{
					XmlAttributeCache xmlAttributeCache = this.Writer as XmlAttributeCache;
					if (xmlAttributeCache != null && xmlAttributeCache.Count != 0)
					{
						throw new XslTransformException("Namespace nodes cannot be added to the parent element after an attribute node has already been added.", new string[] { string.Empty });
					}
					this.WriteNamespaceDeclaration(navigator.LocalName, navigator.Value);
				}
				else
				{
					this.WriteNamespaceDeclarationUnchecked(navigator.LocalName, navigator.Value);
				}
				break;
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				if (callChk)
				{
					this.WriteString(navigator.Value, false);
				}
				else
				{
					this.WriteStringUnchecked(navigator.Value);
				}
				break;
			case XPathNodeType.ProcessingInstruction:
				this.WriteStartProcessingInstruction(navigator.LocalName);
				this.WriteProcessingInstructionString(navigator.Value);
				this.WriteEndProcessingInstruction();
				break;
			case XPathNodeType.Comment:
				this.WriteStartComment();
				this.WriteCommentString(navigator.Value);
				this.WriteEndComment();
				break;
			}
			return flag;
		}

		private void EndCopy(XPathNavigator navigator, bool callChk)
		{
			if (callChk)
			{
				this.WriteEndElement();
				return;
			}
			this.WriteEndElementUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
		}

		private void CopyNamespaces(XPathNavigator navigator, XPathNamespaceScope nsScope)
		{
			if (navigator.NamespaceURI.Length == 0)
			{
				this.WriteNamespaceDeclarationUnchecked(string.Empty, string.Empty);
			}
			if (navigator.MoveToFirstNamespace(nsScope))
			{
				this.CopyNamespacesHelper(navigator, nsScope);
				navigator.MoveToParent();
			}
		}

		private void CopyNamespacesHelper(XPathNavigator navigator, XPathNamespaceScope nsScope)
		{
			string localName = navigator.LocalName;
			string value = navigator.Value;
			if (navigator.MoveToNextNamespace(nsScope))
			{
				this.CopyNamespacesHelper(navigator, nsScope);
			}
			this.WriteNamespaceDeclarationUnchecked(localName, value);
		}

		private void ConstructWithinContent(XPathNodeType rootType)
		{
			switch (this.xstate)
			{
			case XmlState.WithinSequence:
				this.StartTree(rootType);
				this.xstate = XmlState.WithinContent;
				return;
			case XmlState.EnumAttrs:
				this.StartElementContentUnchecked();
				return;
			case XmlState.WithinContent:
				break;
			default:
				this.ThrowInvalidStateError(rootType);
				break;
			}
		}

		private void ConstructInEnumAttrs(XPathNodeType rootType)
		{
			XmlState xmlState = this.xstate;
			if (xmlState != XmlState.WithinSequence)
			{
				if (xmlState != XmlState.EnumAttrs)
				{
					this.ThrowInvalidStateError(rootType);
				}
				return;
			}
			this.StartTree(rootType);
			this.xstate = XmlState.EnumAttrs;
		}

		private void WriteCachedNamespaces()
		{
			while (this.cntNmsp != 0)
			{
				this.cntNmsp--;
				string text;
				string text2;
				this.nsmgr.GetNamespaceDeclaration(this.cntNmsp, out text, out text2);
				this.Writer.WriteNamespaceDeclaration(text, text2);
			}
		}

		private XPathNodeType XmlStateToNodeType(XmlState xstate)
		{
			switch (xstate)
			{
			case XmlState.EnumAttrs:
				return XPathNodeType.Element;
			case XmlState.WithinContent:
				return XPathNodeType.Element;
			case XmlState.WithinAttr:
				return XPathNodeType.Attribute;
			case XmlState.WithinComment:
				return XPathNodeType.Comment;
			case XmlState.WithinPI:
				return XPathNodeType.ProcessingInstruction;
			}
			return XPathNodeType.Element;
		}

		private string CheckAttributePrefix(string prefix, string ns)
		{
			if (this.nsmgr == null)
			{
				this.WriteNamespaceDeclarationUnchecked(prefix, ns);
			}
			else
			{
				for (;;)
				{
					string text = this.nsmgr.LookupNamespace(prefix);
					if (!(text != ns))
					{
						return prefix;
					}
					if (text == null)
					{
						break;
					}
					prefix = this.RemapPrefix(prefix, ns, false);
				}
				this.AddNamespace(prefix, ns);
			}
			return prefix;
		}

		private string RemapPrefix(string prefix, string ns, bool isElemPrefix)
		{
			if (this.conflictPrefixes == null)
			{
				this.conflictPrefixes = new Dictionary<string, string>(16);
			}
			if (this.nsmgr == null)
			{
				this.nsmgr = new XmlNamespaceManager(this.runtime.NameTable);
				this.nsmgr.PushScope();
			}
			string text = this.nsmgr.LookupPrefix(ns);
			if ((text == null || (!isElemPrefix && text.Length == 0)) && (!this.conflictPrefixes.TryGetValue(ns, out text) || !(text != prefix) || (!isElemPrefix && text.Length == 0)))
			{
				string text2 = "xp_";
				int num = this.prefixIndex;
				this.prefixIndex = num + 1;
				text = text2 + num.ToString(CultureInfo.InvariantCulture);
			}
			this.conflictPrefixes[ns] = text;
			return text;
		}

		private void WriteStartComputed(XPathNodeType nodeType, string tagName, int prefixMappingsIndex)
		{
			string text;
			string text2;
			string text3;
			this.runtime.ParseTagName(tagName, prefixMappingsIndex, out text, out text2, out text3);
			text = this.EnsureValidName(text, text2, text3, nodeType);
			if (nodeType == XPathNodeType.Element)
			{
				this.WriteStartElement(text, text2, text3);
				return;
			}
			this.WriteStartAttribute(text, text2, text3);
		}

		private void WriteStartComputed(XPathNodeType nodeType, string tagName, string ns)
		{
			string text;
			string text2;
			ValidateNames.ParseQNameThrow(tagName, out text, out text2);
			text = this.EnsureValidName(text, text2, ns, nodeType);
			if (nodeType == XPathNodeType.Element)
			{
				this.WriteStartElement(text, text2, ns);
				return;
			}
			this.WriteStartAttribute(text, text2, ns);
		}

		private void WriteStartComputed(XPathNodeType nodeType, XPathNavigator navigator)
		{
			string text = navigator.Prefix;
			string localName = navigator.LocalName;
			string namespaceURI = navigator.NamespaceURI;
			if (navigator.NodeType != nodeType)
			{
				text = this.EnsureValidName(text, localName, namespaceURI, nodeType);
			}
			if (nodeType == XPathNodeType.Element)
			{
				this.WriteStartElement(text, localName, namespaceURI);
				return;
			}
			this.WriteStartAttribute(text, localName, namespaceURI);
		}

		private void WriteStartComputed(XPathNodeType nodeType, XmlQualifiedName name)
		{
			string text = ((name.Namespace.Length != 0) ? this.RemapPrefix(string.Empty, name.Namespace, nodeType == XPathNodeType.Element) : string.Empty);
			text = this.EnsureValidName(text, name.Name, name.Namespace, nodeType);
			if (nodeType == XPathNodeType.Element)
			{
				this.WriteStartElement(text, name.Name, name.Namespace);
				return;
			}
			this.WriteStartAttribute(text, name.Name, name.Namespace);
		}

		private string EnsureValidName(string prefix, string localName, string ns, XPathNodeType nodeType)
		{
			if (!ValidateNames.ValidateName(prefix, localName, ns, nodeType, ValidateNames.Flags.AllExceptNCNames))
			{
				prefix = ((ns.Length != 0) ? this.RemapPrefix(string.Empty, ns, nodeType == XPathNodeType.Element) : string.Empty);
				ValidateNames.ValidateNameThrow(prefix, localName, ns, nodeType, ValidateNames.Flags.AllExceptNCNames);
			}
			return prefix;
		}

		private void PushElementNames(string prefix, string localName, string ns)
		{
			if (this.stkNames == null)
			{
				this.stkNames = new Stack<string>(15);
			}
			this.stkNames.Push(prefix);
			this.stkNames.Push(localName);
			this.stkNames.Push(ns);
		}

		private void PopElementNames(out string prefix, out string localName, out string ns)
		{
			ns = this.stkNames.Pop();
			localName = this.stkNames.Pop();
			prefix = this.stkNames.Pop();
		}

		private void ThrowInvalidStateError(XPathNodeType constructorType)
		{
			switch (constructorType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Element:
			case XPathNodeType.Text:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				break;
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
				if (this.depth == 1)
				{
					throw new XslTransformException("An item of type '{0}' cannot be constructed within a node of type '{1}'.", new string[]
					{
						constructorType.ToString(),
						this.rootType.ToString()
					});
				}
				if (this.xstate == XmlState.WithinContent)
				{
					throw new XslTransformException("Attribute and namespace nodes cannot be added to the parent element after a text, comment, pi, or sub-element node has already been added.", new string[] { string.Empty });
				}
				break;
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				goto IL_00D0;
			default:
				goto IL_00D0;
			}
			throw new XslTransformException("An item of type '{0}' cannot be constructed within a node of type '{1}'.", new string[]
			{
				constructorType.ToString(),
				this.XmlStateToNodeType(this.xstate).ToString()
			});
			IL_00D0:
			throw new XslTransformException("An item of type '{0}' cannot be constructed within a node of type '{1}'.", new string[]
			{
				"Unknown",
				this.XmlStateToNodeType(this.xstate).ToString()
			});
		}

		private XmlRawWriter xwrt;

		private XmlQueryRuntime runtime;

		private XmlAttributeCache attrCache;

		private int depth;

		private XmlState xstate;

		private XmlSequenceWriter seqwrt;

		private XmlNamespaceManager nsmgr;

		private int cntNmsp;

		private Dictionary<string, string> conflictPrefixes;

		private int prefixIndex;

		private string piTarget;

		private StringConcat nodeText;

		private Stack<string> stkNames;

		private XPathNodeType rootType;

		private Dictionary<string, string> usedPrefixes = new Dictionary<string, string>();
	}
}
