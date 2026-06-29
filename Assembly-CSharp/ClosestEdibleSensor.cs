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
		FetchManager.Instance.FindFetchTarget(this.worker, base.GetComponent<Storage>(), new TagBits(ClosestEdibleSensor.edibleTag), default(TagBits), new TagBits(base.GetComponent<ConsumableConsumer>().forbiddenTags), 0f, ref pickupable);
		bool flag = this.edibleInReachButNotPermitted;
		Edible edible = null;
		bool flag2 = false;
		if (pickupable != null)
		{
			edible = pickupable.GetComponent<Edible>();
			flag2 = true;
			flag = false;
		}
		else
		{
			Pickupable pickupable2 = null;
			FetchManager.Instance.FindFetchTarget(this.worker, base.GetComponent<Storage>(), new TagBits(ClosestEdibleSensor.edibleTag), default(TagBits), default(TagBits), 0f, ref pickupable2);
			flag = pickupable2 != null;
		}
		if (edible != this.edible || this.hasEdible != flag2)
		{
			this.edible = edible;
			this.hasEdible = flag2;
			this.edibleInReachButNotPermitted = flag;
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

	public bool edibleInReachButNotPermitted;
}
