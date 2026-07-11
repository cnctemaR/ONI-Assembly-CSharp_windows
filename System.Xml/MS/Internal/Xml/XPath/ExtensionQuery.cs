using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal abstract class ExtensionQuery : Query
	{
		public ExtensionQuery(string prefix, string name)
		{
			this.prefix = prefix;
			this.name = name;
		}

		protected ExtensionQuery(ExtensionQuery other)
			: base(other)
		{
			this.prefix = other.prefix;
			this.name = other.name;
			this.xsltContext = other.xsltContext;
			this.queryIterator = (ResetableIterator)Query.Clone(other.queryIterator);
		}

		public override void Reset()
		{
			if (this.queryIterator != null)
			{
				this.queryIterator.Reset();
			}
		}

		public override XPathNavigator Current
		{
			get
			{
				if (this.queryIterator == null)
				{
					throw XPathException.Create("Expression must evaluate to a node-set.");
				}
				if (this.queryIterator.CurrentPosition == 0)
				{
					this.Advance();
				}
				return this.queryIterator.Current;
			}
		}

		public override XPathNavigator Advance()
		{
			if (this.queryIterator == null)
			{
				throw XPathException.Create("Expression must evaluate to a node-set.");
			}
			if (this.queryIterator.MoveNext())
			{
				return this.queryIterator.Current;
			}
			return null;
		}

		public override int CurrentPosition
		{
			get
			{
				if (this.queryIterator != null)
				{
					return this.queryIterator.CurrentPosition;
				}
				return 0;
			}
		}

		protected object ProcessResult(object value)
		{
			if (value is string)
			{
				return value;
			}
			if (value is double)
			{
				return value;
			}
			if (value is bool)
			{
				return value;
			}
			if (value is XPathNavigator)
			{
				return value;
			}
			if (value is int)
			{
				return (double)((int)value);
			}
			if (value == null)
			{
				this.queryIterator = XPathEmptyIterator.Instance;
				return this;
			}
			ResetableIterator resetableIterator = value as ResetableIterator;
			if (resetableIterator != null)
			{
				this.queryIterator = (ResetableIterator)resetableIterator.Clone();
				return this;
			}
			XPathNodeIterator xpathNodeIterator = value as XPathNodeIterator;
			if (xpathNodeIterator != null)
			{
				this.queryIterator = new XPathArrayIterator(xpathNodeIterator);
				return this;
			}
			IXPathNavigable ixpathNavigable = value as IXPathNavigable;
			if (ixpathNavigable != null)
			{
				return ixpathNavigable.CreateNavigator();
			}
			if (value is short)
			{
				return (double)((short)value);
			}
			if (value is long)
			{
				return (double)((long)value);
			}
			if (value is uint)
			{
				return (uint)value;
			}
			if (value is ushort)
			{
				return (double)((ushort)value);
			}
			if (value is ulong)
			{
				return (ulong)value;
			}
			if (value is float)
			{
				return (double)((float)value);
			}
			if (value is decimal)
			{
				return (double)((decimal)value);
			}
			return value.ToString();
		}

		protected string QName
		{
			get
			{
				if (this.prefix.Length == 0)
				{
					return this.name;
				}
				return this.prefix + ":" + this.name;
			}
		}

		public override int Count
		{
			get
			{
				if (this.queryIterator != null)
				{
					return this.queryIterator.Count;
				}
				return 1;
			}
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		protected string prefix;

		protected string name;

		protected XsltContext xsltContext;

		private ResetableIterator queryIterator;
	}
}
