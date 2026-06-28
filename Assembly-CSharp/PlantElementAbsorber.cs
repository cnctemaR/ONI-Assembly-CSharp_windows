using System;

public struct PlantElementAbsorber
{
	public void Clear()
	{
		this.storage = null;
		this.consumedElements = null;
	}

	public Storage storage;

	public PlantElementAbsorber.LocalInfo localInfo;

	public HandleVector<int>.Handle[] accumulators;

	public PlantElementAbsorber.ConsumeInfo[] consumedElements;

	public struct ConsumeInfo
	{
		public ConsumeInfo(Tag tag, float mass_consumption_rate)
		{
			this.tag = tag;
			this.massConsumptionRate = mass_consumption_rate;
		}

		public Tag tag;

		public float massConsumptionRate;
	}

	public struct LocalInfo
	{
		public Tag tag;

		public float massConsumptionRate;
	}
}
