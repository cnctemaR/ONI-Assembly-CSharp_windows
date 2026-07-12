using System;

public abstract class RemoteWorkable : Workable, IRemoteDockWorkTarget
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.RemoteDockWorkTargets.Add(base.gameObject.GetMyWorldId(), this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RemoteDockWorkTargets.Remove(base.gameObject.GetMyWorldId(), this);
	}

	public abstract Chore RemoteDockChore { get; }

	public virtual IApproachable Approachable
	{
		get
		{
			return this;
		}
	}
}
