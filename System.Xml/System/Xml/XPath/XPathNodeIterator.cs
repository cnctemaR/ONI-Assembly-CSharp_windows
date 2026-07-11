using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace System.Xml.XPath
{
	[DebuggerDisplay("Position={CurrentPosition}, Current={debuggerDisplayProxy}")]
	public abstract class XPathNodeIterator : ICloneable, IEnumerable
	{
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public abstract XPathNodeIterator Clone();

		public abstract bool MoveNext();

		public abstract XPathNavigator Current { get; }

		public abstract int CurrentPosition { get; }

		public virtual int Count
		{
			get
			{
				if (this.count == -1)
				{
					XPathNodeIterator xpathNodeIterator = this.Clone();
					while (xpathNodeIterator.MoveNext())
					{
					}
					this.count = xpathNodeIterator.CurrentPosition;
				}
				return this.count;
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new XPathNodeIterator.Enumerator(this);
		}

		private object debuggerDisplayProxy
		{
			get
			{
				if (this.Current != null)
				{
					return new XPathNavigator.DebuggerDisplayProxy(this.Current);
				}
				return null;
			}
		}

		internal int count = -1;

		private class Enumerator : IEnumerator
		{
			public Enumerator(XPathNodeIterator original)
			{
				this.original = original.Clone();
			}

			public virtual object Current
			{
				get
				{
					if (!this.iterationStarted)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", new object[] { string.Empty }));
					}
					if (this.current == null)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", new object[] { string.Empty }));
					}
					return this.current.Current.Clone();
				}
			}

			public virtual bool MoveNext()
			{
				if (!this.iterationStarted)
				{
					this.current = this.original.Clone();
					this.iterationStarted = true;
				}
				if (this.current == null || !this.current.MoveNext())
				{
					this.current = null;
					return false;
				}
				return true;
			}

			public virtual void Reset()
			{
				this.iterationStarted = false;
			}

			private XPathNodeIterator original;

			private XPathNodeIterator current;

			private bool iterationStarted;
		}

		private struct DebuggerDisplayProxy
		{
			public DebuggerDisplayProxy(XPathNodeIterator nodeIterator)
			{
				this.nodeIterator = nodeIterator;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Position=");
				stringBuilder.Append(this.nodeIterator.CurrentPosition);
				stringBuilder.Append(", Current=");
				if (this.nodeIterator.Current == null)
				{
					stringBuilder.Append("null");
				}
				else
				{
					stringBuilder.Append('{');
					stringBuilder.Append(new XPathNavigator.DebuggerDisplayProxy(this.nodeIterator.Current).ToString());
					stringBuilder.Append('}');
				}
				return stringBuilder.ToString();
			}

			private XPathNodeIterator nodeIterator;
		}
	}
}
