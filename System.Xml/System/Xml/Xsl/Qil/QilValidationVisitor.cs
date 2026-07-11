using System;
using System.Diagnostics;
using System.Xml.Utils;

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
			message = Res.GetString("QIL Validation Error! '{0}'.", new object[] { message });
			string text = n.Annotation as string;
			if (text != null)
			{
				message = text + "\n" + message;
			}
			n.Annotation = message;
		}

		private SubstitutionList subs = new SubstitutionList();

		private QilTypeChecker typeCheck = new QilTypeChecker();
	}
}
