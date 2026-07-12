using System;

public class GameplayEventMinionFilter
{
	public string id;

	public GameplayEventMinionFilter.FilterFn filter;

	public delegate bool FilterFn(MinionIdentity minion);
}
