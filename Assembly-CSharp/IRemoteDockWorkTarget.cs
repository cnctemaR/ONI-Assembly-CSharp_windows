using System;

public interface IRemoteDockWorkTarget
{
	Chore RemoteDockChore { get; }

	IApproachable Approachable { get; }
}
