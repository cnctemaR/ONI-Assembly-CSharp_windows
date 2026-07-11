using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class NamespaceDecl
	{
		internal string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		internal string Uri
		{
			get
			{
				return this.nsUri;
			}
		}

		internal string PrevDefaultNsUri
		{
			get
			{
				return this.prevDefaultNsUri;
			}
		}

		internal NamespaceDecl Next
		{
			get
			{
				return this.next;
			}
		}

		internal NamespaceDecl(string prefix, string nsUri, string prevDefaultNsUri, NamespaceDecl next)
		{
			this.Init(prefix, nsUri, prevDefaultNsUri, next);
		}

		internal void Init(string prefix, string nsUri, string prevDefaultNsUri, NamespaceDecl next)
		{
			this.prefix = prefix;
			this.nsUri = nsUri;
			this.prevDefaultNsUri = prevDefaultNsUri;
			this.next = next;
		}

		private string prefix;

		private string nsUri;

		private string prevDefaultNsUri;

		private NamespaceDecl next;
	}
}
