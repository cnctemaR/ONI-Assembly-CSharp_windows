using System;

public class Catchable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void Caught()
	{
		base.Trigger(1272413801, null);
		if (this.body)
		{
			this.body.RemoveObjectFromBody(base.gameObject);
		}
	}

	[MyCmpAdd]
	protected UserMenu userMenu;

	public int Weight;

	public BodyOfWater body;
}
