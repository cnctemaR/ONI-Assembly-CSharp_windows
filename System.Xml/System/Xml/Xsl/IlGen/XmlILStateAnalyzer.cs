using System;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILStateAnalyzer
	{
		public XmlILStateAnalyzer(QilFactory fac)
		{
			this.fac = fac;
		}

		public virtual QilNode Analyze(QilNode ndConstr, QilNode ndContent)
		{
			if (ndConstr == null)
			{
				this.parentInfo = null;
				this.xstates = PossibleXmlStates.WithinSequence;
				this.withinElem = false;
				ndContent = this.AnalyzeContent(ndContent);
			}
			else
			{
				this.parentInfo = XmlILConstructInfo.Write(ndConstr);
				if (ndConstr.NodeType == QilNodeType.Function)
				{
					this.parentInfo.ConstructMethod = XmlILConstructMethod.Writer;
					PossibleXmlStates possibleXmlStates = PossibleXmlStates.None;
					foreach (object obj in this.parentInfo.CallersInfo)
					{
						XmlILConstructInfo xmlILConstructInfo = (XmlILConstructInfo)obj;
						if (possibleXmlStates == PossibleXmlStates.None)
						{
							possibleXmlStates = xmlILConstructInfo.InitialStates;
						}
						else if (possibleXmlStates != xmlILConstructInfo.InitialStates)
						{
							possibleXmlStates = PossibleXmlStates.Any;
						}
						xmlILConstructInfo.PushToWriterFirst = true;
					}
					this.parentInfo.InitialStates = possibleXmlStates;
				}
				else
				{
					if (ndConstr.NodeType != QilNodeType.Choice)
					{
						this.parentInfo.InitialStates = (this.parentInfo.FinalStates = PossibleXmlStates.WithinSequence);
					}
					if (ndConstr.NodeType != QilNodeType.RtfCtor)
					{
						this.parentInfo.ConstructMethod = XmlILConstructMethod.WriterThenIterator;
					}
				}
				this.withinElem = ndConstr.NodeType == QilNodeType.ElementCtor;
				QilNodeType nodeType = ndConstr.NodeType;
				if (nodeType <= QilNodeType.Function)
				{
					if (nodeType != QilNodeType.Choice)
					{
						if (nodeType == QilNodeType.Function)
						{
							this.xstates = this.parentInfo.InitialStates;
						}
					}
					else
					{
						this.xstates = PossibleXmlStates.Any;
					}
				}
				else
				{
					switch (nodeType)
					{
					case QilNodeType.ElementCtor:
						this.xstates = PossibleXmlStates.EnumAttrs;
						break;
					case QilNodeType.AttributeCtor:
						this.xstates = PossibleXmlStates.WithinAttr;
						break;
					case QilNodeType.CommentCtor:
						this.xstates = PossibleXmlStates.WithinComment;
						break;
					case QilNodeType.PICtor:
						this.xstates = PossibleXmlStates.WithinPI;
						break;
					case QilNodeType.TextCtor:
					case QilNodeType.RawTextCtor:
					case QilNodeType.NamespaceDecl:
						break;
					case QilNodeType.DocumentCtor:
						this.xstates = PossibleXmlStates.WithinContent;
						break;
					case QilNodeType.RtfCtor:
						this.xstates = PossibleXmlStates.WithinContent;
						break;
					default:
						if (nodeType != QilNodeType.XsltCopy)
						{
							if (nodeType != QilNodeType.XsltCopyOf)
							{
							}
						}
						else
						{
							this.xstates = PossibleXmlStates.Any;
						}
						break;
					}
				}
				if (ndContent != null)
				{
					ndContent = this.AnalyzeContent(ndContent);
				}
				if (ndConstr.NodeType == QilNodeType.Choice)
				{
					this.AnalyzeChoice(ndConstr as QilChoice, this.parentInfo);
				}
				if (ndConstr.NodeType == QilNodeType.Function)
				{
					this.parentInfo.FinalStates = this.xstates;
				}
			}
			return ndContent;
		}

		protected virtual QilNode AnalyzeContent(QilNode nd)
		{
			QilNodeType qilNodeType = nd.NodeType;
			if (qilNodeType - QilNodeType.For <= 2)
			{
				nd = this.fac.Nop(nd);
			}
			XmlILConstructInfo xmlILConstructInfo = XmlILConstructInfo.Write(nd);
			xmlILConstructInfo.ParentInfo = this.parentInfo;
			xmlILConstructInfo.PushToWriterLast = true;
			xmlILConstructInfo.InitialStates = this.xstates;
			qilNodeType = nd.NodeType;
			if (qilNodeType <= QilNodeType.Warning)
			{
				if (qilNodeType != QilNodeType.Nop)
				{
					if (qilNodeType - QilNodeType.Error <= 1)
					{
						xmlILConstructInfo.ConstructMethod = XmlILConstructMethod.Writer;
						goto IL_00FF;
					}
				}
				else
				{
					QilNode child = (nd as QilUnary).Child;
					QilNodeType nodeType = child.NodeType;
					if (nodeType - QilNodeType.For <= 2)
					{
						this.AnalyzeCopy(nd, xmlILConstructInfo);
						goto IL_00FF;
					}
					xmlILConstructInfo.ConstructMethod = XmlILConstructMethod.Writer;
					this.AnalyzeContent(child);
					goto IL_00FF;
				}
			}
			else
			{
				switch (qilNodeType)
				{
				case QilNodeType.Conditional:
					this.AnalyzeConditional(nd as QilTernary, xmlILConstructInfo);
					goto IL_00FF;
				case QilNodeType.Choice:
					this.AnalyzeChoice(nd as QilChoice, xmlILConstructInfo);
					goto IL_00FF;
				case QilNodeType.Length:
					break;
				case QilNodeType.Sequence:
					this.AnalyzeSequence(nd as QilList, xmlILConstructInfo);
					goto IL_00FF;
				default:
					if (qilNodeType == QilNodeType.Loop)
					{
						this.AnalyzeLoop(nd as QilLoop, xmlILConstructInfo);
						goto IL_00FF;
					}
					break;
				}
			}
			this.AnalyzeCopy(nd, xmlILConstructInfo);
			IL_00FF:
			xmlILConstructInfo.FinalStates = this.xstates;
			return nd;
		}

		protected virtual void AnalyzeLoop(QilLoop ndLoop, XmlILConstructInfo info)
		{
			XmlQueryType xmlType = ndLoop.XmlType;
			info.ConstructMethod = XmlILConstructMethod.Writer;
			if (!xmlType.IsSingleton)
			{
				this.StartLoop(xmlType, info);
			}
			ndLoop.Body = this.AnalyzeContent(ndLoop.Body);
			if (!xmlType.IsSingleton)
			{
				this.EndLoop(xmlType, info);
			}
		}

		protected virtual void AnalyzeSequence(QilList ndSeq, XmlILConstructInfo info)
		{
			info.ConstructMethod = XmlILConstructMethod.Writer;
			for (int i = 0; i < ndSeq.Count; i++)
			{
				ndSeq[i] = this.AnalyzeContent(ndSeq[i]);
			}
		}

		protected virtual void AnalyzeConditional(QilTernary ndCond, XmlILConstructInfo info)
		{
			info.ConstructMethod = XmlILConstructMethod.Writer;
			ndCond.Center = this.AnalyzeContent(ndCond.Center);
			PossibleXmlStates possibleXmlStates = this.xstates;
			this.xstates = info.InitialStates;
			ndCond.Right = this.AnalyzeContent(ndCond.Right);
			if (possibleXmlStates != this.xstates)
			{
				this.xstates = PossibleXmlStates.Any;
			}
		}

		protected virtual void AnalyzeChoice(QilChoice ndChoice, XmlILConstructInfo info)
		{
			int num = ndChoice.Branches.Count - 1;
			ndChoice.Branches[num] = this.AnalyzeContent(ndChoice.Branches[num]);
			PossibleXmlStates possibleXmlStates = this.xstates;
			while (--num >= 0)
			{
				this.xstates = info.InitialStates;
				ndChoice.Branches[num] = this.AnalyzeContent(ndChoice.Branches[num]);
				if (possibleXmlStates != this.xstates)
				{
					possibleXmlStates = PossibleXmlStates.Any;
				}
			}
			this.xstates = possibleXmlStates;
		}

		protected virtual void AnalyzeCopy(QilNode ndCopy, XmlILConstructInfo info)
		{
			XmlQueryType xmlType = ndCopy.XmlType;
			if (!xmlType.IsSingleton)
			{
				this.StartLoop(xmlType, info);
			}
			if (this.MaybeContent(xmlType))
			{
				if (this.MaybeAttrNmsp(xmlType))
				{
					if (this.xstates == PossibleXmlStates.EnumAttrs)
					{
						this.xstates = PossibleXmlStates.Any;
					}
				}
				else if (this.xstates == PossibleXmlStates.EnumAttrs || this.withinElem)
				{
					this.xstates = PossibleXmlStates.WithinContent;
				}
			}
			if (!xmlType.IsSingleton)
			{
				this.EndLoop(xmlType, info);
			}
		}

		private void StartLoop(XmlQueryType typ, XmlILConstructInfo info)
		{
			info.BeginLoopStates = this.xstates;
			if (typ.MaybeMany && this.xstates == PossibleXmlStates.EnumAttrs && this.MaybeContent(typ))
			{
				info.BeginLoopStates = (this.xstates = PossibleXmlStates.Any);
			}
		}

		private void EndLoop(XmlQueryType typ, XmlILConstructInfo info)
		{
			info.EndLoopStates = this.xstates;
			if (typ.MaybeEmpty && info.InitialStates != this.xstates)
			{
				this.xstates = PossibleXmlStates.Any;
			}
		}

		private bool MaybeAttrNmsp(XmlQueryType typ)
		{
			return (typ.NodeKinds & (XmlNodeKindFlags.Attribute | XmlNodeKindFlags.Namespace)) > XmlNodeKindFlags.None;
		}

		private bool MaybeContent(XmlQueryType typ)
		{
			return !typ.IsNode || (typ.NodeKinds & ~(XmlNodeKindFlags.Attribute | XmlNodeKindFlags.Namespace)) > XmlNodeKindFlags.None;
		}

		protected XmlILConstructInfo parentInfo;

		protected QilFactory fac;

		protected PossibleXmlStates xstates;

		protected bool withinElem;
	}
}
