using System;

public class ManuallySetRemoteWorkTargetComponent : RemoteDockWorkTargetComponent
{
	public override Chore RemoteDockChore
	{
		get
		{
			return this.chore;
		}
	}

	public void SetChore(Chore chore_)
	{
		this.chore = chore_;
	}

	private Chore chore;
}
