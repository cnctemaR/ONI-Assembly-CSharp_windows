using System;

public interface IConduitConsumer
{
	Storage Storage { get; }

	ConduitType ConduitType { get; }
}
