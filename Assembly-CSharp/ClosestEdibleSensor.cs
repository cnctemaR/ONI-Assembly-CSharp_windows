using System;

public class ClosestEdibleSensor : Sensor
{
	public ClosestEdibleSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		TagBits tagBits = new TagBits(base.GetComponent<ConsumableConsumer>().forbiddenTags);
		Pickupable pickupable = Game.Instance.fetchManager.FindEdibleFetchTarget(base.GetComponent<Storage>(), ref ClosestEdibleSensor.edibleTagBits, ref TagBits.None, ref tagBits, 0f);
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
			Pickupable pickupable2 = Game.Instance.fetchManager.FindFetchTarget(base.GetComponent<Storage>(), ref ClosestEdibleSensor.edibleTagBits, ref TagBits.None, ref TagBits.None, 0f);
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

	private static TagBits edibleTagBits = new TagBits(GameTags.Edible);

	private Edible edible;

	private bool hasEdible;

	public bool edibleInReachButNotPermitted;
}
