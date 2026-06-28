using System;
using STRINGS;

public class PriorityChoreCommand : PriorityCommand
{
	public PriorityChoreCommand(Chore.Precondition.Context context)
	{
		this.context = context;
	}

	public override void Cleanup()
	{
		base.Cleanup();
		this.context.chore.SetOverrideTarget(null);
		this.context.chore.Fail("Deprioritized");
		if (this.prioritizedChore != null)
		{
			this.prioritizedChore.Cancel("Deprioritized");
			this.prioritizedChore = null;
		}
	}

	public override bool CanBegin()
	{
		return this.context.IsSuccess();
	}

	public override void Begin()
	{
		base.Begin();
		this.context.chore.SetOverrideTarget(this.context.consumer);
		ChoreType choreType = Db.Get().ChoreTypes.PrioritizeChore;
		if (this.context.consumer.GetComponent<StateMachineController>().GetSMI<ManualControlMonitor.Instance>().IsControlled())
		{
			choreType = Db.Get().ChoreTypes.ManualControlPrioritizeChore;
		}
		this.prioritizedChore = new PrioritizedChore(choreType, this.context.consumer, this.context.chore);
	}

	public override string ToString()
	{
		string text = this.context.chore.choreType.Name;
		if (!this.context.IsSuccess())
		{
			string text2 = Strings.Get("STRINGS.UI.CONTEXTSCREEN.PRECONDITIONS." + this.context.chore.preconditions[this.context.failedPreconditionId].id.ToUpper());
			string text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" (",
				UI.CONTEXTSCREEN.UNAVAILABLE,
				": ",
				text2,
				")"
			});
		}
		else if (this.context.chore.driver != null)
		{
			text = text + "\n " + UI.CONTEXTSCREEN.ASSIGNEDTO + this.context.chore.driver.GetComponent<KSelectable>().GetName();
		}
		return text;
	}

	private Chore.Precondition.Context context;

	private Chore prioritizedChore;
}
