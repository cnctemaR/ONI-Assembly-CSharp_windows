using System;

namespace System.Xml.Linq
{
	internal struct NamespaceResolver
	{
		public void PushScope()
		{
			this.scope++;
		}

		public void PopScope()
		{
			NamespaceResolver.NamespaceDeclaration prev = this.declaration;
			if (prev != null)
			{
				do
				{
					prev = prev.prev;
					if (prev.scope != this.scope)
					{
						break;
					}
					if (prev == this.declaration)
					{
						this.declaration = null;
					}
					else
					{
						this.declaration.prev = prev.prev;
					}
					this.rover = null;
				}
				while (prev != this.declaration && this.declaration != null);
			}
			this.scope--;
		}

		public void Add(string prefix, XNamespace ns)
		{
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
			namespaceDeclaration.prefix = prefix;
			namespaceDeclaration.ns = ns;
			namespaceDeclaration.scope = this.scope;
			if (this.declaration == null)
			{
				this.declaration = namespaceDeclaration;
			}
			else
			{
				namespaceDeclaration.prev = this.declaration.prev;
			}
			this.declaration.prev = namespaceDeclaration;
			this.rover = null;
		}

		public void AddFirst(string prefix, XNamespace ns)
		{
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
			namespaceDeclaration.prefix = prefix;
			namespaceDeclaration.ns = ns;
			namespaceDeclaration.scope = this.scope;
			if (this.declaration == null)
			{
				namespaceDeclaration.prev = namespaceDeclaration;
			}
			else
			{
				namespaceDeclaration.prev = this.declaration.prev;
				this.declaration.prev = namespaceDeclaration;
			}
			this.declaration = namespaceDeclaration;
			this.rover = null;
		}

		public string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
		{
			if (this.rover != null && this.rover.ns == ns && (allowDefaultNamespace || this.rover.prefix.Length > 0))
			{
				return this.rover.prefix;
			}
			NamespaceResolver.NamespaceDeclaration prev = this.declaration;
			if (prev != null)
			{
				for (;;)
				{
					prev = prev.prev;
					if (prev.ns == ns)
					{
						NamespaceResolver.NamespaceDeclaration namespaceDeclaration = this.declaration.prev;
						while (namespaceDeclaration != prev && namespaceDeclaration.prefix != prev.prefix)
						{
							namespaceDeclaration = namespaceDeclaration.prev;
						}
						if (namespaceDeclaration == prev)
						{
							if (allowDefaultNamespace)
							{
								break;
							}
							if (prev.prefix.Length > 0)
							{
								goto Block_8;
							}
						}
					}
					if (prev == this.declaration)
					{
						goto IL_00BB;
					}
				}
				this.rover = prev;
				return prev.prefix;
				Block_8:
				return prev.prefix;
			}
			IL_00BB:
			return null;
		}

		private int scope;

		private NamespaceResolver.NamespaceDeclaration declaration;

		private NamespaceResolver.NamespaceDeclaration rover;

		private class NamespaceDeclaration
		{
			public string prefix;

			public XNamespace ns;

			public int scope;

			public NamespaceResolver.NamespaceDeclaration prev;
		}
	}
}
