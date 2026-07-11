using System;
using System.Diagnostics;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal struct SingletonFocus : IFocus
	{
		public SingletonFocus(XPathQilFactory f)
		{
			this.f = f;
			this.focusType = SingletonFocusType.None;
			this.current = null;
		}

		public void SetFocus(SingletonFocusType focusType)
		{
			this.focusType = focusType;
		}

		public void SetFocus(QilIterator current)
		{
			if (current != null)
			{
				this.focusType = SingletonFocusType.Iterator;
				this.current = current;
				return;
			}
			this.focusType = SingletonFocusType.None;
			this.current = null;
		}

		[Conditional("DEBUG")]
		private void CheckFocus()
		{
		}

		public QilNode GetCurrent()
		{
			SingletonFocusType singletonFocusType = this.focusType;
			if (singletonFocusType == SingletonFocusType.InitialDocumentNode)
			{
				return this.f.Root(this.f.XmlContext());
			}
			if (singletonFocusType != SingletonFocusType.InitialContextNode)
			{
				return this.current;
			}
			return this.f.XmlContext();
		}

		public QilNode GetPosition()
		{
			return this.f.Double(1.0);
		}

		public QilNode GetLast()
		{
			return this.f.Double(1.0);
		}

		private XPathQilFactory f;

		private SingletonFocusType focusType;

		private QilIterator current;
	}
}
