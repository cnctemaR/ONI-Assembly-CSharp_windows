using System;
using System.Collections;
using System.Xml.Xsl;

namespace System.Xml.XPath
{
	internal static class ExpressionCache
	{
		public static XPathExpression Get(string xpath, IStaticXsltContext ctx)
		{
			object obj = ((ctx == null) ? ExpressionCache.dummy : ctx);
			object obj2 = ExpressionCache.cache_lock;
			lock (obj2)
			{
				WeakReference weakReference = ExpressionCache.table_per_ctx[obj] as WeakReference;
				if (weakReference == null)
				{
					return null;
				}
				Hashtable hashtable = weakReference.Target as Hashtable;
				if (hashtable == null)
				{
					ExpressionCache.table_per_ctx[obj] = null;
					return null;
				}
				weakReference = hashtable[xpath] as WeakReference;
				if (weakReference != null)
				{
					XPathExpression xpathExpression = weakReference.Target as XPathExpression;
					if (xpathExpression != null)
					{
						return xpathExpression;
					}
					hashtable[xpath] = null;
				}
			}
			return null;
		}

		public static void Set(string xpath, IStaticXsltContext ctx, XPathExpression exp)
		{
			object obj = ((ctx == null) ? ExpressionCache.dummy : ctx);
			Hashtable hashtable = null;
			object obj2 = ExpressionCache.cache_lock;
			lock (obj2)
			{
				WeakReference weakReference = ExpressionCache.table_per_ctx[obj] as WeakReference;
				if (weakReference != null && weakReference.IsAlive)
				{
					hashtable = (Hashtable)weakReference.Target;
				}
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					ExpressionCache.table_per_ctx[obj] = new WeakReference(hashtable);
				}
				hashtable[xpath] = new WeakReference(exp);
			}
		}

		private static readonly Hashtable table_per_ctx = new Hashtable();

		private static object dummy = new object();

		private static object cache_lock = new object();
	}
}
