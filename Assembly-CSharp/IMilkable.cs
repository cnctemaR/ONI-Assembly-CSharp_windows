using System;

public interface IMilkable
{
	bool IsReadyToBeMilked();

	SimHashes GetMilkElement();

	void MilkingComplete(Storage storage);
}
