using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal class MatcherBuilder
	{
		public MatcherBuilder(XPathQilFactory f, ReferenceReplacer refReplacer, InvokeGenerator invkGen)
		{
			this.f = f;
			this.refReplacer = refReplacer;
			this.invkGen = invkGen;
		}

		private void Clear()
		{
			this.priority = -1;
			this.elementPatterns.Clear();
			this.attributePatterns.Clear();
			this.textPatterns.Clear();
			this.documentPatterns.Clear();
			this.commentPatterns.Clear();
			this.piPatterns.Clear();
			this.heterogenousPatterns.Clear();
			this.allMatches.Clear();
		}

		private void AddPatterns(List<TemplateMatch> matches)
		{
			foreach (TemplateMatch templateMatch in matches)
			{
				TemplateMatch templateMatch2 = templateMatch;
				int num = this.priority + 1;
				this.priority = num;
				Pattern pattern = new Pattern(templateMatch2, num);
				XmlNodeKindFlags nodeKind = templateMatch.NodeKind;
				if (nodeKind <= XmlNodeKindFlags.Text)
				{
					switch (nodeKind)
					{
					case XmlNodeKindFlags.Document:
						this.documentPatterns.Add(pattern);
						continue;
					case XmlNodeKindFlags.Element:
						this.elementPatterns.Add(pattern);
						continue;
					case XmlNodeKindFlags.Document | XmlNodeKindFlags.Element:
						break;
					case XmlNodeKindFlags.Attribute:
						this.attributePatterns.Add(pattern);
						continue;
					default:
						if (nodeKind == XmlNodeKindFlags.Text)
						{
							this.textPatterns.Add(pattern);
							continue;
						}
						break;
					}
				}
				else
				{
					if (nodeKind == XmlNodeKindFlags.Comment)
					{
						this.commentPatterns.Add(pattern);
						continue;
					}
					if (nodeKind == XmlNodeKindFlags.PI)
					{
						this.piPatterns.Add(pattern);
						continue;
					}
				}
				this.heterogenousPatterns.Add(pattern);
			}
		}

		private void CollectPatternsInternal(Stylesheet sheet, QilName mode)
		{
			foreach (Stylesheet stylesheet in sheet.Imports)
			{
				this.CollectPatternsInternal(stylesheet, mode);
			}
			List<TemplateMatch> list;
			if (sheet.TemplateMatches.TryGetValue(mode, out list))
			{
				this.AddPatterns(list);
				this.allMatches.Add(list);
			}
		}

		public void CollectPatterns(StylesheetLevel sheet, QilName mode)
		{
			this.Clear();
			foreach (Stylesheet stylesheet in sheet.Imports)
			{
				this.CollectPatternsInternal(stylesheet, mode);
			}
		}

		private QilNode MatchPattern(QilIterator it, TemplateMatch match)
		{
			QilNode qilNode = match.Condition;
			if (qilNode == null)
			{
				return this.f.True();
			}
			qilNode = qilNode.DeepClone(this.f.BaseFactory);
			return this.refReplacer.Replace(qilNode, match.Iterator, it);
		}

		private QilNode MatchPatterns(QilIterator it, List<Pattern> patternList)
		{
			QilNode qilNode = this.f.Int32(-1);
			foreach (Pattern pattern in patternList)
			{
				qilNode = this.f.Conditional(this.MatchPattern(it, pattern.Match), this.f.Int32(pattern.Priority), qilNode);
			}
			return qilNode;
		}

		private QilNode MatchPatterns(QilIterator it, XmlQueryType xt, List<Pattern> patternList, QilNode otherwise)
		{
			if (patternList.Count == 0)
			{
				return otherwise;
			}
			return this.f.Conditional(this.f.IsType(it, xt), this.MatchPatterns(it, patternList), otherwise);
		}

		private bool IsNoMatch(QilNode matcher)
		{
			return matcher.NodeType == QilNodeType.LiteralInt32;
		}

		private QilNode MatchPatternsWhosePriorityGreater(QilIterator it, List<Pattern> patternList, QilNode matcher)
		{
			if (patternList.Count == 0)
			{
				return matcher;
			}
			if (this.IsNoMatch(matcher))
			{
				return this.MatchPatterns(it, patternList);
			}
			QilIterator qilIterator = this.f.Let(matcher);
			QilNode qilNode = this.f.Int32(-1);
			int num = -1;
			foreach (Pattern pattern in patternList)
			{
				if (pattern.Priority > num + 1)
				{
					qilNode = this.f.Conditional(this.f.Gt(qilIterator, this.f.Int32(num)), qilIterator, qilNode);
				}
				qilNode = this.f.Conditional(this.MatchPattern(it, pattern.Match), this.f.Int32(pattern.Priority), qilNode);
				num = pattern.Priority;
			}
			if (num != this.priority)
			{
				qilNode = this.f.Conditional(this.f.Gt(qilIterator, this.f.Int32(num)), qilIterator, qilNode);
			}
			return this.f.Loop(qilIterator, qilNode);
		}

		private QilNode MatchPatterns(QilIterator it, XmlQueryType xt, PatternBag patternBag, QilNode otherwise)
		{
			if (patternBag.FixedNamePatternsNames.Count == 0)
			{
				return this.MatchPatterns(it, xt, patternBag.NonFixedNamePatterns, otherwise);
			}
			QilNode qilNode = this.f.Int32(-1);
			foreach (QilName qilName in patternBag.FixedNamePatternsNames)
			{
				qilNode = this.f.Conditional(this.f.Eq(this.f.NameOf(it), qilName.ShallowClone(this.f.BaseFactory)), this.MatchPatterns(it, patternBag.FixedNamePatterns[qilName]), qilNode);
			}
			qilNode = this.MatchPatternsWhosePriorityGreater(it, patternBag.NonFixedNamePatterns, qilNode);
			return this.f.Conditional(this.f.IsType(it, xt), qilNode, otherwise);
		}

		public QilNode BuildMatcher(QilIterator it, IList<XslNode> actualArgs, QilNode otherwise)
		{
			QilNode qilNode = this.f.Int32(-1);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.PI, this.piPatterns, qilNode);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.Comment, this.commentPatterns, qilNode);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.Document, this.documentPatterns, qilNode);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.Text, this.textPatterns, qilNode);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.Attribute, this.attributePatterns, qilNode);
			qilNode = this.MatchPatterns(it, XmlQueryTypeFactory.Element, this.elementPatterns, qilNode);
			qilNode = this.MatchPatternsWhosePriorityGreater(it, this.heterogenousPatterns, qilNode);
			if (this.IsNoMatch(qilNode))
			{
				return otherwise;
			}
			QilNode[] array = new QilNode[this.priority + 2];
			int num = -1;
			foreach (List<TemplateMatch> list in this.allMatches)
			{
				foreach (TemplateMatch templateMatch in list)
				{
					array[++num] = this.invkGen.GenerateInvoke(templateMatch.TemplateFunction, actualArgs);
				}
			}
			array[++num] = otherwise;
			return this.f.Choice(qilNode, this.f.BranchList(array));
		}

		private XPathQilFactory f;

		private ReferenceReplacer refReplacer;

		private InvokeGenerator invkGen;

		private const int NoMatch = -1;

		private int priority = -1;

		private PatternBag elementPatterns = new PatternBag();

		private PatternBag attributePatterns = new PatternBag();

		private List<Pattern> textPatterns = new List<Pattern>();

		private List<Pattern> documentPatterns = new List<Pattern>();

		private List<Pattern> commentPatterns = new List<Pattern>();

		private PatternBag piPatterns = new PatternBag();

		private List<Pattern> heterogenousPatterns = new List<Pattern>();

		private List<List<TemplateMatch>> allMatches = new List<List<TemplateMatch>>();
	}
}
