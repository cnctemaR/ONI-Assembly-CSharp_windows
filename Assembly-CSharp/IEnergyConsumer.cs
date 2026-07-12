using System;

public interface IEnergyConsumer : ICircuitConnected
{
	float WattsUsed { get; }

	float WattsNeededWhenActive { get; }

	int PowerSortOrder { get; }

	void SetConnectionStatus(CircuitManager.ConnectionStatus status);

	string Name { get; }

	bool IsConnected { get; }

	bool IsPowered { get; }
}
