using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class TextEvent : Event
	{
		protected TextEvent()
		{
		}

		public TextEvent(string text)
		{
			this.text = text;
		}

		public TextEvent(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			this.text = input.Value;
		}

		public override bool Output(Processor processor, ActionFrame frame)
		{
			return processor.TextEvent(this.text);
		}

		public virtual string Evaluate(Processor processor, ActionFrame frame)
		{
			return this.text;
		}

		private string text;
	}
}
