using System;

public class CreatureBrain : Brain
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Navigator component = base.GetComponent<Navigator>();
		if (component != null)
		{
			if (base.GetComponent<KPrefabID>().HasTag(GameTags.Robots.Behaviours.HasDoorPermissions))
			{
				component.SetAbilities(new RobotPathFinderAbilities(component));
				return;
			}
			component.SetAbilities(new CreaturePathFinderAbilities(component));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.onPreUpdate += delegate
		{
			Navigator component = base.GetComponent<Navigator>();
			if (component != null)
			{
				component.UpdateProbe(false);
			}
		};
	}

	public string symbolPrefix;

	public Tag species;
}
