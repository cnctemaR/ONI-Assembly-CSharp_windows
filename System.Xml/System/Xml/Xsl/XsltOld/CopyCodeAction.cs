using System;
using System.Collections;

namespace System.Xml.Xsl.XsltOld
{
	internal class CopyCodeAction : Action
	{
		internal CopyCodeAction()
		{
			this.copyEvents = new ArrayList();
		}

		internal void AddEvent(Event copyEvent)
		{
			this.copyEvents.Add(copyEvent);
		}

		internal void AddEvents(ArrayList copyEvents)
		{
			this.copyEvents.AddRange(copyEvents);
		}

		internal override void ReplaceNamespaceAlias(Compiler compiler)
		{
			int count = this.copyEvents.Count;
			for (int i = 0; i < count; i++)
			{
				((Event)this.copyEvents[i]).ReplaceNamespaceAlias(compiler);
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 2)
				{
					return;
				}
			}
			else
			{
				frame.Counter = 0;
				frame.State = 2;
			}
			while (processor.CanContinue && ((Event)this.copyEvents[frame.Counter]).Output(processor, frame))
			{
				if (frame.IncrementCounter() >= this.copyEvents.Count)
				{
					frame.Finished();
					return;
				}
			}
		}

		internal override DbgData GetDbgData(ActionFrame frame)
		{
			return ((Event)this.copyEvents[frame.Counter]).DbgData;
		}

		private const int Outputting = 2;

		private ArrayList copyEvents;
	}
}
