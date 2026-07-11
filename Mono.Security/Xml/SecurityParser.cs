using System;
using System.Collections;
using System.Security;

namespace Mono.Xml
{
	[CLSCompliant(false)]
	public class SecurityParser : MiniParser, MiniParser.IHandler, MiniParser.IReader
	{
		public SecurityParser()
		{
			this.stack = new Stack();
		}

		public void LoadXml(string xml)
		{
			this.root = null;
			this.xmldoc = xml;
			this.pos = 0;
			this.stack.Clear();
			base.Parse(this, this);
		}

		public SecurityElement ToXml()
		{
			return this.root;
		}

		public int Read()
		{
			if (this.pos >= this.xmldoc.Length)
			{
				return -1;
			}
			string text = this.xmldoc;
			int num = this.pos;
			this.pos = num + 1;
			return (int)text[num];
		}

		public void OnStartParsing(MiniParser parser)
		{
		}

		public void OnStartElement(string name, MiniParser.IAttrList attrs)
		{
			SecurityElement securityElement = new SecurityElement(name);
			if (this.root == null)
			{
				this.root = securityElement;
				this.current = securityElement;
			}
			else
			{
				((SecurityElement)this.stack.Peek()).AddChild(securityElement);
			}
			this.stack.Push(securityElement);
			this.current = securityElement;
			int length = attrs.Length;
			for (int i = 0; i < length; i++)
			{
				this.current.AddAttribute(attrs.GetName(i), SecurityElement.Escape(attrs.GetValue(i)));
			}
		}

		public void OnEndElement(string name)
		{
			this.current = (SecurityElement)this.stack.Pop();
		}

		public void OnChars(string ch)
		{
			this.current.Text = SecurityElement.Escape(ch);
		}

		public void OnEndParsing(MiniParser parser)
		{
		}

		private SecurityElement root;

		private string xmldoc;

		private int pos;

		private SecurityElement current;

		private Stack stack;
	}
}
