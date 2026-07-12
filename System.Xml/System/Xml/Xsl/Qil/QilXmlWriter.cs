using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Xml.Xsl.Qil
{
	internal class QilXmlWriter : QilScopedVisitor
	{
		public QilXmlWriter(XmlWriter writer)
			: this(writer, QilXmlWriter.Options.Annotations | QilXmlWriter.Options.TypeInfo | QilXmlWriter.Options.LineInfo | QilXmlWriter.Options.NodeIdentity | QilXmlWriter.Options.NodeLocation)
		{
		}

		public QilXmlWriter(XmlWriter writer, QilXmlWriter.Options options)
		{
			this.writer = writer;
			this._ngen = new QilXmlWriter.NameGenerator();
			this.options = options;
		}

		public void ToXml(QilNode node)
		{
			this.VisitAssumeReference(node);
		}

		protected virtual void WriteAnnotations(object ann)
		{
			string text = null;
			string text2 = null;
			if (ann == null)
			{
				return;
			}
			if (ann is string)
			{
				text = ann as string;
			}
			else if (ann is IQilAnnotation)
			{
				text2 = (ann as IQilAnnotation).Name;
				text = ann.ToString();
			}
			else if (ann is IList<object>)
			{
				foreach (object obj in ((IList<object>)ann))
				{
					this.WriteAnnotations(obj);
				}
				return;
			}
			if (text != null && text.Length != 0)
			{
				this.writer.WriteComment((text2 != null && text2.Length != 0) ? (text2 + ": " + text) : text);
			}
		}

		protected virtual void WriteLineInfo(QilNode node)
		{
			this.writer.WriteAttributeString("lineInfo", string.Format(CultureInfo.InvariantCulture, "[{0},{1} -- {2},{3}]", new object[]
			{
				node.SourceLine.Start.Line,
				node.SourceLine.Start.Pos,
				node.SourceLine.End.Line,
				node.SourceLine.End.Pos
			}));
		}

		protected virtual void WriteXmlType(QilNode node)
		{
			this.writer.WriteAttributeString("xmlType", node.XmlType.ToString(((this.options & QilXmlWriter.Options.RoundTripTypeInfo) != QilXmlWriter.Options.None) ? "S" : "G"));
		}

		protected override QilNode VisitChildren(QilNode node)
		{
			if (node is QilLiteral)
			{
				this.writer.WriteValue(Convert.ToString(((QilLiteral)node).Value, CultureInfo.InvariantCulture));
				return node;
			}
			if (node is QilReference)
			{
				QilReference qilReference = (QilReference)node;
				this.writer.WriteAttributeString("id", this._ngen.NameOf(node));
				if (qilReference.DebugName != null)
				{
					this.writer.WriteAttributeString("name", qilReference.DebugName.ToString());
				}
				if (node.NodeType == QilNodeType.Parameter)
				{
					QilParameter qilParameter = (QilParameter)node;
					if (qilParameter.DefaultValue != null)
					{
						this.VisitAssumeReference(qilParameter.DefaultValue);
					}
					return node;
				}
			}
			return base.VisitChildren(node);
		}

		protected override QilNode VisitReference(QilNode node)
		{
			QilReference qilReference = (QilReference)node;
			string text = this._ngen.NameOf(node);
			if (text == null)
			{
				text = "OUT-OF-SCOPE REFERENCE";
			}
			this.writer.WriteStartElement("RefTo");
			this.writer.WriteAttributeString("id", text);
			if (qilReference.DebugName != null)
			{
				this.writer.WriteAttributeString("name", qilReference.DebugName.ToString());
			}
			this.writer.WriteEndElement();
			return node;
		}

		protected override QilNode VisitQilExpression(QilExpression qil)
		{
			IList<QilNode> list = new QilXmlWriter.ForwardRefFinder().Find(qil);
			if (list != null && list.Count > 0)
			{
				this.writer.WriteStartElement("ForwardDecls");
				foreach (QilNode qilNode in list)
				{
					this.writer.WriteStartElement(Enum.GetName(typeof(QilNodeType), qilNode.NodeType));
					this.writer.WriteAttributeString("id", this._ngen.NameOf(qilNode));
					this.WriteXmlType(qilNode);
					if (qilNode.NodeType == QilNodeType.Function)
					{
						this.Visit(qilNode[0]);
						this.Visit(qilNode[2]);
					}
					this.writer.WriteEndElement();
				}
				this.writer.WriteEndElement();
			}
			return this.VisitChildren(qil);
		}

		protected override QilNode VisitLiteralType(QilLiteral value)
		{
			this.writer.WriteString(value.ToString(((this.options & QilXmlWriter.Options.TypeInfo) != QilXmlWriter.Options.None) ? "G" : "S"));
			return value;
		}

		protected override QilNode VisitLiteralQName(QilName value)
		{
			this.writer.WriteAttributeString("name", value.ToString());
			return value;
		}

		protected override void BeginScope(QilNode node)
		{
			this._ngen.NameOf(node);
		}

		protected override void EndScope(QilNode node)
		{
			this._ngen.ClearName(node);
		}

		protected override void BeforeVisit(QilNode node)
		{
			base.BeforeVisit(node);
			if ((this.options & QilXmlWriter.Options.Annotations) != QilXmlWriter.Options.None)
			{
				this.WriteAnnotations(node.Annotation);
			}
			this.writer.WriteStartElement("", Enum.GetName(typeof(QilNodeType), node.NodeType), "");
			if ((this.options & (QilXmlWriter.Options.TypeInfo | QilXmlWriter.Options.RoundTripTypeInfo)) != QilXmlWriter.Options.None)
			{
				this.WriteXmlType(node);
			}
			if ((this.options & QilXmlWriter.Options.LineInfo) != QilXmlWriter.Options.None && node.SourceLine != null)
			{
				this.WriteLineInfo(node);
			}
		}

		protected override void AfterVisit(QilNode node)
		{
			this.writer.WriteEndElement();
			base.AfterVisit(node);
		}

		protected XmlWriter writer;

		protected QilXmlWriter.Options options;

		private QilXmlWriter.NameGenerator _ngen;

		[Flags]
		public enum Options
		{
			None = 0,
			Annotations = 1,
			TypeInfo = 2,
			RoundTripTypeInfo = 4,
			LineInfo = 8,
			NodeIdentity = 16,
			NodeLocation = 32
		}

		internal class ForwardRefFinder : QilVisitor
		{
			public IList<QilNode> Find(QilExpression qil)
			{
				this.Visit(qil);
				return this._fwdrefs;
			}

			protected override QilNode Visit(QilNode node)
			{
				if (node is QilIterator || node is QilFunction)
				{
					this._backrefs.Add(node);
				}
				return base.Visit(node);
			}

			protected override QilNode VisitReference(QilNode node)
			{
				if (!this._backrefs.Contains(node) && !this._fwdrefs.Contains(node))
				{
					this._fwdrefs.Add(node);
				}
				return node;
			}

			private List<QilNode> _fwdrefs = new List<QilNode>();

			private List<QilNode> _backrefs = new List<QilNode>();
		}

		private sealed class NameGenerator
		{
			public NameGenerator()
			{
				string text = "$";
				this._len = (this._zero = text.Length);
				this._start = 'a';
				this._end = 'z';
				this._name = new StringBuilder(text, this._len + 2);
				this._name.Append(this._start);
			}

			public string NextName()
			{
				string text = this._name.ToString();
				char c = this._name[this._len];
				if (c == this._end)
				{
					this._name[this._len] = this._start;
					int len = this._len;
					while (len-- > this._zero && this._name[len] == this._end)
					{
						this._name[len] = this._start;
					}
					if (len < this._zero)
					{
						this._len++;
						this._name.Append(this._start);
					}
					else
					{
						StringBuilder name = this._name;
						int num = len;
						char c2 = name[num];
						name[num] = c2 + '\u0001';
					}
				}
				else
				{
					this._name[this._len] = c + '\u0001';
				}
				return text;
			}

			public string NameOf(QilNode n)
			{
				object annotation = n.Annotation;
				QilXmlWriter.NameGenerator.NameAnnotation nameAnnotation = annotation as QilXmlWriter.NameGenerator.NameAnnotation;
				string text;
				if (nameAnnotation == null)
				{
					text = this.NextName();
					n.Annotation = new QilXmlWriter.NameGenerator.NameAnnotation(text, annotation);
				}
				else
				{
					text = nameAnnotation.Name;
				}
				return text;
			}

			public void ClearName(QilNode n)
			{
				if (n.Annotation is QilXmlWriter.NameGenerator.NameAnnotation)
				{
					n.Annotation = ((QilXmlWriter.NameGenerator.NameAnnotation)n.Annotation).PriorAnnotation;
				}
			}

			private StringBuilder _name;

			private int _len;

			private int _zero;

			private char _start;

			private char _end;

			private class NameAnnotation : ListBase<object>
			{
				public NameAnnotation(string s, object a)
				{
					this.Name = s;
					this.PriorAnnotation = a;
				}

				public override int Count
				{
					get
					{
						return 1;
					}
				}

				public override object this[int index]
				{
					get
					{
						if (index == 0)
						{
							return this.PriorAnnotation;
						}
						throw new IndexOutOfRangeException();
					}
					set
					{
						throw new NotSupportedException();
					}
				}

				public string Name;

				public object PriorAnnotation;
			}
		}
	}
}
