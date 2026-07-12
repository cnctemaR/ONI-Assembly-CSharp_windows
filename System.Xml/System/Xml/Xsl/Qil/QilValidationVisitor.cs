using System;
using System.Diagnostics;

namespace System.Xml.Xsl.Qil
{
	internal class QilValidationVisitor : QilScopedVisitor
	{
		[Conditional("DEBUG")]
		public static void Validate(QilNode node)
		{
			new QilValidationVisitor().VisitAssumeReference(node);
		}

		protected QilValidationVisitor()
		{
		}

		[Conditional("DEBUG")]
		internal static void SetError(QilNode n, string message)
		{
			message = SR.Format("QIL Validation Error! '{0}'.", message);
			string text = n.Annotation as string;
			if (text != null)
			{
				message = text + "\n" + message;
			}
			n.Annotation = message;
		}

		private SubstitutionList _subs = new SubstitutionList();

		private QilTypeChecker _typeCheck = new QilTypeChecker();
	}
}
