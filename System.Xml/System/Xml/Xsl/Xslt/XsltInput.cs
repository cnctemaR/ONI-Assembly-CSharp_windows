using System;
using System.Collections.Generic;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.Xslt
{
	internal class XsltInput : IErrorHelper
	{
		public XsltInput(XmlReader reader, Compiler compiler, KeywordsTable atoms)
		{
			XsltInput.EnsureExpandEntities(reader);
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			this.atoms = atoms;
			this.reader = reader;
			this.reatomize = reader.NameTable != atoms.NameTable;
			this.readerLineInfo = ((xmlLineInfo != null && xmlLineInfo.HasLineInfo()) ? xmlLineInfo : null);
			this.topLevelReader = reader.ReadState == ReadState.Initial;
			this.scopeManager = new CompilerScopeManager<VarPar>(atoms);
			this.compiler = compiler;
			this.nodeType = XmlNodeType.Document;
		}

		public XmlNodeType NodeType
		{
			get
			{
				if (this.nodeType != XmlNodeType.Element || 0 >= this.currentRecord)
				{
					return this.nodeType;
				}
				return XmlNodeType.Attribute;
			}
		}

		public string LocalName
		{
			get
			{
				return this.records[this.currentRecord].localName;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this.records[this.currentRecord].nsUri;
			}
		}

		public string Prefix
		{
			get
			{
				return this.records[this.currentRecord].prefix;
			}
		}

		public string Value
		{
			get
			{
				return this.records[this.currentRecord].value;
			}
		}

		public string BaseUri
		{
			get
			{
				return this.records[this.currentRecord].baseUri;
			}
		}

		public string QualifiedName
		{
			get
			{
				return this.records[this.currentRecord].QualifiedName;
			}
		}

		public bool IsEmptyElement
		{
			get
			{
				return this.isEmptyElement;
			}
		}

		public string Uri
		{
			get
			{
				return this.records[this.currentRecord].baseUri;
			}
		}

		public Location Start
		{
			get
			{
				return this.records[this.currentRecord].start;
			}
		}

		public Location End
		{
			get
			{
				return this.records[this.currentRecord].end;
			}
		}

		private static void EnsureExpandEntities(XmlReader reader)
		{
			XmlTextReader xmlTextReader = reader as XmlTextReader;
			if (xmlTextReader != null && xmlTextReader.EntityHandling != EntityHandling.ExpandEntities)
			{
				xmlTextReader.EntityHandling = EntityHandling.ExpandEntities;
			}
		}

		private void ExtendRecordBuffer(int position)
		{
			if (this.records.Length <= position)
			{
				int num = this.records.Length * 2;
				if (num <= position)
				{
					num = position + 1;
				}
				XsltInput.Record[] array = new XsltInput.Record[num];
				Array.Copy(this.records, array, this.records.Length);
				this.records = array;
			}
		}

		public bool FindStylesheetElement()
		{
			if (!this.topLevelReader && this.reader.ReadState != ReadState.Interactive)
			{
				return false;
			}
			IDictionary<string, string> dictionary = null;
			if (this.reader.ReadState == ReadState.Interactive)
			{
				IXmlNamespaceResolver xmlNamespaceResolver = this.reader as IXmlNamespaceResolver;
				if (xmlNamespaceResolver != null)
				{
					dictionary = xmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
				}
			}
			while (this.MoveToNextSibling() && this.nodeType == XmlNodeType.Whitespace)
			{
			}
			if (this.nodeType == XmlNodeType.Element)
			{
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in dictionary)
					{
						if (this.scopeManager.LookupNamespace(keyValuePair.Key) == null)
						{
							string text = this.atoms.NameTable.Add(keyValuePair.Value);
							this.scopeManager.AddNsDeclaration(keyValuePair.Key, text);
							this.ctxInfo.AddNamespace(keyValuePair.Key, text);
						}
					}
				}
				return true;
			}
			return false;
		}

		public void Finish()
		{
			if (this.topLevelReader)
			{
				while (this.reader.ReadState == ReadState.Interactive)
				{
					this.reader.Skip();
				}
			}
		}

		private void FillupRecord(ref XsltInput.Record rec)
		{
			rec.localName = this.reader.LocalName;
			rec.nsUri = this.reader.NamespaceURI;
			rec.prefix = this.reader.Prefix;
			rec.value = this.reader.Value;
			rec.baseUri = this.reader.BaseURI;
			if (this.reatomize)
			{
				rec.localName = this.atoms.NameTable.Add(rec.localName);
				rec.nsUri = this.atoms.NameTable.Add(rec.nsUri);
				rec.prefix = this.atoms.NameTable.Add(rec.prefix);
			}
			if (this.readerLineInfo != null)
			{
				rec.start = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition - XsltInput.PositionAdjustment(this.reader.NodeType));
			}
		}

		private void SetRecordEnd(ref XsltInput.Record rec)
		{
			if (this.readerLineInfo != null)
			{
				rec.end = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition - XsltInput.PositionAdjustment(this.reader.NodeType));
				if (this.reader.BaseURI != rec.baseUri || rec.end.LessOrEqual(rec.start))
				{
					rec.end = new Location(rec.start.Line, int.MaxValue);
				}
			}
		}

		private void FillupTextRecord(ref XsltInput.Record rec)
		{
			rec.localName = string.Empty;
			rec.nsUri = string.Empty;
			rec.prefix = string.Empty;
			rec.value = this.reader.Value;
			rec.baseUri = this.reader.BaseURI;
			if (this.readerLineInfo != null)
			{
				bool flag = this.reader.NodeType == XmlNodeType.CDATA;
				int num = this.readerLineInfo.LineNumber;
				int num2 = this.readerLineInfo.LinePosition;
				rec.start = new Location(num, num2 - (flag ? 9 : 0));
				char c = ' ';
				string value = rec.value;
				int i = 0;
				while (i < value.Length)
				{
					char c2 = value[i];
					if (c2 != '\n')
					{
						if (c2 == '\r')
						{
							goto IL_00B9;
						}
						num2++;
					}
					else if (c != '\r')
					{
						goto IL_00B9;
					}
					IL_00C5:
					c = c2;
					i++;
					continue;
					IL_00B9:
					num++;
					num2 = 1;
					goto IL_00C5;
				}
				rec.end = new Location(num, num2 + (flag ? 3 : 0));
			}
		}

		private void FillupCharacterEntityRecord(ref XsltInput.Record rec)
		{
			string localName = this.reader.LocalName;
			rec.localName = string.Empty;
			rec.nsUri = string.Empty;
			rec.prefix = string.Empty;
			rec.baseUri = this.reader.BaseURI;
			if (this.readerLineInfo != null)
			{
				rec.start = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition - 1);
			}
			this.reader.ResolveEntity();
			this.reader.Read();
			rec.value = this.reader.Value;
			this.reader.Read();
			if (this.readerLineInfo != null)
			{
				int lineNumber = this.readerLineInfo.LineNumber;
				int linePosition = this.readerLineInfo.LinePosition;
				rec.end = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition + 1);
			}
		}

		private bool ReadAttribute(ref XsltInput.Record rec)
		{
			this.FillupRecord(ref rec);
			if (Ref.Equal(rec.prefix, this.atoms.Xmlns))
			{
				string text = this.atoms.NameTable.Add(this.reader.Value);
				if (!Ref.Equal(rec.localName, this.atoms.Xml))
				{
					this.scopeManager.AddNsDeclaration(rec.localName, text);
					this.ctxInfo.AddNamespace(rec.localName, text);
				}
				return false;
			}
			if (rec.prefix.Length == 0 && Ref.Equal(rec.localName, this.atoms.Xmlns))
			{
				string text2 = this.atoms.NameTable.Add(this.reader.Value);
				this.scopeManager.AddNsDeclaration(string.Empty, text2);
				this.ctxInfo.AddNamespace(string.Empty, text2);
				return false;
			}
			if (!this.reader.ReadAttributeValue())
			{
				rec.value = string.Empty;
				this.SetRecordEnd(ref rec);
				return true;
			}
			if (this.readerLineInfo != null)
			{
				int num = ((this.reader.NodeType == XmlNodeType.EntityReference) ? (-2) : (-1));
				rec.valueStart = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition + num);
				if (this.reader.BaseURI != rec.baseUri || rec.valueStart.LessOrEqual(rec.start))
				{
					int num2 = ((rec.prefix.Length != 0) ? (rec.prefix.Length + 1) : 0) + rec.localName.Length;
					rec.end = new Location(rec.start.Line, rec.start.Pos + num2 + 1);
				}
			}
			string text3 = string.Empty;
			this.strConcat.Clear();
			do
			{
				XmlNodeType xmlNodeType = this.reader.NodeType;
				if (xmlNodeType != XmlNodeType.EntityReference)
				{
					if (xmlNodeType != XmlNodeType.EndEntity)
					{
						text3 = this.reader.Value;
						this.strConcat.Concat(text3);
					}
				}
				else
				{
					this.reader.ResolveEntity();
				}
			}
			while (this.reader.ReadAttributeValue());
			rec.value = this.strConcat.GetResult();
			if (this.readerLineInfo != null)
			{
				int num3 = ((this.reader.NodeType == XmlNodeType.EndEntity) ? 1 : text3.Length) + 1;
				rec.end = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition + num3);
				if (this.reader.BaseURI != rec.baseUri || rec.end.LessOrEqual(rec.valueStart))
				{
					rec.end = new Location(rec.start.Line, int.MaxValue);
				}
			}
			return true;
		}

		public bool MoveToFirstChild()
		{
			return !this.IsEmptyElement && this.ReadNextSibling();
		}

		public bool MoveToNextSibling()
		{
			if (this.nodeType == XmlNodeType.Element || this.nodeType == XmlNodeType.EndElement)
			{
				this.scopeManager.ExitScope();
			}
			return this.ReadNextSibling();
		}

		public void SkipNode()
		{
			if (this.nodeType == XmlNodeType.Element && this.MoveToFirstChild())
			{
				do
				{
					this.SkipNode();
				}
				while (this.MoveToNextSibling());
			}
		}

		private int ReadTextNodes()
		{
			bool flag = this.reader.XmlSpace == XmlSpace.Preserve;
			bool flag2 = true;
			int num = 0;
			for (;;)
			{
				XmlNodeType xmlNodeType = this.reader.NodeType;
				if (xmlNodeType <= XmlNodeType.EntityReference)
				{
					if (xmlNodeType - XmlNodeType.Text > 1)
					{
						if (xmlNodeType != XmlNodeType.EntityReference)
						{
							break;
						}
						string localName = this.reader.LocalName;
						if (localName.Length > 0 && (localName[0] == '#' || localName == "lt" || localName == "gt" || localName == "quot" || localName == "apos"))
						{
							this.ExtendRecordBuffer(num);
							this.FillupCharacterEntityRecord(ref this.records[num]);
							if (flag2 && !XmlCharType.Instance.IsOnlyWhitespace(this.records[num].value))
							{
								flag2 = false;
							}
							num++;
							continue;
						}
						this.reader.ResolveEntity();
						this.reader.Read();
						continue;
					}
					else if (flag2 && !XmlCharType.Instance.IsOnlyWhitespace(this.reader.Value))
					{
						flag2 = false;
					}
				}
				else if (xmlNodeType - XmlNodeType.Whitespace > 1)
				{
					if (xmlNodeType != XmlNodeType.EndEntity)
					{
						break;
					}
					this.reader.Read();
					continue;
				}
				this.ExtendRecordBuffer(num);
				this.FillupTextRecord(ref this.records[num]);
				this.reader.Read();
				num++;
			}
			this.nodeType = ((!flag2) ? XmlNodeType.Text : (flag ? XmlNodeType.SignificantWhitespace : XmlNodeType.Whitespace));
			return num;
		}

		private bool ReadNextSibling()
		{
			if (this.currentRecord < this.lastTextNode)
			{
				this.currentRecord++;
				if (this.currentRecord == this.lastTextNode)
				{
					this.lastTextNode = 0;
				}
				return true;
			}
			this.currentRecord = 0;
			while (!this.reader.EOF)
			{
				XmlNodeType xmlNodeType = this.reader.NodeType;
				if (xmlNodeType <= XmlNodeType.EntityReference)
				{
					if (xmlNodeType == XmlNodeType.Element)
					{
						this.scopeManager.EnterScope();
						this.numAttributes = this.ReadElement();
						return true;
					}
					if (xmlNodeType - XmlNodeType.Text > 2)
					{
						goto IL_00D8;
					}
				}
				else if (xmlNodeType - XmlNodeType.Whitespace > 1)
				{
					if (xmlNodeType != XmlNodeType.EndElement)
					{
						goto IL_00D8;
					}
					this.nodeType = XmlNodeType.EndElement;
					this.isEmptyElement = false;
					this.FillupRecord(ref this.records[0]);
					this.reader.Read();
					this.SetRecordEnd(ref this.records[0]);
					return false;
				}
				int num = this.ReadTextNodes();
				if (num != 0)
				{
					this.lastTextNode = num - 1;
					return true;
				}
				continue;
				IL_00D8:
				this.reader.Read();
			}
			return false;
		}

		private int ReadElement()
		{
			this.attributesRead = false;
			this.FillupRecord(ref this.records[0]);
			this.nodeType = XmlNodeType.Element;
			this.isEmptyElement = this.reader.IsEmptyElement;
			this.ctxInfo = new XsltInput.ContextInfo(this);
			int num = 1;
			if (this.reader.MoveToFirstAttribute())
			{
				do
				{
					this.ExtendRecordBuffer(num);
					if (this.ReadAttribute(ref this.records[num]))
					{
						num++;
					}
				}
				while (this.reader.MoveToNextAttribute());
				this.reader.MoveToElement();
			}
			this.reader.Read();
			this.SetRecordEnd(ref this.records[0]);
			this.ctxInfo.lineInfo = this.BuildLineInfo();
			this.attributes = null;
			return num - 1;
		}

		public void MoveToElement()
		{
			this.currentRecord = 0;
		}

		private bool MoveToAttributeBase(int attNum)
		{
			if (0 < attNum && attNum <= this.numAttributes)
			{
				this.currentRecord = attNum;
				return true;
			}
			this.currentRecord = 0;
			return false;
		}

		public bool MoveToLiteralAttribute(int attNum)
		{
			if (0 < attNum && attNum <= this.numAttributes)
			{
				this.currentRecord = attNum;
				return true;
			}
			this.currentRecord = 0;
			return false;
		}

		public bool MoveToXsltAttribute(int attNum, string attName)
		{
			this.currentRecord = this.xsltAttributeNumber[attNum];
			return this.currentRecord != 0;
		}

		public bool IsRequiredAttribute(int attNum)
		{
			return (this.attributes[attNum].flags & ((this.compiler.Version == 2) ? XsltLoader.V2Req : XsltLoader.V1Req)) != 0;
		}

		public bool AttributeExists(int attNum, string attName)
		{
			return this.xsltAttributeNumber[attNum] != 0;
		}

		public XsltInput.DelayedQName ElementName
		{
			get
			{
				return new XsltInput.DelayedQName(ref this.records[0]);
			}
		}

		public bool IsNs(string ns)
		{
			return Ref.Equal(ns, this.NamespaceUri);
		}

		public bool IsKeyword(string kwd)
		{
			return Ref.Equal(kwd, this.LocalName);
		}

		public bool IsXsltNamespace()
		{
			return this.IsNs(this.atoms.UriXsl);
		}

		public bool IsNullNamespace()
		{
			return this.IsNs(string.Empty);
		}

		public bool IsXsltAttribute(string kwd)
		{
			return this.IsKeyword(kwd) && this.IsNullNamespace();
		}

		public bool IsXsltKeyword(string kwd)
		{
			return this.IsKeyword(kwd) && this.IsXsltNamespace();
		}

		public bool CanHaveApplyImports
		{
			get
			{
				return this.scopeManager.CanHaveApplyImports;
			}
			set
			{
				this.scopeManager.CanHaveApplyImports = value;
			}
		}

		public bool IsExtensionNamespace(string uri)
		{
			return this.scopeManager.IsExNamespace(uri);
		}

		public bool ForwardCompatibility
		{
			get
			{
				return this.scopeManager.ForwardCompatibility;
			}
		}

		public bool BackwardCompatibility
		{
			get
			{
				return this.scopeManager.BackwardCompatibility;
			}
		}

		public XslVersion XslVersion
		{
			get
			{
				if (!this.scopeManager.ForwardCompatibility)
				{
					return XslVersion.Version10;
				}
				return XslVersion.ForwardsCompatible;
			}
		}

		private void SetVersion(int attVersion)
		{
			this.MoveToLiteralAttribute(attVersion);
			double num = XPathConvert.StringToDouble(this.Value);
			if (double.IsNaN(num))
			{
				this.ReportError("'{1}' is an invalid value for the '{0}' attribute.", new string[]
				{
					this.atoms.Version,
					this.Value
				});
				num = 1.0;
			}
			this.SetVersion(num);
		}

		private void SetVersion(double version)
		{
			if (this.compiler.Version == 0)
			{
				this.compiler.Version = 1;
			}
			if (this.compiler.Version == 1)
			{
				this.scopeManager.BackwardCompatibility = false;
				this.scopeManager.ForwardCompatibility = version != 1.0;
				return;
			}
			this.scopeManager.BackwardCompatibility = version < 2.0;
			this.scopeManager.ForwardCompatibility = 2.0 < version;
		}

		public XsltInput.ContextInfo GetAttributes()
		{
			return this.GetAttributes(XsltInput.noAttributes);
		}

		public XsltInput.ContextInfo GetAttributes(XsltInput.XsltAttribute[] attributes)
		{
			this.attributes = attributes;
			this.records[0].value = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			bool flag = this.IsXsltNamespace() && this.IsKeyword(this.atoms.Output);
			bool flag2 = this.IsXsltNamespace() && (this.IsKeyword(this.atoms.Stylesheet) || this.IsKeyword(this.atoms.Transform));
			bool flag3 = this.compiler.Version == 2;
			for (int i = 0; i < attributes.Length; i++)
			{
				this.xsltAttributeNumber[i] = 0;
			}
			this.compiler.EnterForwardsCompatible();
			if (flag2 || (flag3 && !flag))
			{
				int num6 = 1;
				while (this.MoveToAttributeBase(num6))
				{
					if (this.IsNullNamespace() && this.IsKeyword(this.atoms.Version))
					{
						this.SetVersion(num6);
						break;
					}
					num6++;
				}
			}
			if (this.compiler.Version == 0)
			{
				this.SetVersion(1.0);
			}
			flag3 = this.compiler.Version == 2;
			int num7 = (flag3 ? (XsltLoader.V2Opt | XsltLoader.V2Req) : (XsltLoader.V1Opt | XsltLoader.V1Req));
			int num8 = 1;
			while (this.MoveToAttributeBase(num8))
			{
				if (this.IsNullNamespace())
				{
					string localName = this.LocalName;
					int j;
					for (j = 0; j < attributes.Length; j++)
					{
						if (Ref.Equal(localName, attributes[j].name) && (attributes[j].flags & num7) != 0)
						{
							this.xsltAttributeNumber[j] = num8;
							break;
						}
					}
					if (j == attributes.Length)
					{
						if (Ref.Equal(localName, this.atoms.ExcludeResultPrefixes) && (flag2 || flag3))
						{
							num2 = num8;
						}
						else if (Ref.Equal(localName, this.atoms.ExtensionElementPrefixes) && (flag2 || flag3))
						{
							num = num8;
						}
						else if (Ref.Equal(localName, this.atoms.XPathDefaultNamespace) && flag3)
						{
							num3 = num8;
						}
						else if (Ref.Equal(localName, this.atoms.DefaultCollation) && flag3)
						{
							num4 = num8;
						}
						else if (Ref.Equal(localName, this.atoms.UseWhen) && flag3)
						{
							num5 = num8;
						}
						else
						{
							this.ReportError("'{0}' is an invalid attribute for the '{1}' element.", new string[]
							{
								this.QualifiedName,
								this.records[0].QualifiedName
							});
						}
					}
				}
				else if (this.IsXsltNamespace())
				{
					this.ReportError("'{0}' is an invalid attribute for the '{1}' element.", new string[]
					{
						this.QualifiedName,
						this.records[0].QualifiedName
					});
				}
				num8++;
			}
			this.attributesRead = true;
			this.compiler.ExitForwardsCompatible(this.ForwardCompatibility);
			this.InsertExNamespaces(num, this.ctxInfo, true);
			this.InsertExNamespaces(num2, this.ctxInfo, false);
			this.SetXPathDefaultNamespace(num3);
			this.SetDefaultCollation(num4);
			if (num5 != 0)
			{
				this.ReportNYI(this.atoms.UseWhen);
			}
			this.MoveToElement();
			for (int k = 0; k < attributes.Length; k++)
			{
				if (this.xsltAttributeNumber[k] == 0)
				{
					int flags = attributes[k].flags;
					if ((this.compiler.Version == 2 && (flags & XsltLoader.V2Req) != 0) || (this.compiler.Version == 1 && (flags & XsltLoader.V1Req) != 0 && (!this.ForwardCompatibility || (flags & XsltLoader.V2Req) != 0)))
					{
						this.ReportError("Missing mandatory attribute '{0}'.", new string[] { attributes[k].name });
					}
				}
			}
			return this.ctxInfo;
		}

		public XsltInput.ContextInfo GetLiteralAttributes(bool asStylesheet)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 1;
			while (this.MoveToLiteralAttribute(num7))
			{
				if (this.IsXsltNamespace())
				{
					string localName = this.LocalName;
					if (Ref.Equal(localName, this.atoms.Version))
					{
						num = num7;
					}
					else if (Ref.Equal(localName, this.atoms.ExtensionElementPrefixes))
					{
						num2 = num7;
					}
					else if (Ref.Equal(localName, this.atoms.ExcludeResultPrefixes))
					{
						num3 = num7;
					}
					else if (Ref.Equal(localName, this.atoms.XPathDefaultNamespace))
					{
						num4 = num7;
					}
					else if (Ref.Equal(localName, this.atoms.DefaultCollation))
					{
						num5 = num7;
					}
					else if (Ref.Equal(localName, this.atoms.UseWhen))
					{
						num6 = num7;
					}
				}
				num7++;
			}
			this.attributesRead = true;
			this.MoveToElement();
			if (num != 0)
			{
				this.SetVersion(num);
			}
			else if (asStylesheet)
			{
				this.ReportError((Ref.Equal(this.NamespaceUri, this.atoms.UriWdXsl) && Ref.Equal(this.LocalName, this.atoms.Stylesheet)) ? "The 'http://www.w3.org/TR/WD-xsl' namespace is no longer supported." : "Stylesheet must start either with an 'xsl:stylesheet' or an 'xsl:transform' element, or with a literal result element that has an 'xsl:version' attribute, where prefix 'xsl' denotes the 'http://www.w3.org/1999/XSL/Transform' namespace.", Array.Empty<string>());
				this.SetVersion(1.0);
			}
			this.InsertExNamespaces(num2, this.ctxInfo, true);
			if (!this.IsExtensionNamespace(this.records[0].nsUri))
			{
				if (this.compiler.Version == 2)
				{
					this.SetXPathDefaultNamespace(num4);
					this.SetDefaultCollation(num5);
					if (num6 != 0)
					{
						this.ReportNYI(this.atoms.UseWhen);
					}
				}
				this.InsertExNamespaces(num3, this.ctxInfo, false);
			}
			return this.ctxInfo;
		}

		public void GetVersionAttribute()
		{
			if (this.compiler.Version == 2)
			{
				int num = 1;
				while (this.MoveToAttributeBase(num))
				{
					if (this.IsNullNamespace() && this.IsKeyword(this.atoms.Version))
					{
						this.SetVersion(num);
						break;
					}
					num++;
				}
			}
			this.attributesRead = true;
		}

		private void InsertExNamespaces(int attExPrefixes, XsltInput.ContextInfo ctxInfo, bool extensions)
		{
			if (this.MoveToLiteralAttribute(attExPrefixes))
			{
				string value = this.Value;
				if (value.Length != 0)
				{
					if (!extensions && this.compiler.Version != 1 && value == "#all")
					{
						ctxInfo.nsList = new NsDecl(ctxInfo.nsList, null, null);
						return;
					}
					this.compiler.EnterForwardsCompatible();
					string[] array = XmlConvert.SplitString(value);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == "#default")
						{
							array[i] = this.LookupXmlNamespace(string.Empty);
							if (array[i].Length == 0 && this.compiler.Version != 1 && !this.BackwardCompatibility)
							{
								this.ReportError("Value '#default' is used within the 'exclude-result-prefixes' attribute and the parent element of this attribute has no default namespace.", Array.Empty<string>());
							}
						}
						else
						{
							array[i] = this.LookupXmlNamespace(array[i]);
						}
					}
					if (!this.compiler.ExitForwardsCompatible(this.ForwardCompatibility))
					{
						return;
					}
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] != null)
						{
							ctxInfo.nsList = new NsDecl(ctxInfo.nsList, null, array[j]);
							if (extensions)
							{
								this.scopeManager.AddExNamespace(array[j]);
							}
						}
					}
				}
			}
		}

		private void SetXPathDefaultNamespace(int attNamespace)
		{
			if (this.MoveToLiteralAttribute(attNamespace) && this.Value.Length != 0)
			{
				this.ReportNYI(this.atoms.XPathDefaultNamespace);
			}
		}

		private void SetDefaultCollation(int attCollation)
		{
			if (this.MoveToLiteralAttribute(attCollation))
			{
				string[] array = XmlConvert.SplitString(this.Value);
				int num = 0;
				while (num < array.Length && XmlCollation.Create(array[num], false) == null)
				{
					num++;
				}
				if (num == array.Length)
				{
					this.ReportErrorFC("The value of an 'default-collation' attribute contains no recognized collation URI.", Array.Empty<string>());
					return;
				}
				if (array[num] != "http://www.w3.org/2004/10/xpath-functions/collation/codepoint")
				{
					this.ReportNYI(this.atoms.DefaultCollation);
				}
			}
		}

		private static int PositionAdjustment(XmlNodeType nt)
		{
			switch (nt)
			{
			case XmlNodeType.Element:
				return 1;
			case XmlNodeType.Attribute:
			case XmlNodeType.Text:
			case XmlNodeType.Entity:
				break;
			case XmlNodeType.CDATA:
				return 9;
			case XmlNodeType.EntityReference:
				return 1;
			case XmlNodeType.ProcessingInstruction:
				return 2;
			case XmlNodeType.Comment:
				return 4;
			default:
				if (nt == XmlNodeType.EndElement)
				{
					return 2;
				}
				break;
			}
			return 0;
		}

		public ISourceLineInfo BuildLineInfo()
		{
			return new SourceLineInfo(this.Uri, this.Start, this.End);
		}

		public ISourceLineInfo BuildNameLineInfo()
		{
			if (this.readerLineInfo == null)
			{
				return this.BuildLineInfo();
			}
			if (this.LocalName == null)
			{
				this.FillupRecord(ref this.records[this.currentRecord]);
			}
			Location start = this.Start;
			int line = start.Line;
			int num = start.Pos + XsltInput.PositionAdjustment(this.NodeType);
			return new SourceLineInfo(this.Uri, new Location(line, num), new Location(line, num + this.QualifiedName.Length));
		}

		public ISourceLineInfo BuildReaderLineInfo()
		{
			Location location;
			if (this.readerLineInfo != null)
			{
				location = new Location(this.readerLineInfo.LineNumber, this.readerLineInfo.LinePosition);
			}
			else
			{
				location = new Location(0, 0);
			}
			return new SourceLineInfo(this.reader.BaseURI, location, location);
		}

		public string LookupXmlNamespace(string prefix)
		{
			string text = this.scopeManager.LookupNamespace(prefix);
			if (text != null)
			{
				return text;
			}
			if (prefix.Length == 0)
			{
				return string.Empty;
			}
			this.ReportError("Prefix '{0}' is not defined.", new string[] { prefix });
			return null;
		}

		public void ReportError(string res, params string[] args)
		{
			this.compiler.ReportError(this.BuildNameLineInfo(), res, args);
		}

		public void ReportErrorFC(string res, params string[] args)
		{
			if (!this.ForwardCompatibility)
			{
				this.compiler.ReportError(this.BuildNameLineInfo(), res, args);
			}
		}

		public void ReportWarning(string res, params string[] args)
		{
			this.compiler.ReportWarning(this.BuildNameLineInfo(), res, args);
		}

		private void ReportNYI(string arg)
		{
			this.ReportErrorFC("'{0}' is not yet implemented.", new string[] { arg });
		}

		private const int InitRecordsSize = 22;

		private XmlReader reader;

		private IXmlLineInfo readerLineInfo;

		private bool topLevelReader;

		private CompilerScopeManager<VarPar> scopeManager;

		private KeywordsTable atoms;

		private Compiler compiler;

		private bool reatomize;

		private XmlNodeType nodeType;

		private XsltInput.Record[] records = new XsltInput.Record[22];

		private int currentRecord;

		private bool isEmptyElement;

		private int lastTextNode;

		private int numAttributes;

		private XsltInput.ContextInfo ctxInfo;

		private bool attributesRead;

		private StringConcat strConcat;

		private XsltInput.XsltAttribute[] attributes;

		private int[] xsltAttributeNumber = new int[21];

		private static XsltInput.XsltAttribute[] noAttributes = new XsltInput.XsltAttribute[0];

		public struct DelayedQName
		{
			public DelayedQName(ref XsltInput.Record rec)
			{
				this.prefix = rec.prefix;
				this.localName = rec.localName;
			}

			public static implicit operator string(XsltInput.DelayedQName qn)
			{
				if (qn.prefix.Length != 0)
				{
					return qn.prefix + ":" + qn.localName;
				}
				return qn.localName;
			}

			private string prefix;

			private string localName;
		}

		public struct XsltAttribute
		{
			public XsltAttribute(string name, int flags)
			{
				this.name = name;
				this.flags = flags;
			}

			public string name;

			public int flags;
		}

		internal class ContextInfo
		{
			internal ContextInfo(ISourceLineInfo lineinfo)
			{
				this.elemNameLi = lineinfo;
				this.endTagLi = lineinfo;
				this.lineInfo = lineinfo;
			}

			public ContextInfo(XsltInput input)
			{
				this.elemNameLength = input.QualifiedName.Length;
			}

			public void AddNamespace(string prefix, string nsUri)
			{
				this.nsList = new NsDecl(this.nsList, prefix, nsUri);
			}

			public void SaveExtendedLineInfo(XsltInput input)
			{
				if (this.lineInfo.Start.Line == 0)
				{
					this.elemNameLi = (this.endTagLi = null);
					return;
				}
				this.elemNameLi = new SourceLineInfo(this.lineInfo.Uri, this.lineInfo.Start.Line, this.lineInfo.Start.Pos + 1, this.lineInfo.Start.Line, this.lineInfo.Start.Pos + 1 + this.elemNameLength);
				if (!input.IsEmptyElement)
				{
					this.endTagLi = input.BuildLineInfo();
					return;
				}
				this.endTagLi = new XsltInput.ContextInfo.EmptyElementEndTag(this.lineInfo);
			}

			public NsDecl nsList;

			public ISourceLineInfo lineInfo;

			public ISourceLineInfo elemNameLi;

			public ISourceLineInfo endTagLi;

			private int elemNameLength;

			internal class EmptyElementEndTag : ISourceLineInfo
			{
				public EmptyElementEndTag(ISourceLineInfo elementTagLi)
				{
					this.elementTagLi = elementTagLi;
				}

				public string Uri
				{
					get
					{
						return this.elementTagLi.Uri;
					}
				}

				public bool IsNoSource
				{
					get
					{
						return this.elementTagLi.IsNoSource;
					}
				}

				public Location Start
				{
					get
					{
						return new Location(this.elementTagLi.End.Line, this.elementTagLi.End.Pos - 2);
					}
				}

				public Location End
				{
					get
					{
						return this.elementTagLi.End;
					}
				}

				private ISourceLineInfo elementTagLi;
			}
		}

		internal struct Record
		{
			public string QualifiedName
			{
				get
				{
					if (this.prefix.Length != 0)
					{
						return this.prefix + ":" + this.localName;
					}
					return this.localName;
				}
			}

			public string localName;

			public string nsUri;

			public string prefix;

			public string value;

			public string baseUri;

			public Location start;

			public Location valueStart;

			public Location end;
		}
	}
}
