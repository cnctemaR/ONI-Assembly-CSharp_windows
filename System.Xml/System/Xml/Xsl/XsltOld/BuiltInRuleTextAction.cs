using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class BuiltInRuleTextAction : Action
	{
		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 2)
				{
					return;
				}
				processor.TextEvent(frame.StoredOutput);
				frame.Finished();
				return;
			}
			else
			{
				string text = processor.ValueOf(frame.NodeSet.Current);
				if (processor.TextEvent(text, false))
				{
					frame.Finished();
					return;
				}
				frame.StoredOutput = text;
				frame.State = 2;
				return;
			}
		}

		private const int ResultStored = 2;
	}
}
