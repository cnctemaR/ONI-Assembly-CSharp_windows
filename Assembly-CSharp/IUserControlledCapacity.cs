using System;

public interface IUserControlledCapacity
{
	float UserMaxCapacity { get; set; }

	float MinCapacity { get; }

	float MaxCapacity { get; }

	LocString CapacityUnits { get; }
}
