using System;
using System.Collections.Generic;

namespace System.Xml.Xsl.Qil
{
	internal class QilDepthChecker
	{
		public static void Check(QilNode input)
		{
			if (LocalAppContextSwitches.LimitXPathComplexity)
			{
				new QilDepthChecker().Check(input, 0);
			}
		}

		private void Check(QilNode input, int depth)
		{
			if (depth > 800)
			{
				throw XsltException.Create("The stylesheet is too complex.", Array.Empty<string>());
			}
			if (input is QilReference)
			{
				if (this._visitedRef.ContainsKey(input))
				{
					return;
				}
				this._visitedRef[input] = true;
			}
			int num = depth + 1;
			for (int i = 0; i < input.Count; i++)
			{
				QilNode qilNode = input[i];
				if (qilNode != null)
				{
					this.Check(qilNode, num);
				}
			}
		}

		private const int MAX_QIL_DEPTH = 800;

		private Dictionary<QilNode, bool> _visitedRef = new Dictionary<QilNode, bool>();
	}
}
