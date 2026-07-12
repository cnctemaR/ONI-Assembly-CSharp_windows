using System;
using System.Collections;

namespace System.Xml.Linq
{
	internal struct Inserter
	{
		public Inserter(XContainer parent, XNode anchor)
		{
			this._parent = parent;
			this._previous = anchor;
			this._text = null;
		}

		public void Add(object content)
		{
			this.AddContent(content);
			if (this._text != null)
			{
				if (this._parent.content == null)
				{
					if (this._parent.SkipNotify())
					{
						this._parent.content = this._text;
						return;
					}
					if (this._text.Length > 0)
					{
						this.InsertNode(new XText(this._text));
						return;
					}
					if (!(this._parent is XElement))
					{
						this._parent.content = this._text;
						return;
					}
					this._parent.NotifyChanging(this._parent, XObjectChangeEventArgs.Value);
					if (this._parent.content != null)
					{
						throw new InvalidOperationException("This operation was corrupted by external code.");
					}
					this._parent.content = this._text;
					this._parent.NotifyChanged(this._parent, XObjectChangeEventArgs.Value);
					return;
				}
				else if (this._text.Length > 0)
				{
					XText xtext = this._previous as XText;
					if (xtext != null && !(this._previous is XCData))
					{
						XText xtext2 = xtext;
						xtext2.Value += this._text;
						return;
					}
					this._parent.ConvertTextToNode();
					this.InsertNode(new XText(this._text));
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
				throw new ArgumentException("An attribute cannot be added to content.");
			}
			this.AddString(XContainer.GetStringValue(content));
		}

		private void AddNode(XNode n)
		{
			this._parent.ValidateNode(n, this._previous);
			if (n.parent != null)
			{
				n = n.CloneNode();
			}
			else
			{
				XNode xnode = this._parent;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
				}
				if (n == xnode)
				{
					n = n.CloneNode();
				}
			}
			this._parent.ConvertTextToNode();
			if (this._text != null)
			{
				if (this._text.Length > 0)
				{
					XText xtext = this._previous as XText;
					if (xtext != null && !(this._previous is XCData))
					{
						XText xtext2 = xtext;
						xtext2.Value += this._text;
					}
					else
					{
						this.InsertNode(new XText(this._text));
					}
				}
				this._text = null;
			}
			this.InsertNode(n);
		}

		private void AddString(string s)
		{
			this._parent.ValidateString(s);
			this._text += s;
		}

		private void InsertNode(XNode n)
		{
			bool flag = this._parent.NotifyChanging(n, XObjectChangeEventArgs.Add);
			if (n.parent != null)
			{
				throw new InvalidOperationException("This operation was corrupted by external code.");
			}
			n.parent = this._parent;
			if (this._parent.content == null || this._parent.content is string)
			{
				n.next = n;
				this._parent.content = n;
			}
			else if (this._previous == null)
			{
				XNode xnode = (XNode)this._parent.content;
				n.next = xnode.next;
				xnode.next = n;
			}
			else
			{
				n.next = this._previous.next;
				this._previous.next = n;
				if (this._parent.content == this._previous)
				{
					this._parent.content = n;
				}
			}
			this._previous = n;
			if (flag)
			{
				this._parent.NotifyChanged(n, XObjectChangeEventArgs.Add);
			}
		}

		private XContainer _parent;

		private XNode _previous;

		private string _text;
	}
}
