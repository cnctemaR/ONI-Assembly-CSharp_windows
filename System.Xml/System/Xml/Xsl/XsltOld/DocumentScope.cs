using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class DocumentScope
	{
		internal NamespaceDecl Scopes
		{
			get
			{
				return this.scopes;
			}
		}

		internal NamespaceDecl AddNamespace(string prefix, string uri, string prevDefaultNsUri)
		{
			this.scopes = new NamespaceDecl(prefix, uri, prevDefaultNsUri, this.scopes);
			return this.scopes;
		}

		internal string ResolveAtom(string prefix)
		{
			for (NamespaceDecl next = this.scopes; next != null; next = next.Next)
			{
				if (Ref.Equal(next.Prefix, prefix))
				{
					return next.Uri;
				}
			}
			return null;
		}

		internal string ResolveNonAtom(string prefix)
		{
			for (NamespaceDecl next = this.scopes; next != null; next = next.Next)
			{
				if (next.Prefix == prefix)
				{
					return next.Uri;
				}
			}
			return null;
		}

		protected NamespaceDecl scopes;
	}
}
