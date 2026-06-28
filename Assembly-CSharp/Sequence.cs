using System;
using System.Collections.Generic;

public class Sequence
{
	public int Length
	{
		get
		{
			return this.commands.Count;
		}
	}

	public Sequence.Command GetNextCommand(Sequence.Command current_command)
	{
		return null;
	}

	private List<Sequence.Command> commands = new List<Sequence.Command>();

	public class Command
	{
		public virtual void Enter(Sequenceable Sequenceable)
		{
		}

		public virtual bool Update(Sequenceable Sequenceable)
		{
			return true;
		}

		public virtual void Exit(Sequenceable Sequenceable)
		{
		}
	}

	public class PlayAnimCommand : Sequence.Command
	{
		public override void Enter(Sequenceable Sequenceable)
		{
		}

		public override bool Update(Sequenceable Sequenceable)
		{
			return true;
		}

		public override void Exit(Sequenceable Sequenceable)
		{
		}
	}
}
