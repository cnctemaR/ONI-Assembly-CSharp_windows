using System;

public class ReefGeneratorPower : Generator
{
	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		this.operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		bool isOperational = this.operational.IsOperational;
		if (isOperational)
		{
			base.GenerateJoules(base.WattageRating * dt, false);
		}
		this.operational.SetActive(isOperational, false);
	}
}
