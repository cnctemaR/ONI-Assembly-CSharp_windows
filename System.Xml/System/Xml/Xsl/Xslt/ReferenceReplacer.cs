using System;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class ReferenceReplacer : QilReplaceVisitor
	{
		public ReferenceReplacer(QilFactory f)
			: base(f)
		{
		}

		public QilNode Replace(QilNode expr, QilReference lookFor, QilReference replaceBy)
		{
			QilDepthChecker.Check(expr);
			this.lookFor = lookFor;
			this.replaceBy = replaceBy;
			return this.VisitAssumeReference(expr);
		}

		protected override QilNode VisitReference(QilNode n)
		{
			if (n != this.lookFor)
			{
				return n;
			}
			return this.replaceBy;
		}

		private QilReference lookFor;

		private QilReference replaceBy;
	}
}
