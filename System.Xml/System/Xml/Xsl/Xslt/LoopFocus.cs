using System;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal struct LoopFocus : IFocus
	{
		public LoopFocus(XPathQilFactory f)
		{
			this.f = f;
			this.current = (this.cached = (this.last = null));
		}

		public void SetFocus(QilIterator current)
		{
			this.current = current;
			this.cached = (this.last = null);
		}

		public bool IsFocusSet
		{
			get
			{
				return this.current != null;
			}
		}

		public QilNode GetCurrent()
		{
			return this.current;
		}

		public QilNode GetPosition()
		{
			return this.f.XsltConvert(this.f.PositionOf(this.current), XmlQueryTypeFactory.DoubleX);
		}

		public QilNode GetLast()
		{
			if (this.last == null)
			{
				this.last = this.f.Let(this.f.Double(0.0));
			}
			return this.last;
		}

		public void EnsureCache()
		{
			if (this.cached == null)
			{
				this.cached = this.f.Let(this.current.Binding);
				this.current.Binding = this.cached;
			}
		}

		public void Sort(QilNode sortKeys)
		{
			if (sortKeys != null)
			{
				this.EnsureCache();
				this.current = this.f.For(this.f.Sort(this.current, sortKeys));
			}
		}

		public QilLoop ConstructLoop(QilNode body)
		{
			if (this.last != null)
			{
				this.EnsureCache();
				this.last.Binding = this.f.XsltConvert(this.f.Length(this.cached), XmlQueryTypeFactory.DoubleX);
			}
			QilLoop qilLoop = this.f.BaseFactory.Loop(this.current, body);
			if (this.last != null)
			{
				qilLoop = this.f.BaseFactory.Loop(this.last, qilLoop);
			}
			if (this.cached != null)
			{
				qilLoop = this.f.BaseFactory.Loop(this.cached, qilLoop);
			}
			return qilLoop;
		}

		private XPathQilFactory f;

		private QilIterator current;

		private QilIterator cached;

		private QilIterator last;
	}
}
