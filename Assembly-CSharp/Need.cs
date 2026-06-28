using System;
using Klei.AI;

public abstract class Need : KMonoBehaviour
{
	public string Name { get; protected set; }

	public string ExpectationTooltip { get; protected set; }

	public string Tooltip { get; protected set; }

	public abstract Klei.AI.Attribute GetExpectationAttribute();
}
