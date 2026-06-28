using System;

public class Sequenceable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		if (this.currentSequence == null)
		{
			base.enabled = false;
		}
	}

	public void Begin(Sequence sequence)
	{
		this.End();
		this.currentCommand = this.currentSequence.GetNextCommand(null);
		if (this.currentCommand != null)
		{
			base.enabled = true;
			this.currentCommand.Enter(this);
		}
		if (this.currentSequence.Length > 0)
		{
			this.currentSequence = sequence;
			base.enabled = true;
		}
	}

	public void Update()
	{
		if (this.currentCommand != null && this.currentCommand.Update(this))
		{
			this.currentCommand.Exit(this);
			this.currentCommand = this.currentSequence.GetNextCommand(this.currentCommand);
			if (this.currentCommand != null)
			{
				this.currentCommand.Enter(this);
			}
			else
			{
				this.End();
			}
		}
	}

	public void End()
	{
		if (this.currentSequence != null)
		{
			if (this.currentCommand != null)
			{
				this.currentCommand.Exit(this);
				this.currentCommand = null;
			}
			this.currentSequence = null;
			base.enabled = false;
		}
	}

	private Sequence currentSequence;

	private Sequence.Command currentCommand;
}
