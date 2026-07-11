using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml
{
	public class XmlNamespaceManager : IXmlNamespaceResolver, IEnumerable
	{
		internal static IXmlNamespaceResolver EmptyResolver
		{
			get
			{
				if (XmlNamespaceManager.s_EmptyResolver == null)
				{
					XmlNamespaceManager.s_EmptyResolver = new XmlNamespaceManager(new NameTable());
				}
				return XmlNamespaceManager.s_EmptyResolver;
			}
		}

		internal XmlNamespaceManager()
		{
		}

		public XmlNamespaceManager(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
			this.xml = nameTable.Add("xml");
			this.xmlNs = nameTable.Add("xmlns");
			this.nsdecls = new XmlNamespaceManager.NamespaceDeclaration[8];
			string text = nameTable.Add(string.Empty);
			this.nsdecls[0].Set(text, text, -1, -1);
			this.nsdecls[1].Set(this.xmlNs, nameTable.Add("http://www.w3.org/2000/xmlns/"), -1, -1);
			this.nsdecls[2].Set(this.xml, nameTable.Add("http://www.w3.org/XML/1998/namespace"), 0, -1);
			this.lastDecl = 2;
			this.scopeId = 1;
		}

		public virtual XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public virtual string DefaultNamespace
		{
			get
			{
				string text = this.LookupNamespace(string.Empty);
				if (text != null)
				{
					return text;
				}
				return string.Empty;
			}
		}

		public virtual void PushScope()
		{
			this.scopeId++;
		}

		public virtual bool PopScope()
		{
			int num = this.lastDecl;
			if (this.scopeId == 1)
			{
				return false;
			}
			while (this.nsdecls[num].scopeId == this.scopeId)
			{
				if (this.useHashtable)
				{
					this.hashTable[this.nsdecls[num].prefix] = this.nsdecls[num].previousNsIndex;
				}
				num--;
			}
			this.lastDecl = num;
			this.scopeId--;
			return true;
		}

		public virtual void AddNamespace(string prefix, string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			prefix = this.nameTable.Add(prefix);
			uri = this.nameTable.Add(uri);
			if (Ref.Equal(this.xml, prefix) && !uri.Equals("http://www.w3.org/XML/1998/namespace"))
			{
				throw new ArgumentException(Res.GetString("Prefix \"xml\" is reserved for use by XML and can be mapped only to namespace name \"http://www.w3.org/XML/1998/namespace\"."));
			}
			if (Ref.Equal(this.xmlNs, prefix))
			{
				throw new ArgumentException(Res.GetString("Prefix \"xmlns\" is reserved for use by XML."));
			}
			int num = this.LookupNamespaceDecl(prefix);
			int num2 = -1;
			if (num != -1)
			{
				if (this.nsdecls[num].scopeId == this.scopeId)
				{
					this.nsdecls[num].uri = uri;
					return;
				}
				num2 = num;
			}
			if (this.lastDecl == this.nsdecls.Length - 1)
			{
				XmlNamespaceManager.NamespaceDeclaration[] array = new XmlNamespaceManager.NamespaceDeclaration[this.nsdecls.Length * 2];
				Array.Copy(this.nsdecls, 0, array, 0, this.nsdecls.Length);
				this.nsdecls = array;
			}
			XmlNamespaceManager.NamespaceDeclaration[] array2 = this.nsdecls;
			int num3 = this.lastDecl + 1;
			this.lastDecl = num3;
			array2[num3].Set(prefix, uri, this.scopeId, num2);
			if (this.useHashtable)
			{
				this.hashTable[prefix] = this.lastDecl;
				return;
			}
			if (this.lastDecl >= 16)
			{
				this.hashTable = new Dictionary<string, int>(this.lastDecl);
				for (int i = 0; i <= this.lastDecl; i++)
				{
					this.hashTable[this.nsdecls[i].prefix] = i;
				}
				this.useHashtable = true;
			}
		}

		public virtual void RemoveNamespace(string prefix, string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			for (int num = this.LookupNamespaceDecl(prefix); num != -1; num = this.nsdecls[num].previousNsIndex)
			{
				if (string.Equals(this.nsdecls[num].uri, uri) && this.nsdecls[num].scopeId == this.scopeId)
				{
					this.nsdecls[num].uri = null;
				}
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(this.lastDecl + 1);
			for (int i = 0; i <= this.lastDecl; i++)
			{
				if (this.nsdecls[i].uri != null)
				{
					dictionary[this.nsdecls[i].prefix] = this.nsdecls[i].prefix;
				}
			}
			return dictionary.Keys.GetEnumerator();
		}

		public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			int i = 0;
			switch (scope)
			{
			case XmlNamespaceScope.All:
				i = 2;
				break;
			case XmlNamespaceScope.ExcludeXml:
				i = 3;
				break;
			case XmlNamespaceScope.Local:
				i = this.lastDecl;
				while (this.nsdecls[i].scopeId == this.scopeId)
				{
					i--;
				}
				i++;
				break;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(this.lastDecl - i + 1);
			while (i <= this.lastDecl)
			{
				string prefix = this.nsdecls[i].prefix;
				string uri = this.nsdecls[i].uri;
				if (uri != null)
				{
					if (uri.Length > 0 || prefix.Length > 0 || scope == XmlNamespaceScope.Local)
					{
						dictionary[prefix] = uri;
					}
					else
					{
						dictionary.Remove(prefix);
					}
				}
				i++;
			}
			return dictionary;
		}

		public virtual string LookupNamespace(string prefix)
		{
			int num = this.LookupNamespaceDecl(prefix);
			if (num != -1)
			{
				return this.nsdecls[num].uri;
			}
			return null;
		}

		private int LookupNamespaceDecl(string prefix)
		{
			if (!this.useHashtable)
			{
				for (int i = this.lastDecl; i >= 0; i--)
				{
					if (this.nsdecls[i].prefix == prefix && this.nsdecls[i].uri != null)
					{
						return i;
					}
				}
				for (int j = this.lastDecl; j >= 0; j--)
				{
					if (string.Equals(this.nsdecls[j].prefix, prefix) && this.nsdecls[j].uri != null)
					{
						return j;
					}
				}
				return -1;
			}
			int previousNsIndex;
			if (this.hashTable.TryGetValue(prefix, out previousNsIndex))
			{
				while (previousNsIndex != -1 && this.nsdecls[previousNsIndex].uri == null)
				{
					previousNsIndex = this.nsdecls[previousNsIndex].previousNsIndex;
				}
				return previousNsIndex;
			}
			return -1;
		}

		public virtual string LookupPrefix(string uri)
		{
			for (int i = this.lastDecl; i >= 0; i--)
			{
				if (string.Equals(this.nsdecls[i].uri, uri))
				{
					string prefix = this.nsdecls[i].prefix;
					if (string.Equals(this.LookupNamespace(prefix), uri))
					{
						return prefix;
					}
				}
			}
			return null;
		}

		public virtual bool HasNamespace(string prefix)
		{
			int num = this.lastDecl;
			while (this.nsdecls[num].scopeId == this.scopeId)
			{
				if (string.Equals(this.nsdecls[num].prefix, prefix) && this.nsdecls[num].uri != null)
				{
					return prefix.Length > 0 || this.nsdecls[num].uri.Length > 0;
				}
				num--;
			}
			return false;
		}

		internal bool GetNamespaceDeclaration(int idx, out string prefix, out string uri)
		{
			idx = this.lastDecl - idx;
			if (idx < 0)
			{
				string text;
				uri = (text = null);
				prefix = text;
				return false;
			}
			prefix = this.nsdecls[idx].prefix;
			uri = this.nsdecls[idx].uri;
			return true;
		}

		private static volatile IXmlNamespaceResolver s_EmptyResolver;

		private XmlNamespaceManager.NamespaceDeclaration[] nsdecls;

		private int lastDecl;

		private XmlNameTable nameTable;

		private int scopeId;

		private Dictionary<string, int> hashTable;

		private bool useHashtable;

		private string xml;

		private string xmlNs;

		private const int MinDeclsCountForHashtable = 16;

		private struct NamespaceDeclaration
		{
			public void Set(string prefix, string uri, int scopeId, int previousNsIndex)
			{
				this.prefix = prefix;
				this.uri = uri;
				this.scopeId = scopeId;
				this.previousNsIndex = previousNsIndex;
			}

			public string prefix;

			public string uri;

			public int scopeId;

			public int previousNsIndex;
		}
	}
}
