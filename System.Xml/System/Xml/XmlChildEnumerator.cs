using System;
using System.Collections;

namespace System.Xml
{
	internal sealed class XmlChildEnumerator : IEnumerator
	{
		internal XmlChildEnumerator(XmlNode container)
		{
			this.container = container;
			this.child = container.FirstChild;
			this.isFirst = true;
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		internal bool MoveNext()
		{
			if (this.isFirst)
			{
				this.child = this.container.FirstChild;
				this.isFirst = false;
			}
			else if (this.child != null)
			{
				this.child = this.child.NextSibling;
			}
			return this.child != null;
		}

		void IEnumerator.Reset()
		{
			this.isFirst = true;
			this.child = this.container.FirstChild;
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		internal XmlNode Current
		{
			get
			{
				if (this.isFirst || this.child == null)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				return this.child;
			}
		}

		internal XmlNode container;

		internal XmlNode child;

		internal bool isFirst;
	}
}
