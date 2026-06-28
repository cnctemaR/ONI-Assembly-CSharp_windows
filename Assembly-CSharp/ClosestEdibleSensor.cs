using System;

public class ClosestEdibleSensor : Sensor
{
	public ClosestEdibleSensor(Sensors sensors)
		: base(sensors)
	{
		this.worker = base.GetComponent<Worker>();
	}

	public override void Update()
	{
		Pickupable pickupable = null;
		FetchManager.Instance.FindFetchTarget(this.worker, base.GetComponent<Storage>(), new TagBits(ClosestEdibleSensor.edibleTag), null, base.GetComponent<ConsumableConsumer>().forbiddenTags, 0f, ref pickupable);
		Edible edible = null;
		bool flag = false;
		if (pickupable != null)
		{
			edible = pickupable.GetComponent<Edible>();
			flag = true;
		}
		if (edible != this.edible || this.hasEdible != flag)
		{
			this.edible = edible;
			this.hasEdible = flag;
			base.Trigger(86328522, this.edible);
		}
	}

	public Edible GetEdible()
	{
		return this.edible;
	}

	private static Tag[] edibleTag = new Tag[] { GameTags.Edible };

	private Edible edible;

	private Worker worker;

	private bool hasEdible;
}
