using System;

public interface IConduitDispenser
{
	Storage Storage { get; }

	ConduitType ConduitType { get; }
}
