using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class DesignerVerb : MenuCommand
	{
		public DesignerVerb(string text, EventHandler handler)
			: this(text, handler, StandardCommands.VerbFirst)
		{
		}

		public DesignerVerb(string text, EventHandler handler, CommandID startCommandID)
			: base(handler, startCommandID)
		{
			this.text = text;
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public override string ToString()
		{
			return this.text + " : " + base.ToString();
		}

		private string text;

		private string description;
	}
}
