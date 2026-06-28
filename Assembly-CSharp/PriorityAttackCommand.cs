using System;

public class PriorityAttackCommand : PriorityCommand
{
	public PriorityAttackCommand(ChoreConsumer attacker, AttackableBase target)
	{
		AttackChore attackChore = new AttackChore(attacker, target.gameObject);
		attackChore.AddPrecondition(PriorityAttackCommand.HasCommandStarted, this);
		attackChore.AddPrecondition(ChorePreconditions.CanMoveTo, target);
		this.context = new Chore.Precondition.Context(attackChore, attacker, true, null);
		this.context.RunPreconditions();
	}

	// Note: this type is marked as 'beforefieldinit'.
	static PriorityAttackCommand()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "HasCommandStarted";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			PriorityAttackCommand priorityAttackCommand = (PriorityAttackCommand)data;
			return context.isAttemptingOverride || priorityAttackCommand.hasStarted;
		};
		PriorityAttackCommand.HasCommandStarted = precondition;
	}

	public override void Cleanup()
	{
		base.Cleanup();
		this.context.chore.Fail("Deprioritized");
	}

	public override bool CanBegin()
	{
		return this.context.IsSuccess();
	}

	public override void Begin()
	{
		base.Begin();
		this.hasStarted = true;
	}

	public override string ToString()
	{
		string text = this.context.chore.choreType.Name;
		if (!this.context.IsSuccess())
		{
			text += "\n";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				" (",
				this.context.failedPreconditionId,
				")"
			});
		}
		return text;
	}

	private Chore.Precondition.Context context;

	public bool hasStarted;

	public static Chore.Precondition HasCommandStarted;
}
