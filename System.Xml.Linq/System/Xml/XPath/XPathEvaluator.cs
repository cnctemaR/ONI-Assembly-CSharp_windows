using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	internal struct XPathEvaluator
	{
		public object Evaluate<T>(XNode node, string expression, IXmlNamespaceResolver resolver) where T : class
		{
			object obj = node.CreateNavigator().Evaluate(expression, resolver);
			if (obj is XPathNodeIterator)
			{
				return this.EvaluateIterator<T>((XPathNodeIterator)obj);
			}
			if (!(obj is T))
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedEvaluation", new object[] { obj.GetType() }));
			}
			return (T)((object)obj);
		}

		private IEnumerable<T> EvaluateIterator<T>(XPathNodeIterator result)
		{
			foreach (object obj in result)
			{
				XPathNavigator xpathNavigator = (XPathNavigator)obj;
				object r = xpathNavigator.UnderlyingObject;
				if (!(r is T))
				{
					throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedEvaluation", new object[] { r.GetType() }));
				}
				yield return (T)((object)r);
				XText t = r as XText;
				if (t != null && t.parent != null)
				{
					while (t != t.parent.content)
					{
						t = t.next as XText;
						if (t == null)
						{
							break;
						}
						yield return (T)((object)t);
					}
				}
				r = null;
				t = null;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}
	}
}
