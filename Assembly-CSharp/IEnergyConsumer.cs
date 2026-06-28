using System;

public interface IEnergyConsumer
{
	float WattsUsed { get; }

	float WattsNeededWhenActive { get; }

	int PowerSortOrder { get; }

	void SetConnectionStatus(CircuitManager.ConnectionStatus status);

	string Name { get; }

	int PowerCell { get; }
}
