using System;

public interface IDiningSeat
{
	bool HasSalt { get; }

	HashedString EatAnim { get; }

	HashedString ReloadElectrobankAnim { get; }

	Storage FindStorage();

	Operational FindOperational();

	KPrefabID Diner { get; set; }
}
