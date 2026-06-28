using System;
using System.Collections.Generic;

public class PriorityCommandCollector
{
	public PriorityCommandCollector(ChoreConsumer consumer)
	{
		this.consumer = consumer;
	}

	public ChoreConsumer consumer { get; private set; }

	public List<PriorityCommand> commands = new List<PriorityCommand>();
}
