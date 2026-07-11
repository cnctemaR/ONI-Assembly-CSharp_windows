using System;
using System.Collections;

namespace System.Xml.Linq
{
	internal struct Inserter
	{
		public Inserter(XContainer parent, XNode anchor)
		{
			this.parent = parent;
			this.previous = anchor;
			this.text = null;
		}

		public void Add(object content)
		{
			this.AddContent(content);
			if (this.text != null)
			{
				if (this.parent.content == null)
				{
					if (this.parent.SkipNotify())
					{
						this.parent.content = this.text;
						return;
					}
					if (this.text.Length > 0)
					{
						this.InsertNode(new XText(this.text));
						return;
					}
					if (!(this.parent is XElement))
					{
						this.parent.content = this.text;
						return;
					}
					this.parent.NotifyChanging(this.parent, XObjectChangeEventArgs.Value);
					if (this.parent.content != null)
					{
						throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
					}
					this.parent.content = this.text;
					this.parent.NotifyChanged(this.parent, XObjectChangeEventArgs.Value);
					return;
				}
				else if (this.text.Length > 0)
				{
					if (this.previous is XText && !(this.previous is XCData))
					{
						XText xtext = (XText)this.previous;
						xtext.Value += this.text;
						return;
					}
					this.parent.ConvertTextToNode();
					this.InsertNode(new XText(this.text));
				}
			}
		}

		private void AddContent(object content)
		{
			if (content == null)
			{
				return;
			}
			XNode xnode = content as XNode;
			if (xnode != null)
			{
				this.AddNode(xnode);
				return;
			}
			string text = content as string;
			if (text != null)
			{
				this.AddString(text);
				return;
			}
			XStreamingElement xstreamingElement = content as XStreamingElement;
			if (xstreamingElement != null)
			{
				this.AddNode(new XElement(xstreamingElement));
				return;
			}
			object[] array = content as object[];
			if (array != null)
			{
				foreach (object obj in array)
				{
					this.AddContent(obj);
				}
				return;
			}
			IEnumerable enumerable = content as IEnumerable;
			if (enumerable != null)
			{
				foreach (object obj2 in enumerable)
				{
					this.AddContent(obj2);
				}
				return;
			}
			if (content is XAttribute)
			{
				throw new ArgumentException(Res.GetString("Argument_AddAttribute"));
			}
			this.AddString(XContainer.GetStringValue(content));
		}

		private void AddNode(XNode n)
		{
			this.parent.ValidateNode(n, this.previous);
			if (n.parent != null)
			{
				n = n.CloneNode();
			}
			else
			{
				XNode xnode = this.parent;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
				}
				if (n == xnode)
				{
					n = n.CloneNode();
				}
			}
			this.parent.ConvertTextToNode();
			if (this.text != null)
			{
				if (this.text.Length > 0)
				{
					if (this.previous is XText && !(this.previous is XCData))
					{
						XText xtext = (XText)this.previous;
						xtext.Value += this.text;
					}
					else
					{
						this.InsertNode(new XText(this.text));
					}
				}
				this.text = null;
			}
			this.InsertNode(n);
		}

		private void AddString(string s)
		{
			this.parent.ValidateString(s);
			this.text += s;
		}

		private void InsertNode(XNode n)
		{
			bool flag = this.parent.NotifyChanging(n, XObjectChangeEventArgs.Add);
			if (n.parent != null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			n.parent = this.parent;
			if (this.parent.content == null || this.parent.content is string)
			{
				n.next = n;
				this.parent.content = n;
			}
			else if (this.previous == null)
			{
				XNode xnode = (XNode)this.parent.content;
				n.next = xnode.next;
				xnode.next = n;
			}
			else
			{
				n.next = this.previous.next;
				this.previous.next = n;
				if (this.parent.content == this.previous)
				{
					this.parent.content = n;
				}
			}
			this.previous = n;
			if (flag)
			{
				this.parent.NotifyChanged(n, XObjectChangeEventArgs.Add);
			}
		}

		private XContainer parent;

		private XNode previous;

		private string text;
	}
}
