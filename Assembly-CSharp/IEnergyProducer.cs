using System;

public interface IEnergyProducer
{
	float JoulesAvailable { get; }

	void ConsumeEnergy(float joules);
}
