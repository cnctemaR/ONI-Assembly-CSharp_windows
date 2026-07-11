using System;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class AvtEvent : TextEvent
	{
		public AvtEvent(int key)
		{
			this.key = key;
		}

		public override bool Output(Processor processor, ActionFrame frame)
		{
			return processor.TextEvent(processor.EvaluateString(frame, this.key));
		}

		public override string Evaluate(Processor processor, ActionFrame frame)
		{
			return processor.EvaluateString(frame, this.key);
		}

		private int key;
	}
}
