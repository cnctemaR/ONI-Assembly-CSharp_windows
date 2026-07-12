using System;

public class GameplayEventPrecondition
{
	public string description;

	public GameplayEventPrecondition.PreconditionFn condition;

	public bool required;

	public int priorityModifier;

	public delegate bool PreconditionFn();
}
