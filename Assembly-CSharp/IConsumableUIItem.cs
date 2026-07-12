using System;

public interface IConsumableUIItem
{
	string ConsumableId { get; }

	string ConsumableName { get; }

	int MajorOrder { get; }

	int MinorOrder { get; }

	bool Display { get; }

	string OverrideSpriteName()
	{
		return null;
	}

	bool RevealTest()
	{
		return ConsumerManager.instance.isDiscovered(this.ConsumableId.ToTag());
	}
}
