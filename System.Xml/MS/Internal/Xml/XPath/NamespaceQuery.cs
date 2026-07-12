using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class NamespaceQuery : BaseAxisQuery
	{
		public NamespaceQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type)
			: base(qyParent, Name, Prefix, Type)
		{
		}

		private NamespaceQuery(NamespaceQuery other)
			: base(other)
		{
			this._onNamespace = other._onNamespace;
		}

		public override void Reset()
		{
			this._onNamespace = false;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (!this._onNamespace)
				{
					this.currentNode = this.qyInput.Advance();
					if (this.currentNode == null)
					{
						break;
					}
					this.position = 0;
					this.currentNode = this.currentNode.Clone();
					this._onNamespace = this.currentNode.MoveToFirstNamespace();
				}
				else
				{
					this._onNamespace = this.currentNode.MoveToNextNamespace();
				}
				if (this._onNamespace && this.matches(this.currentNode))
				{
					goto Block_3;
				}
			}
			return null;
			Block_3:
			this.position++;
			return this.currentNode;
		}

		public override bool matches(XPathNavigator e)
		{
			return e.Value.Length != 0 && (!base.NameTest || base.Name.Equals(e.LocalName));
		}

		public override XPathNodeIterator Clone()
		{
			return new NamespaceQuery(this);
		}

		private bool _onNamespace;
	}
}
