using System;

namespace System.Xml.Linq
{
	internal struct NamespaceResolver
	{
		public void PushScope()
		{
			this._scope++;
		}

		public void PopScope()
		{
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = this._declaration;
			if (namespaceDeclaration != null)
			{
				do
				{
					namespaceDeclaration = namespaceDeclaration.prev;
					if (namespaceDeclaration.scope != this._scope)
					{
						break;
					}
					if (namespaceDeclaration == this._declaration)
					{
						this._declaration = null;
					}
					else
					{
						this._declaration.prev = namespaceDeclaration.prev;
					}
					this._rover = null;
				}
				while (namespaceDeclaration != this._declaration && this._declaration != null);
			}
			this._scope--;
		}

		public void Add(string prefix, XNamespace ns)
		{
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
			namespaceDeclaration.prefix = prefix;
			namespaceDeclaration.ns = ns;
			namespaceDeclaration.scope = this._scope;
			if (this._declaration == null)
			{
				this._declaration = namespaceDeclaration;
			}
			else
			{
				namespaceDeclaration.prev = this._declaration.prev;
			}
			this._declaration.prev = namespaceDeclaration;
			this._rover = null;
		}

		public void AddFirst(string prefix, XNamespace ns)
		{
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
			namespaceDeclaration.prefix = prefix;
			namespaceDeclaration.ns = ns;
			namespaceDeclaration.scope = this._scope;
			if (this._declaration == null)
			{
				namespaceDeclaration.prev = namespaceDeclaration;
			}
			else
			{
				namespaceDeclaration.prev = this._declaration.prev;
				this._declaration.prev = namespaceDeclaration;
			}
			this._declaration = namespaceDeclaration;
			this._rover = null;
		}

		public string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
		{
			if (this._rover != null && this._rover.ns == ns && (allowDefaultNamespace || this._rover.prefix.Length > 0))
			{
				return this._rover.prefix;
			}
			NamespaceResolver.NamespaceDeclaration namespaceDeclaration = this._declaration;
			if (namespaceDeclaration != null)
			{
				for (;;)
				{
					namespaceDeclaration = namespaceDeclaration.prev;
					if (namespaceDeclaration.ns == ns)
					{
						NamespaceResolver.NamespaceDeclaration namespaceDeclaration2 = this._declaration.prev;
						while (namespaceDeclaration2 != namespaceDeclaration && namespaceDeclaration2.prefix != namespaceDeclaration.prefix)
						{
							namespaceDeclaration2 = namespaceDeclaration2.prev;
						}
						if (namespaceDeclaration2 == namespaceDeclaration)
						{
							if (allowDefaultNamespace)
							{
								break;
							}
							if (namespaceDeclaration.prefix.Length > 0)
							{
								goto Block_8;
							}
						}
					}
					if (namespaceDeclaration == this._declaration)
					{
						goto IL_00BB;
					}
				}
				this._rover = namespaceDeclaration;
				return namespaceDeclaration.prefix;
				Block_8:
				return namespaceDeclaration.prefix;
			}
			IL_00BB:
			return null;
		}

		private int _scope;

		private NamespaceResolver.NamespaceDeclaration _declaration;

		private NamespaceResolver.NamespaceDeclaration _rover;

		private class NamespaceDeclaration
		{
			public string prefix;

			public XNamespace ns;

			public int scope;

			public NamespaceResolver.NamespaceDeclaration prev;
		}
	}
}
