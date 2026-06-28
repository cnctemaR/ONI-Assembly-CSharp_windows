using System;

namespace Database
{
	public class StateMachineCategories : ResourceSet<StateMachine.Category>
	{
		public StateMachineCategories()
		{
			this.Ai = base.Add(new StateMachine.Category("Ai"));
			this.Monitor = base.Add(new StateMachine.Category("Monitor"));
			this.Chore = base.Add(new StateMachine.Category("Chore"));
			this.Misc = base.Add(new StateMachine.Category("Misc"));
		}

		public StateMachine.Category Ai;

		public StateMachine.Category Monitor;

		public StateMachine.Category Chore;

		public StateMachine.Category Misc;
	}
}
